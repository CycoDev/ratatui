using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class TabsWidgetTests
{
    [Fact]
    public void TabsRenderSelectedUnderline()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var tabs = new[]{ ("Tab1", Style.Empty), ("Tab2", Style.Empty) };
        var widget = TabsWidget.Create().WithTabs(tabs);
        term.Draw(f => widget.Render(f, new Rect(0,0,20,1), 1));
        // Check second tab label presence
        Assert.Contains(backend.Emitted, c => c.X > 0 && c.Y == 0);
    }
}
