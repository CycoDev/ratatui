using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;
using Xunit;

namespace CycoTui.Core.Tests;

public class TestBackendRawSequenceTests
{
    [Fact]
    public void CapturesStyleTransitionSequences()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        term.Draw(f => {
            f.WriteString(0,0,"A", StyleType.Empty.Add(TextModifier.Bold));
            f.WriteString(1,0,"B", StyleType.Empty.Add(TextModifier.Dim));
        });
        Assert.Contains(backend.RawSequences, s => s.Contains("\u001b[22m")); // intensity reset
    }
}
