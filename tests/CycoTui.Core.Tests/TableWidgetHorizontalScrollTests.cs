using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using System.Linq;

namespace CycoTui.Core.Tests;

public class TableWidgetHorizontalScrollTests
{
    [Fact]
    public void HorizontalOffsetSkipsCharactersInCells()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var columns = new[]{ new TableColumn("HEADER", Constraint.Fill(), StyleType.Empty) };
        var rows = new[]{ new TableRow(new[]{ ("ABCDEFGHIJ", StyleType.Empty) }) };
        var table = TableWidget.Create().WithColumns(columns).WithRows(rows).WithHorizontalOffset(4);
        term.Draw(f => table.Render(f, new Rect(0,0,8,3)));
        var rowText = backend.Emitted.Where(c => c.Y==1).OrderBy(c=>c.X).Select(c=>c.Cell.Grapheme).Aggregate("",(a,b)=>a+b);
        Assert.Equal("EFGHIJ", rowText.Substring(0,6));
    }
}
