using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Buffer;
using Xunit;

namespace CycoTui.Core.Tests;

public class TerminalDrawTests
{
    private LoggingContext _logging = new(null);

    [Fact]
    public void DrawEmitsCells()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, _logging);
        term.Draw(f => f.WriteString(0,0,"Hi", Style.Empty));
        Assert.NotEmpty(backend.Emitted);
        term.Dispose();
    }
}
