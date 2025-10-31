using System.Collections.Generic;
using System.Linq;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Delegate for providing completion items when completion is triggered.
/// </summary>
/// <returns>List of items to show in completion popup (e.g., file paths)</returns>
public delegate IReadOnlyList<string> CompletionItemsProvider();

/// <summary>
/// State for file completion popup triggered by '@' character.
/// Tracks whether completion is active, the search query, matched files, and selection.
/// </summary>
public sealed record FileCompletionState
{
    /// <summary>
    /// Whether the completion popup is currently active/visible.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// The search query (text typed after '@').
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// All files in the workspace (cached when '@' is first pressed).
    /// </summary>
    public IReadOnlyList<string> AllFiles { get; init; } = new List<string>();

    /// <summary>
    /// Files matching the current query (filtered from AllFiles).
    /// </summary>
    public IReadOnlyList<string> MatchedFiles { get; init; } = new List<string>();

    /// <summary>
    /// Index of the currently selected/highlighted file in MatchedFiles.
    /// </summary>
    public int SelectedIndex { get; init; }

    /// <summary>
    /// The line index where '@' was typed (for later insertion).
    /// </summary>
    public int TriggerLineIndex { get; init; }

    /// <summary>
    /// The column index where '@' was typed (for later insertion).
    /// </summary>
    public int TriggerColumn { get; init; }

    /// <summary>
    /// Creates a new inactive completion state.
    /// </summary>
    public static FileCompletionState CreateInactive() => new();

    /// <summary>
    /// Activates completion mode with the given file list and trigger position.
    /// </summary>
    public FileCompletionState Activate(IReadOnlyList<string> allFiles, int lineIndex, int column)
    {
        return new FileCompletionState
        {
            IsActive = true,
            Query = string.Empty,
            AllFiles = allFiles,
            MatchedFiles = allFiles, // Initially show all files
            SelectedIndex = 0,
            TriggerLineIndex = lineIndex,
            TriggerColumn = column
        };
    }

    /// <summary>
    /// Updates the query and re-filters matched files.
    /// </summary>
    public FileCompletionState UpdateQuery(string query)
    {
        if (!IsActive) return this;

        var filtered = FilterFiles(AllFiles, query);
        return new FileCompletionState
        {
            IsActive = true,
            Query = query,
            AllFiles = AllFiles,
            MatchedFiles = filtered,
            SelectedIndex = System.Math.Min(SelectedIndex, System.Math.Max(0, filtered.Count - 1)),
            TriggerLineIndex = TriggerLineIndex,
            TriggerColumn = TriggerColumn
        };
    }

    /// <summary>
    /// Moves selection up by one (wraps to bottom).
    /// </summary>
    public FileCompletionState SelectPrevious()
    {
        if (!IsActive || MatchedFiles.Count == 0) return this;

        int newIndex = SelectedIndex - 1;
        if (newIndex < 0) newIndex = MatchedFiles.Count - 1;

        return this with { SelectedIndex = newIndex };
    }

    /// <summary>
    /// Moves selection down by one (wraps to top).
    /// </summary>
    public FileCompletionState SelectNext()
    {
        if (!IsActive || MatchedFiles.Count == 0) return this;

        int newIndex = (SelectedIndex + 1) % MatchedFiles.Count;
        return this with { SelectedIndex = newIndex };
    }

    /// <summary>
    /// Gets the currently selected file path, or null if none.
    /// </summary>
    public string? GetSelectedFile()
    {
        if (!IsActive || MatchedFiles.Count == 0) return null;
        if (SelectedIndex < 0 || SelectedIndex >= MatchedFiles.Count) return null;
        return MatchedFiles[SelectedIndex];
    }

    /// <summary>
    /// Deactivates completion mode.
    /// </summary>
    public FileCompletionState Deactivate()
    {
        return CreateInactive();
    }

    /// <summary>
    /// Filters files based on the query (case-insensitive substring match).
    /// </summary>
    private static IReadOnlyList<string> FilterFiles(IReadOnlyList<string> files, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return files;

        var lowerQuery = query.ToLowerInvariant();
        return files
            .Where(f => f.ToLowerInvariant().Contains(lowerQuery))
            .ToList();
    }
}
