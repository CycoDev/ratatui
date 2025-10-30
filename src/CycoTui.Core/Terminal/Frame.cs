using CycoTui.Core.Buffer;
using CycoTui.Core.Style;

namespace CycoTui.Core.Terminal;

/// <summary>
/// Represents a rendering frame for a single Draw cycle, exposing write helpers into the underlying buffer.
/// </summary>
public sealed class Frame
{
    /// <summary>The underlying mutable buffer for this frame.</summary>
    public BufferType Buffer { get; }

    internal Frame(BufferType buffer)
    {
        Buffer = buffer;
    }
    public bool TryGetCell(int x, int y, out CycoTui.Core.Buffer.Cell cell) => Buffer.TryGetCell(x, y, out cell);


    /// <summary>
    /// Write a string at (x,y) using the provided style. Grapheme clusters are measured and may span multiple cells.
    /// </summary>
    /// <param name="x">Column (0-based).</param>
    /// <param name="y">Row (0-based).</param>
    /// <param name="text">Text to write (null safe; ignored if null).</param>
    /// <param name="style">Style applied to each grapheme.</param>
    public void WriteString(int x, int y, string text, StyleType style)
    {
        Buffer.SetString(x, y, text, style);
    }

    /// <summary>
    /// Set a single grapheme at (x,y) by using SetString (which handles width calculation).
    /// </summary>
    public void SetCell(int x, int y, string grapheme, StyleType style)
    {
        // Use SetString to handle grapheme width calculation properly
        Buffer.SetString(x, y, grapheme, style);
    }
}
