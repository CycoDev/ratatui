using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;
using CycoTui.Core.Text;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Simple branding / header widget for CycoTui demonstrations.
/// </summary>
public sealed class LogoWidget : IWidget
{
    public string Text { get; init; } = "CycoTui";
    public StyleType Style { get; init; } = StyleType.Empty.Add(TextModifier.Bold);
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Center;

    private LogoWidget() {}
    public static LogoWidget Create() => new();
    public LogoWidget WithText(string text) => new() { Text = text, Style = Style, Alignment = Alignment };
    public LogoWidget WithStyle(StyleType style) => new() { Text = Text, Style = style, Alignment = Alignment };
    public LogoWidget WithAlignment(ParagraphAlignment a) => new() { Text = Text, Style = Style, Alignment = a };

    public void Render(Frame frame, Rect area)
    {
        if (string.IsNullOrEmpty(Text) || area.Width <= 0 || area.Height <= 0) return;
        var graphemes = GraphemeEnumerator.EnumerateWithZwj(Text).ToList();
        int width = graphemes.Sum(g => WidthService.GetWidth(g));
        int xOffset = Alignment switch
        {
            ParagraphAlignment.Left => 0,
            ParagraphAlignment.Center => System.Math.Max(0, (area.Width - width) / 2),
            ParagraphAlignment.Right => System.Math.Max(0, area.Width - width),
            _ => 0
        };
        int x = area.X + xOffset;
        int y = area.Y; // single line
        foreach (var g in graphemes)
        {
            int w = WidthService.GetWidth(g);
            if (x + w > area.X + area.Width) break; // truncate
            frame.SetCell(x, y, g, Style);
            x += w;
        }
    }
}
