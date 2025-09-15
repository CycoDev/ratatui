using System;
using System.Text;
using CycoAI.CycoTui.Core.Backend;
using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using Microsoft.Extensions.Logging;

namespace CycoAI.CycoTui.Core.Terminal
{
    /// <summary>
    /// High-level terminal management class that coordinates backend operations and buffer rendering.
    /// Provides the main interface for terminal UI applications.
    /// </summary>
    public class Terminal : IDisposable
    {
        private readonly IBackend _backend;
        private readonly ILogger<Terminal>? _logger;
        private Buffer.Buffer? _currentBuffer;
        private Buffer.Buffer? _previousBuffer;
        private bool _disposed;

        /// <summary>
        /// Gets the backend used by this terminal.
        /// </summary>
        public IBackend Backend => _backend;

        /// <summary>
        /// Gets the current size of the terminal.
        /// </summary>
        public Rect Size
        {
            get
            {
                var (width, height) = _backend.GetSize();
                return new Rect(0, 0, width, height);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the terminal is in alternate screen mode.
        /// </summary>
        public bool IsInAlternateScreen { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the terminal is in raw mode.
        /// </summary>
        public bool IsInRawMode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Terminal"/> class.
        /// </summary>
        /// <param name="backend">The backend to use for terminal operations.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        /// <exception cref="ArgumentNullException">Thrown when backend is null.</exception>
        public Terminal(IBackend backend, ILogger<Terminal>? logger = null)
        {
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));
            _logger = logger;

            var size = Size;
            _currentBuffer = new Buffer.Buffer(size.Width, size.Height);
            _previousBuffer = new Buffer.Buffer(size.Width, size.Height);

            _logger?.LogDebug("Terminal initialized with size {Width}x{Height}", size.Width, size.Height);
        }

        /// <summary>
        /// Enters alternate screen mode and enables raw mode for full-screen terminal applications.
        /// </summary>
        public void EnterFullScreen()
        {
            try
            {
                _backend.EnterAlternateScreen();
                IsInAlternateScreen = true;

                _backend.EnableRawMode();
                IsInRawMode = true;

                _backend.HideCursor();
                _backend.Clear();

                _logger?.LogDebug("Entered full-screen mode");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to enter full-screen mode");
                throw;
            }
        }

        /// <summary>
        /// Exits alternate screen mode and disables raw mode, restoring normal terminal operation.
        /// </summary>
        public void ExitFullScreen()
        {
            try
            {
                _backend.ShowCursor();

                if (IsInRawMode)
                {
                    _backend.DisableRawMode();
                    IsInRawMode = false;
                }

                if (IsInAlternateScreen)
                {
                    _backend.LeaveAlternateScreen();
                    IsInAlternateScreen = false;
                }

                _logger?.LogDebug("Exited full-screen mode");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to exit full-screen mode");
                throw;
            }
        }

        /// <summary>
        /// Clears the terminal screen.
        /// </summary>
        public void Clear()
        {
            try
            {
                _backend.Clear();
                _currentBuffer?.Clear();
                _previousBuffer?.Clear();
                _logger?.LogDebug("Terminal cleared");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to clear terminal");
            }
        }

        /// <summary>
        /// Draws the current buffer to the terminal, only updating changed cells for efficiency.
        /// </summary>
        public void Draw()
        {
            try
            {
                if (_currentBuffer == null || _previousBuffer == null)
                    return;

                // Check if terminal size has changed
                var currentSize = Size;
                if (currentSize.Width != _currentBuffer.Width || currentSize.Height != _currentBuffer.Height)
                {
                    ResizeBuffers(currentSize.Width, currentSize.Height);
                }

                // Get dirty cells and render only what has changed
                var dirtyCells = _currentBuffer.GetDirtyCells();
                var sb = new StringBuilder();

                foreach (var (x, y, cell) in dirtyCells)
                {
                    // Position cursor
                    _backend.SetCursor(x, y);

                    // Generate ANSI escape sequences for the cell
                    var ansiSequence = GenerateAnsiSequence(cell);
                    sb.Append(ansiSequence);
                    sb.Append(cell.Character);

                    // Write to backend
                    _backend.Write(sb.ToString());
                    sb.Clear();
                }

                _backend.Flush();

                // Swap buffers and reset dirty tracking
                (_currentBuffer, _previousBuffer) = (_previousBuffer, _currentBuffer);
                _currentBuffer.Clear();
                _previousBuffer.ResetDirtyTracking();

                _logger?.LogTrace("Terminal drawn successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to draw terminal");
                throw;
            }
        }

        /// <summary>
        /// Gets the current buffer for rendering. Applications should render their content to this buffer.
        /// </summary>
        /// <returns>The current buffer.</returns>
        public IBuffer GetBuffer()
        {
            if (_currentBuffer == null)
                throw new InvalidOperationException("Terminal has been disposed.");

            return _currentBuffer;
        }

        /// <summary>
        /// Forces a complete redraw of the terminal on the next draw operation.
        /// </summary>
        public void Invalidate()
        {
            try
            {
                _currentBuffer?.MarkAllDirty();
                _logger?.LogDebug("Terminal invalidated");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to invalidate terminal");
            }
        }

        /// <summary>
        /// Sets the cursor position in the terminal.
        /// </summary>
        /// <param name="x">The x-coordinate (column).</param>
        /// <param name="y">The y-coordinate (row).</param>
        public void SetCursor(int x, int y)
        {
            try
            {
                _backend.SetCursor(x, y);
                _logger?.LogTrace("Cursor set to ({X}, {Y})", x, y);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to set cursor position");
            }
        }

        /// <summary>
        /// Shows the terminal cursor.
        /// </summary>
        public void ShowCursor()
        {
            try
            {
                _backend.ShowCursor();
                _logger?.LogTrace("Cursor shown");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to show cursor");
            }
        }

        /// <summary>
        /// Hides the terminal cursor.
        /// </summary>
        public void HideCursor()
        {
            try
            {
                _backend.HideCursor();
                _logger?.LogTrace("Cursor hidden");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to hide cursor");
            }
        }

        private void ResizeBuffers(int width, int height)
        {
            try
            {
                _currentBuffer?.Resize(width, height);
                _previousBuffer?.Resize(width, height);

                // Force complete redraw after resize
                _currentBuffer?.MarkAllDirty();

                _logger?.LogDebug("Buffers resized to {Width}x{Height}", width, height);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to resize buffers");

                // Recreate buffers if resize fails
                _currentBuffer = new Buffer.Buffer(width, height);
                _previousBuffer = new Buffer.Buffer(width, height);
            }
        }

        private string GenerateAnsiSequence(Cell cell)
        {
            var sb = new StringBuilder();

            // Reset any previous formatting
            sb.Append("\x1b[0m");

            // Set foreground color
            if (cell.Foreground.Kind != ColorKind.Default)
            {
                sb.Append(GenerateColorSequence(cell.Foreground, true));
            }

            // Set background color
            if (cell.Background.Kind != ColorKind.Default)
            {
                sb.Append(GenerateColorSequence(cell.Background, false));
            }

            // Set modifiers
            if (cell.Modifiers != Modifier.None)
            {
                if (cell.Modifiers.IsBold())
                    sb.Append("\x1b[1m");
                if (cell.Modifiers.IsDim())
                    sb.Append("\x1b[2m");
                if (cell.Modifiers.IsItalic())
                    sb.Append("\x1b[3m");
                if (cell.Modifiers.IsUnderlined())
                    sb.Append("\x1b[4m");
                if (cell.Modifiers.IsSlowBlink())
                    sb.Append("\x1b[5m");
                if (cell.Modifiers.IsRapidBlink())
                    sb.Append("\x1b[6m");
                if (cell.Modifiers.IsReversed())
                    sb.Append("\x1b[7m");
                if (cell.Modifiers.IsHidden())
                    sb.Append("\x1b[8m");
            }

            return sb.ToString();
        }

        private string GenerateColorSequence(Color color, bool isForeground)
        {
            var baseCode = isForeground ? 30 : 40;
            var brightBaseCode = isForeground ? 90 : 100;

            return color.Kind switch
            {
                ColorKind.Ansi when color.Index < 8 => $"\x1b[{baseCode + color.Index}m",
                ColorKind.Ansi when color.Index < 16 => $"\x1b[{brightBaseCode + (color.Index - 8)}m",
                ColorKind.Indexed => $"\x1b[{(isForeground ? 38 : 48)};5;{color.Index}m",
                ColorKind.Rgb => $"\x1b[{(isForeground ? 38 : 48)};2;{color.R};{color.G};{color.B}m",
                _ => ""
            };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                ExitFullScreen();
                _backend?.Dispose();
                _logger?.LogDebug("Terminal disposed");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error during terminal disposal");
            }
            finally
            {
                _currentBuffer = null;
                _previousBuffer = null;
                _disposed = true;
            }
        }
    }
}