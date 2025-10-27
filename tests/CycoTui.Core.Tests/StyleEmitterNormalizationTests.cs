using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class StyleEmitterNormalizationTests
{
    [Fact]
    public void IntensityChangeEmitsResetBoldDim()
    {
        var from = StyleType.Empty.Add(TextModifier.Bold);
        var to = StyleType.Empty.Add(TextModifier.Dim);
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[22m", seq); // intensity reset
        Assert.Contains("\u001b[2m", seq); // dim set
    }

    [Fact]
    public void RemovingItalicEmitsResetItalic()
    {
        var from = StyleType.Empty.Add(TextModifier.Italic);
        var to = StyleType.Empty; // remove italic
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[23m", seq);
    }
}
