using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Horizontal progress bar (gauge) widget showing completion ratio with configurable characters and styles.
/// </summary>
public sealed class GaugeWidget : IWidget
{
    public double Value { get; init; } = 0.0; // 0..1
    public StyleType FilledStyle { get; init; } = StyleType.Empty.Add(TextModifier.Invert);
    public StyleType EmptyStyle { get; init; } = StyleType.Empty;
    public char FilledChar { get; init; } = '█';
    public char EmptyChar { get; init; } = '░';

    private GaugeWidget() { }
    public static GaugeWidget Create() => new();

    public GaugeWidget WithValue(double value) => new()
    {
        Value = value < 0 ? 0 : (value > 1 ? 1 : value),
        FilledStyle = FilledStyle,
        EmptyStyle = EmptyStyle,
        FilledChar = FilledChar,
        EmptyChar = EmptyChar
    };

    public GaugeWidget WithFilledStyle(StyleType style) => new()
    {
        Value = Value,
        FilledStyle = style,
        EmptyStyle = EmptyStyle,
        FilledChar = FilledChar,
        EmptyChar = EmptyChar
    };

    public GaugeWidget WithEmptyStyle(StyleType style) => new()
    {
        Value = Value,
        FilledStyle = FilledStyle,
        EmptyStyle = style,
        FilledChar = FilledChar,
        EmptyChar = EmptyChar
    };

    public GaugeWidget WithChars(char filled, char empty) => new()
    {
        Value = Value,
        FilledStyle = FilledStyle,
        EmptyStyle = EmptyStyle,
        FilledChar = filled,
        EmptyChar = empty
    };

    public void Render(Frame frame, Rect area)
    {
        if (area.Width <= 0 || area.Height <= 0) return;
        int fillCount = (int)(area.Width * Value);
        int y = area.Y;
        for (int x = 0; x < area.Width; x++)
        {
            var cellChar = x < fillCount ? FilledChar.ToString() : EmptyChar.ToString();
            var style = x < fillCount ? FilledStyle : EmptyStyle;
            frame.SetCell(area.X + x, y, cellChar, style);
        }
    }
}
