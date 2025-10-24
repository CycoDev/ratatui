using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class BlockWidgetTests
{
    [Fact]
    public void BlockRendersBorder()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var block = Block.Create().WithTitle("Title");
        term.Draw(f => block.Render(f, new Rect(0,0,10,5)));
        // Check some corner cells
        Assert.Contains(backend.Emitted, c => c.X == 0 && c.Y == 0);
        Assert.Contains(backend.Emitted, c => c.X == 9 && c.Y == 0);
        Assert.True(backend.Emitted.Count > 0);
    }
}
