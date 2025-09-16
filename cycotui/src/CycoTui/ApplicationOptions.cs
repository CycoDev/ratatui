using System;

namespace CycoAI.CycoTui
{
    /// <summary>
    /// Configuration options for the CycoTui application.
    /// </summary>
    public class ApplicationOptions
    {
        /// <summary>
        /// Gets or sets the terminal backend type to use.
        /// </summary>
        public TerminalBackendType Backend { get; set; } = TerminalBackendType.Crossterm;

        /// <summary>
        /// Gets or sets the viewport configuration.
        /// </summary>
        public Viewport Viewport { get; set; } = Viewport.Fullscreen;

        /// <summary>
        /// Gets or sets a value indicating whether mouse input is enabled.
        /// </summary>
        public bool EnableMouse { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether resize events are enabled.
        /// </summary>
        public bool EnableResize { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether raw mode is enabled.
        /// </summary>
        public bool EnableRawMode { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether alternate screen mode is enabled.
        /// </summary>
        public bool EnableAlternateScreen { get; set; } = true;

        /// <summary>
        /// Gets or sets the render interval for the application (target frame rate).
        /// Default is approximately 60 FPS.
        /// </summary>
        public TimeSpan RenderInterval { get; set; } = TimeSpan.FromMilliseconds(16);

        /// <summary>
        /// Gets or sets the input polling interval.
        /// </summary>
        public TimeSpan InputPollInterval { get; set; } = TimeSpan.FromMilliseconds(10);

        /// <summary>
        /// Creates a copy of the current options.
        /// </summary>
        /// <returns>A new instance with the same values.</returns>
        public ApplicationOptions Clone()
        {
            return new ApplicationOptions
            {
                Backend = Backend,
                Viewport = Viewport,
                EnableMouse = EnableMouse,
                EnableResize = EnableResize,
                EnableRawMode = EnableRawMode,
                EnableAlternateScreen = EnableAlternateScreen,
                RenderInterval = RenderInterval,
                InputPollInterval = InputPollInterval
            };
        }
    }

    /// <summary>
    /// Enumeration of supported terminal backend types.
    /// </summary>
    public enum TerminalBackendType
    {
        /// <summary>
        /// Cross-platform terminal backend using Crossterm-like functionality.
        /// </summary>
        Crossterm,

        /// <summary>
        /// Unix-specific terminal backend.
        /// </summary>
        Unix,

        /// <summary>
        /// Windows-specific terminal backend.
        /// </summary>
        Windows,

        /// <summary>
        /// Automatically detect the best backend for the current platform.
        /// </summary>
        Auto
    }

    /// <summary>
    /// Viewport configuration for the application.
    /// </summary>
    public class Viewport
    {
        /// <summary>
        /// Gets the viewport type.
        /// </summary>
        public ViewportType Type { get; private set; }

        /// <summary>
        /// Gets the height in lines for inline viewports.
        /// </summary>
        public int Height { get; private set; }

        private Viewport(ViewportType type, int height = 0)
        {
            Type = type;
            Height = height;
        }

        /// <summary>
        /// Creates a fullscreen viewport that uses the entire terminal.
        /// </summary>
        public static Viewport Fullscreen => new(ViewportType.Fullscreen);

        /// <summary>
        /// Creates an inline viewport with the specified height.
        /// </summary>
        /// <param name="height">The height in lines.</param>
        /// <returns>A new inline viewport.</returns>
        public static Viewport Inline(int height)
        {
            if (height <= 0)
                throw new ArgumentException("Height must be positive.", nameof(height));

            return new Viewport(ViewportType.Inline, height);
        }

        /// <summary>
        /// Gets a string representation of the viewport.
        /// </summary>
        /// <returns>A string describing the viewport.</returns>
        public override string ToString()
        {
            return Type switch
            {
                ViewportType.Fullscreen => "Fullscreen",
                ViewportType.Inline => $"Inline({Height})",
                _ => "Unknown"
            };
        }
    }

    /// <summary>
    /// Enumeration of viewport types.
    /// </summary>
    public enum ViewportType
    {
        /// <summary>
        /// Fullscreen viewport that uses the entire terminal.
        /// </summary>
        Fullscreen,

        /// <summary>
        /// Inline viewport with a fixed height.
        /// </summary>
        Inline
    }
}