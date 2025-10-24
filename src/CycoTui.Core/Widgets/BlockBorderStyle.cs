using CycoTui.Core.Style;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Defines border characters for the Block widget.
/// </summary>
public sealed class BlockBorderStyle
{
    public string Top { get; init; } = "─";
    public string Bottom { get; init; } = "─";
    public string Left { get; init; } = "│";
    public string Right { get; init; } = "│";
    public string TopLeft { get; init; } = "┌";
    public string TopRight { get; init; } = "┐";
    public string BottomLeft { get; init; } = "└";
    public string BottomRight { get; init; } = "┘";

    public static BlockBorderStyle SingleLine { get; } = new();
}
