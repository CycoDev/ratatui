using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class StyleEmitterTests
{
    [Fact]
    public void EmitsAnsiForForeground()
    {
        var from = StyleType.Empty;
        var to = StyleType.Empty.WithForeground(Color.Red);
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[31m", seq); // Red basic
    }

    [Fact]
    public void EmitsBoldModifier()
    {
        var from = StyleType.Empty;
        var to = StyleType.Empty.Add(TextModifier.Bold);
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[1m", seq);
    }
}
