using CycoTui.Core.Buffer;
using CycoTui.Core.Style;
using CycoTui.Core.Backend;
using Xunit;

namespace CycoTui.Core.Tests;

public class BufferTests
{
    [Fact]
    public void SetAndGetCellWorks()
    {
        var buf = new BufferType(new Position(0,0), new Size(5,1));
        buf.SetCell(2,0,new Cell("X", StyleType.Empty));
        Assert.Equal("X", buf.GetCell(2,0).Grapheme);
    }

    [Fact]
    public void SetStringTruncatesAtWidth()
    {
        var buf = BufferType.Empty(new Size(4,1));
        buf.SetString(0,0,"HELLO", StyleType.Empty);
        Assert.Equal("H", buf.GetCell(0,0).Grapheme);
        Assert.Equal("E", buf.GetCell(1,0).Grapheme);
        Assert.Equal("L", buf.GetCell(2,0).Grapheme);
        Assert.Equal("L", buf.GetCell(3,0).Grapheme);
    }

    [Fact]
    public void TryGetCellOutOfBoundsReturnsFalse()
    {
        var buf = BufferType.Empty(new Size(2,1));
        var ok = buf.TryGetCell(5,0, out var cell);
        Assert.False(ok);
        Assert.Equal(" ", cell.Grapheme); // default returned
    }
}
