using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class ListWidgetScrollTests
{
    [Fact]
    public void ListScrollsAndSelectedStaysVisible()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var items = Enumerable.Range(0, 15).Select(i => new ListItem($"Item{i}", Style.Empty)).ToArray();
        var list = ListWidget.Create().WithItems(items);
        var state = new ListState(count: items.Length);
        state.Select(0);
        term.Draw(f => list.Render(f, new Rect(0,0,10,5), state));
        // Scroll down multiple times
        for (int i = 0; i < 5; i++) state.ScrollDown(viewportHeight:5);
        state.Select(12); // select far item; should auto adjust offset
        term.Draw(f => list.Render(f, new Rect(0,0,10,5), state));
        Assert.True(state.Offset + 5 > state.Selected); // selected visible
    }
}
