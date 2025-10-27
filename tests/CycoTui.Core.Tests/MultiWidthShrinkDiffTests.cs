using Xunit;
using CycoTui.Core.Buffer;
using CycoTui.Core.Style;
using CycoTui.Core.Text;
using CycoTui.Core.Backend;

namespace CycoTui.Core.Tests;

public class MultiWidthShrinkDiffTests
{
    [Fact]
    public void ShrinkingWideGraphemeClearsTrailingCell()
    {
        WidthService.SetMode(WidthMode.Standard);
        var buf = BufferType.Empty(new Size(4,1));
        buf.SetString(0,0,"😀", StyleType.Empty); // width 2
        // Overwrite with single-width character
        buf.SetString(0,0,"A", StyleType.Empty);
        // Trailing cell at x=1 should be cleared to space
        var c1 = buf.GetCell(1,0);
        Assert.Equal(" ", c1.Grapheme);
    }
}
