using System;
using System.Collections.Generic;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;
using CycoTui.Core.Text;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Tabs widget rendering a horizontal set of labels with alignment and selected tab styling.
/// </summary>
public sealed class TabsWidget : IStatefulWidget<int>
{
    public IReadOnlyList<(string Label, StyleType Style)> Tabs { get; init; } = Array.Empty<(string, StyleType)>();
    public StyleType SelectedStyle { get; init; } = StyleType.Empty.Add(TextModifier.Underline);
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Left;

    private TabsWidget() { }
    public static TabsWidget Create() => new();

    public TabsWidget WithTabs(IReadOnlyList<(string Label, StyleType Style)> tabs) => new()
    {
        Tabs = tabs,
        SelectedStyle = SelectedStyle,
        Alignment = Alignment
    };

    public TabsWidget WithSelectedStyle(StyleType style) => new()
    {
        Tabs = Tabs,
        SelectedStyle = style,
        Alignment = Alignment
    };

    public TabsWidget WithAlignment(ParagraphAlignment alignment) => new()
    {
        Tabs = Tabs,
        SelectedStyle = SelectedStyle,
        Alignment = alignment
    };

    public void Render(Frame frame, Rect area, int selectedIndex)
    {
        if (area.Width <= 0 || area.Height <= 0 || Tabs.Count == 0) return;
        // Compute total width of labels (including a single space between)
        var segments = new List<(List<string> Graphemes, StyleType Style, int Width)>();
        foreach (var (Label, Style) in Tabs)
        {
            var gs = new List<string>();
            int w = 0;
            foreach (var g in GraphemeEnumerator.EnumerateWithZwj(Label)) { gs.Add(g); w += WidthService.GetWidth(g); }
            segments.Add((gs, Style, w));
        }
        int combinedWidth = 0;
        for (int i = 0; i < segments.Count; i++) combinedWidth += segments[i].Width + (i < segments.Count - 1 ? 1 : 0);
        int offset = Alignment switch
        {
            ParagraphAlignment.Left => 0,
            ParagraphAlignment.Center => Math.Max(0, (area.Width - combinedWidth) / 2),
            ParagraphAlignment.Right => Math.Max(0, area.Width - combinedWidth),
            _ => 0
        };
        int x = area.X + offset;
        int y = area.Y;
        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            var style = i == selectedIndex ? SelectedStyle : seg.Style;
            foreach (var g in seg.Graphemes)
            {
                int gw = WidthService.GetWidth(g);
                if (x + gw > area.X + area.Width) break;
                frame.SetCell(x, y, g, style);
                x += gw;
            }
            if (i < segments.Count - 1 && x < area.X + area.Width)
            {
                frame.SetCell(x, y, " ", StyleType.Empty);
                x += 1;
            }
        }
    }
}
