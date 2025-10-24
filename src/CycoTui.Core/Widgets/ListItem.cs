using CycoTui.Core.Style;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Represents a single list item (text + style).
/// </summary>
public sealed class ListItem
{
    public string Text { get; }
    public Style Style { get; }

    public ListItem(string text, Style style)
    {
        Text = text;
        Style = style;
    }
}
