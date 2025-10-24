using System;
using CycoTui.Core.Backend;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CycoTui.Core.Tests;

public class BackendCapabilityHeuristicsTests
{
    [Fact]
    public void WindowsStubTrueColorWhenColortermTruecolor()
    {
        if (!OperatingSystem.IsWindows()) return; // only meaningful on Windows
        var original = Environment.GetEnvironmentVariable("COLORTERM");
        try
        {
            Environment.SetEnvironmentVariable("COLORTERM", "truecolor");
            var backend = BackendFactory.Create(BackendPreference.Windows, NullLogger.Instance);
            Assert.Equal(ColorLevel.TrueColor, backend.Capabilities.ColorLevel);
            backend.Dispose();
        }
        finally
        {
            Environment.SetEnvironmentVariable("COLORTERM", original);
        }
    }

    [Fact]
    public void UnixStubAnsi256WhenTermContains256()
    {
        if (!(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())) return;
        var original = Environment.GetEnvironmentVariable("TERM");
        try
        {
            Environment.SetEnvironmentVariable("TERM", "xterm-256color");
            var backend = BackendFactory.Create(BackendPreference.Unix, NullLogger.Instance);
            Assert.True(backend.Capabilities.ColorLevel == ColorLevel.Ansi256 || backend.Capabilities.ColorLevel == ColorLevel.TrueColor);
            backend.Dispose();
        }
        finally
        {
            Environment.SetEnvironmentVariable("TERM", original);
        }
    }
}
