using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class CompositionDemoTests
{
    [Fact]
    public void BlockContainsParagraphAndGauge()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var block = Block.Create().WithTitle("Stats");
        var para = Paragraph.Create().WithText("Progress", StyleType.Empty);
        var gauge = GaugeWidget.Create().WithValue(0.3);
        term.Draw(f => {
            var area = new Rect(0,0,20,5);
            block.Render(f, area);
            // Inner content after border: naive area shrink
            var inner = new Rect(area.X+1, area.Y+1, area.Width-2, area.Height-2);
            para.Render(f, new Rect(inner.X, inner.Y, inner.Width, 1));
            gauge.Render(f, new Rect(inner.X, inner.Y+1, inner.Width, 1));
        });
        Assert.True(backend.Emitted.Any(c => c.Y == 0)); // block border/title
        Assert.True(backend.Emitted.Any(c => c.Y == 1)); // paragraph line
        Assert.True(backend.Emitted.Any(c => c.Y == 2)); // gauge line
    }
}
