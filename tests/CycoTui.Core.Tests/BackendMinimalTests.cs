using System;
using Xunit;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CycoTui.Core.Tests;

public class BackendMinimalTests
{
    [Fact]
    public void UnixBackendProvidesReasonableSize()
    {
        if (!(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())) return; // skip on non-Unix
        using var backend = new CycoTui.Backend.Unix.UnixTerminalBackend();
        var size = backend.GetSize();
        Assert.True(size.Width > 0 && size.Height > 0);
    }

    [Fact]
    public void WindowsBackendProvidesReasonableSize()
    {
        if (!OperatingSystem.IsWindows()) return; // skip on non-Windows
        using var backend = new CycoTui.Backend.Windows.WindowsTerminalBackend();
        var size = backend.GetSize();
        Assert.True(size.Width > 0 && size.Height > 0);
    }

    [Fact]
    public void BackendsEmitRawSequences()
    {
        // Platform-specific instantiation with preference
        ITerminalBackend backend = OperatingSystem.IsWindows()
            ? new CycoTui.Backend.Windows.WindowsTerminalBackend()
            : new CycoTui.Backend.Unix.UnixTerminalBackend();
        using (backend)
        {
            backend.WriteRaw("TEST-SEQUENCE");
            Assert.True(true); // placeholder; ensure no exceptions
        }
    }
}
