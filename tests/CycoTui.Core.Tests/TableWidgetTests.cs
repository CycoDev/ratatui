using System.Linq;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;

namespace CycoTui.Core.Tests;

public class TableWidgetTests
{
    [Fact]
    public void TableRendersHeadersAndRows()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var columns = new[]{ new TableColumn("H1", Constraint.Fill(), Style.Empty), new TableColumn("H2", Constraint.Fill(), Style.Empty) };
        var rows = new[]{ new TableRow(new[]{ ("R1C1", Style.Empty), ("R1C2", Style.Empty)}) };
        var table = TableWidget.Create().WithColumns(columns).WithRows(rows);
        term.Draw(f => table.Render(f, new Rect(0,0,20,5)));
        Assert.True(backend.Emitted.Any(c => c.Y == 0)); // header line
        Assert.True(backend.Emitted.Any(c => c.Y == 1)); // first row
    }
}
