using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using Xunit;

namespace CycoTui.Core.Tests;

public class TestBackendScrollbackTests
{
    [Fact]
    public void FramesCapturePerDrawCall()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        term.Draw(f => f.WriteString(0,0,"A", Style.Empty));
        term.Draw(f => f.WriteString(0,0,"B", Style.Empty));
        Assert.Equal(2, backend.Frames.Count);
        Assert.Single(backend.Frames[0]);
        Assert.Single(backend.Frames[1]);
    }
}
