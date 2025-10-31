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
    // Completion state (handles '@', '#', etc. completion popup)
    private static CompletionState _completionState = CompletionState.CreateInactive();

    // Configure multiple completion triggers
    // Each trigger can have its own provider and label
    private static readonly List<CompletionTrigger> _completionTriggers = new()
    {
        // '@' for file completion
        new CompletionTrigger(
            '@',
            async () =>
            {
                var workspaceRoot = Environment.CurrentDirectory;
                return await Task.Run(() => WorkspaceFileScanner.ScanFiles(workspaceRoot, maxFiles: 1000));
            },
            "Files"
        ),

        // '#' for tag completion (example)
        new CompletionTrigger(
            '#',
            async () =>
            {
                await Task.CompletedTask;
                return new List<string>
                {
                    "bug", "feature", "enhancement", "documentation",
                    "refactor", "test", "performance", "security"
                };
            },
            "Tags"
        )
    };

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

            // Handle ESC key - cancel completion or quit
            if (key.Key == ConsoleKey.Escape)
            {
                if (_completionState.IsActive)
                {
                    _completionState = _completionState.Deactivate();
                    Render();
                    continue;
                }
                _cts.Cancel();
                continue;
            }

            // Handle Ctrl+Q for quitting
            if (key.Key == ConsoleKey.Q && (key.Modifiers & ConsoleModifiers.Control) != 0)
            {
                _cts.Cancel();
                continue;
            }

            // If completion is active, intercept navigation keys
            if (_completionState.IsActive)
            {
                bool handled = HandleCompletionKey(key);
                if (handled)
                {
                    Render();
                    continue;
                }
            }

            // Let the input state handle the key
            if (_inputState.HandleKey(key))
            {
                // After input changes, check if we should activate/update completion
                UpdateCompletionState();
                Render();
            }
        }
    }

    private static bool HandleCompletionKey(ConsoleKeyInfo key)
    {
        // Up arrow or Ctrl+P - select previous item
        if (key.Key == ConsoleKey.UpArrow ||
            (key.Key == ConsoleKey.P && (key.Modifiers & ConsoleModifiers.Control) != 0))
        {
            _completionState = _completionState.SelectPrevious();
            return true;
        }

        // Down arrow or Ctrl+N - select next item
        if (key.Key == ConsoleKey.DownArrow ||
            (key.Key == ConsoleKey.N && (key.Modifiers & ConsoleModifiers.Control) != 0))
        {
            _completionState = _completionState.SelectNext();
            return true;
        }

        // Enter or Tab - insert selected file
        if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Tab)
        {
            var selectedItem = _completionState.GetSelectedItem();
            if (selectedItem != null)
            {
                InsertSelectedFile(selectedItem);
                _completionState = _completionState.Deactivate();
                return true;
            }
        }

        return false;
    }

    private static void UpdateCompletionState()
    {
        // Get current line and cursor position
        if (_inputState.Lines.Count == 0) return;

        var currentLine = _inputState.Lines[_inputState.CursorLineIndex];
        var cursorColumn = _inputState.CursorColumn;

        // Collect all trigger characters
        var triggerChars = _completionTriggers.Select(t => t.TriggerChar);

        // Detect if there's an active completion trigger
        var query = CompletionHelper.DetectCompletionQuery(
            currentLine,
            cursorColumn,
            triggerChars,
            out int triggerColumn,
            out char foundTriggerChar);

        if (query != null)
        {
            // Find the matching trigger configuration
            var trigger = _completionTriggers.FirstOrDefault(t => t.TriggerChar == foundTriggerChar);
            if (trigger.Provider == null)
            {
                // No provider configured for this trigger
                _completionState = _completionState.Deactivate();
                return;
            }

            // Activate or update completion
            if (!_completionState.IsActive || _completionState.TriggerChar != foundTriggerChar)
            {
                // First time or different trigger - call the provider callback to get items
                try
                {
                    // Note: This blocks the UI thread briefly
                    var items = trigger.Provider().GetAwaiter().GetResult();
                    _completionState = _completionState.Activate(
                        items,
                        _inputState.CursorLineIndex,
                        triggerColumn,
                        foundTriggerChar,
                        trigger.Label);
                }
                catch (Exception ex)
                {
                    // Show error in popup instead of crashing
                    _completionState = _completionState.ActivateWithError(
                        $"Error loading items: {ex.Message}",
                        _inputState.CursorLineIndex,
                        triggerColumn,
                        foundTriggerChar,
                        trigger.Label);
                }
            }

            // Update query (only if not in error state)
            if (!_completionState.IsError)
            {
                _completionState = _completionState.UpdateQuery(query);
            }
        }
        else if (_completionState.IsActive)
        {
            // No longer in completion context - deactivate
            _completionState = _completionState.Deactivate();
        }
    }

    private static void InsertSelectedFile(string selectedFile)
    {
        // Get current state
        var currentLine = _inputState.Lines[_inputState.CursorLineIndex];
        var cursorColumn = _inputState.CursorColumn;

        // Insert the selected item with the appropriate trigger character
        var newLine = CompletionHelper.InsertCompletion(
            currentLine,
            _completionState.TriggerColumn,
            cursorColumn,
            _completionState.TriggerChar,
            selectedFile,
            out int newCursorColumn);

        // Update the line in the input state
        _inputState.SetLine(_inputState.CursorLineIndex, newLine, newCursorColumn);
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
            var inputRect = new Rect(0, sepAboveInputY + 1, width, inputHeight);
            new CycoTui.Core.Widgets.MultiLineInputWidget()
                .WithStyles(Style.Empty, Style.Empty.Add(TextModifier.Invert))
                .Render(frame, inputRect, _inputState);

            int sepBelowInputY = sepAboveInputY + 1 + inputHeight;
            frame.WriteString(0, sepBelowInputY, new string('─', width), Style.Empty.Add(TextModifier.Dim));

            // Render file completion popup if active (overlays content above input)
            if (_completionState.IsActive)
            {
                var popupWidget = CompletionPopupWidget.Create()
                    .WithMaxVisibleItems(10)
                    .WithStyles(
                        border: Style.Empty.WithForeground(Color.Cyan),
                        title: Style.Empty.WithForeground(Color.Cyan).Add(TextModifier.Bold),
                        selected: Style.Empty.Add(TextModifier.Invert),
                        item: Style.Empty);

                var popupRect = popupWidget.CalculatePopupRect(inputRect, _completionState, height);
                popupWidget.Render(frame, popupRect, _completionState);
            }

            // Alt/Cmd+Arrow for word navigation (detected as Alt+Arrow or Alt+B/F)
            string wordNav = OperatingSystem.IsWindows() ? "Alt+←→" : "Cmd+←→";
            string status = $"Messages: {_messages.Count}  Line: {_inputState.CursorLineIndex + 1}/{_inputState.Lines.Count}  Col: {_inputState.CursorColumn}  ←→↑↓=Move  {wordNav}=Word  Home/End  Enter=Submit  Ctrl+J=NewLine  Esc=Quit";
            if (status.Length > width) status = status[..width];
            frame.WriteString(0, sepBelowInputY + 1, status.PadRight(width), Style.Empty.Add(TextModifier.Bold));
        });
    }
}
