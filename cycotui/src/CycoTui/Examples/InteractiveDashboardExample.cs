using System;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;
using CycoAI.CycoTui.Core.Widgets;

namespace CycoAI.CycoTui.Examples
{
    /// <summary>
    /// Interactive dashboard example demonstrating event handling and complex layouts.
    /// </summary>
    public static class InteractiveDashboardExample
    {
        /// <summary>
        /// Runs the interactive dashboard example.
        /// </summary>
        /// <returns>A task representing the application execution.</returns>
        public static async Task RunAsync()
        {
            var dashboard = new DashboardWidget();

            var app = Application.Builder()
                .WithRootWidget(dashboard)
                .EnableMouse()
                .OnEvent<KeyEvent>(HandleKey)
                .OnEvent<ResizeEvent>(HandleResize)
                .OnException(HandleException)
                .Build();

            await app.RunAsync();
        }

        private static bool HandleKey(KeyEvent keyEvent)
        {
            // Example key handling - exit on 'q' or Escape
            if (keyEvent.Key == Key.Q || keyEvent.Key == Key.Escape)
            {
                Application.Current?.Stop();
                return true;
            }

            // Handle other keys
            switch (keyEvent.Key)
            {
                case Key.R:
                    // Refresh dashboard
                    Application.Current?.RequestRender();
                    return true;

                case Key.H:
                    // Show help (toggle help mode in global state)
                    var showHelp = Application.Current?.TryGetGlobalState<bool>("ShowHelp", out var help) == true ? help : false;
                    Application.Current?.SetGlobalState("ShowHelp", !showHelp);
                    Application.Current?.RequestRender();
                    return true;

                default:
                    return false;
            }
        }

        private static bool HandleResize(ResizeEvent resizeEvent)
        {
            // Handle terminal resize
            Application.Current?.RequestRender();
            return true;
        }

        private static void HandleException(object? sender, UnhandledExceptionEventArgs e)
        {
            // Log the exception (in a real application)
            Console.Error.WriteLine($"Unhandled exception: {e.Exception.Message}");
        }
    }

    /// <summary>
    /// Dashboard widget that demonstrates a complex layout with multiple panels.
    /// This is a placeholder implementation until the full widget system is available.
    /// </summary>
    internal class DashboardWidget : IWidget
    {
        public void Render(CycoAI.CycoTui.Core.Layout.Rect area, CycoAI.CycoTui.Core.Buffer.IBuffer buffer)
        {
            if (buffer == null)
                return;

            // Render dashboard title
            var title = "CycoTui Interactive Dashboard";
            var titleX = Math.Max(0, (area.Width - title.Length) / 2);
            // buffer.SetString(area.X + titleX, area.Y, title, titleStyle);

            // Render help text if enabled
            var showHelp = Application.Current?.TryGetGlobalState<bool>("ShowHelp", out var help) == true ? help : false;
            if (showHelp)
            {
                var helpText = "Press 'q' to quit, 'r' to refresh, 'h' to toggle help";
                var helpX = Math.Max(0, (area.Width - helpText.Length) / 2);
                // buffer.SetString(area.X + helpX, area.Y + 2, helpText, helpStyle);
            }

            // Render dashboard panels (placeholder)
            var panelHeight = (area.Height - 4) / 2;
            var panelWidth = area.Width / 2;

            // Top-left panel: System Info
            RenderPanel(buffer, new CycoAI.CycoTui.Core.Layout.Rect(area.X, area.Y + 3, panelWidth, panelHeight), "System Info");

            // Top-right panel: Metrics
            RenderPanel(buffer, new CycoAI.CycoTui.Core.Layout.Rect(area.X + panelWidth, area.Y + 3, panelWidth, panelHeight), "Metrics");

            // Bottom-left panel: Logs
            RenderPanel(buffer, new CycoAI.CycoTui.Core.Layout.Rect(area.X, area.Y + 3 + panelHeight, panelWidth, panelHeight), "Logs");

            // Bottom-right panel: Status
            RenderPanel(buffer, new CycoAI.CycoTui.Core.Layout.Rect(area.X + panelWidth, area.Y + 3 + panelHeight, panelWidth, panelHeight), "Status");
        }

        private void RenderPanel(CycoAI.CycoTui.Core.Buffer.IBuffer buffer, CycoAI.CycoTui.Core.Layout.Rect area, string title)
        {
            // Render panel border (simplified)
            // In a real implementation, this would use proper border rendering from the widget system

            // Render panel title
            var titleX = Math.Max(0, (area.Width - title.Length) / 2);
            // buffer.SetString(area.X + titleX, area.Y, title, panelTitleStyle);

            // Render panel content placeholder
            var content = $"[{title} content goes here]";
            var contentX = Math.Max(0, (area.Width - content.Length) / 2);
            var contentY = area.Y + area.Height / 2;
            // buffer.SetString(area.X + contentX, contentY, content, panelContentStyle);
        }
    }

    // Placeholder event types until the full event system is implemented
    internal class KeyEvent : IEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public EventType Type => EventType.Key;
        public bool IsHandled { get; private set; }
        public Key Key { get; set; }

        public void Handle() => IsHandled = true;
    }

    internal class ResizeEvent : IEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public EventType Type => EventType.Resize;
        public bool IsHandled { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Handle() => IsHandled = true;
    }

    // Placeholder key enumeration
    internal enum Key
    {
        Q,
        R,
        H,
        Escape
    }
}