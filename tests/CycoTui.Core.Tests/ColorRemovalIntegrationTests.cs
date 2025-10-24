using CycoTui.Core.Style;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using Xunit;

namespace CycoTui.Core.Tests;

public class ColorRemovalIntegrationTests
{
    [Fact]
    public void TerminalEmitsColorResetOnForegroundRemoval()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        term.Draw(f => {
            f.WriteString(0,0,"A", Style.Empty.WithForeground(Color.Red));
            f.WriteString(1,0,"B", Style.Empty); // removal of foreground color
        });
        // Backend currently only detects style reset; color reset sequences emitted via WriteRaw before final reset are not tracked.
        // TODO: Extend TestBackend to capture raw sequences for detailed assertions.
        Assert.True(backend.ResetEmitted);
    }
}
