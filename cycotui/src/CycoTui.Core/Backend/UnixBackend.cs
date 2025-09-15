using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace CycoAI.CycoTui.Core.Backend
{
    /// <summary>
    /// Unix/macOS terminal backend implementation using ANSI escape sequences.
    /// Optimized for macOS but compatible with most Unix-like systems.
    /// </summary>
    public class UnixBackend : IBackend
    {
        private readonly ILogger<UnixBackend>? _logger;
        private readonly TextWriter _output;
        private readonly bool _isOutputRedirected;
        private bool _rawModeEnabled;
        private bool _alternateScreenEnabled;
        private bool _disposed;

        // Terminal capability flags
        private bool? _supportsColor;
        private bool? _supportsAlternateScreen;
        private bool? _supportsMouse;

        /// <inheritdoc />
        public TextWriter Output => _output;

        /// <inheritdoc />
        public bool SupportsColor => _supportsColor ??= DetectColorSupport();

        /// <inheritdoc />
        public bool SupportsAlternateScreen => _supportsAlternateScreen ??= DetectAlternateScreenSupport();

        /// <inheritdoc />
        public bool SupportsMouse => _supportsMouse ??= DetectMouseSupport();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixBackend"/> class.
        /// </summary>
        /// <param name="output">The output stream to write to. If null, uses Console.Out.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public UnixBackend(TextWriter? output = null, ILogger<UnixBackend>? logger = null)
        {
            _output = output ?? Console.Out;
            _logger = logger;
            _isOutputRedirected = Console.IsOutputRedirected;

            if (_isOutputRedirected)
            {
                _logger?.LogWarning("Output is redirected, some terminal features may not work correctly");
            }

            _logger?.LogDebug("UnixBackend initialized with output: {OutputType}", _output.GetType().Name);
        }

        /// <inheritdoc />
        public (int Width, int Height) GetSize()
        {
            try
            {
                // Try using .NET's built-in console size detection first
                if (!_isOutputRedirected)
                {
                    return (Console.WindowWidth, Console.WindowHeight);
                }

                // Fallback: Query terminal size using ANSI escape sequences
                return QueryTerminalSize();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to get terminal size, using fallback");
                return (80, 24); // Standard fallback size
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            try
            {
                if (_isOutputRedirected)
                {
                    _logger?.LogDebug("Skipping clear on redirected output");
                    return;
                }

                // ANSI escape sequence to clear entire screen
                _output.Write("\x1b[2J");
                SetCursor(0, 0);
                Flush();
                _logger?.LogDebug("Screen cleared");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to clear screen");
            }
        }

        /// <inheritdoc />
        public void HideCursor()
        {
            try
            {
                if (_isOutputRedirected) return;

                _output.Write("\x1b[?25l");
                Flush();
                _logger?.LogDebug("Cursor hidden");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to hide cursor");
            }
        }

        /// <inheritdoc />
        public void ShowCursor()
        {
            try
            {
                if (_isOutputRedirected) return;

                _output.Write("\x1b[?25h");
                Flush();
                _logger?.LogDebug("Cursor shown");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to show cursor");
            }
        }

        /// <inheritdoc />
        public (int X, int Y) GetCursor()
        {
            try
            {
                if (_isOutputRedirected)
                    return (0, 0);

                // Query cursor position using ANSI escape sequence
                return QueryCursorPosition();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to get cursor position");
                return (0, 0);
            }
        }

        /// <inheritdoc />
        public void SetCursor(int x, int y)
        {
            try
            {
                if (_isOutputRedirected) return;

                // ANSI escape sequence for cursor positioning (1-based)
                _output.Write($"\x1b[{y + 1};{x + 1}H");
                _logger?.LogTrace("Cursor set to ({X}, {Y})", x, y);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to set cursor position to ({X}, {Y})", x, y);
            }
        }

        /// <inheritdoc />
        public void Flush()
        {
            try
            {
                _output.Flush();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to flush output");
            }
        }

        /// <inheritdoc />
        public void EnterAlternateScreen()
        {
            try
            {
                if (_isOutputRedirected || !SupportsAlternateScreen || _alternateScreenEnabled)
                    return;

                _output.Write("\x1b[?1049h");
                Flush();
                _alternateScreenEnabled = true;
                _logger?.LogDebug("Entered alternate screen");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to enter alternate screen");
            }
        }

        /// <inheritdoc />
        public void LeaveAlternateScreen()
        {
            try
            {
                if (_isOutputRedirected || !SupportsAlternateScreen || !_alternateScreenEnabled)
                    return;

                _output.Write("\x1b[?1049l");
                Flush();
                _alternateScreenEnabled = false;
                _logger?.LogDebug("Left alternate screen");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to leave alternate screen");
            }
        }

        /// <inheritdoc />
        public void EnableRawMode()
        {
            try
            {
                if (_isOutputRedirected || _rawModeEnabled)
                    return;

                // On Unix systems, this would typically involve tcgetattr/tcsetattr
                // For now, we'll use .NET's approach where possible
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    EnableRawModeNative();
                }

                _rawModeEnabled = true;
                _logger?.LogDebug("Raw mode enabled");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to enable raw mode");
            }
        }

        /// <inheritdoc />
        public void DisableRawMode()
        {
            try
            {
                if (_isOutputRedirected || !_rawModeEnabled)
                    return;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    DisableRawModeNative();
                }

                _rawModeEnabled = false;
                _logger?.LogDebug("Raw mode disabled");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to disable raw mode");
            }
        }

        /// <inheritdoc />
        public void Write(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;

                _output.Write(text);
                _logger?.LogTrace("Wrote {Length} characters", text.Length);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to write text");
            }
        }

        private bool DetectColorSupport()
        {
            try
            {
                if (_isOutputRedirected)
                    return false;

                // Check COLORTERM environment variable
                var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");
                if (!string.IsNullOrEmpty(colorTerm))
                    return true;

                // Check TERM environment variable for color support
                var term = Environment.GetEnvironmentVariable("TERM");
                if (!string.IsNullOrEmpty(term))
                {
                    return term.Contains("color") || term.Contains("xterm") ||
                           term.Contains("screen") || term.Contains("tmux");
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to detect color support");
                return false;
            }
        }

        private bool DetectAlternateScreenSupport()
        {
            try
            {
                if (_isOutputRedirected)
                    return false;

                // Most modern terminals support alternate screen
                var term = Environment.GetEnvironmentVariable("TERM");
                return !string.IsNullOrEmpty(term) &&
                       (term.Contains("xterm") || term.Contains("screen") ||
                        term.Contains("tmux") || term.StartsWith("vt"));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to detect alternate screen support");
                return false;
            }
        }

        private bool DetectMouseSupport()
        {
            try
            {
                if (_isOutputRedirected)
                    return false;

                // Mouse support is available in most modern terminals
                var term = Environment.GetEnvironmentVariable("TERM");
                return !string.IsNullOrEmpty(term) &&
                       (term.Contains("xterm") || term.Contains("screen") || term.Contains("tmux"));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to detect mouse support");
                return false;
            }
        }

        private (int Width, int Height) QueryTerminalSize()
        {
            // This is a simplified implementation
            // In a full implementation, we would send ANSI queries and parse responses
            _logger?.LogDebug("Querying terminal size via ANSI sequences not fully implemented");
            return (80, 24);
        }

        private (int X, int Y) QueryCursorPosition()
        {
            // This is a simplified implementation
            // In a full implementation, we would send "\x1b[6n" and parse the response
            _logger?.LogDebug("Querying cursor position via ANSI sequences not fully implemented");
            return (0, 0);
        }

        private void EnableRawModeNative()
        {
            // This would involve P/Invoke to tcgetattr/tcsetattr on Unix systems
            // For now, we'll use Console.TreatControlCAsInput where available
            try
            {
                Console.TreatControlCAsInput = true;
            }
            catch (Exception ex)
            {
                _logger?.LogDebug(ex, "Could not set TreatControlCAsInput");
            }
        }

        private void DisableRawModeNative()
        {
            try
            {
                Console.TreatControlCAsInput = false;
            }
            catch (Exception ex)
            {
                _logger?.LogDebug(ex, "Could not reset TreatControlCAsInput");
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                // Restore terminal state
                if (_alternateScreenEnabled)
                    LeaveAlternateScreen();

                if (_rawModeEnabled)
                    DisableRawMode();

                ShowCursor();
                Flush();

                _logger?.LogDebug("UnixBackend disposed");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error during disposal");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}