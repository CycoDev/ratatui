using System.Collections.Generic;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Renders multiple input lines and a visible caret after the last character of the last line.
/// Ensures that inserting a space immediately advances the caret visually.
/// Can be used as either an immutable widget (IWidget) or with external state (IStatefulWidget).
/// </summary>
public sealed class MultiLineInputWidget : IWidget, IStatefulWidget<MultiLineInputState>
{
    public IReadOnlyList<string> Lines { get; init; } = new List<string>();
    public StyleType TextStyle { get; init; } = StyleType.Empty;
    public StyleType CaretStyle { get; init; } = StyleType.Empty.Add(TextModifier.Invert);
    public string CaretGrapheme { get; init; } = " "; // Space with inverted style shows as full block
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

            // If this is the caret line, insert the caret into the text
            if (i == CaretLineIndex && CaretColumn < width)
            {
                // Split line at caret position
                string beforeCaret = line.Substring(0, System.Math.Min(CaretColumn, line.Length));
                string afterCaret = CaretColumn < line.Length ? line.Substring(CaretColumn + 1) : "";

                // Character at cursor position (or space if at end)
                string caretChar = CaretColumn < line.Length ? line[CaretColumn].ToString() : CaretGrapheme;

                // Write: text + caret + remaining text + padding
                int x = area.X;

                // Write text before caret
                if (beforeCaret.Length > 0)
                {
                    frame.WriteString(x, area.Y + i, beforeCaret, TextStyle);
                    x += beforeCaret.Length;
                }

                // Write caret (character at cursor position with inverted style)
                var caretCell = new CycoTui.Core.Buffer.Cell(caretChar, CaretStyle);
                frame.Buffer.SetCell(x, area.Y + i, caretCell);
                x += 1;

                // Write text after caret
                if (afterCaret.Length > 0 && x < area.X + width)
                {
                    int remaining = area.X + width - x;
                    if (afterCaret.Length > remaining) afterCaret = afterCaret.Substring(0, remaining);
                    frame.WriteString(x, area.Y + i, afterCaret, TextStyle);
                    x += afterCaret.Length;
                }

                // Pad the rest with spaces
                if (x < area.X + width)
                {
                    frame.WriteString(x, area.Y + i, new string(' ', area.X + width - x), TextStyle);
                }
            }
            else
            {
                // Normal line without caret - just pad to width
                string paddedLine = line.PadRight(width);
                frame.WriteString(area.X, area.Y + i, paddedLine, TextStyle);
            }
        }
        // Clear remaining lines if any
        for (int i = linesToRender; i < maxLines; i++)
            frame.WriteString(area.X, area.Y + i, new string(' ', width), TextStyle);
    }

    /// <summary>
    /// Renders the widget using external state (stateful pattern).
    /// </summary>
    public void Render(Frame frame, Rect area, MultiLineInputState state)
    {
        // Create a temporary instance with the state's data and render it
        var widget = new MultiLineInputWidget
        {
            Lines = state.Lines,
            TextStyle = TextStyle,
            CaretStyle = CaretStyle,
            CaretGrapheme = CaretGrapheme,
            CaretLineIndex = state.CursorLineIndex,
            CaretColumn = state.CursorColumn
        };
        widget.Render(frame, area);
    }
}
