using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class ListWidgetTests
{
    [Fact]
    public void ListRendersItemsAndHighlightSelected()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var items = new[]{ new ListItem("One", StyleType.Empty), new ListItem("Two", StyleType.Empty) };
        var state = new ListState();
        state.Select(1, viewportHeight:5);
        var list = ListWidget.Create().WithItems(items);
        term.Draw(f => list.Render(f, new Rect(0,0,10,5), state));
        Assert.True(backend.Emitted.Any(c => c.X == 0 && c.Y == 1)); // second item line
    }
}
