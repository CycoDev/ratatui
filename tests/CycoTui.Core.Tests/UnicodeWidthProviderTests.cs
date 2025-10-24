using CycoTui.Core.Text;
using Xunit;

namespace CycoTui.Core.Tests;

public class UnicodeWidthProviderTests
{
    [Fact]
    public void AsciiCharWidthIsOne()
    {
        var p = new UnicodeWidthProvider(WidthMode.Standard);
        Assert.Equal(1, p.GetWidth("A"));
    }

    [Fact]
    public void CombiningMarkWidthZero()
    {
        var p = new UnicodeWidthProvider(WidthMode.Standard);
        Assert.Equal(0, p.GetWidth("\u0301")); // combining acute
    }

    [Fact]
    public void CjkWideCharWidthTwo()
    {
        var p = new UnicodeWidthProvider(WidthMode.Standard);
        Assert.Equal(2, p.GetWidth("漢"));
    }

    [Fact]
    public void AmbiguousWidthDependsOnMode()
    {
        var standard = new UnicodeWidthProvider(WidthMode.Standard);
        var eastAsian = new UnicodeWidthProvider(WidthMode.EastAsian);
        // Using U+00A1 (Ambiguous in table)
        Assert.Equal(1, standard.GetWidth("\u00A1"));
        Assert.Equal(2, eastAsian.GetWidth("\u00A1"));
    }

    [Fact]
    public void EmojiWidthTwo()
    {
        var p = new UnicodeWidthProvider(WidthMode.Standard);
        Assert.Equal(2, p.GetWidth("🙂"));
    }
}
