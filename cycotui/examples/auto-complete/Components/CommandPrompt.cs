using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Events;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Widgets;
using CycoAI.CycoTui.Examples.AutoComplete.Models;

namespace CycoAI.CycoTui.Examples.AutoComplete.Components;

/// <summary>
/// A command prompt widget that supports text input and auto-completion.
/// </summary>
public class CommandPrompt : IWidget
{
    private readonly CommandState _state = new();
    private readonly FileSystemExplorer _fileExplorer = new();
    private FileCompletionDropdown? _dropdown;

    /// <summary>
    /// Gets the current command state.
    /// </summary>
    public CommandState State => _state;

    /// <summary>
    /// Event raised when a command is entered.
    /// </summary>
    public event System.EventHandler<string>? CommandEntered;

    /// <summary>
    /// Event raised when the application should exit.
    /// </summary>
    public event System.EventHandler? ExitRequested;

    /// <summary>
    /// Initializes a new instance of the CommandPrompt class.
    /// </summary>
    public CommandPrompt()
    {
        _dropdown = new FileCompletionDropdown(_state, _fileExplorer);
    }

    /// <summary>
    /// Renders the command prompt to the buffer.
    /// </summary>
    /// <param name="area">The area to render within.</param>
    /// <param name="buffer">The buffer to render to.</param>
    public void Render(Rect area, IBuffer buffer)
    {
        // Clear the area
        var clearCell = new Cell(' ', Color.White, Color.Black, Modifier.None);
        for (int y = area.Y; y < area.Y + area.Height; y++)
        {
            for (int x = area.X; x < area.X + area.Width; x++)
            {
                buffer[x, y] = clearCell;
            }
        }

        // Render the prompt and input text
        RenderPromptLine(area, buffer);

        // Render the dropdown if active
        if (_state.IsCompletionActive && _dropdown != null)
        {
            var dropdownArea = CalculateDropdownArea(area);
            _dropdown.Render(dropdownArea, buffer);
        }

        // Render status line at the bottom
        RenderStatusLine(area, buffer);
    }

    /// <summary>
    /// Handles keyboard input for the command prompt.
    /// </summary>
    /// <param name="keyEvent">The keyboard event.</param>
    /// <returns>True if the event was handled.</returns>
    public bool HandleKeyInput(KeyboardEvent keyEvent)
    {
        // Handle dropdown navigation if dropdown is active
        if (_state.IsCompletionActive && _dropdown?.HandleKeyInput(keyEvent) == true)
        {
            return true;
        }

        // Handle regular input
        return HandleRegularInput(keyEvent);
    }

    private void RenderPromptLine(Rect area, IBuffer buffer)
    {
        var promptCell = new Cell('>', Color.Green, Color.Black, Modifier.None);
        var inputCell = new Cell(' ', Color.White, Color.Black, Modifier.None);
        var cursorCell = new Cell(' ', Color.Black, Color.White, Modifier.None);

        // Render the prompt symbol
        if (area.Width > 0)
        {
            buffer[area.X, area.Y] = promptCell;
        }

        if (area.Width > 1)
        {
            buffer[area.X + 1, area.Y] = inputCell;
        }

        // Render the input text
        var inputStartX = area.X + 2;
        var availableWidth = Math.Max(0, area.Width - 2);
        var visibleText = GetVisibleText(availableWidth);

        for (int i = 0; i < visibleText.Length && i < availableWidth; i++)
        {
            var x = inputStartX + i;
            if (x < area.X + area.Width)
            {
                var cell = (i == GetVisibleCursorPosition(availableWidth)) ? cursorCell.WithCharacter(visibleText[i]) : inputCell.WithCharacter(visibleText[i]);
                buffer[x, area.Y] = cell;
            }
        }

        // Render cursor if it's at the end
        var cursorPos = GetVisibleCursorPosition(availableWidth);
        if (cursorPos == visibleText.Length && cursorPos < availableWidth)
        {
            var x = inputStartX + cursorPos;
            if (x < area.X + area.Width)
            {
                buffer[x, area.Y] = cursorCell;
            }
        }
    }

    private void RenderStatusLine(Rect area, IBuffer buffer)
    {
        if (area.Height < 2)
            return;

        var statusY = area.Y + area.Height - 1;
        var statusCell = new Cell(' ', Color.Gray, Color.Black, Modifier.None);

        // Create status text
        var statusText = _state.IsCompletionActive
            ? "Arrow keys: navigate, Enter: select, Escape: cancel, Ctrl+C: exit"
            : "Type '@' for file completion, Ctrl+C: exit, 'exit': quit";

        // Render status text (truncated if necessary)
        for (int i = 0; i < Math.Min(statusText.Length, area.Width); i++)
        {
            buffer[area.X + i, statusY] = statusCell.WithCharacter(statusText[i]);
        }
    }

