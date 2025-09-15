using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using CycoAI.CycoTui.Core.Events;
using CycoAI.CycoTui.Core.Input;

namespace CycoTui.Tests.Input
{
    public class InputTests
    {
        [Fact]
        public void InputConfiguration_DefaultValues_AreCorrect()
        {
            var config = new InputConfiguration();

            Assert.True(config.MouseEnabled);
            Assert.True(config.FocusEnabled);
            Assert.True(config.PasteEnabled);
            Assert.Equal(1024, config.BufferSize);
            Assert.Equal(TimeSpan.FromMilliseconds(16), config.PollingInterval);
        }

        [Fact]
        public void InputErrorEventArgs_Constructor_SetsProperties()
        {
            var exception = new Exception("Test exception");
            var context = "Test context";

            var args = new InputErrorEventArgs(exception, context);

            Assert.Same(exception, args.Exception);
            Assert.Equal(context, args.Context);
        }

        [Fact]
        public void InputErrorEventArgs_WithNullException_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new InputErrorEventArgs(null!));
        }

        [Fact]
        public void ConsoleInputHandler_Constructor_SetsDefaultProperties()
        {
            using var handler = new ConsoleInputHandler();

            Assert.False(handler.IsRunning);
            Assert.False(handler.SupportsMouseEvents);
            Assert.False(handler.SupportsFocusEvents);
            Assert.False(handler.SupportsPasteEvents);
        }

        [Fact]
        public void ConsoleInputHandler_WithCustomConfiguration_UsesConfiguration()
        {
            var config = new InputConfiguration
            {
                MouseEnabled = false,
                FocusEnabled = false,
                PasteEnabled = false,
                BufferSize = 512,
                PollingInterval = TimeSpan.FromMilliseconds(32)
            };

            using var handler = new ConsoleInputHandler(config);

            Assert.False(handler.IsRunning);
        }

        [Fact]
        public async Task ConsoleInputHandler_StartAsync_WithNullEventQueue_ThrowsArgumentNullException()
        {
            using var handler = new ConsoleInputHandler();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.StartAsync(null!));
        }

        [Fact]
        public async Task ConsoleInputHandler_StartAsync_WhenAlreadyRunning_ThrowsInvalidOperationException()
        {
            using var handler = new ConsoleInputHandler();
            using var eventQueue = new EventQueue();
            using var cts = new CancellationTokenSource();

            var startTask = handler.StartAsync(eventQueue, cts.Token);

            // Wait a moment for the handler to start
            await Task.Delay(10);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.StartAsync(eventQueue, cts.Token));

            cts.Cancel();
            try { await startTask; } catch (OperationCanceledException) { }
        }

        [Fact]
        public void ConsoleInputHandler_Stop_WhenNotRunning_DoesNotThrow()
        {
            using var handler = new ConsoleInputHandler();

            // Should not throw
            handler.Stop();
        }

        [Fact]
        public void ConsoleInputHandler_PollEvent_WithoutEvents_ReturnsNull()
        {
            using var handler = new ConsoleInputHandler();

            var result = handler.PollEvent(TimeSpan.FromMilliseconds(10));

            Assert.Null(result);
        }

        [Fact]
        public async Task ConsoleInputHandler_ReadEventAsync_WhenDisposed_ThrowsObjectDisposedException()
        {
            var handler = new ConsoleInputHandler();
            handler.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                handler.ReadEventAsync());
        }

        [Fact]
        public void ConsoleInputHandler_SetMethods_DoNotThrow()
        {
            using var handler = new ConsoleInputHandler();

            // These methods should not throw for console handler
            handler.SetMouseEnabled(true);
            handler.SetFocusEnabled(true);
            handler.SetPasteEnabled(true);
        }

        [Fact]
        public void InputManager_Constructor_WithValidParameters_InitializesCorrectly()
        {
            using var handler = new TestInputHandler();
            using var eventQueue = new EventQueue();
            var eventRouter = new EventRouter();

            using var manager = new InputManager(handler, eventQueue, eventRouter);

            Assert.Same(handler, manager.InputHandler);
            Assert.Same(eventQueue, manager.EventQueue);
            Assert.Same(eventRouter, manager.EventRouter);
            Assert.False(manager.IsRunning);
        }

        [Theory]
        [InlineData(null, typeof(IEventQueue), typeof(IEventRouter))]
        [InlineData(typeof(IInputHandler), null, typeof(IEventRouter))]
        [InlineData(typeof(IInputHandler), typeof(IEventQueue), null)]
        public void InputManager_Constructor_WithNullParameters_ThrowsArgumentNullException(
            Type? handlerType, Type? queueType, Type? routerType)
        {
            var handler = handlerType != null ? new TestInputHandler() : null;
            var eventQueue = queueType != null ? new EventQueue() : null;
            var eventRouter = routerType != null ? new EventRouter() : null;

            Assert.Throws<ArgumentNullException>(() =>
                new InputManager(handler!, eventQueue!, eventRouter!));

            handler?.Dispose();
            eventQueue?.Dispose();
        }

        [Fact]
        public void InputManager_AddInputFilter_WithValidFilter_AddsFilter()
        {
            using var handler = new TestInputHandler();
            using var eventQueue = new EventQueue();
            var eventRouter = new EventRouter();
            using var manager = new InputManager(handler, eventQueue, eventRouter);

            var filter = new TestInputFilter();

            // Should not throw
            manager.AddInputFilter(filter);
        }

        [Fact]
        public void InputManager_AddInputFilter_WithNullFilter_ThrowsArgumentNullException()
        {
            using var handler = new TestInputHandler();
            using var eventQueue = new EventQueue();
            var eventRouter = new EventRouter();
            using var manager = new InputManager(handler, eventQueue, eventRouter);

            Assert.Throws<ArgumentNullException>(() => manager.AddInputFilter(null!));
        }

        [Fact]
        public void InputManager_RegisterHandler_WithValidHandler_RegistersHandler()
        {
            using var handler = new TestInputHandler();
            using var eventQueue = new EventQueue();
            var eventRouter = new EventRouter();
            using var manager = new InputManager(handler, eventQueue, eventRouter);

            var handlerCalled = false;
            manager.RegisterHandler<KeyEvent>(e => { handlerCalled = true; return true; });

            // Test that the handler was registered by routing an event
            var keyEvent = new KeyEvent(Key.A);
            var result = eventRouter.RouteEvent(keyEvent);

            Assert.True(result);
            Assert.True(handlerCalled);
        }

        [Fact]
        public void InputManager_SetMethods_CallsHandlerMethods()
        {
            using var handler = new TestInputHandler();
            using var eventQueue = new EventQueue();
            var eventRouter = new EventRouter();
            using var manager = new InputManager(handler, eventQueue, eventRouter);

            // Should not throw
            manager.SetMouseEnabled(true);
            manager.SetFocusEnabled(false);
            manager.SetPasteEnabled(true);

            Assert.True(handler.MouseEnabledSet);
            Assert.True(handler.FocusEnabledSet);
            Assert.True(handler.PasteEnabledSet);
        }

        [Fact]
        public void PredicateInputFilter_WithMatchingPredicate_ReturnsEvent()
        {
            var filter = new PredicateInputFilter(e => e.Type == EventType.Key);
            var keyEvent = new KeyEvent(Key.A);

            var result = filter.FilterEvent(keyEvent);

            Assert.Same(keyEvent, result);
        }

        [Fact]
        public void PredicateInputFilter_WithNonMatchingPredicate_ReturnsNull()
        {
            var filter = new PredicateInputFilter(e => e.Type == EventType.Mouse);
            var keyEvent = new KeyEvent(Key.A);

            var result = filter.FilterEvent(keyEvent);

            Assert.Null(result);
        }

        [Fact]
        public void TransformInputFilter_WithTransform_ReturnsTransformedEvent()
        {
            var transformedEvent = new ResizeEvent(100, 50);
            var filter = new TransformInputFilter(_ => transformedEvent);
            var keyEvent = new KeyEvent(Key.A);

            var result = filter.FilterEvent(keyEvent);

            Assert.Same(transformedEvent, result);
        }

        private class TestInputHandler : IInputHandler
        {
            public bool IsRunning { get; private set; }
            public bool SupportsMouseEvents => true;
            public bool SupportsFocusEvents => true;
            public bool SupportsPasteEvents => true;

            public bool MouseEnabledSet { get; private set; }
            public bool FocusEnabledSet { get; private set; }
            public bool PasteEnabledSet { get; private set; }

            public event System.EventHandler<InputErrorEventArgs>? InputError;

            public void Dispose() { }

            public IEvent? PollEvent(TimeSpan timeout) => null;

            public Task<IEvent> ReadEventAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IEvent>(new KeyEvent(Key.A));
            }

            public void SetFocusEnabled(bool enable) => FocusEnabledSet = true;
            public void SetMouseEnabled(bool enable) => MouseEnabledSet = true;
            public void SetPasteEnabled(bool enable) => PasteEnabledSet = true;

            public Task StartAsync(IEventQueue eventQueue, CancellationToken cancellationToken = default)
            {
                IsRunning = true;
                return Task.CompletedTask;
            }

            public void Stop() => IsRunning = false;
        }

        private class TestInputFilter : IInputFilter
        {
            public int Priority => 0;

            public IEvent? FilterEvent(IEvent @event) => @event;
        }
    }
}