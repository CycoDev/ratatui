using CycoTui.Core.Text;
using Xunit;
using System.Linq;

namespace CycoTui.Core.Tests;

public class GraphemeEnumeratorTests
{
    [Fact]
    public void EnumeratesAsciiCharactersIndividually()
    {
        var input = "TEST";
        var graphemes = GraphemeEnumerator.Enumerate(input).ToList();
        Assert.Equal(new[]{"T","E","S","T"}, graphemes);
    }

    [Fact]
    public void EnumeratesCombinedAccents()
    {
        var input = "e\u0301"; // e + combining acute
        var graphemes = GraphemeEnumerator.Enumerate(input).ToList();
        Assert.Single(graphemes);
        Assert.Equal("e\u0301", graphemes[0]);
    }

    [Fact]
    public void EnumeratesEmojiSequenceSimple()
    {
        var input = "🙂";
        var graphemes = GraphemeEnumerator.Enumerate(input).ToList();
        Assert.Single(graphemes);
        Assert.Equal("🙂", graphemes[0]);
    }
}
