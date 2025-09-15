using System;
using Xunit;
using CycoAI.CycoTui.Core.Events;

namespace CycoTui.Tests.Events
{
    public class EventRouterTests
    {
        [Fact]
        public void RegisterHandler_WithValidHandler_AddsHandler()
        {
            var router = new EventRouter();
            var handlerCalled = false;

            router.RegisterHandler<KeyEvent>(e => { handlerCalled = true; return true; });

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.True(result);
            Assert.True(handlerCalled);
            Assert.True(keyEvent.IsHandled);
        }

        [Fact]
        public void RegisterHandler_WithNullHandler_ThrowsArgumentNullException()
        {
            var router = new EventRouter();

            Assert.Throws<ArgumentNullException>(() =>
                router.RegisterHandler<KeyEvent>(null!));
        }

        [Fact]
        public void UnregisterHandler_WithExistingHandler_RemovesHandler()
        {
            var router = new EventRouter();
            var handlerCalled = false;

            CycoAI.CycoTui.Core.Events.EventHandler<KeyEvent> handler = e => { handlerCalled = true; return true; };
            router.RegisterHandler(handler);
            router.UnregisterHandler(handler);

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.False(result);
            Assert.False(handlerCalled);
            Assert.False(keyEvent.IsHandled);
        }

        [Fact]
        public void UnregisterHandler_WithNullHandler_ThrowsArgumentNullException()
        {
            var router = new EventRouter();

            Assert.Throws<ArgumentNullException>(() =>
                router.UnregisterHandler<KeyEvent>(null!));
        }

        [Fact]
        public void RouteEvent_WithMultipleHandlers_CallsAllHandlers()
        {
            var router = new EventRouter();
            var handler1Called = false;
            var handler2Called = false;

            router.RegisterHandler<KeyEvent>(e => { handler1Called = true; return false; });
            router.RegisterHandler<KeyEvent>(e => { handler2Called = true; return true; });

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.True(result);
            Assert.True(handler1Called);
            Assert.True(handler2Called);
            Assert.True(keyEvent.IsHandled);
        }

        [Fact]
        public void RouteEvent_WithHandlerReturningTrue_StopsProcessing()
        {
            var router = new EventRouter();
            var handler1Called = false;
            var handler2Called = false;

            router.RegisterHandler<KeyEvent>(e => { handler1Called = true; return true; });
            router.RegisterHandler<KeyEvent>(e => { handler2Called = true; return false; });

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.True(result);
            Assert.True(handler1Called);
            Assert.False(handler2Called); // Should not be called after first handler returns true
            Assert.True(keyEvent.IsHandled);
        }

        [Fact]
        public void RouteEvent_WithNullEvent_ReturnsFalse()
        {
            var router = new EventRouter();

            var result = router.RouteEvent(null!);

            Assert.False(result);
        }

        [Fact]
        public void RouteEvent_WithNoHandlers_ReturnsFalse()
        {
            var router = new EventRouter();
            var keyEvent = new KeyEvent(Key.A);

            var result = router.RouteEvent(keyEvent);

            Assert.False(result);
            Assert.False(keyEvent.IsHandled);
        }

        [Fact]
        public void AddFilter_WithValidFilter_AddsFilter()
        {
            var router = new EventRouter();
            var filterCalled = false;

            var filter = new TestFilter(() => { filterCalled = true; return null; });
            router.AddFilter(filter);

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.True(result); // Event was filtered out (considered handled)
            Assert.True(filterCalled);
        }

        [Fact]
        public void AddFilter_WithNullFilter_ThrowsArgumentNullException()
        {
            var router = new EventRouter();

            Assert.Throws<ArgumentNullException>(() => router.AddFilter(null!));
        }

        [Fact]
        public void RemoveFilter_WithExistingFilter_RemovesFilter()
        {
            var router = new EventRouter();
            var filterCalled = false;

            var filter = new TestFilter(() => { filterCalled = true; return null; });
            router.AddFilter(filter);
            router.RemoveFilter(filter);

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.False(result);
            Assert.False(filterCalled);
        }

