using CycoTui.Core.Backend;
using System.Runtime.Versioning;
using System;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CycoTui.Core.Tests;

public class BackendCapabilitiesTests
{
    [Fact]
    public void MinimalIsAllFalseAndNoColor()
    {
        var caps = BackendCapabilities.Minimal;
        Assert.Equal(ColorLevel.None, caps.ColorLevel);
        Assert.False(caps.SupportsUnderlineColor);
        Assert.False(caps.SupportsMouse);
        Assert.False(caps.SupportsScrollingRegions);
        Assert.False(caps.SupportsTrueColor);
        Assert.False(caps.SupportsUnicodeWidthReliably);
    }

    [Fact]
    public void StubCapabilitiesDiffer()
    {
        // Adapt to platform guards: choose two different backends available on this OS.
        ITerminalBackend first;
        ITerminalBackend second;
        if (OperatingSystem.IsWindows())
        {
            first = BackendFactory.Create(BackendPreference.Windows, NullLogger.Instance);
            second = BackendFactory.Create(BackendPreference.Minimal, NullLogger.Instance);
        }
        else
        {
            first = BackendFactory.Create(BackendPreference.Unix, NullLogger.Instance);
            second = BackendFactory.Create(BackendPreference.Minimal, NullLogger.Instance);
        }
        Assert.NotEqual(first.Capabilities.ColorLevel, second.Capabilities.ColorLevel);
        first.Dispose();
        second.Dispose();
    }
}
