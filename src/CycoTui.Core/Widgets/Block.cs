using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Renders a bordered container with optional title. Serves as a foundational container widget.
/// </summary>
public sealed class Block : IWidget, IContainerWidget
{
    public string? Title { get; init; }
    public StyleType TitleStyle { get; init; } = StyleType.Empty;
    public BlockBorderStyle Border { get; init; } = BlockBorderStyle.SingleLine;
    public Padding Padding { get; init; } = Padding.Zero;
    public MergeStrategy MergeStrategy { get; init; } = MergeStrategy.Replace;
    public StyleType BorderStyle { get; init; } = StyleType.Empty;

    private Block() { }

    public static Block Create() => new();
    public Block WithTitle(string? title, StyleType? style = null) => new()
    {
        Title = title,
        TitleStyle = style ?? TitleStyle,
        Border = Border,
        Padding = Padding,
        BorderStyle = BorderStyle,
        MergeStrategy = MergeStrategy
    };
    public Block WithBorder(BlockBorderStyle border, StyleType? style = null) => new()
    {
        Title = Title,
        TitleStyle = TitleStyle,
        Border = border,
        Padding = Padding,
        BorderStyle = style ?? BorderStyle,
        MergeStrategy = MergeStrategy
    };
    public Block WithPadding(Padding padding) => new()
    {
        Title = Title,
        TitleStyle = TitleStyle,
        Border = Border,
        Padding = padding,
        BorderStyle = BorderStyle,
        MergeStrategy = MergeStrategy
    };
    public Block WithMergeStrategy(MergeStrategy strategy) => new()
    {
        Title = Title,
        TitleStyle = TitleStyle,
        Border = Border,
        Padding = Padding,
        BorderStyle = BorderStyle,
        MergeStrategy = strategy
    };

    public void Render(Frame frame, Rect area)
    {
        if (area.Width < 2 || area.Height < 2) return; // not enough space for border
        DrawBorder(frame, area);
        DrawTitle(frame, area);
    }

    private void DrawBorder(Frame frame, Rect area)
    {
        TrySet(frame, area.X, area.Y, Border.TopLeft, BorderStyle);
        TrySet(frame, area.X + area.Width - 1, area.Y, Border.TopRight, BorderStyle);
        TrySet(frame, area.X, area.Y + area.Height - 1, Border.BottomLeft, BorderStyle);
        TrySet(frame, area.X + area.Width - 1, area.Y + area.Height - 1, Border.BottomRight, BorderStyle);
        for (int x = area.X + 1; x < area.X + area.Width - 1; x++)
        {
            TrySet(frame, x, area.Y, Border.Top, BorderStyle);
            TrySet(frame, x, area.Y + area.Height - 1, Border.Bottom, BorderStyle);
        }
        for (int y = area.Y + 1; y < area.Y + area.Height - 1; y++)
        {
            TrySet(frame, area.X, y, Border.Left, BorderStyle);
            TrySet(frame, area.X + area.Width - 1, y, Border.Right, BorderStyle);
        }
    }

    private void TrySet(Frame frame, int x, int y, string grapheme, StyleType style)
    {
        if (MergeStrategy == MergeStrategy.Preserve && frame.TryGetCell(x, y, out var existing))
        {
            if (!string.IsNullOrWhiteSpace(existing.Grapheme) && existing.Grapheme != " ")
                return;
        }
        frame.SetCell(x, y, grapheme, style);
    }

    public static Rect GetInnerContentRect(Rect area, Padding padding)
    {
        var innerX = area.X + 1 + padding.Left;
        var innerY = area.Y + 1 + padding.Top;
        var innerWidth = area.Width - 2 - padding.Left - padding.Right;
        var innerHeight = area.Height - 2 - padding.Top - padding.Bottom;
        if (innerWidth < 0) innerWidth = 0;
        if (innerHeight < 0) innerHeight = 0;
        return new Rect(innerX, innerY, innerWidth, innerHeight);
    }

    public Rect GetContentArea(Rect outerArea) => GetInnerContentRect(outerArea, Padding);

    private void DrawTitle(Frame frame, Rect area)
    {
        if (string.IsNullOrEmpty(Title)) return;
        var titleText = Title!.Length > area.Width - 2 ? Title!.Substring(0, area.Width - 2) : Title!;
        int startX = area.X + 1;
        frame.WriteString(startX, area.Y, titleText, TitleStyle);
    }
}
