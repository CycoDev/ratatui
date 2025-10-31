using System;
using System.Collections.Generic;
using System.Linq;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Popup position preference for completion widget.
/// </summary>
public enum PopupPosition
{
    /// <summary>Always show popup above the input area.</summary>
    Above,
    /// <summary>Always show popup below the input area.</summary>
    Below,
    /// <summary>Automatically choose position based on available space.</summary>
    Auto
}

/// <summary>
/// Renders a completion popup overlay with a bordered list of matching items.
/// Designed to appear above or below the input area when a trigger character is pressed.
/// </summary>
public sealed class CompletionPopupWidget : IStatefulWidget<CompletionState>
{
    public int MaxVisibleItems { get; init; } = 10;
    public PopupPosition PreferredPosition { get; init; } = PopupPosition.Auto;
    public StyleType BorderStyle { get; init; } = StyleType.Empty;
    public StyleType TitleStyle { get; init; } = StyleType.Empty.Add(TextModifier.Bold);
    public StyleType SelectedStyle { get; init; } = StyleType.Empty.Add(TextModifier.Invert);
    public StyleType ItemStyle { get; init; } = StyleType.Empty;

    public static CompletionPopupWidget Create() => new();

    public CompletionPopupWidget WithMaxVisibleItems(int max) => new()
    {
        MaxVisibleItems = max,
        PreferredPosition = PreferredPosition,
        BorderStyle = BorderStyle,
        TitleStyle = TitleStyle,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle
    };

    public CompletionPopupWidget WithPreferredPosition(PopupPosition position) => new()
    {
        MaxVisibleItems = MaxVisibleItems,
        PreferredPosition = position,
        BorderStyle = BorderStyle,
        TitleStyle = TitleStyle,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle
    };

    public CompletionPopupWidget WithStyles(
        StyleType border,
        StyleType title,
        StyleType selected,
        StyleType item) => new()
    {
        MaxVisibleItems = MaxVisibleItems,
        PreferredPosition = PreferredPosition,
        BorderStyle = border,
        TitleStyle = title,
        SelectedStyle = selected,
        ItemStyle = item
    };

    /// <summary>
    /// Calculates the required rectangle for the popup based on matched items.
    /// Width is determined by the longest item + border + prefix.
    /// Height is capped by MaxVisibleItems + border.
    /// Position is determined by PreferredPosition and available terminal space.
    /// </summary>
    /// <param name="inputArea">The input area rectangle</param>
    /// <param name="state">The completion state</param>
    /// <param name="terminalHeight">Total terminal height for positioning calculations</param>
    public Rect CalculatePopupRect(Rect inputArea, CompletionState state, int terminalHeight)
    {
        if (!state.IsActive || state.MatchedItems.Count == 0)
        {
            // Return minimal rect when nothing to show
            return new Rect(inputArea.X, inputArea.Y - 3, 20, 3);
        }

        // Calculate width from longest item
        int longestItem = state.MatchedItems.Max(f => f.Length);
        int selectionPrefixWidth = 2; // "> " or "  "
        int borderWidth = 2; // left + right border
        int totalWidth = longestItem + selectionPrefixWidth + borderWidth;

        // Clamp to reasonable bounds (minimum 20, maximum based on terminal width)
        totalWidth = Math.Max(20, totalWidth);
        totalWidth = Math.Min(totalWidth, 120); // Reasonable max width

        // Calculate ideal height from number of items
        int visibleItems = Math.Min(state.MatchedItems.Count, MaxVisibleItems);
        int borderHeight = 2; // top + bottom border
        int idealHeight = visibleItems + borderHeight;

        // Calculate available space above and below input
        int spaceAbove = inputArea.Y;
        int spaceBelow = terminalHeight - (inputArea.Y + inputArea.Height);

        // Determine position based on preference and available space
        bool showAbove;
        switch (PreferredPosition)
        {
            case PopupPosition.Above:
                showAbove = true;
                break;
            case PopupPosition.Below:
                showAbove = false;
                break;
            case PopupPosition.Auto:
            default:
                // Auto: choose position with more space
                showAbove = spaceAbove >= spaceBelow;
                break;
        }

        // Calculate final position and height
        int popupX = inputArea.X;
        int popupY;
        int totalHeight;

        if (showAbove)
        {
            // Position above input
            totalHeight = Math.Min(idealHeight, spaceAbove);
            popupY = inputArea.Y - totalHeight;

            // Clamp to ensure Y >= 0
            if (popupY < 0)
            {
                popupY = 0;
                totalHeight = inputArea.Y;
            }
        }
        else
        {
            // Position below input
            popupY = inputArea.Y + inputArea.Height;
            totalHeight = Math.Min(idealHeight, spaceBelow);

            // Clamp to ensure doesn't exceed terminal height
            if (popupY + totalHeight > terminalHeight)
            {
                totalHeight = Math.Max(3, terminalHeight - popupY);
            }
        }

        // Ensure minimum height for usability
        totalHeight = Math.Max(3, totalHeight);

        return new Rect(popupX, popupY, totalWidth, totalHeight);
    }

    public void Render(Frame frame, Rect area, CompletionState state)
    {
        if (!state.IsActive || area.Width < 4 || area.Height < 3)
            return;

        // Show error state if present
        if (state.IsError)
        {
            var block = Block.Create()
                .WithTitle("Error", TitleStyle.WithForeground(Color.Red))
                .WithBorder(BlockBorderStyle.SingleLine, BorderStyle.WithForeground(Color.Red));

            block.Render(frame, area);

            var innerArea = Block.GetInnerContentRect(area, Padding.Zero);
            if (innerArea.Width > 0 && innerArea.Height > 0)
            {
                var errorText = state.ErrorMessage ?? "Unknown error";
                frame.WriteString(innerArea.X, innerArea.Y, errorText, ItemStyle.WithForeground(Color.Red));
            }
            return;
        }

        // Create title with match count
        string title;
        if (state.MatchedItems.Count == 0)
        {
            title = "No matches";
        }
        else if (state.Query.Length == 0)
        {
            // Show label if available, otherwise just count
            title = string.IsNullOrEmpty(state.TriggerLabel)
                ? $"Items ({state.MatchedItems.Count})"
                : $"{state.TriggerLabel} ({state.MatchedItems.Count})";
        }
        else
        {
            // Show trigger + query + count
            title = $"{state.TriggerChar}{state.Query} ({state.MatchedItems.Count})";
        }

        // Render border with title
        var block2 = Block.Create()
            .WithTitle(title, TitleStyle)
            .WithBorder(BlockBorderStyle.SingleLine, BorderStyle);

        block2.Render(frame, area);

        // Get inner area for list
        var innerArea2 = Block.GetInnerContentRect(area, Padding.Zero);
        if (innerArea2.Width <= 0 || innerArea2.Height <= 0)
            return;

        // No items to show
        if (state.MatchedItems.Count == 0)
        {
            var noMatchText = state.Query.Length == 0 ? "Type to search..." : "No items found";
            frame.WriteString(innerArea2.X, innerArea2.Y, noMatchText, ItemStyle);
            return;
        }

        // Create ListItems from matched items
        var listItems = state.MatchedItems
            .Select(f => new ListItem(f, ItemStyle))
            .ToList();

        // Create ListState with current selection
        var listState = new ListState(listItems.Count);
        listState.Select(state.SelectedIndex, innerArea2.Height);

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

        listWidget.Render(frame, innerArea2, listState);
    }
}
