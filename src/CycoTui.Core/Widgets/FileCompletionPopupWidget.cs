using System;
using System.Collections.Generic;
using System.Linq;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Renders a file completion popup overlay with a bordered list of matching files.
/// Designed to appear above the input area when '@' is pressed.
/// </summary>
public sealed class FileCompletionPopupWidget : IStatefulWidget<FileCompletionState>
{
    public int MaxVisibleItems { get; init; } = 10;
    public StyleType BorderStyle { get; init; } = StyleType.Empty;
    public StyleType TitleStyle { get; init; } = StyleType.Empty.Add(TextModifier.Bold);
    public StyleType SelectedStyle { get; init; } = StyleType.Empty.Add(TextModifier.Invert);
    public StyleType ItemStyle { get; init; } = StyleType.Empty;

    public static FileCompletionPopupWidget Create() => new();

    public FileCompletionPopupWidget WithMaxVisibleItems(int max) => new()
    {
        MaxVisibleItems = max,
        BorderStyle = BorderStyle,
        TitleStyle = TitleStyle,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle
    };

    public FileCompletionPopupWidget WithStyles(
        StyleType border,
        StyleType title,
        StyleType selected,
        StyleType item) => new()
    {
        MaxVisibleItems = MaxVisibleItems,
        BorderStyle = border,
        TitleStyle = title,
        SelectedStyle = selected,
        ItemStyle = item
    };

    /// <summary>
    /// Calculates the required rectangle for the popup based on matched files.
    /// Width is determined by the longest file path + border + prefix.
    /// Height is capped by MaxVisibleItems + border.
    /// </summary>
    public Rect CalculatePopupRect(Rect inputArea, FileCompletionState state)
    {
        if (!state.IsActive || state.MatchedFiles.Count == 0)
        {
            // Return minimal rect when nothing to show
            return new Rect(inputArea.X, inputArea.Y - 3, 20, 3);
        }

        // Calculate width from longest file path
        int longestPath = state.MatchedFiles.Max(f => f.Length);
        int selectionPrefixWidth = 2; // "> " or "  "
        int borderWidth = 2; // left + right border
        int totalWidth = longestPath + selectionPrefixWidth + borderWidth;

        // Clamp to reasonable bounds (minimum 20, maximum based on terminal width)
        totalWidth = Math.Max(20, totalWidth);
        totalWidth = Math.Min(totalWidth, 120); // Reasonable max width

        // Calculate height from number of items
        int visibleItems = Math.Min(state.MatchedFiles.Count, MaxVisibleItems);
        int borderHeight = 2; // top + bottom border
        int totalHeight = visibleItems + borderHeight;

        // Position popup ABOVE the input area
        int popupX = inputArea.X;
        int popupY = inputArea.Y - totalHeight;

        // Ensure popup doesn't go off-screen (clamp to Y >= 0)
        if (popupY < 0)
        {
            popupY = 0;
            // Recalculate height to fit available space
            totalHeight = Math.Min(totalHeight, inputArea.Y);
        }

        return new Rect(popupX, popupY, totalWidth, totalHeight);
    }

    public void Render(Frame frame, Rect area, FileCompletionState state)
    {
        if (!state.IsActive || area.Width < 4 || area.Height < 3)
            return;

        // Create title with match count
        string title = state.MatchedFiles.Count == 0
            ? "No matches"
            : state.Query.Length == 0
                ? $"Files ({state.MatchedFiles.Count})"
                : $"Files: @{state.Query} ({state.MatchedFiles.Count})";

        // Render border with title
        var block = Block.Create()
            .WithTitle(title, TitleStyle)
            .WithBorder(BlockBorderStyle.SingleLine, BorderStyle);

        block.Render(frame, area);

        // Get inner area for list
        var innerArea = Block.GetInnerContentRect(area, Padding.Zero);
        if (innerArea.Width <= 0 || innerArea.Height <= 0)
            return;

        // No items to show
        if (state.MatchedFiles.Count == 0)
        {
            var noMatchText = state.Query.Length == 0 ? "Type to search..." : "No files found";
            frame.WriteString(innerArea.X, innerArea.Y, noMatchText, ItemStyle);
            return;
        }

        // Create ListItems from matched files
        var listItems = state.MatchedFiles
            .Select(f => new ListItem(f, ItemStyle))
            .ToList();

        // Create ListState with current selection
        var listState = new ListState(listItems.Count);
        listState.Select(state.SelectedIndex, innerArea.Height);

        // Render list widget
        var listWidget = ListWidget.Create()
            .WithItems(listItems)
            .WithFocused(true)
            .WithSelectionPrefix("> ", "  ");

        listWidget = new ListWidget
        {
            Items = listItems,
            Focused = true,
            UseSelectionPrefix = true,
            SelectedPrefix = "> ",
            UnselectedPrefix = "  ",
            SelectedFocusedStyle = SelectedStyle,
            ItemStyle = ItemStyle,
            ClearEachRow = false // Block already cleared the area
        };

        listWidget.Render(frame, innerArea, listState);
    }
}
