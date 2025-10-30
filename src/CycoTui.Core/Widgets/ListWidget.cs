using System;
using System.Collections.Generic;
using System.Linq;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

public sealed class ListWidget : IStatefulWidget<ListState>
{
    public IReadOnlyList<ListItem> Items { get; init; } = Array.Empty<ListItem>();
    public StyleType SelectedStyle { get; init; } = StyleType.Empty; // fallback when not focused
    public StyleType ItemStyle { get; init; } = StyleType.Empty;
    public bool WrapItems { get; init; } = false;
    public int HorizontalOffset { get; init; } = 0;

    public bool Focused { get; init; } = false;
    public bool ClearEachRow { get; init; } = true;
    public bool UseSelectionPrefix { get; init; } = true;
    public string SelectedPrefix { get; init; } = "> ";
    public string UnselectedPrefix { get; init; } = "  ";
    public StyleType SelectedFocusedStyle { get; init; } = StyleType.Empty.Add(TextModifier.Bold);
    public StyleType SelectedBlurStyle { get; init; } = StyleType.Empty;

    public static ListWidget Create() => new();

    public ListWidget WithItems(IReadOnlyList<ListItem> items) => new()
    {
        Items = items,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle,
        WrapItems = WrapItems,
        HorizontalOffset = HorizontalOffset,
        Focused = Focused,
        ClearEachRow = ClearEachRow,
        UseSelectionPrefix = UseSelectionPrefix,
        SelectedPrefix = SelectedPrefix,
        UnselectedPrefix = UnselectedPrefix,
        SelectedFocusedStyle = SelectedFocusedStyle,
        SelectedBlurStyle = SelectedBlurStyle
    };

    public ListWidget WithFocused(bool focused) => new()
    {
        Items = Items,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle,
        WrapItems = WrapItems,
        HorizontalOffset = HorizontalOffset,
        Focused = focused,
        ClearEachRow = ClearEachRow,
        UseSelectionPrefix = UseSelectionPrefix,
        SelectedPrefix = SelectedPrefix,
        UnselectedPrefix = UnselectedPrefix,
        SelectedFocusedStyle = SelectedFocusedStyle,
        SelectedBlurStyle = SelectedBlurStyle
    };

    public ListWidget WithSelectionPrefix(string selected, string unselected) => new()
    {
        Items = Items,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle,
        WrapItems = WrapItems,
        HorizontalOffset = HorizontalOffset,
        Focused = Focused,
        ClearEachRow = ClearEachRow,
        UseSelectionPrefix = true,
        SelectedPrefix = selected,
        UnselectedPrefix = unselected,
        SelectedFocusedStyle = SelectedFocusedStyle,
        SelectedBlurStyle = SelectedBlurStyle
    };

    public void Render(Frame frame, Rect rect, ListState state)
    {
        if (Items.Count == 0 || rect.Height <= 0 || rect.Width <= 0) return;
        int offset = state.Offset;
        int visible = Math.Min(rect.Height, Items.Count - offset);
        for (int localRow = 0; localRow < visible; localRow++)
        {
            int itemIndex = offset + localRow;
            bool isSelected = itemIndex == state.Selected;
            RenderRow(frame, rect, localRow, itemIndex, isSelected);
        }
    }

    private void RenderRow(Frame frame, Rect rect, int localRow, int itemIndex, bool isSelected)
    {
        int y = rect.Y + localRow;
        int x = rect.X;
        int rowWidth = rect.Width;
        if (rowWidth <= 0) return;

        if (ClearEachRow)
            frame.WriteString(x, y, new string(' ', rowWidth), StyleType.Empty);

        var prefix = UseSelectionPrefix ? (isSelected ? SelectedPrefix : UnselectedPrefix) : string.Empty;
        int prefixLen = prefix.Length;
        var baseText = itemIndex < Items.Count ? Items[itemIndex].Text : string.Empty;

        var prefixStyle = isSelected
            ? (Focused ? SelectedFocusedStyle : (SelectedStyle != StyleType.Empty ? SelectedStyle : SelectedBlurStyle))
            : ItemStyle;

        if (prefixLen > 0 && prefixLen <= rowWidth)
            frame.WriteString(x, y, prefix, prefixStyle);

        int remaining = rowWidth - prefixLen;
        if (remaining > 0)
        {
            var txt = baseText.Length > remaining ? baseText[..remaining] : baseText;
            if (!WrapItems && HorizontalOffset > 0 && txt.Length > HorizontalOffset)
            {
                var sliced = txt.Substring(Math.Min(HorizontalOffset, txt.Length - 1));
                sliced = sliced.Length > remaining ? sliced[..remaining] : sliced;
                frame.WriteString(x + prefixLen, y, sliced, StyleType.Empty);
            }
            else
            {
                frame.WriteString(x + prefixLen, y, txt, StyleType.Empty);
            }
        }
    }
}