using System;
using System.IO;

#if NET9_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace CycoAI.CycoTui.Core.Backend
{
    /// <summary>
    /// Defines the interface for terminal backend implementations.
    /// Backends provide platform-specific terminal control capabilities.
    /// </summary>
    public interface IBackend : IDisposable
    {
        /// <summary>
        /// Gets the size of the terminal in characters (columns, rows).
        /// </summary>
        /// <returns>A tuple containing (width, height) in characters.</returns>
        (int Width, int Height) GetSize();

        /// <summary>
        /// Clears the entire terminal screen.
        /// </summary>
        void Clear();

        /// <summary>
        /// Hides the terminal cursor.
        /// </summary>
        void HideCursor();

        /// <summary>
        /// Shows the terminal cursor.
        /// </summary>
        void ShowCursor();

        /// <summary>
        /// Gets the current cursor position.
        /// </summary>
        /// <returns>A tuple containing (x, y) coordinates.</returns>
        (int X, int Y) GetCursor();

        /// <summary>
        /// Sets the cursor position.
        /// </summary>
        /// <param name="x">The column position (0-based).</param>
        /// <param name="y">The row position (0-based).</param>
        void SetCursor(int x, int y);

        /// <summary>
        /// Flushes any buffered output to the terminal.
        /// </summary>
        void Flush();

        /// <summary>
        /// Enters alternate screen mode if supported.
        /// This creates a separate screen buffer that can be restored later.
        /// </summary>
        void EnterAlternateScreen();

        /// <summary>
        /// Exits alternate screen mode, restoring the previous screen content.
        /// </summary>
        void LeaveAlternateScreen();

        /// <summary>
        /// Enables raw mode for the terminal.
        /// In raw mode, input is not processed by the terminal and is passed directly to the application.
        /// </summary>
        void EnableRawMode();

        /// <summary>
        /// Disables raw mode, restoring normal terminal input processing.
        /// </summary>
        void DisableRawMode();

        /// <summary>
        /// Writes text to the terminal at the current cursor position.
        /// </summary>
        /// <param name="text">The text to write.</param>
        void Write(string text);

        /// <summary>
        /// Gets the output stream used by this backend.
        /// This may be stdout, stderr, or another stream depending on the implementation.
        /// </summary>
        TextWriter Output { get; }

        /// <summary>
        /// Gets a value indicating whether this backend supports color output.
        /// </summary>
        bool SupportsColor { get; }

        /// <summary>
        /// Gets a value indicating whether this backend supports alternate screen mode.
        /// </summary>
        bool SupportsAlternateScreen { get; }

        /// <summary>
        /// Gets a value indicating whether this backend supports mouse input.
        /// </summary>
        bool SupportsMouse { get; }
    }
}