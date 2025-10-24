using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class GaugeWidgetTests
{
    [Fact]
    public void GaugeRendersFilledPortion()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var gauge = GaugeWidget.Create().WithValue(0.5);
        term.Draw(f => gauge.Render(f, new Rect(0,0,10,1)));
        // Expect at least one emitted cell at x=0 and at x beyond half (fill/unfill)
        Assert.Contains(backend.Emitted, c => c.X == 0);
        Assert.Contains(backend.Emitted, c => c.X == 9);
    }
}
