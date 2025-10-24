using CycoTui.Core.Buffer;
using CycoTui.Core.Style;

namespace CycoTui.Core.Terminal;

/// <summary>
/// Represents a rendering frame wrapper allowing widgets/content to write into the buffer.
/// </summary>
public sealed class Frame
{
    /// <summary>The underlying mutable buffer for this frame.</summary>
    public Buffer Buffer { get; }

    internal Frame(Buffer buffer)
    {
        Buffer = buffer;
    }

    /// <summary>
    /// Write a string at (x,y) using the provided style. Grapheme clusters are measured and may span multiple cells.
    /// </summary>
    /// <param name="x">Column (0-based).</param>
    /// <param name="y">Row (0-based).</param>
    /// <param name="text">Text to write (null safe; ignored if null).</param>
    /// <param name="style">Style applied to each grapheme.</param>
    public void WriteString(int x, int y, string text, Style style)
    {
        Buffer.SetString(x, y, text, style);
    }

    /// <summary>
    /// Set a single grapheme at (x,y). Caller must ensure area bounds.
    /// </summary>
    public void SetCell(int x, int y, string grapheme, Style style)
    {
        Buffer.SetCell(x, y, new Cell(grapheme, style));
    }
}
