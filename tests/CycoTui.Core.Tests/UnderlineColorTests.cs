using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class UnderlineColorTests
{
    [Fact]
    public void StyleEmitterNoUnderlineColorSequenceYet()
    {
        var from = Style.Empty;
        var to = Style.Empty.WithUnderlineColor(Color.Red);
        var seq = StyleEmitter.Emit(from, to);
        // Placeholder: underline color not emitted until backend capability integrated.
        Assert.DoesNotContain("38;", seq); // naive check; refine later
    }
}
