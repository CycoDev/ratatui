using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Widgets;

namespace CycoAI.CycoTui.Examples
{
    /// <summary>
    /// Example showing how to integrate CycoTui with existing .NET CLI applications.
    /// </summary>
    public static class IntegrationExample
    {
        /// <summary>
        /// Main entry point that demonstrates command-line integration patterns.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A task representing the application execution.</returns>
        public static async Task Main(string[] args)
        {
            // Pattern 1: Command-line switch for interactive mode
            if (args.Length > 0 && args[0] == "--interactive")
            {
                await RunInteractiveMode();
            }
            else
            {
                await RunCommandLineMode(args);
            }
        }

        /// <summary>
        /// Pattern 2: Optional interactive fallback.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A task representing the application execution.</returns>
        public static async Task RunWithFallback(string[] args)
        {
            try
            {
                await RunCommandLineMode(args);
            }
            catch (InvalidOperationException) when (ApplicationPlatform.IsInteractiveTerminal())
            {
                Console.WriteLine("Falling back to interactive mode...");
                await RunInteractiveMode();
            }
        }

        /// <summary>
        /// Shows an interactive report for the given data.
        /// Example of embedding CycoTui in an existing application.
        /// </summary>
        /// <param name="data">The report data to display.</param>
        /// <returns>A task representing the report display operation.</returns>
        public static async Task ShowInteractiveReport(ReportData data)
        {
            var reportWidget = new InteractiveReportWidget(data);

            using var app = Application.Create(reportWidget, new ApplicationOptions
            {
                Viewport = Viewport.Inline(25), // 25 lines high
                EnableAlternateScreen = false   // Don't take over entire terminal
            });

            await app.RunAsync();

            // Terminal automatically restored when app is disposed
            Console.WriteLine("Report viewing completed.");
        }

        private static async Task RunInteractiveMode()
        {
            var mainMenu = new MainMenuWidget();

            var app = Application.Builder()
                .WithRootWidget(mainMenu)
                .EnableMouse()
                .OnEvent<CycoAI.CycoTui.Core.Events.IEvent>(HandleGlobalKey)
                .Build();

            await app.RunAsync();
        }

        private static async Task RunCommandLineMode(string[] args)
        {
            // Simulate command-line processing
            Console.WriteLine("Running in command-line mode...");
            Console.WriteLine($"Arguments: {string.Join(" ", args)}");

            // Simulate some work
            await Task.Delay(1000);

            // Check if we should offer interactive mode
            if (args.Length == 0 && ApplicationPlatform.IsInteractiveTerminal())
            {
                Console.WriteLine("No arguments provided. Would you like to run in interactive mode? (y/n)");
                var response = Console.ReadLine();
                if (response?.ToLower() == "y" || response?.ToLower() == "yes")
                {
                    await RunInteractiveMode();
                }
            }
        }

        private static bool HandleGlobalKey(CycoAI.CycoTui.Core.Events.IEvent @event)
        {
            // Global key handling for all interactive modes
            // This could handle common shortcuts like Ctrl+C, F1 for help, etc.
            return false; // Let other handlers process the event
        }
    }

    /// <summary>
    /// Example data structure for reports.
    /// </summary>
    public class ReportData
    {
        public string Title { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string[] Items { get; set; } = Array.Empty<string>();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Widget for displaying interactive reports.
    /// </summary>
    internal class InteractiveReportWidget : IWidget
    {
        private readonly ReportData _data;

        public InteractiveReportWidget(ReportData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public void Render(CycoAI.CycoTui.Core.Layout.Rect area, CycoAI.CycoTui.Core.Buffer.IBuffer buffer)
        {
            if (buffer == null)
                return;

            var y = area.Y;

            // Render title
            var title = $"Report: {_data.Title}";
            // buffer.SetString(area.X, y++, title, titleStyle);

            // Render generation date
            var dateStr = $"Generated: {_data.GeneratedAt:yyyy-MM-dd HH:mm:ss}";
            // buffer.SetString(area.X, y++, dateStr, dateStyle);

            y++; // Empty line

            // Render items
            foreach (var item in _data.Items.Take(area.Height - y - 2))
            {
                // buffer.SetString(area.X + 2, y++, $"• {item}", itemStyle);
            }

            // Render instructions at bottom
            var instructions = "Press 'q' to exit, arrow keys to scroll";
            // buffer.SetString(area.X, area.Y + area.Height - 1, instructions, instructionStyle);
        }
    }

    /// <summary>
    /// Main menu widget for interactive mode.
    /// </summary>
    internal class MainMenuWidget : IWidget
    {
        public void Render(CycoAI.CycoTui.Core.Layout.Rect area, CycoAI.CycoTui.Core.Buffer.IBuffer buffer)
        {
            if (buffer == null)
                return;

            var title = "CycoTui Integration Example";
            var titleX = Math.Max(0, (area.Width - title.Length) / 2);
            // buffer.SetString(area.X + titleX, area.Y + 2, title, titleStyle);

            var options = new[]
            {
                "1. View Report",
                "2. Settings",
                "3. Help",
                "4. Exit"
            };

            var startY = area.Y + area.Height / 2 - options.Length / 2;
            for (int i = 0; i < options.Length; i++)
            {
                var optionX = Math.Max(0, (area.Width - options[i].Length) / 2);
                // buffer.SetString(area.X + optionX, startY + i, options[i], optionStyle);
            }

            var instructions = "Use number keys to select an option, or 'q' to quit";
            var instrX = Math.Max(0, (area.Width - instructions.Length) / 2);
            // buffer.SetString(area.X + instrX, area.Y + area.Height - 3, instructions, instructionStyle);
        }
    }
}