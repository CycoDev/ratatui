namespace CycoAI.CycoTui.Examples.AutoComplete.Models;

/// <summary>
/// Represents the current state of the command prompt and auto-completion.
/// </summary>
public class CommandState
{
    /// <summary>
    /// The current input text being typed by the user.
    /// </summary>
    public string InputText { get; set; } = string.Empty;

    /// <summary>
    /// The current cursor position within the input text.
    /// </summary>
    public int CursorPosition { get; set; } = 0;

    /// <summary>
    /// Whether auto-completion is currently active (dropdown is shown).
    /// </summary>
    public bool IsCompletionActive { get; set; } = false;

    /// <summary>
    /// The position in the input text where the '@' trigger was found.
    /// </summary>
    public int CompletionTriggerPosition { get; set; } = -1;

    /// <summary>
    /// The currently selected completion item index.
    /// </summary>
    public int SelectedCompletionIndex { get; set; } = 0;

    /// <summary>
    /// History of previously entered commands.
    /// </summary>
    public List<string> CommandHistory { get; } = new();

    /// <summary>
    /// Current position in command history (for up/down arrow navigation).
    /// </summary>
    public int HistoryPosition { get; set; } = -1;

    /// <summary>
    /// Gets the text after the '@' trigger for filtering completions.
    /// </summary>
    public string CompletionFilter
    {
        get
        {
            if (!IsCompletionActive || CompletionTriggerPosition < 0)
                return string.Empty;

            var startPos = CompletionTriggerPosition + 1; // Skip the '@' character
            if (startPos >= InputText.Length)
                return string.Empty;

            var endPos = Math.Min(CursorPosition, InputText.Length);
            if (endPos <= startPos)
                return string.Empty;

            return InputText[startPos..endPos];
        }
    }

    /// <summary>
    /// Adds a command to the history.
    /// </summary>
    /// <param name="command">The command to add.</param>
    public void AddToHistory(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return;

        // Remove duplicate if it exists
        CommandHistory.Remove(command);

        // Add to the end
        CommandHistory.Add(command);

        // Keep only last 100 commands
        if (CommandHistory.Count > 100)
            CommandHistory.RemoveAt(0);

        // Reset history position
        HistoryPosition = -1;
    }

    /// <summary>
    /// Gets the previous command from history.
    /// </summary>
    /// <returns>The previous command, or null if none available.</returns>
    public string? GetPreviousHistoryCommand()
    {
        if (CommandHistory.Count == 0)
            return null;

        if (HistoryPosition == -1)
            HistoryPosition = CommandHistory.Count - 1;
        else if (HistoryPosition > 0)
            HistoryPosition--;

        return CommandHistory[HistoryPosition];
    }

    /// <summary>
    /// Gets the next command from history.
    /// </summary>
    /// <returns>The next command, or null if none available.</returns>
    public string? GetNextHistoryCommand()
    {
        if (CommandHistory.Count == 0 || HistoryPosition == -1)
            return null;

        if (HistoryPosition < CommandHistory.Count - 1)
            HistoryPosition++;
        else
        {
            HistoryPosition = -1;
            return string.Empty; // Return empty to clear current input
        }

        return CommandHistory[HistoryPosition];
    }

    /// <summary>
    /// Resets the command state to initial values.
    /// </summary>
    public void Reset()
    {
        InputText = string.Empty;
        CursorPosition = 0;
        IsCompletionActive = false;
        CompletionTriggerPosition = -1;
        SelectedCompletionIndex = 0;
        HistoryPosition = -1;
    }

    /// <summary>
    /// Activates auto-completion at the current cursor position.
    /// </summary>
    public void ActivateCompletion()
    {
        // Find the '@' character at or before the cursor position
        var atPosition = InputText.LastIndexOf('@', Math.Max(0, CursorPosition - 1));
        if (atPosition >= 0)
        {
            IsCompletionActive = true;
            CompletionTriggerPosition = atPosition;
            SelectedCompletionIndex = 0;
        }
    }

    /// <summary>
    /// Deactivates auto-completion.
    /// </summary>
    public void DeactivateCompletion()
    {
        IsCompletionActive = false;
        CompletionTriggerPosition = -1;
        SelectedCompletionIndex = 0;
    }

    /// <summary>
    /// Inserts a completion item into the input text.
    /// </summary>
    /// <param name="completion">The completion text to insert.</param>
    public void InsertCompletion(string completion)
    {
        if (!IsCompletionActive || CompletionTriggerPosition < 0)
            return;

        // Replace from '@' position to cursor position with the completion
        var beforeAt = InputText[..CompletionTriggerPosition];
        var afterCursor = InputText[CursorPosition..];

        InputText = beforeAt + completion + afterCursor;
        CursorPosition = beforeAt.Length + completion.Length;

        DeactivateCompletion();
    }
}