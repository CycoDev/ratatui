using System.Collections.Generic;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Renders multiple input lines and a visible caret after the last character of the last line.
/// Ensures that inserting a space immediately advances the caret visually.
/// </summary>
public sealed class MultiLineInputWidget : IWidget
{
    public IReadOnlyList<string> Lines { get; init; } = new List<string>();
    public StyleType TextStyle { get; init; } = StyleType.Empty;
    public StyleType CaretStyle { get; init; } = StyleType.Empty.Add(TextModifier.Invert);
    public string CaretGrapheme { get; init; } = "▌";
    public int CaretLineIndex { get; init; } = 0; // last line index containing caret
    public int CaretColumn { get; init; } = 0;    // column after the last character

    public static MultiLineInputWidget Create() => new();

    public MultiLineInputWidget WithLines(IReadOnlyList<string> lines) => new()
    {
        Lines = lines,
        TextStyle = TextStyle,
        CaretStyle = CaretStyle,
        CaretGrapheme = CaretGrapheme,
        CaretLineIndex = lines.Count - 1,
        CaretColumn = lines.Count > 0 ? lines[^1].Length : 0
    };

    public MultiLineInputWidget WithCaret(int lineIndex, int column) => new()
    {
        Lines = Lines,
        TextStyle = TextStyle,
        CaretStyle = CaretStyle,
        CaretGrapheme = CaretGrapheme,
        CaretLineIndex = lineIndex,
        CaretColumn = column
    };

    public MultiLineInputWidget WithStyles(StyleType text, StyleType caret) => new()
    {
        Lines = Lines,
        TextStyle = text,
        CaretStyle = caret,
        CaretGrapheme = CaretGrapheme,
        CaretLineIndex = CaretLineIndex,
        CaretColumn = CaretColumn
    };

    public void Render(Frame frame, Rect area)
    {
        if (area.Width <= 0 || area.Height <= 0) return;
        int maxLines = area.Height;
        int width = area.Width;
        int linesToRender = System.Math.Min(Lines.Count, maxLines);

        for (int i = 0; i < linesToRender; i++)
        {
            string line = Lines[i];
            if (line.Length > width) line = line.Substring(0, width);
            frame.WriteString(area.X, area.Y + i, line.PadRight(width), TextStyle);
        }
        // Clear remaining lines if any
        for (int i = linesToRender; i < maxLines; i++)
            frame.WriteString(area.X, area.Y + i, new string(' ', width), TextStyle);

        // Caret rendering if within visible region
        if (CaretLineIndex < linesToRender)
        {
            int caretX = area.X + (CaretColumn < width ? CaretColumn : width - 1);
            int caretY = area.Y + CaretLineIndex;
            frame.SetCell(caretX, caretY, CaretGrapheme, CaretStyle);
        }
    }
}
