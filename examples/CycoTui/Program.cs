using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Sample;

internal static class Program
{
    private static readonly CancellationTokenSource _cts = new();
    private static ITerminalBackend? _backend;
    private static Terminal? _terminal;

    // Content history (top area)
    private static readonly List<string> _messages = new();
    // Current input lines (expandable)
    private static List<string> _inputLines = new() { string.Empty };

    static void Main(string[] args)
    {
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; _cts.Cancel(); };
        _backend = CreateBackend();
        _terminal = new Terminal(_backend, new LoggingContext(null));
        Render();
        InputLoop();
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
            if (!HandleKey(key)) continue;
            Render();
        }
    }

    private static bool HandleKey(ConsoleKeyInfo k)
    {
        if (k.Key == ConsoleKey.Escape) { _cts.Cancel(); return false; }
        if (k.Key == ConsoleKey.Q && (k.Modifiers & ConsoleModifiers.Control) != 0) { _cts.Cancel(); return false; }

        if (k.Key == ConsoleKey.Enter)
        {
            SubmitInput();
            return true;
        }
        if (k.Key == ConsoleKey.J && (k.Modifiers & ConsoleModifiers.Control) != 0)
        {
            _inputLines.Add(string.Empty);
            return true;
        }
        if (k.Key == ConsoleKey.Backspace)
        {
            if (_inputLines.Count > 0)
            {
                int last = _inputLines.Count - 1;
                if (_inputLines[last].Length > 0)
                    _inputLines[last] = _inputLines[last][..^1];
                else if (_inputLines.Count > 1)
                    _inputLines.RemoveAt(last);
            }
            return true;
        }
        if (!char.IsControl(k.KeyChar))
        {
            int last = _inputLines.Count - 1;
            _inputLines[last] += k.KeyChar;
            return true;
        }
        return false;
    }

    private static void SubmitInput()
    {
        if (_inputLines.Count == 0) return;
        var combined = string.Join("\n", _inputLines).TrimEnd();
        if (combined.Length > 0) _messages.Add(combined);
        _inputLines = new List<string> { string.Empty };
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
            int inputHeight = Math.Max(1, _inputLines.Count);
            int separators = 2; // lines above and below input
            int reserved = inputHeight + separators + statusLineHeight;
            int contentHeight = Math.Max(0, height - reserved);

            // Content area: newest at bottom, oldest truncated at top
            var visibleMessages = _messages.TakeLast(contentHeight).ToList();
            for (int i = 0; i < contentHeight; i++)
            {
                string line = i < visibleMessages.Count ? visibleMessages[i] : string.Empty;
                if (line.Length > width) line = line[..width];
                frame.WriteString(0, i, line.PadRight(width), StyleType.Empty);
            }

            int sepAboveInputY = contentHeight;
            frame.WriteString(0, sepAboveInputY, new string('─', width), StyleType.Empty.Add(TextModifier.Dim));

            for (int i = 0; i < inputHeight; i++)
            {
                string inputLine = _inputLines[i];
                if (inputLine.Length > width) inputLine = inputLine[..width];
                frame.WriteString(0, sepAboveInputY + 1 + i, inputLine.PadRight(width), StyleType.Empty);
            }

            int sepBelowInputY = sepAboveInputY + 1 + inputHeight;
            frame.WriteString(0, sepBelowInputY, new string('─', width), StyleType.Empty.Add(TextModifier.Dim));

            string status = $"Messages: {_messages.Count}  Lines: {_inputLines.Count}  Enter=Submit  Ctrl+J=NewLine  Esc/Ctrl+Q=Quit";
            if (status.Length > width) status = status[..width];
            frame.WriteString(0, sepBelowInputY + 1, status.PadRight(width), StyleType.Empty.Add(TextModifier.Bold));
        });
    }
}
