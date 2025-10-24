using CycoTui.Core.Backend;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CycoTui.Core.Tests;

public class BackendFactoryTests
{
    [Fact]
    public void AutoCreatesBackend()
    {
        var backend = BackendFactory.Create(BackendPreference.Auto, NullLogger.Instance);
        Assert.NotNull(backend);
        backend.Dispose();
    }

    [Fact]
    public void MinimalPreferenceCreatesMinimal()
    {
        var backend = BackendFactory.Create(BackendPreference.Minimal, NullLogger.Instance);
        Assert.Equal(ColorLevel.None, backend.Capabilities.ColorLevel);
        backend.Dispose();
    }
}