        [Fact]
        public void RemoveFilter_WithNullFilter_ThrowsArgumentNullException()
        {
            var router = new EventRouter();

            Assert.Throws<ArgumentNullException>(() => router.RemoveFilter(null!));
        }

        [Fact]
        public void AddFilter_WithMultipleFilters_SortsByPriority()
        {
            var router = new EventRouter();
            var callOrder = 0;
            var filter1Order = 0;
            var filter2Order = 0;

            var filter1 = new TestFilter(() => { filter1Order = ++callOrder; return new KeyEvent(Key.B); }, priority: 10);
            var filter2 = new TestFilter(() => { filter2Order = ++callOrder; return new KeyEvent(Key.C); }, priority: 5);

            router.AddFilter(filter1);
            router.AddFilter(filter2);

            var keyEvent = new KeyEvent(Key.A);
            router.RouteEvent(keyEvent);

            Assert.Equal(1, filter2Order); // Higher priority (lower number) should be called first
            Assert.Equal(2, filter1Order);
        }

        [Fact]
        public void PredicateEventFilter_WithMatchingPredicate_ReturnsEvent()
        {
            var filter = new PredicateEventFilter(e => e.Type == EventType.Key);
            var keyEvent = new KeyEvent(Key.A);

            var result = filter.FilterEvent(keyEvent);

            Assert.Same(keyEvent, result);
        }

        [Fact]
        public void PredicateEventFilter_WithNonMatchingPredicate_ReturnsNull()
        {
            var filter = new PredicateEventFilter(e => e.Type == EventType.Mouse);
            var keyEvent = new KeyEvent(Key.A);

            var result = filter.FilterEvent(keyEvent);

            Assert.Null(result);
        }

        [Fact]
        public void TransformEventFilter_WithTransform_ReturnsTransformedEvent()
        {
            var transformedEvent = new ResizeEvent(100, 50);
            var filter = new TransformEventFilter(_ => transformedEvent);
            var keyEvent = new KeyEvent(Key.A);

            var result = filter.FilterEvent(keyEvent);

            Assert.Same(transformedEvent, result);
        }

        [Fact]
        public void TransformEventFilter_WithNullTransform_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TransformEventFilter(null!));
        }

        [Fact]
        public void Clear_RemovesAllHandlersAndFilters()
        {
            var router = new EventRouter();
            var handlerCalled = false;
            var filterCalled = false;

            router.RegisterHandler<KeyEvent>(e => { handlerCalled = true; return true; });
            router.AddFilter(new TestFilter(() => { filterCalled = true; return null; }));

            router.Clear();

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.False(result);
            Assert.False(handlerCalled);
            Assert.False(filterCalled);
        }

        [Fact]
        public void RouteEvent_WithExceptionInHandler_ContinuesProcessing()
        {
            var router = new EventRouter();
            var secondHandlerCalled = false;

            router.RegisterHandler<KeyEvent>(e => throw new Exception("Test exception"));
            router.RegisterHandler<KeyEvent>(e => { secondHandlerCalled = true; return true; });

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.True(result);
            Assert.True(secondHandlerCalled);
            Assert.True(keyEvent.IsHandled);
        }

        [Fact]
        public void RouteEvent_WithExceptionInFilter_ContinuesWithOriginalEvent()
        {
            var router = new EventRouter();
            var handlerCalled = false;

            router.AddFilter(new TestFilter(() => throw new Exception("Test exception")));
            router.RegisterHandler<KeyEvent>(e => { handlerCalled = true; return true; });

            var keyEvent = new KeyEvent(Key.A);
            var result = router.RouteEvent(keyEvent);

            Assert.True(result);
            Assert.True(handlerCalled);
            Assert.True(keyEvent.IsHandled);
        }

        private class TestFilter : IEventFilter
        {
            private readonly Func<IEvent?> _filterAction;

            public TestFilter(Func<IEvent?> filterAction, int priority = 0)
            {
                _filterAction = filterAction;
                Priority = priority;
            }

            public int Priority { get; }

            public IEvent? FilterEvent(IEvent @event)
            {
                return _filterAction();
            }
        }
    }
}