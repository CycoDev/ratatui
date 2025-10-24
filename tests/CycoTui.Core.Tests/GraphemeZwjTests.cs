using CycoTui.Core.Text;
using Xunit;
using System.Linq;

namespace CycoTui.Core.Tests;

public class GraphemeZwjTests
{
    [Fact]
    public void FamilyEmojiChainMerged()
    {
        // Common family emoji sequence using ZWJ
        var family = "👩\u200D👨\u200D👧\u200D👦"; // simplified example
        var graphemes = GraphemeEnumerator.EnumerateWithZwj(family).ToList();
        Assert.Single(graphemes);
        Assert.Equal(family, graphemes[0]);
    }

    [Fact]
    public void MultipleSeparateEmojisRemainSeparate()
    {
        var text = "🙂😀";
        var graphemes = GraphemeEnumerator.EnumerateWithZwj(text).ToList();
        Assert.Equal(2, graphemes.Count);
    }
}
