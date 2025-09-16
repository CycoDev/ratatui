using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui.Plugins
{
    /// <summary>
    /// Plugin that monitors application performance including frame rate, memory usage, and event processing times.
    /// </summary>
    public class PerformanceMonitorPlugin : ApplicationPlugin
    {
        private readonly Stopwatch _uptime = new();
        private readonly Queue<FrameInfo> _frameHistory = new();
        private readonly Queue<EventInfo> _eventHistory = new();
        private readonly object _lockObject = new object();

        private long _totalFrames;
        private long _totalEvents;
        private DateTime _lastStatsUpdate = DateTime.MinValue;
        private readonly TimeSpan _statsUpdateInterval = TimeSpan.FromSeconds(1);

        /// <inheritdoc />
        public override string Name => "Performance Monitor";

        /// <summary>
        /// Gets or sets the maximum number of frame samples to keep in history.
        /// </summary>
        public int MaxFrameHistory { get; set; } = 60; // 1 second at 60 FPS

        /// <summary>
        /// Gets or sets the maximum number of event samples to keep in history.
        /// </summary>
        public int MaxEventHistory { get; set; } = 100;

        /// <summary>
        /// Gets or sets a value indicating whether to track detailed performance metrics.
        /// </summary>
        public bool DetailedTracking { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically adjust render rate based on performance.
        /// </summary>
        public bool AutoOptimize { get; set; } = false;

        /// <inheritdoc />
        public override Task InitializeAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            _uptime.Start();
            application.SetGlobalState("PerformanceStats", new PerformanceStats());
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task<bool> HandleEventAsync(IEvent @event, IApplication application, CancellationToken cancellationToken = default)
        {
            if (DetailedTracking)
            {
                var stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();

                // Record event processing time
                RecordEvent(@event.Type, stopwatch.Elapsed);
            }

            // Handle performance-related events
            if (@event is PerformanceEvent perfEvent)
            {
                await HandlePerformanceEvent(perfEvent, application);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            // Update statistics periodically
            if (now - _lastStatsUpdate >= _statsUpdateInterval)
            {
                await UpdateStatistics(application);
                _lastStatsUpdate = now;
            }

            // Auto-optimize if enabled
            if (AutoOptimize)
            {
                await PerformAutoOptimization(application);
            }
        }

        /// <inheritdoc />
        public override Task ShutdownAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            _uptime.Stop();

            // Save final performance report
            var stats = GetCurrentStats();
            application.SetGlobalState("FinalPerformanceReport", stats);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Records a frame render operation.
        /// </summary>
        /// <param name="renderTime">The time taken to render the frame.</param>
        public void RecordFrame(TimeSpan renderTime)
        {
            lock (_lockObject)
            {
                _totalFrames++;

                var frameInfo = new FrameInfo
                {
                    Timestamp = DateTime.UtcNow,
                    RenderTime = renderTime,
                    FrameNumber = _totalFrames
                };

                _frameHistory.Enqueue(frameInfo);

                // Limit history size
                while (_frameHistory.Count > MaxFrameHistory)
                {
                    _frameHistory.Dequeue();
                }
            }
        }

        /// <summary>
        /// Records an event processing operation.
        /// </summary>
        /// <param name="eventType">The type of event processed.</param>
        /// <param name="processingTime">The time taken to process the event.</param>
        public void RecordEvent(EventType eventType, TimeSpan processingTime)
        {
            lock (_lockObject)
            {
                _totalEvents++;

                var eventInfo = new EventInfo
                {
                    Timestamp = DateTime.UtcNow,
                    EventType = eventType,
                    ProcessingTime = processingTime
                };

                _eventHistory.Enqueue(eventInfo);

                // Limit history size
                while (_eventHistory.Count > MaxEventHistory)
                {
                    _eventHistory.Dequeue();
                }
            }
        }

        /// <summary>
        /// Gets the current performance statistics.
        /// </summary>
        /// <returns>Current performance statistics.</returns>
        public PerformanceStats GetCurrentStats()
        {
            lock (_lockObject)
            {
                var stats = new PerformanceStats
                {
                    Uptime = _uptime.Elapsed,
                    TotalFrames = _totalFrames,
                    TotalEvents = _totalEvents
                };

                if (_frameHistory.Count > 0)
                {
                    var recentFrames = _frameHistory.ToArray();
                    stats.AverageFrameTime = TimeSpan.FromTicks((long)recentFrames.Average(f => f.RenderTime.Ticks));
                    stats.MinFrameTime = recentFrames.Min(f => f.RenderTime);
                    stats.MaxFrameTime = recentFrames.Max(f => f.RenderTime);

                    // Calculate FPS over the last second
                    var oneSecondAgo = DateTime.UtcNow - TimeSpan.FromSeconds(1);
                    var recentFrameCount = recentFrames.Count(f => f.Timestamp >= oneSecondAgo);
                    stats.CurrentFPS = recentFrameCount;
                }

                if (_eventHistory.Count > 0)
                {
                    var recentEvents = _eventHistory.ToArray();
                    stats.AverageEventTime = TimeSpan.FromTicks((long)recentEvents.Average(e => e.ProcessingTime.Ticks));
                    stats.EventsPerSecond = recentEvents.Count(e => e.Timestamp >= DateTime.UtcNow - TimeSpan.FromSeconds(1));
                }

                // Memory information
                try
                {
                    var process = Process.GetCurrentProcess();
                    stats.MemoryUsageMB = process.WorkingSet64 / (1024.0 * 1024.0);
                    stats.GCMemoryMB = GC.GetTotalMemory(false) / (1024.0 * 1024.0);
                }
                catch
                {
                    // Ignore errors getting memory info
                }

                return stats;
            }
        }

        private async Task HandlePerformanceEvent(PerformanceEvent perfEvent, IApplication application)
        {
            switch (perfEvent.PerformanceType)
            {
                case PerformanceEventType.FrameRendered:
                    RecordFrame(perfEvent.Duration);
                    break;

                case PerformanceEventType.EventProcessed:
                    RecordEvent(perfEvent.EventType ?? Core.Events.EventType.Key, perfEvent.Duration);
                    break;

                case PerformanceEventType.MemoryPressure:
                    await HandleMemoryPressure(application);
                    break;

                case PerformanceEventType.PerformanceWarning:
                    await HandlePerformanceWarning(perfEvent, application);
                    break;
            }
        }

        private async Task UpdateStatistics(IApplication application)
        {
            var stats = GetCurrentStats();
            application.SetGlobalState("PerformanceStats", stats);

            // Check for performance warnings
            if (stats.CurrentFPS < 30 && stats.CurrentFPS > 0)
            {
                application.SetGlobalState("PerformanceWarning", "Low frame rate detected");
            }

            if (stats.MemoryUsageMB > 100) // Arbitrary threshold
            {
                application.SetGlobalState("MemoryWarning", $"High memory usage: {stats.MemoryUsageMB:F1} MB");
            }

            await Task.CompletedTask;
        }

        private async Task PerformAutoOptimization(IApplication application)
        {
            var stats = GetCurrentStats();

            // Auto-adjust render rate if performance is poor
            if (stats.CurrentFPS < 30 && stats.AverageFrameTime > TimeSpan.FromMilliseconds(20))
            {
                // Suggest reducing render rate
                var suggestedFPS = Math.Max(15, stats.CurrentFPS - 5);
                application.SetGlobalState("SuggestedRenderRate", suggestedFPS);
            }
            else if (stats.CurrentFPS > 50 && stats.AverageFrameTime < TimeSpan.FromMilliseconds(10))
            {
                // Performance is good, could potentially increase quality
                application.SetGlobalState("PerformanceHeadroom", true);
            }

            await Task.CompletedTask;
        }

        private async Task HandleMemoryPressure(IApplication application)
        {
            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            application.SetGlobalState("MemoryCleanupPerformed", DateTime.UtcNow);
            await Task.CompletedTask;
        }

        private async Task HandlePerformanceWarning(PerformanceEvent perfEvent, IApplication application)
        {
            application.SetGlobalState("LastPerformanceWarning", new
            {
                Message = perfEvent.Message,
                Timestamp = DateTime.UtcNow,
                Details = perfEvent.Details
            });

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Performance statistics for the application.
    /// </summary>
    public class PerformanceStats
    {
        /// <summary>
        /// Gets or sets the application uptime.
        /// </summary>
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// Gets or sets the total number of frames rendered.
        /// </summary>
        public long TotalFrames { get; set; }

        /// <summary>
        /// Gets or sets the total number of events processed.
        /// </summary>
        public long TotalEvents { get; set; }

        /// <summary>
        /// Gets or sets the current frames per second.
        /// </summary>
        public int CurrentFPS { get; set; }

        /// <summary>
        /// Gets or sets the average frame render time.
        /// </summary>
        public TimeSpan AverageFrameTime { get; set; }

        /// <summary>
        /// Gets or sets the minimum frame render time.
        /// </summary>
        public TimeSpan MinFrameTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum frame render time.
        /// </summary>
        public TimeSpan MaxFrameTime { get; set; }

        /// <summary>
        /// Gets or sets the average event processing time.
        /// </summary>
        public TimeSpan AverageEventTime { get; set; }

        /// <summary>
        /// Gets or sets the events processed per second.
        /// </summary>
        public int EventsPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the memory usage in megabytes.
        /// </summary>
        public double MemoryUsageMB { get; set; }

        /// <summary>
        /// Gets or sets the garbage collector memory usage in megabytes.
        /// </summary>
        public double GCMemoryMB { get; set; }
    }

    /// <summary>
    /// Information about a rendered frame.
    /// </summary>
    internal class FrameInfo
    {
        public DateTime Timestamp { get; set; }
        public TimeSpan RenderTime { get; set; }
        public long FrameNumber { get; set; }
    }

    /// <summary>
    /// Information about a processed event.
    /// </summary>
    internal class EventInfo
    {
        public DateTime Timestamp { get; set; }
        public EventType EventType { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    /// <summary>
    /// Performance-related event.
    /// </summary>
    internal class PerformanceEvent : IEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Core.Events.EventType Type => Core.Events.EventType.Key; // Placeholder
        public bool IsHandled { get; private set; }

        public PerformanceEventType PerformanceType { get; set; }
        public TimeSpan Duration { get; set; }
        public Core.Events.EventType? EventType { get; set; }
        public string? Message { get; set; }
        public object? Details { get; set; }

        public void Handle() => IsHandled = true;
    }

    /// <summary>
    /// Types of performance events.
    /// </summary>
    internal enum PerformanceEventType
    {
        FrameRendered,
        EventProcessed,
        MemoryPressure,
        PerformanceWarning
    }
}