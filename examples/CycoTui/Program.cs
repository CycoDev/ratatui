using System;
using System.Threading;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Widgets;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Input;

namespace CycoTui.Sample;

internal static class Program
{
    private static CancellationTokenSource _cts = new();
    private static FocusManager _focus = new();

    // Demo state
    private static ListState _listState = new(count: 10);
    private static int _horizontalOffset = 0;

    static void Main(string[] args)
    {
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; _cts.Cancel(); };
        using ITerminalBackend backend = CreateBackend();
        var terminal = new Terminal(backend, new LoggingContext(null));

        // Focus registration (placeholder widgets simulated by indices)
        _focus.Register(new DummyFocusable());
        _focus.Register(new DummyFocusable());
        _focus.Register(new DummyFocusable());

        // Render initial frame
        Render(terminal);

        var inputSource = new StdinBlockingInputSource();
        var loop = new BlockingInputLoop(inputSource);
        loop.Run(_cts.Token, evt => HandleInput(evt, terminal), shouldStop: () => _cts.IsCancellationRequested);
    }

    private static ITerminalBackend CreateBackend()
    {
        // Simple preference: use Unix backend on Unix, Windows backend on Windows else minimal.
        if (OperatingSystem.IsWindows()) return new CycoTui.Backend.Windows.WindowsTerminalBackend();
        return new CycoTui.Backend.Unix.UnixTerminalBackend();
    }

    private static void HandleInput(InputEvent evt, Terminal terminal)
    {
        if (evt.Type == InputEventType.Key && evt.Key.HasValue)
        {
            var k = evt.Key.Value;
            // Quit keys
            if ((k.Code == KeyCode.Character && (k.Char == 'q' || k.Char == 'Q') && (k.Modifiers & KeyModifiers.Ctrl) != 0) || k.Code == KeyCode.Escape)
            {
                _cts.Cancel();
                return;
            }
            switch (k.Code)
            {
                case KeyCode.Tab:
                    if ((k.Modifiers & KeyModifiers.Shift) != 0) _focus.Previous(); else _focus.Next();
                    break;
                case KeyCode.ArrowDown:
                    _listState.ScrollDown(viewportHeight:5);
                    break;
                case KeyCode.ArrowUp:
                    _listState.ScrollUp(viewportHeight:5);
                    break;
                case KeyCode.ArrowRight:
                    _horizontalOffset += 2;
                    break;
                case KeyCode.ArrowLeft:
                    _horizontalOffset = Math.Max(0, _horizontalOffset - 2);
                    break;
            }
            Render(terminal);
        }
        else if (evt.Type == InputEventType.Resize && evt.Resize.HasValue)
        {
            Render(terminal);
        }
    }

    private static void Render(Terminal terminal)
    {
        // Terminal does not expose backend publicly; reuse captured backend reference
        var size = CreateBackend().GetSize(); // TODO: refactor to avoid re-instantiation
        terminal.Draw(frame =>
        {
            var rootRect = new Rect(0,0,size.Width, size.Height);
            var outer = Block.Create().WithTitle("CycoTui Demo", Style.Empty.Add(TextModifier.Bold));
            outer.Render(frame, rootRect);
            var inner = Block.GetInnerContentRect(rootRect, Padding.Zero);

            // Logo
            var logo = LogoWidget.Create();
            logo.Render(frame, new Rect(inner.X, inner.Y, inner.Width, 1));

            // Paragraph (status)
            var status = Paragraph.Create()
                .WithText($"Offset={_horizontalOffset} FocusIndex={(_focus.Current==null?-1:_focus.Widgets.ToList().IndexOf(_focus.Current))}  Use Ctrl+Q or Esc to quit")
                .WithWrap(false)
                .WithHorizontalOffset(_horizontalOffset);
            status.Render(frame, new Rect(inner.X, inner.Y+1, inner.Width, 1));

            // List widget area
            var listWidget = ListWidget.Create().WithItems(BuildListItems().ToList()).WithHorizontalOffset(0);
            listWidget.Render(frame, new Rect(inner.X, inner.Y+2, inner.Width/2, Math.Max(3, inner.Height - 3)), _listState);

            // Table area (placeholder data mirrored from selection)
            var table = TableWidget.Create().WithColumns(new[]{ new TableColumn("Selected", Constraint.Fill(), Style.Empty)})
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

    private static TableRow BuildSelectedRow()
    {
        var selected = _listState.Selected ?? 0;
        return new TableRow(new[]{ ($"You selected {selected}", Style.Empty) });
    }

    private sealed class DummyFocusable : IFocusableWidget
    {
        public bool CanFocus => true;
        public void OnFocusGained() {}
        public void OnFocusLost() {}
    }
}
