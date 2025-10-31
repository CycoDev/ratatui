using System.Collections.Generic;
using System.Linq;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Delegate for providing completion items when completion is triggered.
/// </summary>
/// <returns>List of items to show in completion popup (e.g., file paths)</returns>
public delegate IReadOnlyList<string> CompletionItemsProvider();

/// <summary>
/// State for completion popup triggered by a character (e.g., '@' for files, '#' for tags).
/// Tracks whether completion is active, the search query, matched items, and selection.
/// </summary>
public sealed record CompletionState
{
    /// <summary>
    /// Whether the completion popup is currently active/visible.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// The search query (text typed after trigger character).
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// All items available for completion (cached when trigger is first activated).
    /// </summary>
    public IReadOnlyList<string> AllItems { get; init; } = new List<string>();

    /// <summary>
    /// Items matching the current query (filtered from AllItems).
    /// </summary>
    public IReadOnlyList<string> MatchedItems { get; init; } = new List<string>();

    /// <summary>
    /// Index of the currently selected/highlighted item in MatchedItems.
    /// </summary>
    public int SelectedIndex { get; init; }

    /// <summary>
    /// The line index where trigger character was typed (for later insertion).
    /// </summary>
    public int TriggerLineIndex { get; init; }

    /// <summary>
    /// The column index where trigger character was typed (for later insertion).
    /// </summary>
    public int TriggerColumn { get; init; }

    /// <summary>
    /// Creates a new inactive completion state.
    /// </summary>
    public static CompletionState CreateInactive() => new();

    /// <summary>
    /// Activates completion mode with the given items and trigger position.
    /// </summary>
    public CompletionState Activate(IReadOnlyList<string> allItems, int lineIndex, int column)
    {
        return new CompletionState
        {
            IsActive = true,
            Query = string.Empty,
            AllItems = allItems,
            MatchedItems = allItems, // Initially show all items
            SelectedIndex = 0,
            TriggerLineIndex = lineIndex,
            TriggerColumn = column
        };
    }

    /// <summary>
    /// Updates the query and re-filters matched items.
    /// </summary>
    public CompletionState UpdateQuery(string query)
    {
        if (!IsActive) return this;

        var filtered = FilterItems(AllItems, query);
        return new CompletionState
        {
            IsActive = true,
            Query = query,
            AllItems = AllItems,
            MatchedItems = filtered,
            SelectedIndex = System.Math.Min(SelectedIndex, System.Math.Max(0, filtered.Count - 1)),
            TriggerLineIndex = TriggerLineIndex,
            TriggerColumn = TriggerColumn
        };
    }

    /// <summary>
    /// Moves selection up by one (wraps to bottom).
    /// </summary>
    public CompletionState SelectPrevious()
    {
        if (!IsActive || MatchedItems.Count == 0) return this;

        int newIndex = SelectedIndex - 1;
        if (newIndex < 0) newIndex = MatchedItems.Count - 1;

        return this with { SelectedIndex = newIndex };
    }

    /// <summary>
    /// Moves selection down by one (wraps to top).
    /// </summary>
    public CompletionState SelectNext()
    {
        if (!IsActive || MatchedItems.Count == 0) return this;

        int newIndex = (SelectedIndex + 1) % MatchedItems.Count;
        return this with { SelectedIndex = newIndex };
    }

    /// <summary>
    /// Gets the currently selected item, or null if none.
    /// </summary>
    public string? GetSelectedItem()
    {
        if (!IsActive || MatchedItems.Count == 0) return null;
        if (SelectedIndex < 0 || SelectedIndex >= MatchedItems.Count) return null;
        return MatchedItems[SelectedIndex];
    }

    /// <summary>
    /// Deactivates completion mode.
    /// </summary>
    public CompletionState Deactivate()
    {
        return CreateInactive();
    }

    /// <summary>
    /// Filters items based on the query (case-insensitive substring match).
    /// </summary>
    private static IReadOnlyList<string> FilterItems(IReadOnlyList<string> items, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return items;

        var lowerQuery = query.ToLowerInvariant();
        return items
            .Where(f => f.ToLowerInvariant().Contains(lowerQuery))
            .ToList();
    }
}
