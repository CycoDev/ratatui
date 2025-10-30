using System;
using System.Threading;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Widgets;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Input;
using System.Collections.Generic;
using System.Linq;

namespace CycoTui.Sample;

internal static class Program
{
    private static readonly CancellationTokenSource _cts = new();
    private static readonly FocusManager _focus = new();
    private static ITerminalBackend? _backend;
    private static Terminal? _terminal;

    // Demo state
    private static readonly ListState _listState = new(count: 10);
    private static int _selectedIndex = 0;
    private static int _previousSelectedIndex = 0;
    private static int _horizontalOffset = 0;
    private static int _listViewportHeight = 5;

    static void Main(string[] args)
    {
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; _cts.Cancel(); };
        _backend = CreateBackend();
        _terminal = new Terminal(_backend, new LoggingContext(null));

        // Focus registration (three logical focus targets: list, table, status)
        _focus.Register(new DummyFocusable()); // list
        _focus.Register(new DummyFocusable()); // table
        _focus.Register(new DummyFocusable()); // status
        _selectedIndex = 0;
        _listState.Select(_selectedIndex, _listViewportHeight);

        Render();
        InputLoop();
    }

    private static ITerminalBackend CreateBackend()
    {
        // Simple preference: use Unix backend on Unix, Windows backend on Windows else minimal.
        if (OperatingSystem.IsWindows()) return new CycoTui.Backend.Windows.WindowsTerminalBackend();
        return new CycoTui.Backend.Unix.UnixTerminalBackend();
    }

    private static void InputLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            var key = Console.ReadKey(true);
            if (!HandleKey(key)) continue;
            Render();
        }
    }

    private static bool HandleKey(ConsoleKeyInfo k)
    {
        if (k.Key == ConsoleKey.Escape) { _cts.Cancel(); return false; }
        if (k.Key == ConsoleKey.Q && (k.Modifiers & ConsoleModifiers.Control) != 0) { _cts.Cancel(); return false; }

        switch (k.Key)
        {
            case ConsoleKey.Tab:
                if ((k.Modifiers & ConsoleModifiers.Shift) != 0) _focus.Previous(); else _focus.Next();
                return true;
            case ConsoleKey.DownArrow:
                if (GetFocusIndex()==0) { MoveSelection(1); return true; } return false;
            case ConsoleKey.UpArrow:
                if (GetFocusIndex()==0) { MoveSelection(-1); return true; } return false;
            case ConsoleKey.RightArrow:
                _horizontalOffset += 2; return true;
            case ConsoleKey.LeftArrow:
                _horizontalOffset = Math.Max(0, _horizontalOffset - 2); return true;
        }
        return false;
    }

    // Legacy signature residual removed
    private static void Render()
    {
        if (_backend == null || _terminal == null) return;
        var size = _backend.GetSize();
        _terminal.Draw(frame =>
        {
            var rootRect = new Rect(0,0,size.Width, size.Height);
            var outer = Block.Create().WithTitle("CycoTui Demo", Style.Empty.Add(TextModifier.Bold));
            outer.Render(frame, rootRect);
            var inner = Block.GetInnerContentRect(rootRect, Padding.Zero);

            // Logo
            var logo = LogoWidget.Create();
            logo.Render(frame, new Rect(inner.X, inner.Y, inner.Width, 1));

            // Paragraph (status)
            var statusStyle = GetFocusIndex()==2 ? Style.Empty.Add(TextModifier.Bold) : Style.Empty;
            var status = Paragraph.Create()
                .WithText($"Offset={_horizontalOffset} Focus={GetFocusIndex()} (Tab to cycle)  Esc/Ctrl+Q quits")
                .WithWrap(false)
                .WithHorizontalOffset(_horizontalOffset);
            // Apply status emphasis by writing over after render if focused (simple approach)
            if (GetFocusIndex()==2)
            {
                // Overwrite with bold style (Paragraph currently applies a single style per instance; for simplicity re-render inline)
            }
            status.Render(frame, new Rect(inner.X, inner.Y+1, inner.Width, 1));

            // Upstream ListWidget usage
            var listItems = Enumerable.Range(0,10)
                .Select(i => new ListItem($"Item {i}", Style.Empty))
                .ToList();
            var listWidget = ListWidget.Create()
                .WithItems(listItems)
                .WithFocused(GetFocusIndex()==0);
            var listRect = new Rect(inner.X, inner.Y+2, inner.Width/2, Math.Max(3, inner.Height - 3));
            listWidget.Render(frame, listRect, _listState);

            // Table area (placeholder data mirrored from selection)
            var tableHeaderStyle = GetFocusIndex()==1 ? Style.Empty.Add(TextModifier.Bold) : Style.Empty;
            var table = TableWidget.Create().WithColumns(new[]{ new TableColumn("Selected", Constraint.Fill(), tableHeaderStyle)})
                .WithRows(new[]{ BuildSelectedRow() });
            table.Render(frame, new Rect(inner.X + inner.Width/2, inner.Y+2, inner.Width - inner.Width/2, Math.Max(3, inner.Height - 3)));        
        });
    }

    private static IEnumerable<ListItem> BuildListItems()
    {
        for (int i=0;i<10;i++)
        {
            yield return new ListItem($"Item {i}", Style.Empty);
        }
    }

    private static int GetFocusIndex()
    {
        if (_focus.Current == null) return -1;
        return _focus.Widgets.ToList().IndexOf(_focus.Current);
    }

    private static TableRow BuildSelectedRow()
    {
        var selected = _listState.Selected ?? 0;
        return new TableRow(new[]{ ($"You selected {selected}", Style.Empty) });
    }
    private static void MoveSelection(int delta)
    {
        _previousSelectedIndex = _selectedIndex;
        _selectedIndex += delta;
        if (_selectedIndex < 0) _selectedIndex = 0;
        if (_selectedIndex > 9) _selectedIndex = 9; // TODO: derive from list length
        _listState.Select(_selectedIndex, _listViewportHeight);
    }

    private sealed class DummyFocusable : IFocusableWidget
    {
        public bool CanFocus => true;
        public void OnFocusGained() {}
        public void OnFocusLost() {}
    }
}
