using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class StyleTests
{
    [Fact]
    public void PatchMergesNonNullColorsAndOrsModifiers()
    {
        var baseStyle = StyleType.Empty
            .WithForeground(Color.Red)
            .Add(TextModifier.Bold);
        var overlay = StyleType.Empty
            .WithBackground(Color.Blue)
            .Add(TextModifier.Italic);

        var merged = baseStyle.Patch(overlay);
        Assert.Equal(Color.Red, merged.Foreground);
        Assert.Equal(Color.Blue, merged.Background);
        Assert.True((merged.AddModifier & TextModifier.Bold) != 0);
        Assert.True((merged.AddModifier & TextModifier.Italic) != 0);
    }

    [Fact]
    public void RemoveModifierMovesItToSub()
    {
        var style = StyleType.Empty.Add(TextModifier.Bold);
        var removed = style.Remove(TextModifier.Bold);
        Assert.True((removed.AddModifier & TextModifier.Bold) == 0);
        Assert.True((removed.SubModifier & TextModifier.Bold) != 0);
    }
}
