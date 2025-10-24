using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class StyleEmitterTests
{
    [Fact]
    public void EmitsAnsiForForeground()
    {
        var from = Style.Empty;
        var to = Style.Empty.WithForeground(Color.Red);
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[31m", seq); // Red basic
    }

    [Fact]
    public void EmitsBoldModifier()
    {
        var from = Style.Empty;
        var to = Style.Empty.Add(TextModifier.Bold);
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[1m", seq);
    }
}
