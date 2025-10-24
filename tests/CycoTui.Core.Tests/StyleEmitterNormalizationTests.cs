using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class StyleEmitterNormalizationTests
{
    [Fact]
    public void IntensityChangeEmitsResetBoldDim()
    {
        var from = Style.Empty.Add(TextModifier.Bold);
        var to = Style.Empty.Add(TextModifier.Dim);
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[22m", seq); // intensity reset
        Assert.Contains("\u001b[2m", seq); // dim set
    }

    [Fact]
    public void RemovingItalicEmitsResetItalic()
    {
        var from = Style.Empty.Add(TextModifier.Italic);
        var to = Style.Empty; // remove italic
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[23m", seq);
    }
}
