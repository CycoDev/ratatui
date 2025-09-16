using System;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Widgets;

namespace CycoAI.CycoTui.Examples
{
    /// <summary>
    /// Simple "Hello, World!" example demonstrating basic CycoTui application usage.
    /// </summary>
    public static class HelloWorldExample
    {
        /// <summary>
        /// Runs the Hello World example.
        /// </summary>
        /// <returns>A task representing the application execution.</returns>
        public static async Task RunAsync()
        {
            // Create a simple text widget
            var helloWidget = new SimpleTextWidget("Hello, World!");

            // Create and run the application
            var app = Application.Create(helloWidget);
            await app.RunAsync();
        }

        /// <summary>
        /// Runs the Hello World example with custom options.
        /// </summary>
        /// <returns>A task representing the application execution.</returns>
        public static async Task RunWithOptionsAsync()
        {
            var helloWidget = new SimpleTextWidget("Hello, CycoTui!");

            var app = Application.Create(helloWidget, new ApplicationOptions
            {
                EnableMouse = false,
                RenderInterval = TimeSpan.FromMilliseconds(33) // 30 FPS
            });

            await app.RunAsync();
        }

        /// <summary>
        /// Runs the Hello World example using the builder pattern.
        /// </summary>
        /// <returns>A task representing the application execution.</returns>
        public static async Task RunWithBuilderAsync()
        {
            var helloWidget = new SimpleTextWidget("Hello, Builder Pattern!");

            var app = Application.Builder()
                .WithRootWidget(helloWidget)
                .EnableMouse(false)
                .WithRenderRate(30) // 30 FPS
                .OnEvent<CycoAI.CycoTui.Core.Events.IEvent>(HandleKeyPress)
                .Build();

            await app.RunAsync();
        }

        private static bool HandleKeyPress(CycoAI.CycoTui.Core.Events.IEvent keyEvent)
        {
            // Exit on any key press
            Application.Current?.Stop();
            return true;
        }
    }

    /// <summary>
    /// Simple text widget for demonstration purposes.
    /// This is a placeholder implementation until the full widget system is available.
    /// </summary>
    internal class SimpleTextWidget : IWidget
    {
        private readonly string _text;

        public SimpleTextWidget(string text)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public void Render(CycoAI.CycoTui.Core.Layout.Rect area, CycoAI.CycoTui.Core.Buffer.IBuffer buffer)
        {
            // Simple implementation that writes text at the center of the area
            if (buffer == null || string.IsNullOrEmpty(_text))
                return;

            var x = Math.Max(0, (area.Width - _text.Length) / 2);
            var y = area.Height / 2;

            if (x < area.Width && y < area.Height)
            {
                // This is a simplified rendering - in reality, we'd use the buffer's
                // proper API to set characters with styling
                for (int i = 0; i < _text.Length && (x + i) < area.Width; i++)
                {
                    // buffer.SetCell(area.X + x + i, area.Y + y, _text[i], defaultStyle);
                }
            }
        }
    }
}