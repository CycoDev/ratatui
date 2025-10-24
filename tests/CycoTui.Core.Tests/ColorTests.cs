using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class ColorTests
{
    [Fact]
    public void RgbEquality()
    {
        var a = Color.Rgb(10, 20, 30);
        var b = Color.Rgb(10, 20, 30);
        Assert.Equal(a, b);
    }

    [Fact]
    public void DifferentKindsNotEqual()
    {
        var a = Color.Rgb(10, 20, 30);
        var b = Color.Indexed(10);
        Assert.NotEqual(a, b);
    }
}
