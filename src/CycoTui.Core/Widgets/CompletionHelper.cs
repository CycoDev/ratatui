using System;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Helper methods for detecting and managing file completion triggers.
/// </summary>
public static class CompletionHelper
{
    /// <summary>
    /// Detects if there's an active '@' completion trigger at the cursor position.
    /// Returns the query string after '@' if found, otherwise null.
    /// </summary>
    /// <param name="line">The current input line</param>
    /// <param name="cursorColumn">The cursor position in the line</param>
    /// <param name="triggerColumn">Output: column where '@' was found</param>
    /// <returns>Query string after '@', or null if no active trigger</returns>
    public static string? DetectCompletionQuery(string line, int cursorColumn, out int triggerColumn)
    {
        triggerColumn = -1;

        if (string.IsNullOrEmpty(line) || cursorColumn <= 0)
            return null;

        // Look backwards from cursor to find '@'
        // Stop at whitespace or start of line
        for (int i = cursorColumn - 1; i >= 0; i--)
        {
            char c = line[i];

            if (c == '@')
            {
                // Found '@' - extract query from '@' to cursor
                triggerColumn = i;
                int queryStart = i + 1;
                int queryLength = cursorColumn - queryStart;
                return queryLength > 0 ? line.Substring(queryStart, queryLength) : string.Empty;
            }

            // Stop searching if we hit whitespace (except if immediately after '@')
            if (char.IsWhiteSpace(c))
            {
                break;
            }
        }

        return null;
    }

    /// <summary>
    /// Replaces the completion query (from '@' to cursor) with the selected file path.
    /// </summary>
    /// <param name="line">The current input line</param>
    /// <param name="triggerColumn">Column where '@' appears</param>
    /// <param name="cursorColumn">Current cursor position</param>
    /// <param name="selectedFile">The file path to insert</param>
    /// <param name="newCursorColumn">Output: new cursor position after insertion</param>
    /// <returns>The modified line with file path inserted</returns>
    public static string InsertCompletion(
        string line,
        int triggerColumn,
        int cursorColumn,
        string selectedFile,
        out int newCursorColumn)
    {
        if (triggerColumn < 0 || triggerColumn >= line.Length || line[triggerColumn] != '@')
        {
            newCursorColumn = cursorColumn;
            return line;
        }

        // Remove from '@' to cursor (inclusive of '@')
        string before = line.Substring(0, triggerColumn);
        string after = cursorColumn < line.Length ? line.Substring(cursorColumn) : string.Empty;

        // Insert file reference with '@' prefix
        string insertion = "@" + selectedFile;
        string newLine = before + insertion + after;

        newCursorColumn = before.Length + insertion.Length;
        return newLine;
    }
}
