using System.Linq;
using Xunit;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using Microsoft.Extensions.Logging.Abstractions;

namespace CycoTui.Core.Tests;

public class StyleTransitionTests
{
    [Fact]
    public void BoldToDimProducesSingleResetSequence()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        term.Draw(f =>
        {
            f.SetCell(0,0,"A", StyleType.Empty.Add(TextModifier.Bold));
            f.SetCell(1,0,"B", StyleType.Empty.Add(TextModifier.Dim));
        });
        // Expect at least one reset at end and not multiple resets mid-frame for intensity change
        int resetCount = backend.RawSequences.Where(s => s.Contains("\u001b[0m")).Count();
        Assert.Equal(1, resetCount);
    }
}
