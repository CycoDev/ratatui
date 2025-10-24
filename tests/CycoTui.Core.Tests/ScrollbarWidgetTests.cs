using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class ScrollbarWidgetTests
{
    [Fact]
    public void VerticalScrollbarRendersBar()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var sb = ScrollbarWidget.Create().WithData(ScrollbarOrientation.Vertical, contentLength: 100, viewportLength: 20, offset: 40);
        term.Draw(f => sb.Render(f, new Rect(0,0,1,10)));
        Assert.True(backend.Emitted.Count > 0);
    }

    [Fact]
    public void HorizontalScrollbarRendersBar()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var sb = ScrollbarWidget.Create().WithData(ScrollbarOrientation.Horizontal, contentLength: 50, viewportLength: 10, offset: 25);
        term.Draw(f => sb.Render(f, new Rect(0,0,20,1)));
        Assert.True(backend.Emitted.Any(c => c.X > 0));
    }
}
