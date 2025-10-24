using CycoTui.Core.Features;
using Xunit;

namespace CycoTui.Core.Tests;

public class FeatureFlagsTests
{
    [Fact]
    public void EmptyIsDisabled()
    {
        Assert.False(FeatureFlags.Empty.IsEnabled("FEATURE_CALENDAR"));
    }

    [Fact]
    public void BuilderEnablesCaseInsensitive()
    {
        var flags = FeatureFlags.CreateBuilder()
            .Enable("feature_calendar")
            .Build();
        Assert.True(flags.IsEnabled("FEATURE_CALENDAR"));
    }
}
