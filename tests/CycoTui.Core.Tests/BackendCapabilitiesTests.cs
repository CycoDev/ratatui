using CycoTui.Core.Backend;
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
        var win = BackendFactory.Create(BackendPreference.Windows, NullLogger.Instance);
        var nix = BackendFactory.Create(BackendPreference.Unix, NullLogger.Instance);
        Assert.NotEqual(win.Capabilities.ColorLevel, nix.Capabilities.ColorLevel);
        win.Dispose();
        nix.Dispose();
    }
}
