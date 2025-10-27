using System.Linq;
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
        Assert.True(backend.RawSequences.Any(s => s.Contains("[22")), "Expected intensity reset (22) in consolidated sequence");
    }
}
