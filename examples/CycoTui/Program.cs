using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using CycoTui.Core.Terminal;
using CycoTui.Core.Widgets;

namespace CycoTui.Sample;

internal static class Program
{
    private static readonly CancellationTokenSource _cts = new();
    private static ITerminalBackend? _backend;
    private static Terminal? _terminal;

    // Content history (top area)
    private static readonly List<string> _messages = new();
    // Multi-line input state (handles input, cursor, etc.)
    private static readonly MultiLineInputState _inputState = new();

    static void Main(string[] args)
    {
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; _cts.Cancel(); };

        // Handle input submissions
        _inputState.OnSubmit += text => _messages.Add(text);

        _backend = CreateBackend();
        _backend.Clear();
        _backend.HideCursor();
        _terminal = new Terminal(_backend, new LoggingContext(null));
        try
        {
            Render();
            InputLoop();
        }
        finally
        {
            _terminal?.Dispose();
            _backend?.Dispose();
        }
    }

    private static ITerminalBackend CreateBackend()
    {
        if (OperatingSystem.IsWindows()) return new CycoTui.Backend.Windows.WindowsTerminalBackend();
        return new CycoTui.Backend.Unix.UnixTerminalBackend();
    }

    private static void InputLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            var key = Console.ReadKey(intercept: true);

            // Handle ESC key for quitting
            if (key.Key == ConsoleKey.Escape)
            {
                _cts.Cancel();
                continue;
            }

            // Handle Ctrl+Q for quitting
            if (key.Key == ConsoleKey.Q && (key.Modifiers & ConsoleModifiers.Control) != 0)
            {
                _cts.Cancel();
                continue;
            }

            // Let the input state handle the key
            if (_inputState.HandleKey(key))
            {
                Render();
            }
        }
    }

    private static void Render()
    {
        if (_backend == null || _terminal == null) return;
        var size = _backend.GetSize();
        _terminal.Draw(frame =>
        {
            int width = size.Width;
            int height = size.Height;
            int statusLineHeight = 1;
            int inputHeight = Math.Max(1, _inputState.Lines.Count);
            int separators = 2; // lines above and below input
            int reserved = inputHeight + separators + statusLineHeight;
            int contentHeight = Math.Max(0, height - reserved);

            // Content area: newest at bottom, oldest truncated at top
            var visibleMessages = _messages.TakeLast(contentHeight).ToList();
            for (int i = 0; i < contentHeight; i++)
            {
                string line = i < visibleMessages.Count ? visibleMessages[i] : string.Empty;
                if (line.Length > width) line = line[..width];
                frame.WriteString(0, i, line.PadRight(width), Style.Empty);
            }

            int sepAboveInputY = contentHeight;
            frame.WriteString(0, sepAboveInputY, new string('─', width), Style.Empty.Add(TextModifier.Dim));

            // Use MultiLineInputWidget with state for input area
            new CycoTui.Core.Widgets.MultiLineInputWidget()
                .WithStyles(Style.Empty, Style.Empty.Add(TextModifier.Invert))
                .Render(frame, new Rect(0, sepAboveInputY + 1, width, inputHeight), _inputState);

            int sepBelowInputY = sepAboveInputY + 1 + inputHeight;
            frame.WriteString(0, sepBelowInputY, new string('─', width), Style.Empty.Add(TextModifier.Dim));

            // Alt/Cmd+Arrow for word navigation (detected as Alt+Arrow or Alt+B/F)
            string wordNav = OperatingSystem.IsWindows() ? "Alt+←→" : "Cmd+←→";
            string status = $"Messages: {_messages.Count}  Line: {_inputState.CursorLineIndex + 1}/{_inputState.Lines.Count}  Col: {_inputState.CursorColumn}  ←→↑↓=Move  {wordNav}=Word  Home/End  Enter=Submit  Ctrl+J=NewLine  Esc=Quit";
            if (status.Length > width) status = status[..width];
            frame.WriteString(0, sepBelowInputY + 1, status.PadRight(width), Style.Empty.Add(TextModifier.Bold));
        });
    }
}
