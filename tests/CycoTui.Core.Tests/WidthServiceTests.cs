using CycoTui.Core.Text;
using Xunit;

namespace CycoTui.Core.Tests;

public class WidthServiceTests
{
    [Fact]
    public void ModeSwitchAffectsAmbiguousWidth()
    {
        WidthService.SetMode(WidthMode.Standard);
        var w1 = WidthService.GetWidth("\u00A1");
        WidthService.SetMode(WidthMode.EastAsian);
        var w2 = WidthService.GetWidth("\u00A1");
        Assert.Equal(1, w1);
        Assert.Equal(2, w2);
    }
}
