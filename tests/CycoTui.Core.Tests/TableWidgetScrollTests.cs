using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class TableWidgetScrollTests
{
    [Fact]
    public void TableScrollsRows()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var columns = new[]{ new TableColumn("H", Constraint.Fill(), StyleType.Empty) };
        var rows = Enumerable.Range(0, 10).Select(i => new TableRow(new[]{ ($"Row{i}", StyleType.Empty) })).ToArray();
        var table = TableWidget.Create().WithColumns(columns).WithRows(rows);
        var state = new TableState(rowCount: rows.Length);
        state.Select(0);

        // initial
        term.Draw(f => table.Render(f, new Rect(0,0,10,5), state));
        int initialCount = backend.Emitted.Count(c => c.Y > 0);
        // scroll down
        state.ScrollDown(viewportHeight: 4); // area height - header
        term.Draw(f => table.Render(f, new Rect(0,0,10,5), state));
        int scrolledCount = backend.Emitted.Count(c => c.Y > 0);
        Assert.True(scrolledCount > 0);
        Assert.True(initialCount > 0);
        Assert.Equal(1, state.Offset); // verify scroll occurred despite selected top row
    }
}
