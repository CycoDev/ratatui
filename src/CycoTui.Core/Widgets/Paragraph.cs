using System;
using System.Collections.Generic;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;
using CycoTui.Core.Text;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Multi-line text widget supporting grapheme-aware wrapping, horizontal alignment, and max line limiting.
/// </summary>
public sealed class Paragraph : IWidget
{
    public string? Text { get; init; }
    public StyleType Style { get; init; } = StyleType.Empty;
    public int HorizontalOffset { get; init; } = 0; // applies when Wrap=false or when windowing wrapped lines
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Left;
    public bool Wrap { get; init; } = true;
    public int? MaxLines { get; init; }

    private Paragraph() { }

    public static Paragraph Create() => new();

    public Paragraph WithHorizontalOffset(int offset) => new()
    {
        Text = Text,
        Style = Style,
        Alignment = Alignment,
        Wrap = Wrap,
        MaxLines = MaxLines,
        HorizontalOffset = offset
    };

    public Paragraph WithText(string? text, StyleType? style = null) => new()
    {
        Text = text,
        Style = style ?? Style,
        Alignment = Alignment,
        Wrap = Wrap,
        MaxLines = MaxLines
    };

    public Paragraph WithAlignment(ParagraphAlignment alignment) => new()
    {
        Text = Text,
        Style = Style,
        Alignment = alignment,
        Wrap = Wrap,
        MaxLines = MaxLines
    };

    public Paragraph WithWrap(bool wrap) => new()
    {
        Text = Text,
        Style = Style,
        Alignment = Alignment,
        Wrap = wrap,
        MaxLines = MaxLines
    };

    public Paragraph WithMaxLines(int? maxLines) => new()
    {
        Text = Text,
        Style = Style,
        Alignment = Alignment,
        Wrap = Wrap,
        MaxLines = maxLines
    };

    public void Render(Frame frame, Rect area)
    {
        if (string.IsNullOrEmpty(Text) || area.Width <= 0 || area.Height <= 0) return;
        var effectiveWidth = (HorizontalOffset > 0) ? area.Width + HorizontalOffset : area.Width;
        var lines = BuildLines(Text!, effectiveWidth, Wrap);
        int renderLines = MaxLines.HasValue ? Math.Min(MaxLines.Value, lines.Count) : lines.Count;
        for (int i = 0; i < renderLines && i < area.Height; i++)
        {
            var line = lines[i];
            int offset = Alignment switch
            {
                ParagraphAlignment.Left => 0,
                ParagraphAlignment.Center => Math.Max(0, (area.Width - line.DisplayWidth) / 2),
                ParagraphAlignment.Right => Math.Max(0, area.Width - line.DisplayWidth),
                _ => 0
            };
            int x = area.X + offset;
            int y = area.Y + i;
            IEnumerable<string> graphemes = line.Graphemes;
            if (HorizontalOffset > 0)
            {
                graphemes = CycoTui.Core.Text.HorizontalTextScroller.EnumerateVisibleGraphemes(string.Concat(line.Graphemes), HorizontalOffset, area.Width - offset);
            }
            foreach (var g in graphemes)
            {
                if (x >= area.X + area.Width) break;
                frame.SetCell(x, y, g, Style);
                x += WidthService.GetWidth(g);
            }
        }
    }

    private static List<ParagraphLine> BuildLines(string text, int maxWidth, bool wrap)
    {
        var result = new List<ParagraphLine>();
        var current = new List<string>();
        int currentWidth = 0;
        foreach (var g in GraphemeEnumerator.EnumerateWithZwj(text))
        {
            int w = WidthService.GetWidth(g);
            if (!wrap && currentWidth + w > maxWidth)
            {
                // Truncate
                break;
            }
            if (wrap && currentWidth + w > maxWidth && currentWidth > 0)
            {
                result.Add(new ParagraphLine(current, currentWidth));
                current = new List<string>();
                currentWidth = 0;
            }
            if (currentWidth + w <= maxWidth)
            {
                current.Add(g);
                currentWidth += w;
            }
        }
        if (current.Count > 0)
            result.Add(new ParagraphLine(current, currentWidth));
        return result;
    }

    private sealed class ParagraphLine
    {
        public IReadOnlyList<string> Graphemes { get; }
        public int DisplayWidth { get; }
        public ParagraphLine(IReadOnlyList<string> graphemes, int width)
        {
            Graphemes = graphemes;
            DisplayWidth = width;
        }
    }
}
