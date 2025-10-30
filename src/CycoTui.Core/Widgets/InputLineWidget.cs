using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Renders a single line of text with a visible caret. The caret ensures cursor movement is visible even when inserting spaces.
/// </summary>
public sealed class InputLineWidget : IWidget
{
    public string Text { get; init; } = string.Empty;
    public int CaretIndex { get; init; } = 0; // logical position after last inserted character
    public StyleType TextStyle { get; init; } = StyleType.Empty;
    public StyleType CaretStyle { get; init; } = StyleType.Empty.Add(TextModifier.Invert);
    public string CaretGrapheme { get; init; } = " "; // Space with inverted style shows as full block

    public static InputLineWidget Create() => new();

    public InputLineWidget WithText(string text, int caretIndex) => new()
    {
        Text = text,
        CaretIndex = caretIndex,
        TextStyle = TextStyle,
        CaretStyle = CaretStyle,
        CaretGrapheme = CaretGrapheme
    };

    public InputLineWidget WithStyles(StyleType text, StyleType caret) => new()
    {
        Text = Text,
        CaretIndex = CaretIndex,
        TextStyle = text,
        CaretStyle = caret,
        CaretGrapheme = CaretGrapheme
    };

    public void Render(Frame frame, Rect area)
    {
        if (area.Width <= 0 || area.Height <= 0) return;
        int maxChars = area.Width;
        var display = Text.Length > maxChars ? Text[..maxChars] : Text;
        // Write base text
        frame.WriteString(area.X, area.Y, display.PadRight(maxChars), TextStyle);
        // Caret position clamped
        int caretX = area.X + (CaretIndex < maxChars ? CaretIndex : maxChars - 1);
        // Overwrite caret cell with caret grapheme styled
        frame.SetCell(caretX, area.Y, CaretGrapheme, CaretStyle);
    }
}
