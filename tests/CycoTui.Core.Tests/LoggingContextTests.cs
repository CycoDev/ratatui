using CycoTui.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CycoTui.Core.Tests;

public class LoggingContextTests
{
    [Fact]
    public void UsesNullLoggerFactoryWhenNull()
    {
        var ctx = new LoggingContext(null);
        var logger = ctx.GetLogger<LoggingContextTests>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void CreatesLoggerFromFactory()
    {
        using var factory = LoggerFactory.Create(b => b.AddFilter(_ => false));
        var ctx = new LoggingContext(factory);
        var logger = ctx.GetLogger("CustomCategory");
        Assert.NotNull(logger);
    }
}
