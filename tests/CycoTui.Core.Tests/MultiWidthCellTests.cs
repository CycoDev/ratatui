using CycoTui.Core.Buffer;
using CycoTui.Core.Style;
using CycoTui.Core.Backend;
using CycoTui.Core.Text;
using Xunit;
using System.Linq;

namespace CycoTui.Core.Tests;

public class MultiWidthCellTests
{
    [Fact]
    public void EmojiOccupiesTwoCellsAndSecondIsSkip()
    {
        WidthService.SetMode(WidthMode.Standard);
        var buf = Buffer.Empty(new Size(5,1));
        buf.SetString(0,0,"🙂A", Style.Empty); // emoji width 2 then A
        var first = buf.GetCell(0,0);
        var second = buf.GetCell(1,0);
        var third = buf.GetCell(2,0);
        Assert.Equal("🙂", first.Grapheme);
        Assert.False(first.Skip);
        Assert.True(second.Skip);
        Assert.Equal("A", third.Grapheme);
    }
}
