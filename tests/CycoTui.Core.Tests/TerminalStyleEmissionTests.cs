using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class TerminalStyleEmissionTests
{
    [Fact]
    public void StyleChangesEmitNormalizationSequence()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        term.Draw(f => {
            f.WriteString(0,0,"A", StyleType.Empty.Add(TextModifier.Bold));
            f.WriteString(1,0,"B", StyleType.Empty.Add(TextModifier.Dim));
        });
        // With raw sequence support, reset captured via TestBackend.ResetEmitted
        Assert.True(backend.ResetEmitted);
    }
}
