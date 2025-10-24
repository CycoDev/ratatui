using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Block provides an optional border, title, and inner content area calculation.
/// </summary>
public sealed class Block : IWidget
{
    public string? Title { get; init; }
    public Style TitleStyle { get; init; } = Style.Empty;
    public BlockBorderStyle Border { get; init; } = BlockBorderStyle.SingleLine;
    public Padding Padding { get; init; } = Padding.Zero;
    public Style BorderStyle { get; init; } = Style.Empty;

    private Block() { }

    public static Block Create() => new();
    public Block WithTitle(string? title, Style? style = null) => new()
    {
        Title = title,
        TitleStyle = style ?? TitleStyle,
        Border = Border,
        Padding = Padding,
        BorderStyle = BorderStyle
    };
    public Block WithBorder(BlockBorderStyle border, Style? style = null) => new()
    {
        Title = Title,
        TitleStyle = TitleStyle,
        Border = border,
        Padding = Padding,
        BorderStyle = style ?? BorderStyle
    };
    public Block WithPadding(Padding padding) => new()
    {
        Title = Title,
        TitleStyle = TitleStyle,
        Border = Border,
        Padding = padding,
        BorderStyle = BorderStyle
    };

    public void Render(Frame frame, Rect area)
    {
        if (area.Width < 2 || area.Height < 2) return; // not enough space for border
        DrawBorder(frame, area);
        DrawTitle(frame, area);
        // Inner content area would be returned for nested rendering (future API returns Rect)
    }

    private void DrawBorder(Frame frame, Rect area)
    {
        // Corners (no merging for now; MergeStrategy.Preserve could skip if cell already occupied)
        frame.SetCell(area.X, area.Y, Border.TopLeft, BorderStyle);
        frame.SetCell(area.X + area.Width - 1, area.Y, Border.TopRight, BorderStyle);
        frame.SetCell(area.X, area.Y + area.Height - 1, Border.BottomLeft, BorderStyle);
        frame.SetCell(area.X + area.Width - 1, area.Y + area.Height - 1, Border.BottomRight, BorderStyle);
        // Horizontal lines
        for (int x = area.X + 1; x < area.X + area.Width - 1; x++)
        {
            frame.SetCell(x, area.Y, Border.Top, BorderStyle);
            frame.SetCell(x, area.Y + area.Height - 1, Border.Bottom, BorderStyle);
        }
        // Vertical lines
        for (int y = area.Y + 1; y < area.Y + area.Height - 1; y++)
        {
            frame.SetCell(area.X, y, Border.Left, BorderStyle);
            frame.SetCell(area.X + area.Width - 1, y, Border.Right, BorderStyle);
        }
    }

    private void DrawTitle(Frame frame, Rect area)
    {
        if (string.IsNullOrEmpty(Title)) return;
        var titleText = Title!.Length > area.Width - 2 ? Title!.Substring(0, area.Width - 2) : Title!;
        // TODO: Implement MergeStrategy.Preserve logic by checking existing buffer cell for non-space grapheme.

        int startX = area.X + 1;
        frame.WriteString(startX, area.Y, titleText, TitleStyle);
    }
}
