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
        var term = new Terminal(backend, new LoggingContext(null));
        term.Draw(f => {
            f.WriteString(0,0,"A", Style.Empty.Add(TextModifier.Bold));
            f.WriteString(1,0,"B", Style.Empty.Add(TextModifier.Dim));
        });
        // With raw sequence support, reset captured via TestBackend.ResetEmitted
        Assert.True(backend.ResetEmitted);
    }
}
