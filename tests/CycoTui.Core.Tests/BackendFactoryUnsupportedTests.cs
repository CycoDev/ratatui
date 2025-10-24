using System;
using CycoTui.Core.Backend;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CycoTui.Core.Tests;

public class BackendFactoryUnsupportedTests
{
    [Fact]
    public void WindowsPreferenceThrowsOnNonWindows()
    {
        if (OperatingSystem.IsWindows()) return; // Skip test on Windows
        Assert.Throws<PlatformNotSupportedException>(() => BackendFactory.Create(BackendPreference.Windows, NullLogger.Instance));
    }
}
