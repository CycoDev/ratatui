using System.Linq;
using Xunit;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Widgets;
using CycoTui.Core.Layout;

namespace CycoTui.Core.Tests;

public class StyleBatchingTests
{
    [Fact]
    public void ConsecutiveStyleChangesEmitOneSequencePerSegment()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        term.Draw(f =>
        {
            f.SetCell(0,0,"A", StyleType.Empty.Add(TextModifier.Bold));
            f.SetCell(1,0,"B", StyleType.Empty.Add(TextModifier.Italic));
            f.SetCell(2,0,"C", StyleType.Empty.Add(TextModifier.Underline));
        });
        // Expect at most number of segments (1) style sequences emitted (batched)
        int styleSeqs = backend.RawSequences.Where(s => !string.IsNullOrEmpty(s) && !s.Contains("[0m")).Count();
        Assert.True(styleSeqs <= 1, $"Expected <=1 style sequences, got {styleSeqs}");
    }
}
