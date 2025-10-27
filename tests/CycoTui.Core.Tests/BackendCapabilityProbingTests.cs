using System;
using Xunit;
using CycoTui.Core.Backend;

namespace CycoTui.Core.Tests;

public class BackendCapabilityProbingTests
{
    [Fact]
    public void UnixBackendColorLevelIsAnsi256OrTrueColor()
    {
        if (!(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())) return;
        using var backend = new CycoTui.Backend.Unix.UnixTerminalBackend();
        Assert.True(backend.Capabilities.ColorLevel == ColorLevel.Ansi256 || backend.Capabilities.ColorLevel == ColorLevel.TrueColor);
    }

    [Fact]
    public void WindowsBackendColorLevelIsAnsi16OrTrueColor()
    {
        if (!OperatingSystem.IsWindows()) return;
        using var backend = new CycoTui.Backend.Windows.WindowsTerminalBackend();
        Assert.True(backend.Capabilities.ColorLevel == ColorLevel.Ansi16 || backend.Capabilities.ColorLevel == ColorLevel.TrueColor);
    }
}
