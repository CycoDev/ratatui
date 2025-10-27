using System.Linq;
using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;

namespace CycoTui.Core.Tests;

public class BlockMergeStrategyTests
{
    [Fact]
    public void PreserveDoesNotOverwriteExistingGlyph()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new CycoTui.Core.Logging.LoggingContext(null));
        // First draw a block
        var block1 = Block.Create().WithTitle("A", StyleType.Empty).WithMergeStrategy(MergeStrategy.Replace);
        term.Draw(f => block1.Render(f, new Rect(0,0,6,3)));
        int existingCount = backend.Emitted.Count;
        // Second block overlaps with Preserve strategy
        var block2 = Block.Create().WithTitle("B", StyleType.Empty).WithMergeStrategy(MergeStrategy.Preserve);
        term.Draw(f => {
            block2.Render(f, new Rect(0,0,6,3));
        });
        // Precise check: corner glyph at (0,0) should not be duplicated when Preserve is used.
        int cornerOccurrences = backend.Emitted.Where(c => c.X == 0 && c.Y == 0 && c.Cell.Grapheme == block1.Border.TopLeft).Count();
        Assert.Equal(1, cornerOccurrences);
        // Emission count should grow, but not by full border size again.
        Assert.True(backend.Emitted.Count < existingCount + 10);
    }
}
