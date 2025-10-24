using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class TerminalResetEmissionTests
{
    [Fact]
    public void NoStyleChangeSkipsReset()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        term.Draw(f => f.WriteString(0,0,"A", Style.Empty));
        Assert.False(backend.ResetEmitted);
    }

    [Fact]
    public void StyleChangeEmitsReset()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        term.Draw(f => f.WriteString(0,0,"A", Style.Empty.Add(TextModifier.Bold)));
        Assert.True(backend.ResetEmitted);
    }
}
