using System;
using System.Runtime.InteropServices;

namespace CycoAI.CycoTui
{
    /// <summary>
    /// Platform-specific utilities and configuration for CycoTui applications.
    /// </summary>
    public static class ApplicationPlatform
    {
        /// <summary>
        /// Gets a value indicating whether the current platform is Windows.
        /// </summary>
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Gets a value indicating whether the current platform is Linux.
        /// </summary>
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// Gets a value indicating whether the current platform is macOS.
        /// </summary>
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// Gets a value indicating whether the current platform is a Unix-like system (Linux or macOS).
        /// </summary>
        public static bool IsUnix => IsLinux || IsMacOS;

        /// <summary>
        /// Gets the current platform architecture.
        /// </summary>
        public static Architecture Architecture => RuntimeInformation.ProcessArchitecture;

        /// <summary>
        /// Gets platform-specific default application options.
        /// </summary>
        /// <returns>Application options optimized for the current platform.</returns>
        public static ApplicationOptions GetPlatformDefaults()
        {
            var options = new ApplicationOptions();

            if (IsWindows)
            {
                // Windows-specific defaults
                options.Backend = TerminalBackendType.Crossterm; // Best Windows compatibility
                options.InputPollInterval = TimeSpan.FromMilliseconds(15); // Slightly slower on Windows
                options.EnableAlternateScreen = true; // Windows Console supports this well
            }
            else if (IsMacOS)
            {
                // macOS-specific defaults
                options.Backend = TerminalBackendType.Unix;
                options.EnableMouse = true; // iTerm2/Terminal.app have good mouse support
                options.InputPollInterval = TimeSpan.FromMilliseconds(8); // Faster on macOS
                options.RenderInterval = TimeSpan.FromMilliseconds(16); // 60 FPS works well on macOS
            }
            else if (IsLinux)
            {
                // Linux-specific defaults
                options.Backend = TerminalBackendType.Unix;
                options.InputPollInterval = TimeSpan.FromMilliseconds(10);
                options.EnableMouse = true; // Most modern Linux terminals support mouse
            }
            else
            {
                // Fallback for other Unix-like systems
                options.Backend = TerminalBackendType.Unix;
            }

            return options;
        }

        /// <summary>
        /// Detects terminal capabilities for the current environment.
        /// </summary>
        /// <returns>Terminal capabilities information.</returns>
        public static TerminalCapabilities DetectTerminalCapabilities()
        {
            var capabilities = new TerminalCapabilities();

            // Basic capability detection based on environment variables
            var term = Environment.GetEnvironmentVariable("TERM");
            var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");

            // Color support detection
            capabilities.SupportsColor = !string.IsNullOrEmpty(colorTerm) ||
                                       (term?.Contains("256color") == true) ||
                                       (term?.Contains("color") == true);

            capabilities.SupportsTrueColor = colorTerm?.Contains("truecolor") == true ||
                                           colorTerm?.Contains("24bit") == true;

            // Terminal-specific feature detection
            capabilities.SupportsAlternateScreen = !string.IsNullOrEmpty(term) &&
                                                 term != "dumb";

            // Mouse support is generally available on modern terminals
            capabilities.SupportsMouse = capabilities.SupportsAlternateScreen;

            // Platform-specific adjustments
            if (IsWindows)
            {
                // Windows Terminal and newer versions of Command Prompt support these features
                var windowsTerminal = Environment.GetEnvironmentVariable("WT_SESSION");
                if (!string.IsNullOrEmpty(windowsTerminal))
                {
                    capabilities.SupportsColor = true;
                    capabilities.SupportsTrueColor = true;
                    capabilities.SupportsMouse = true;
                }
            }

            return capabilities;
        }

        /// <summary>
        /// Gets the recommended backend type for the current platform and terminal.
        /// </summary>
        /// <returns>The recommended backend type.</returns>
        public static TerminalBackendType GetRecommendedBackend()
        {
            if (IsWindows)
            {
                // Check if we're running in Windows Terminal or a modern console
                var windowsTerminal = Environment.GetEnvironmentVariable("WT_SESSION");
                if (!string.IsNullOrEmpty(windowsTerminal))
                {
                    return TerminalBackendType.Crossterm;
                }

                // Check Windows version and console capabilities
                return TerminalBackendType.Crossterm;
            }
            else
            {
                // Unix-like systems generally work well with the Unix backend
                return TerminalBackendType.Unix;
            }
        }

