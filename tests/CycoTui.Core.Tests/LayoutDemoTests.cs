using CycoTui.Core.Layout;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class LayoutDemoTests
{
    [Fact]
    public void LayoutEngineDemoHorizontalSplit()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var engine = new LayoutEngine(new LoggingContext(null));
        var area = new Rect(0,0,30,5);
        var rects = engine.Distribute(area, new[]{ Constraint.Length(10), Constraint.Fill(), Constraint.Percentage(20)}, LayoutDirection.Horizontal);
        term.Draw(f => {
            var block1 = Block.Create().WithTitle("A");
            var block2 = Block.Create().WithTitle("B");
            var block3 = Block.Create().WithTitle("C");
            block1.Render(f, rects[0]);
            block2.Render(f, rects[1]);
            block3.Render(f, rects[2]);
        });
        Assert.True(backend.Emitted.Count > 0);
    }
}
