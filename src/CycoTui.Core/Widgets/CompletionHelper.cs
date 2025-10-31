using System;
using System.Collections.Generic;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Helper methods for detecting and managing completion triggers.
/// </summary>
public static class CompletionHelper
{
    /// <summary>
    /// Detects if there's an active completion trigger at the cursor position.
    /// Searches backwards for any of the specified trigger characters.
    /// Returns the query string after the trigger if found, otherwise null.
    /// </summary>
    /// <param name="line">The current input line</param>
    /// <param name="cursorColumn">The cursor position in the line</param>
    /// <param name="triggerChars">Set of characters that can trigger completion</param>
    /// <param name="triggerColumn">Output: column where trigger was found</param>
    /// <param name="foundTriggerChar">Output: the trigger character that was found</param>
    /// <returns>Query string after trigger, or null if no active trigger</returns>
    public static string? DetectCompletionQuery(
        string line,
        int cursorColumn,
        IEnumerable<char> triggerChars,
        out int triggerColumn,
        out char foundTriggerChar)
    {
        triggerColumn = -1;
        foundTriggerChar = '\0';

        if (string.IsNullOrEmpty(line) || cursorColumn <= 0)
            return null;

        var triggerSet = new HashSet<char>(triggerChars);

        // Look backwards from cursor to find any trigger character
        // Stop at whitespace or start of line
        for (int i = cursorColumn - 1; i >= 0; i--)
        {
            char c = line[i];

            if (triggerSet.Contains(c))
            {
                // Found trigger - extract query from trigger to cursor
                triggerColumn = i;
                foundTriggerChar = c;
                int queryStart = i + 1;
                int queryLength = cursorColumn - queryStart;
                return queryLength > 0 ? line.Substring(queryStart, queryLength) : string.Empty;
            }

            // Stop searching if we hit whitespace
            if (char.IsWhiteSpace(c))
            {
                break;
            }
        }

        return null;
    }

    /// <summary>
    /// Detects if there's an active completion trigger at the cursor position (single trigger version).
    /// </summary>
    [Obsolete("Use the overload that accepts multiple trigger characters")]
    public static string? DetectCompletionQuery(string line, int cursorColumn, out int triggerColumn)
    {
        return DetectCompletionQuery(line, cursorColumn, new[] { '@' }, out triggerColumn, out _);
    }

    /// <summary>
    /// Replaces the completion query (from trigger to cursor) with the selected item.
    /// </summary>
    /// <param name="line">The current input line</param>
    /// <param name="triggerColumn">Column where trigger character appears</param>
    /// <param name="cursorColumn">Current cursor position</param>
    /// <param name="triggerChar">The trigger character to use (e.g., '@', '#', '/')</param>
    /// <param name="selectedItem">The selected item to insert</param>
    /// <param name="newCursorColumn">Output: new cursor position after insertion</param>
    /// <returns>The modified line with selected item inserted</returns>
    public static string InsertCompletion(
        string line,
        int triggerColumn,
        int cursorColumn,
        char triggerChar,
        string selectedItem,
        out int newCursorColumn)
    {
        if (triggerColumn < 0 || triggerColumn >= line.Length || line[triggerColumn] != triggerChar)
        {
            newCursorColumn = cursorColumn;
            return line;
        }

        // Remove from trigger to cursor (inclusive of trigger)
        string before = line.Substring(0, triggerColumn);
        string after = cursorColumn < line.Length ? line.Substring(cursorColumn) : string.Empty;

        // Insert selected item with trigger prefix
        string insertion = triggerChar + selectedItem;
        string newLine = before + insertion + after;

        newCursorColumn = before.Length + insertion.Length;
        return newLine;
    }

    /// <summary>
    /// Replaces the completion query with selected item (deprecated single-trigger version).
    /// </summary>
    [Obsolete("Use the overload that accepts triggerChar parameter")]
    public static string InsertCompletion(
        string line,
        int triggerColumn,
        int cursorColumn,
        string selectedFile,
        out int newCursorColumn)
    {
        return InsertCompletion(line, triggerColumn, cursorColumn, '@', selectedFile, out newCursorColumn);
    }
}