        /// <summary>
        /// Checks if the current terminal supports interactive mode.
        /// </summary>
        /// <returns>true if the terminal supports interactive mode; otherwise, false.</returns>
        public static bool IsInteractiveTerminal()
        {
            try
            {
                // Check if stdin/stdout are connected to a terminal
                if (IsWindows)
                {
                    // On Windows, check if we have a console window
                    return !Console.IsInputRedirected && !Console.IsOutputRedirected;
                }
                else
                {
                    // On Unix, check if stdout is a TTY
                    return !Console.IsOutputRedirected &&
                           Environment.GetEnvironmentVariable("TERM") != "dumb";
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets platform-specific performance recommendations.
        /// </summary>
        /// <returns>Performance recommendations for the current platform.</returns>
        public static PerformanceRecommendations GetPerformanceRecommendations()
        {
            var recommendations = new PerformanceRecommendations();

            if (IsWindows)
            {
                recommendations.RecommendedRenderInterval = TimeSpan.FromMilliseconds(20); // 50 FPS
                recommendations.RecommendedInputPollInterval = TimeSpan.FromMilliseconds(15);
                recommendations.UseDoubleBuffering = true;
                recommendations.OptimizeForLatency = false;
            }
            else if (IsMacOS)
            {
                recommendations.RecommendedRenderInterval = TimeSpan.FromMilliseconds(16); // 60 FPS
                recommendations.RecommendedInputPollInterval = TimeSpan.FromMilliseconds(8);
                recommendations.UseDoubleBuffering = true;
                recommendations.OptimizeForLatency = true;
            }
            else
            {
                recommendations.RecommendedRenderInterval = TimeSpan.FromMilliseconds(16); // 60 FPS
                recommendations.RecommendedInputPollInterval = TimeSpan.FromMilliseconds(10);
                recommendations.UseDoubleBuffering = true;
                recommendations.OptimizeForLatency = true;
            }

            return recommendations;
        }
    }

    /// <summary>
    /// Information about terminal capabilities.
    /// </summary>
    public class TerminalCapabilities
    {
        /// <summary>
        /// Gets or sets a value indicating whether the terminal supports color output.
        /// </summary>
        public bool SupportsColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the terminal supports true color (24-bit) output.
        /// </summary>
        public bool SupportsTrueColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the terminal supports alternate screen mode.
        /// </summary>
        public bool SupportsAlternateScreen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the terminal supports mouse input.
        /// </summary>
        public bool SupportsMouse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the terminal supports focus events.
        /// </summary>
        public bool SupportsFocusEvents { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the terminal supports paste events.
        /// </summary>
        public bool SupportsPasteEvents { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of colors supported.
        /// </summary>
        public int MaxColors { get; set; } = 256;

        /// <summary>
        /// Gets or sets the terminal size in characters.
        /// </summary>
        public (int Width, int Height) Size { get; set; } = (80, 24);
    }

    /// <summary>
    /// Platform-specific performance recommendations.
    /// </summary>
    public class PerformanceRecommendations
    {
        /// <summary>
        /// Gets or sets the recommended render interval.
        /// </summary>
        public TimeSpan RecommendedRenderInterval { get; set; } = TimeSpan.FromMilliseconds(16);

        /// <summary>
        /// Gets or sets the recommended input polling interval.
        /// </summary>
        public TimeSpan RecommendedInputPollInterval { get; set; } = TimeSpan.FromMilliseconds(10);

        /// <summary>
        /// Gets or sets a value indicating whether double buffering should be used.
        /// </summary>
        public bool UseDoubleBuffering { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to optimize for latency over throughput.
        /// </summary>
        public bool OptimizeForLatency { get; set; } = true;

        /// <summary>
        /// Gets or sets the recommended buffer size for operations.
        /// </summary>
        public int RecommendedBufferSize { get; set; } = 8192;
    }
}