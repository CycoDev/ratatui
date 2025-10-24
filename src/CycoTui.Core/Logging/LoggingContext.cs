using System;
using Microsoft.Extensions.Logging;

namespace CycoTui.Core.Logging;

/// <summary>
/// Central logging context to acquire typed loggers.
/// Acquire a logger once per component and cache it; avoid calling per-frame in hot paths.
/// </summary>
public sealed class LoggingContext
{
    private readonly ILoggerFactory _factory;

    public LoggingContext(ILoggerFactory? factory)
    {
        _factory = factory ?? Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance;
    }

    public ILogger GetLogger<T>() => _factory.CreateLogger<T>();
    public ILogger GetLogger(string category) => _factory.CreateLogger(category);
}