    private Rect CalculateDropdownArea(Rect area)
    {
        // Position dropdown below the prompt line
        var dropdownY = area.Y + 1;
        var dropdownHeight = Math.Min(10, area.Height - 2); // Max 10 items, leave room for status
        var dropdownWidth = Math.Min(60, area.Width); // Max 60 chars wide

        return new Rect(area.X, dropdownY, dropdownWidth, dropdownHeight);
    }

    private string GetVisibleText(int availableWidth)
    {
        if (string.IsNullOrEmpty(_state.InputText))
            return string.Empty;

        // Simple scrolling: if cursor is beyond visible area, scroll
        var startPos = Math.Max(0, _state.CursorPosition - availableWidth + 1);
        var length = Math.Min(_state.InputText.Length - startPos, availableWidth);

        return _state.InputText.Substring(startPos, Math.Max(0, length));
    }

    private int GetVisibleCursorPosition(int availableWidth)
    {
        var startPos = Math.Max(0, _state.CursorPosition - availableWidth + 1);
        return _state.CursorPosition - startPos;
    }

    private bool HandleRegularInput(KeyboardEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case KeyCode.Enter:
                HandleEnterKey();
                return true;

            case KeyCode.Backspace:
                HandleBackspace();
                return true;

            case KeyCode.Delete:
                HandleDelete();
                return true;

            case KeyCode.Left:
                HandleLeftArrow();
                return true;

            case KeyCode.Right:
                HandleRightArrow();
                return true;

            case KeyCode.Up:
                HandleUpArrow();
                return true;

            case KeyCode.Down:
                HandleDownArrow();
                return true;

            case KeyCode.Escape:
                HandleEscape();
                return true;

            case KeyCode.Tab:
                // Tab could be used for completion in the future
                return true;

            default:
                // Handle character input
                if (keyEvent.Character.HasValue)
                {
                    HandleCharacterInput(keyEvent.Character.Value);
                    return true;
                }
                break;
        }

        return false;
    }

    private void HandleEnterKey()
    {
        var command = _state.InputText.Trim();

        if (command.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            command.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Add to history and raise event
        if (!string.IsNullOrWhiteSpace(command))
        {
            _state.AddToHistory(command);
            CommandEntered?.Invoke(this, command);
        }

        // Reset for next command
        _state.Reset();
    }

    private void HandleBackspace()
    {
        if (_state.CursorPosition > 0)
        {
            _state.InputText = _state.InputText.Remove(_state.CursorPosition - 1, 1);
            _state.CursorPosition--;

            // Check if we need to deactivate completion
            CheckCompletionTrigger();
        }
    }

    private void HandleDelete()
    {
        if (_state.CursorPosition < _state.InputText.Length)
        {
            _state.InputText = _state.InputText.Remove(_state.CursorPosition, 1);

            // Check if we need to deactivate completion
            CheckCompletionTrigger();
        }
    }

    private void HandleLeftArrow()
    {
        if (_state.CursorPosition > 0)
        {
            _state.CursorPosition--;
        }
    }

    private void HandleRightArrow()
    {
        if (_state.CursorPosition < _state.InputText.Length)
        {
            _state.CursorPosition++;
        }
    }

    private void HandleUpArrow()
    {
        var previousCommand = _state.GetPreviousHistoryCommand();
        if (previousCommand != null)
        {
            _state.InputText = previousCommand;
            _state.CursorPosition = _state.InputText.Length;
            _state.DeactivateCompletion();
        }
    }

    private void HandleDownArrow()
    {
        var nextCommand = _state.GetNextHistoryCommand();
        if (nextCommand != null)
        {
            _state.InputText = nextCommand;
            _state.CursorPosition = _state.InputText.Length;
            _state.DeactivateCompletion();
        }
    }

    private void HandleEscape()
    {
        if (_state.IsCompletionActive)
        {
            _state.DeactivateCompletion();
        }
        else
        {
            // Clear current input
            _state.Reset();
        }
    }

    private void HandleCharacterInput(char character)
    {
        // Insert character at cursor position
        _state.InputText = _state.InputText.Insert(_state.CursorPosition, character.ToString());
        _state.CursorPosition++;

        // Check for completion trigger
        CheckCompletionTrigger();
    }

    private void CheckCompletionTrigger()
    {
        // Look for '@' character at or before cursor position
        var atPosition = _state.InputText.LastIndexOf('@', Math.Max(0, _state.CursorPosition - 1));

        if (atPosition >= 0)
        {
            // Activate completion
            _state.ActivateCompletion();
        }
        else if (_state.IsCompletionActive)
        {
            // Deactivate completion if '@' is no longer present
            _state.DeactivateCompletion();
        }
    }
}

// Placeholder for KeyboardEvent and KeyCode - these should be defined in the CycoTui.Core events system
public class KeyboardEvent
{
    public KeyCode Key { get; set; }
    public char? Character { get; set; }
    public bool CtrlPressed { get; set; }
    public bool AltPressed { get; set; }
    public bool ShiftPressed { get; set; }
}

public enum KeyCode
{
    Enter,
    Backspace,
    Delete,
    Left,
    Right,
    Up,
    Down,
    Escape,
    Tab,
    Space,
    Character
}