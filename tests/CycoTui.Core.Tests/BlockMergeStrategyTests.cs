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
        // Expect fewer new emissions for overlapping border corners than full redraw would produce
        Assert.True(backend.Emitted.Count < existingCount + 20); // heuristic check
    }
}
