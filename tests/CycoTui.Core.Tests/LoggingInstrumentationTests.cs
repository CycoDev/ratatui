using System;
using System.Collections.Generic;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Style;
using CycoTui.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CycoTui.Core.Tests;

public class LoggingInstrumentationTests
{
    private sealed class ListLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _messages = new();
        public IReadOnlyList<string> Messages => _messages;
        public ILogger CreateLogger(string categoryName) => new ListLogger(_messages, categoryName);
        public void Dispose() { }
    }

    private sealed class ListLogger : ILogger
    {
        private readonly List<string> _messages;
        private readonly string _category;
        public ListLogger(List<string> messages, string category) { _messages = messages; _category = category; }
        public IDisposable BeginScope<TState>(TState state) => default!;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception? exception, Func<TState, System.Exception?, string> formatter)
        {
            _messages.Add($"{_category}:{logLevel}:{formatter(state, exception)}");
        }
    }

    [Fact]
    public void TerminalLogsChangedSegments()
    {
        using var provider = new ListLoggerProvider();
        var factory = Microsoft.Extensions.Logging.LoggerFactory.Create(b => b.AddProvider(provider));
        var loggingCtx = new LoggingContext(factory);
        var backend = new TestBackend();
        var term = new TerminalType(backend, loggingCtx);
        term.Draw(f => f.WriteString(0,0,"Hello", StyleType.Empty));
        Assert.Contains(provider.Messages, m => m.Contains("Changed segments"));
    }
}
