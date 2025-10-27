using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using System.Linq;

namespace CycoTui.Core.Tests;

public class ListWidgetHorizontalScrollTests
{
    [Fact]
    public void HorizontalOffsetSkipsCharacters()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var items = new[]{ new ListItem("ABCDEFGHIJ", StyleType.Empty) };
        var list = ListWidget.Create().WithItems(items).WithHorizontalOffset(4);
        var state = new ListState(count:1);
        state.Select(0, viewportHeight:1);
        term.Draw(f => list.Render(f, new Rect(0,0,5,1), state));
        var text = backend.Emitted.Where(c => c.Y==0).OrderBy(c=>c.X).Select(c=>c.Cell.Grapheme).Aggregate("",(a,b)=>a+b);
        Assert.Equal("EFGHI", text); // skipped first 4 chars ABCD
    }
}
