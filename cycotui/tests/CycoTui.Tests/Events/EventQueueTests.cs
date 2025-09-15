using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using CycoAI.CycoTui.Core.Events;

namespace CycoTui.Tests.Events
{
    public class EventQueueTests : IDisposable
    {
        private readonly EventQueue _eventQueue;

        public EventQueueTests()
        {
            _eventQueue = new EventQueue();
        }

        public void Dispose()
        {
            _eventQueue?.Dispose();
        }

        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            Assert.Equal(0, _eventQueue.Count);
        }

        [Fact]
        public void Enqueue_WithValidEvent_AddsToQueue()
        {
            var testEvent = new TestEvent();
            _eventQueue.Enqueue(testEvent);

            Assert.Equal(1, _eventQueue.Count);
        }

        [Fact]
        public void Enqueue_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _eventQueue.Enqueue(null!));
        }

        [Fact]
        public void TryDequeue_WithEvents_ReturnsTrue()
        {
            var testEvent = new TestEvent();
            _eventQueue.Enqueue(testEvent);

            var result = _eventQueue.TryDequeue(out var dequeuedEvent);

            Assert.True(result);
            Assert.Same(testEvent, dequeuedEvent);
            Assert.Equal(0, _eventQueue.Count);
        }

        [Fact]
        public void TryDequeue_WithoutEvents_ReturnsFalse()
        {
            var result = _eventQueue.TryDequeue(out var dequeuedEvent);

            Assert.False(result);
            Assert.Null(dequeuedEvent);
        }

        [Fact]
        public void DrainAll_WithMultipleEvents_ReturnsAllEvents()
        {
            var event1 = new TestEvent();
            var event2 = new TestEvent();
            var event3 = new TestEvent();

            _eventQueue.Enqueue(event1);
            _eventQueue.Enqueue(event2);
            _eventQueue.Enqueue(event3);

            var events = _eventQueue.DrainAll();

            Assert.Equal(3, events.Count);
            Assert.Equal(0, _eventQueue.Count);
            Assert.Contains(event1, events);
            Assert.Contains(event2, events);
            Assert.Contains(event3, events);
        }

        [Fact]
        public void DrainAll_WithoutEvents_ReturnsEmptyCollection()
        {
            var events = _eventQueue.DrainAll();

            Assert.Empty(events);
        }

        [Fact]
        public void Clear_WithEvents_RemovesAllEvents()
        {
            _eventQueue.Enqueue(new TestEvent());
            _eventQueue.Enqueue(new TestEvent());

            _eventQueue.Clear();

            Assert.Equal(0, _eventQueue.Count);
        }

        [Fact]
        public async Task WaitForEventAsync_WithExistingEvent_ReturnsImmediately()
        {
            var testEvent = new TestEvent();
            _eventQueue.Enqueue(testEvent);

            var result = await _eventQueue.WaitForEventAsync();

            Assert.Same(testEvent, result);
        }

        [Fact]
        public async Task WaitForEventAsync_WithNewEvent_WaitsForEvent()
        {
            var testEvent = new TestEvent();

            // Start waiting for event
            var waitTask = _eventQueue.WaitForEventAsync();

            // Add event after a short delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(50);
                _eventQueue.Enqueue(testEvent);
            });

            var result = await waitTask;

            Assert.Same(testEvent, result);
        }

        [Fact]
        public async Task WaitForEventAsync_WithCancellation_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource(100);

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                _eventQueue.WaitForEventAsync(cts.Token));
        }

        [Fact]
        public void FilterEvents_WithMatchingPredicate_ReturnsMatchingEvents()
        {
            var keyEvent = new KeyEvent(Key.A);
            var resizeEvent = new ResizeEvent(80, 24);
            var mouseEvent = new MouseEvent(MouseEventKind.Down, MouseButton.Left, 10, 20);

            _eventQueue.Enqueue(keyEvent);
            _eventQueue.Enqueue(resizeEvent);
            _eventQueue.Enqueue(mouseEvent);

            var filteredEvents = _eventQueue.FilterEvents(e => e.Type == EventType.Key);

            Assert.Single(filteredEvents);
            Assert.Same(keyEvent, filteredEvents.First());
            Assert.Equal(2, _eventQueue.Count); // Non-matching events should remain
        }

        [Fact]
        public void FilterEvents_WithNullPredicate_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _eventQueue.FilterEvents(null!));
        }

        [Fact]
        public void FilterEvents_WithNoMatchingEvents_ReturnsEmptyCollection()
        {
            var resizeEvent = new ResizeEvent(80, 24);
            _eventQueue.Enqueue(resizeEvent);

            var filteredEvents = _eventQueue.FilterEvents(e => e.Type == EventType.Key);

            Assert.Empty(filteredEvents);
            Assert.Equal(1, _eventQueue.Count); // Original event should remain
        }

        [Fact]
        public void Dispose_WhenDisposed_OperationsReturnSafely()
        {
            _eventQueue.Dispose();

            // These operations should not throw and should return safe defaults
            _eventQueue.Enqueue(new TestEvent()); // Should be ignored
            Assert.Equal(0, _eventQueue.Count);

            var result = _eventQueue.TryDequeue(out var @event);
            Assert.False(result);
            Assert.Null(@event);

            var events = _eventQueue.DrainAll();
            Assert.Empty(events);

            var filteredEvents = _eventQueue.FilterEvents(e => true);
            Assert.Empty(filteredEvents);
        }

        [Fact]
        public async Task WaitForEventAsync_WhenDisposed_ThrowsObjectDisposedException()
        {
            _eventQueue.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                _eventQueue.WaitForEventAsync());
        }

        private class TestEvent : BaseEvent
        {
            public TestEvent() : base(EventType.Key) { }
        }
    }
}