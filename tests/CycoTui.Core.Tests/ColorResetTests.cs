using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class ColorResetTests
{
    [Fact]
    public void ForegroundRemovalEmitsReset39()
    {
        var from = StyleType.Empty.WithForeground(Color.Red);
        var to = StyleType.Empty; // remove fg
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[39m", seq);
    }

    [Fact]
    public void BackgroundRemovalEmitsReset49()
    {
        var from = StyleType.Empty.WithBackground(Color.Blue);
        var to = StyleType.Empty;
        var seq = StyleEmitter.Emit(from, to);
        Assert.Contains("\u001b[49m", seq);
    }
}
