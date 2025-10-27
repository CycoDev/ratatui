using System.Linq;
using Xunit;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Buffer;

namespace CycoTui.Core.Tests;

public class StyleBatchingAdjacentSegmentsTests
{
    [Fact]
    public void IdenticalStyleAcrossSegmentsEmitsOnce()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        // First frame prepares previous buffer baseline
        term.Draw(f => { });
        // Second frame: set two non-adjacent styled cells (causing two segments)
        term.Draw(f =>
        {
            f.SetCell(0,0,"A", StyleType.Empty.Add(TextModifier.Bold));
            f.SetCell(2,0,"B", StyleType.Empty.Add(TextModifier.Bold)); // gap at x=1 remains empty => new segment
        });
        var styleSeqs = backend.RawSequences.Where(s => s.Contains("[1m")).ToList();
        Assert.True(styleSeqs.Count <= 1, $"Expected <=1 bold sequence, got {styleSeqs.Count}");
    }
}
