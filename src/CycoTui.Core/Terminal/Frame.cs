using CycoTui.Core.Buffer;
using CycoTui.Core.Style;

namespace CycoTui.Core.Terminal;

/// <summary>
/// Represents a rendering frame wrapper allowing widgets/content to write into the buffer.
/// </summary>
public sealed class Frame
{
    public Buffer Buffer { get; }

    internal Frame(Buffer buffer)
    {
        Buffer = buffer;
    }

    public void WriteString(int x, int y, string text, Style style)
    {
        Buffer.SetString(x, y, text, style);
    }

    public void SetCell(int x, int y, string grapheme, Style style)
    {
        Buffer.SetCell(x, y, new Cell(grapheme, style));
    }
}
