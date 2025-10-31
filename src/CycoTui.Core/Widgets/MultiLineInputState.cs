using System;
using System.Collections.Generic;
using System.Linq;

namespace CycoTui.Core.Widgets;

/// <summary>
/// State for MultiLineInputWidget - handles text content, cursor position, and keyboard input.
/// </summary>
public class MultiLineInputState
{
    private List<string> _lines = new() { string.Empty };
    private int _cursorLineIndex = 0;
    private int _cursorColumn = 0;

    /// <summary>
    /// The current input lines.
    /// </summary>
    public IReadOnlyList<string> Lines => _lines;

    /// <summary>
    /// The current cursor line index.
    /// </summary>
    public int CursorLineIndex => _cursorLineIndex;

    /// <summary>
    /// The current cursor column position within the current line.
    /// </summary>
    public int CursorColumn => _cursorColumn;

    /// <summary>
    /// Event raised when the user submits input (presses Enter).
    /// </summary>
    public event Action<string>? OnSubmit;

    /// <summary>
    /// Handles a keyboard input. Returns true if the key was handled.
    /// </summary>
    public bool HandleKey(ConsoleKeyInfo key)
    {
        string currentLine = _lines[_cursorLineIndex];

        // Arrow key navigation
        if (key.Key == ConsoleKey.LeftArrow)
        {
            if ((key.Modifiers & ConsoleModifiers.Alt) != 0)
            {
                // Alt+Left (Option/Win key): move to previous word boundary
                _cursorColumn = FindPreviousWordBoundary(currentLine, _cursorColumn);
            }
            else
            {
                // Left: move one character left
                if (_cursorColumn > 0) _cursorColumn--;
            }
            return true;
        }
        if (key.Key == ConsoleKey.RightArrow)
        {
            if ((key.Modifiers & ConsoleModifiers.Alt) != 0)
            {
                // Alt+Right (Option/Win key): move to next word boundary
                _cursorColumn = FindNextWordBoundary(currentLine, _cursorColumn);
            }
            else
            {
                // Right: move one character right
                if (_cursorColumn < currentLine.Length) _cursorColumn++;
            }
            return true;
        }
        if (key.Key == ConsoleKey.UpArrow)
        {
            // Up: move to previous line, keeping column position if possible
            if (_cursorLineIndex > 0)
            {
                _cursorLineIndex--;
                string newLine = _lines[_cursorLineIndex];
                // Clamp cursor column to new line length
                if (_cursorColumn > newLine.Length)
                    _cursorColumn = newLine.Length;
            }
            return true;
        }
        if (key.Key == ConsoleKey.DownArrow)
        {
            // Down: move to next line, keeping column position if possible
            if (_cursorLineIndex < _lines.Count - 1)
            {
                _cursorLineIndex++;
                string newLine = _lines[_cursorLineIndex];
                // Clamp cursor column to new line length
                if (_cursorColumn > newLine.Length)
                    _cursorColumn = newLine.Length;
            }
            return true;
        }

        // Emacs-style word navigation (Alt+B/F - used by macOS Terminal)
        if (key.Key == ConsoleKey.B && (key.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            _cursorColumn = FindPreviousWordBoundary(currentLine, _cursorColumn);
            return true;
        }
        if (key.Key == ConsoleKey.F && (key.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            _cursorColumn = FindNextWordBoundary(currentLine, _cursorColumn);
            return true;
        }

        // Emacs-style line navigation
        if (key.Key == ConsoleKey.A && (key.Modifiers & ConsoleModifiers.Control) != 0)
        {
            _cursorColumn = 0;
            return true;
        }
        if (key.Key == ConsoleKey.E && (key.Modifiers & ConsoleModifiers.Control) != 0)
        {
            _cursorColumn = currentLine.Length;
            return true;
        }
        if (key.Key == ConsoleKey.Home)
        {
            _cursorColumn = 0;
            return true;
        }
        if (key.Key == ConsoleKey.End)
        {
            _cursorColumn = currentLine.Length;
            return true;
        }

        // Ctrl+J (which arrives as Ctrl+Enter): new line (split current line at cursor position)
        if (key.Key == ConsoleKey.Enter && (key.Modifiers & ConsoleModifiers.Control) != 0)
        {
            // Split the current line at cursor position
            string beforeCursor = currentLine.Substring(0, _cursorColumn);
            string afterCursor = currentLine.Substring(_cursorColumn);

            // Update current line to keep only the part before cursor
            _lines[_cursorLineIndex] = beforeCursor;

            // Insert new line with the part after cursor
            _cursorLineIndex++;
            _lines.Insert(_cursorLineIndex, afterCursor);
            _cursorColumn = 0;
            return true;
        }

        // Enter: submit input
        if (key.Key == ConsoleKey.Enter)
        {
            SubmitInput();
            return true;
        }

        // Backspace
        if (key.Key == ConsoleKey.Backspace)
        {
            if (_cursorColumn > 0)
            {
                // Delete character before cursor
                _lines[_cursorLineIndex] = currentLine.Remove(_cursorColumn - 1, 1);
                _cursorColumn--;
            }
            else if (_cursorLineIndex > 0)
            {
                // At start of line, merge with previous line
                string remainingText = _lines[_cursorLineIndex];
                _lines.RemoveAt(_cursorLineIndex);
                _cursorLineIndex--;
                _cursorColumn = _lines[_cursorLineIndex].Length;
                _lines[_cursorLineIndex] += remainingText;
            }
            return true;
        }

        // Delete
        if (key.Key == ConsoleKey.Delete)
        {
            if (_cursorColumn < currentLine.Length)
            {
                // Delete character at cursor
                _lines[_cursorLineIndex] = currentLine.Remove(_cursorColumn, 1);
            }
            else if (_cursorLineIndex < _lines.Count - 1)
            {
                // At end of line, merge next line with current
                _lines[_cursorLineIndex] += _lines[_cursorLineIndex + 1];
                _lines.RemoveAt(_cursorLineIndex + 1);
            }
            return true;
        }

        // Regular character input
        if (!char.IsControl(key.KeyChar))
        {
            // Insert character at cursor position
            _lines[_cursorLineIndex] = currentLine.Insert(_cursorColumn, key.KeyChar.ToString());
            _cursorColumn++;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Clears the input and resets to initial state.
    /// </summary>
    public void Clear()
    {
        _lines = new List<string> { string.Empty };
        _cursorLineIndex = 0;
        _cursorColumn = 0;
    }

    /// <summary>
    /// Gets the current input as a single string with newlines.
    /// </summary>
    public string GetText()
    {
        return string.Join("\n", _lines);
    }

    /// <summary>
    /// Sets the content of a specific line and updates cursor position.
    /// </summary>
    public void SetLine(int lineIndex, string content, int newCursorColumn)
    {
        if (lineIndex < 0 || lineIndex >= _lines.Count)
            return;

        _lines[lineIndex] = content;
        _cursorColumn = Math.Max(0, Math.Min(newCursorColumn, content.Length));
    }

    private void SubmitInput()
    {
        if (_lines.Count == 0) return;
        var combined = string.Join("\n", _lines).TrimEnd();
        if (combined.Length > 0)
        {
            OnSubmit?.Invoke(combined);
        }
        Clear();
    }

    private static int FindPreviousWordBoundary(string text, int position)
    {
        if (position <= 0) return 0;

        // Skip whitespace backwards
        int pos = position - 1;
        while (pos > 0 && char.IsWhiteSpace(text[pos]))
            pos--;

        // Skip word characters backwards
        while (pos > 0 && !char.IsWhiteSpace(text[pos - 1]))
            pos--;

        return pos;
    }

    private static int FindNextWordBoundary(string text, int position)
    {
        if (position >= text.Length) return text.Length;

        int pos = position;

        // Skip current word
        while (pos < text.Length && !char.IsWhiteSpace(text[pos]))
            pos++;

        // Skip whitespace
        while (pos < text.Length && char.IsWhiteSpace(text[pos]))
            pos++;

        return pos;
    }
}
