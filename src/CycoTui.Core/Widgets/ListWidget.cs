using System;
using System.Collections.Generic;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;
using CycoTui.Core.Text;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Simple vertical list widget with selection highlighting.
/// </summary>
public sealed class ListWidget : IStatefulWidget<ListState>
{
    public IReadOnlyList<ListItem> Items { get; init; } = Array.Empty<ListItem>();
    public Style SelectedStyle { get; init; } = Style.Empty.Add(TextModifier.Invert);
    public Style ItemStyle { get; init; } = Style.Empty;
    public bool WrapItems { get; init; } = false; // if true, item text wraps line width

    private ListWidget() { }

    public static ListWidget Create() => new();

    public ListWidget WithItems(IReadOnlyList<ListItem> items) => new()
    {
        Items = items,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle,
        WrapItems = WrapItems
    };

    public ListWidget WithSelectedStyle(Style style) => new()
    {
        Items = Items,
        SelectedStyle = style,
        ItemStyle = ItemStyle,
        WrapItems = WrapItems
    };

    public ListWidget WithItemStyle(Style style) => new()
    {
        Items = Items,
        SelectedStyle = SelectedStyle,
        ItemStyle = style,
        WrapItems = WrapItems
    };

    public ListWidget WithWrapItems(bool wrap) => new()
    {
        Items = Items,
        SelectedStyle = SelectedStyle,
        ItemStyle = ItemStyle,
        WrapItems = wrap
    };

    public void Render(Frame frame, Rect area, ListState state)
    {
        if (area.Height <= 0 || area.Width <= 0) return;
        int line = 0;
        for (int i = 0; i < Items.Count && line < area.Height; i++)
        {
            var item = Items[i];
            var style = (state.Selected.HasValue && state.Selected.Value == i) ? SelectedStyle : ItemStyle;
            RenderItem(frame, area, line, item, style);
            line++;
        }
    }

    private void RenderItem(Frame frame, Rect area, int lineIndex, ListItem item, Style style)
    {
        var text = item.Text ?? string.Empty;
        int x = area.X;
        int y = area.Y + lineIndex;
        if (!WrapItems)
        {
            foreach (var g in GraphemeEnumerator.EnumerateWithZwj(text))
            {
                if (x >= area.X + area.Width) break;
                frame.SetCell(x, y, g, style);
                x += WidthService.GetWidth(g);
            }
        }
        else
        {
            // Wrap logic similar to Paragraph
            int currentWidth = 0;
            foreach (var g in GraphemeEnumerator.EnumerateWithZwj(text))
            {
                int w = WidthService.GetWidth(g);
                if (currentWidth + w > area.Width) break; // single-line wrap only for now
                frame.SetCell(x, y, g, style);
                x += w;
                currentWidth += w;
            }
        }
    }
}
