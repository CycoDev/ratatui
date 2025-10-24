using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class MarginPaddingTests
{
    [Fact]
    public void MarginApplyShrinksRect()
    {
        var rect = new Rect(0,0,10,5);
        var margin = new Margin(1,1,2,2);
        var applied = margin.Apply(rect);
        Assert.Equal(1, applied.X);
        Assert.Equal(1, applied.Y);
        Assert.Equal(10 - (1+2), applied.Width);
        Assert.Equal(5 - (1+2), applied.Height);
    }

    [Fact]
    public void PaddingApplyShrinksRect()
    {
        var rect = new Rect(0,0,10,5);
        var padding = new Padding(1,1,2,2);
        var applied = padding.Apply(rect);
        Assert.Equal(1, applied.X);
        Assert.Equal(1, applied.Y);
        Assert.Equal(10 - (1+2), applied.Width);
        Assert.Equal(5 - (1+2), applied.Height);
    }
}
