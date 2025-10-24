using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class StyleDiffTests
{
    [Fact]
    public void AddedAndRemovedComputed()
    {
        var from = TextModifier.Bold | TextModifier.Underline;
        var to = TextModifier.Italic | TextModifier.Bold;
        var diff = new StyleDiff(from, to);
        Assert.Equal(TextModifier.Italic, diff.Added);
        Assert.Equal(TextModifier.Underline, diff.Removed);
    }

    [Fact]
    public void EmptyDiffWhenSame()
    {
        var diff = new StyleDiff(TextModifier.Bold, TextModifier.Bold);
        Assert.True(diff.IsEmpty);
    }
}
