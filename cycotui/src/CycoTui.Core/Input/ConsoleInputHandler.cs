using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui.Core.Input
{
    /// <summary>
    /// Cross-platform console input handler that captures keyboard events.
    /// </summary>
    public class ConsoleInputHandler : IInputHandler
    {
        private readonly IInputConfiguration _configuration;
        private readonly ConcurrentQueue<IEvent> _inputBuffer;
        private readonly SemaphoreSlim _eventSemaphore;
        private readonly object _lockObject;

        private IEventQueue? _eventQueue;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _inputTask;
        private volatile bool _isRunning;
        private volatile bool _disposed;

        private bool _mouseEnabled;
        private bool _focusEnabled;
        private bool _pasteEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleInputHandler"/> class.
        /// </summary>
        /// <param name="configuration">The input configuration.</param>
        public ConsoleInputHandler(IInputConfiguration? configuration = null)
        {
            _configuration = configuration ?? new InputConfiguration();
            _inputBuffer = new ConcurrentQueue<IEvent>();
            _eventSemaphore = new SemaphoreSlim(0);
            _lockObject = new object();

            _mouseEnabled = _configuration.MouseEnabled;
            _focusEnabled = _configuration.FocusEnabled;
            _pasteEnabled = _configuration.PasteEnabled;
        }

        /// <inheritdoc />
        public bool IsRunning => _isRunning;

        /// <inheritdoc />
        public bool SupportsMouseEvents => false; // Console doesn't support mouse events

        /// <inheritdoc />
        public bool SupportsFocusEvents => false; // Basic console doesn't support focus events

        /// <inheritdoc />
        public bool SupportsPasteEvents => false; // Basic console doesn't support paste detection

        /// <inheritdoc />
        public event System.EventHandler<InputErrorEventArgs>? InputError;

        /// <inheritdoc />
        public async Task StartAsync(IEventQueue eventQueue, CancellationToken cancellationToken = default)
        {
            if (_isRunning)
                throw new InvalidOperationException("Input handler is already running.");

            if (_disposed)
                throw new ObjectDisposedException(nameof(ConsoleInputHandler));

            _eventQueue = eventQueue ?? throw new ArgumentNullException(nameof(eventQueue));

            lock (_lockObject)
            {
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _isRunning = true;
            }

            _inputTask = Task.Run(() => InputProcessingLoop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

            try
            {
                await _inputTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            finally
            {
                Stop();
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (!_isRunning)
                return;

            lock (_lockObject)
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
            }

            try
            {
                _inputTask?.Wait(TimeSpan.FromSeconds(1));
            }
            catch (AggregateException)
            {
                // Ignore cancellation exceptions
            }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _inputTask = null;
            _eventQueue = null;
        }

        /// <inheritdoc />
        public IEvent? PollEvent(TimeSpan timeout)
        {
            if (_disposed)
                return null;

            var endTime = DateTime.UtcNow.Add(timeout);

            while (DateTime.UtcNow < endTime)
            {
                if (_inputBuffer.TryDequeue(out var bufferedEvent))
                    return bufferedEvent;

                try
                {
                    if (Console.KeyAvailable)
                    {
                        var consoleKeyInfo = Console.ReadKey(true);
                        var keyEvent = ConvertToKeyEvent(consoleKeyInfo);
                        if (keyEvent != null)
                            return keyEvent;
                    }
                }
                catch (InvalidOperationException)
                {
                    // Console.KeyAvailable not available in test environments or when redirected
                    break;
                }

                Thread.Sleep(1);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<IEvent> ReadEventAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ConsoleInputHandler));

            // First check buffered events
            if (_inputBuffer.TryDequeue(out var bufferedEvent))
                return bufferedEvent;

            // Wait for a new event
            await _eventSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            if (_inputBuffer.TryDequeue(out var @event))
                return @event;

            throw new InvalidOperationException("Event semaphore was signaled but no event was available.");
        }

        /// <inheritdoc />
        public void SetMouseEnabled(bool enable)
        {
            _mouseEnabled = enable;
            // Console input handler doesn't support mouse events
        }

        /// <inheritdoc />
        public void SetFocusEnabled(bool enable)
        {
            _focusEnabled = enable;
            // Console input handler doesn't support focus events
        }

        /// <inheritdoc />
        public void SetPasteEnabled(bool enable)
        {
            _pasteEnabled = enable;
            // Console input handler doesn't support paste events
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ConsoleInputHandler"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    _eventSemaphore?.Dispose();
                }

                _disposed = true;
            }
        }

        private async Task InputProcessingLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _isRunning)
                {
                    try
                    {
                        // Check for console key input
                        try
                        {
                            if (Console.KeyAvailable)
                            {
                                var consoleKeyInfo = Console.ReadKey(true);
                                var keyEvent = ConvertToKeyEvent(consoleKeyInfo);

                                if (keyEvent != null)
                                {
                                    _eventQueue?.Enqueue(keyEvent);
                                    _inputBuffer.Enqueue(keyEvent);
                                    _eventSemaphore.Release();
                                }
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // Console.KeyAvailable not available in test environments or when redirected
                            // Just continue without reading console input
                        }

                        // Check for window resize (simplified detection)
                        await CheckForResizeEvent().ConfigureAwait(false);

                        // Small delay to prevent busy waiting
                        await Task.Delay(_configuration.PollingInterval, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (!(ex is OperationCanceledException))
                    {
                        OnInputError(ex, "Error during input processing");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        private KeyEvent? ConvertToKeyEvent(ConsoleKeyInfo keyInfo)
        {
            var key = ConvertConsoleKey(keyInfo.Key);
            if (key == Key.Unknown)
                return null;

            var modifiers = ConvertModifiers(keyInfo.Modifiers);
            return new KeyEvent(key, modifiers);
        }

        private static Key ConvertConsoleKey(ConsoleKey consoleKey)
        {
            return consoleKey switch
            {
                // Letters
                ConsoleKey.A => Key.A,
                ConsoleKey.B => Key.B,
                ConsoleKey.C => Key.C,
                ConsoleKey.D => Key.D,
                ConsoleKey.E => Key.E,
                ConsoleKey.F => Key.F,
                ConsoleKey.G => Key.G,
                ConsoleKey.H => Key.H,
                ConsoleKey.I => Key.I,
                ConsoleKey.J => Key.J,
                ConsoleKey.K => Key.K,
                ConsoleKey.L => Key.L,
                ConsoleKey.M => Key.M,
                ConsoleKey.N => Key.N,
                ConsoleKey.O => Key.O,
                ConsoleKey.P => Key.P,
                ConsoleKey.Q => Key.Q,
                ConsoleKey.R => Key.R,
                ConsoleKey.S => Key.S,
                ConsoleKey.T => Key.T,
                ConsoleKey.U => Key.U,
                ConsoleKey.V => Key.V,
                ConsoleKey.W => Key.W,
                ConsoleKey.X => Key.X,
                ConsoleKey.Y => Key.Y,
                ConsoleKey.Z => Key.Z,

                // Numbers
                ConsoleKey.D0 => Key.D0,
                ConsoleKey.D1 => Key.D1,
                ConsoleKey.D2 => Key.D2,
                ConsoleKey.D3 => Key.D3,
                ConsoleKey.D4 => Key.D4,
                ConsoleKey.D5 => Key.D5,
                ConsoleKey.D6 => Key.D6,
                ConsoleKey.D7 => Key.D7,
                ConsoleKey.D8 => Key.D8,
                ConsoleKey.D9 => Key.D9,

                // Function keys
                ConsoleKey.F1 => Key.F1,
                ConsoleKey.F2 => Key.F2,
                ConsoleKey.F3 => Key.F3,
                ConsoleKey.F4 => Key.F4,
                ConsoleKey.F5 => Key.F5,
                ConsoleKey.F6 => Key.F6,
                ConsoleKey.F7 => Key.F7,
                ConsoleKey.F8 => Key.F8,
                ConsoleKey.F9 => Key.F9,
                ConsoleKey.F10 => Key.F10,
                ConsoleKey.F11 => Key.F11,
                ConsoleKey.F12 => Key.F12,

                // Navigation
                ConsoleKey.UpArrow => Key.Up,
                ConsoleKey.DownArrow => Key.Down,
                ConsoleKey.LeftArrow => Key.Left,
                ConsoleKey.RightArrow => Key.Right,
                ConsoleKey.Home => Key.Home,
                ConsoleKey.End => Key.End,
                ConsoleKey.PageUp => Key.PageUp,
                ConsoleKey.PageDown => Key.PageDown,

                // Special keys
                ConsoleKey.Enter => Key.Enter,
                ConsoleKey.Escape => Key.Escape,
                ConsoleKey.Tab => Key.Tab,
                ConsoleKey.Spacebar => Key.Space,
                ConsoleKey.Backspace => Key.Backspace,
                ConsoleKey.Delete => Key.Delete,
                ConsoleKey.Insert => Key.Insert,

                // Punctuation (some OEM keys not available in .NET Standard 2.1)
                ConsoleKey.OemPeriod => Key.Period,
                ConsoleKey.OemComma => Key.Comma,

                _ => Key.Unknown
            };
        }

        private static KeyModifiers ConvertModifiers(ConsoleModifiers consoleModifiers)
        {
            var modifiers = KeyModifiers.None;

            if (consoleModifiers.HasFlag(ConsoleModifiers.Shift))
                modifiers |= KeyModifiers.Shift;

            if (consoleModifiers.HasFlag(ConsoleModifiers.Control))
                modifiers |= KeyModifiers.Control;

            if (consoleModifiers.HasFlag(ConsoleModifiers.Alt))
                modifiers |= KeyModifiers.Alt;

            return modifiers;
        }

        private int _lastConsoleWidth = -1;
        private int _lastConsoleHeight = -1;

        private async Task CheckForResizeEvent()
        {
            await Task.Yield(); // Make this method async

            try
            {
                var currentWidth = Console.WindowWidth;
                var currentHeight = Console.WindowHeight;

                if (_lastConsoleWidth == -1)
                {
                    _lastConsoleWidth = currentWidth;
                    _lastConsoleHeight = currentHeight;
                    return;
                }

                if (currentWidth != _lastConsoleWidth || currentHeight != _lastConsoleHeight)
                {
                    var resizeEvent = new ResizeEvent(currentWidth, currentHeight);
                    _eventQueue?.Enqueue(resizeEvent);
                    _inputBuffer.Enqueue(resizeEvent);
                    _eventSemaphore.Release();

                    _lastConsoleWidth = currentWidth;
                    _lastConsoleHeight = currentHeight;
                }
            }
            catch (Exception ex)
            {
                // Console size might not be available in some environments
                OnInputError(ex, "Error checking console size for resize events");
            }
        }

        private void OnInputError(Exception exception, string? context = null)
        {
            InputError?.Invoke(this, new InputErrorEventArgs(exception, context));
        }
    }
}