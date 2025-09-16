using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui.Plugins
{
    /// <summary>
    /// Plugin that handles global keyboard shortcuts for the application.
    /// </summary>
    public class KeyboardShortcutsPlugin : ApplicationPlugin
    {
        private readonly Dictionary<string, Func<IApplication, Task<bool>>> _shortcuts = new();
        private readonly Dictionary<string, string> _descriptions = new();

        /// <inheritdoc />
        public override string Name => "Keyboard Shortcuts";

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardShortcutsPlugin"/> class.
        /// </summary>
        public KeyboardShortcutsPlugin()
        {
            RegisterDefaultShortcuts();
        }

        /// <summary>
        /// Registers a keyboard shortcut.
        /// </summary>
        /// <param name="keyCombo">The key combination (e.g., "Ctrl+Q", "F1", "Escape").</param>
        /// <param name="handler">The handler function.</param>
        /// <param name="description">Description of what the shortcut does.</param>
        public void RegisterShortcut(string keyCombo, Func<IApplication, Task<bool>> handler, string description = "")
        {
            if (string.IsNullOrEmpty(keyCombo))
                throw new ArgumentException("Key combination cannot be null or empty.", nameof(keyCombo));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _shortcuts[keyCombo.ToUpperInvariant()] = handler;
            _descriptions[keyCombo.ToUpperInvariant()] = description;
        }

        /// <summary>
        /// Registers a synchronous keyboard shortcut.
        /// </summary>
        /// <param name="keyCombo">The key combination.</param>
        /// <param name="handler">The synchronous handler function.</param>
        /// <param name="description">Description of what the shortcut does.</param>
        public void RegisterShortcut(string keyCombo, Func<IApplication, bool> handler, string description = "")
        {
            RegisterShortcut(keyCombo, app => Task.FromResult(handler(app)), description);
        }

        /// <summary>
        /// Unregisters a keyboard shortcut.
        /// </summary>
        /// <param name="keyCombo">The key combination to unregister.</param>
        public void UnregisterShortcut(string keyCombo)
        {
            if (!string.IsNullOrEmpty(keyCombo))
            {
                var key = keyCombo.ToUpperInvariant();
                _shortcuts.Remove(key);
                _descriptions.Remove(key);
            }
        }

        /// <summary>
        /// Gets all registered shortcuts and their descriptions.
        /// </summary>
        /// <returns>A dictionary of shortcuts and their descriptions.</returns>
        public IReadOnlyDictionary<string, string> GetShortcuts()
        {
            return new Dictionary<string, string>(_descriptions);
        }

        /// <inheritdoc />
        public override Task InitializeAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            // Subscribe to key events
            // In a real implementation, we'd register with the event system here
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task<bool> HandleEventAsync(IEvent @event, IApplication application, CancellationToken cancellationToken = default)
        {
            // Handle keyboard events
            if (@event.Type == EventType.Key && @event is KeyboardEvent keyEvent)
            {
                var keyCombo = FormatKeyCombo(keyEvent);

                if (_shortcuts.TryGetValue(keyCombo, out var handler))
                {
                    try
                    {
                        return await handler(application);
                    }
                    catch (Exception)
                    {
                        // Log error in real implementation
                        return false;
                    }
                }
            }

            return false;
        }

        private void RegisterDefaultShortcuts()
        {
            // Register common shortcuts
            RegisterShortcut("CTRL+Q", app => { app.Stop(); return true; }, "Quit application");
            RegisterShortcut("CTRL+C", app => { app.Stop(); return true; }, "Cancel/Quit application");
            RegisterShortcut("F5", app => { app.RequestRender(); return true; }, "Refresh display");
            RegisterShortcut("CTRL+R", app => { app.RequestRender(); return true; }, "Refresh display");
            RegisterShortcut("F1", app => ShowHelp(app), "Show help");
            RegisterShortcut("ESCAPE", app => HandleEscape(app), "Cancel current operation");
        }

        private bool ShowHelp(IApplication app)
        {
            // Toggle help display
            var showHelp = app.TryGetGlobalState<bool>("ShowHelp", out var help) ? help : false;
            app.SetGlobalState("ShowHelp", !showHelp);
            app.SetGlobalState("HelpContent", GenerateHelpContent());
            app.RequestRender();
            return true;
        }

        private bool HandleEscape(IApplication app)
        {
            // Check if we're in a modal state that can be cancelled
            if (app.TryGetGlobalState<bool>("InModal", out var inModal) && inModal)
            {
                app.SetGlobalState("InModal", false);
                app.RequestRender();
                return true;
            }

            // Check if help is shown and hide it
            if (app.TryGetGlobalState<bool>("ShowHelp", out var showHelp) && showHelp)
            {
                app.SetGlobalState("ShowHelp", false);
                app.RequestRender();
                return true;
            }

            return false;
        }

        private string GenerateHelpContent()
        {
            var help = new System.Text.StringBuilder();
            help.AppendLine("Keyboard Shortcuts:");
            help.AppendLine();

            foreach (var kvp in _descriptions)
            {
                help.AppendLine($"  {kvp.Key.PadRight(15)} - {kvp.Value}");
            }

            return help.ToString();
        }

        private string FormatKeyCombo(KeyboardEvent keyEvent)
        {
            var combo = new List<string>();

            if (keyEvent.Modifiers.HasFlag(KeyModifiers.Control))
                combo.Add("CTRL");
            if (keyEvent.Modifiers.HasFlag(KeyModifiers.Alt))
                combo.Add("ALT");
            if (keyEvent.Modifiers.HasFlag(KeyModifiers.Shift))
                combo.Add("SHIFT");

            combo.Add(keyEvent.Key.ToString().ToUpperInvariant());

            return string.Join("+", combo);
        }
    }

    // Placeholder keyboard event types
    internal class KeyboardEvent : IEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public EventType Type => EventType.Key;
        public bool IsHandled { get; private set; }
        public KeyCode Key { get; set; }
        public KeyModifiers Modifiers { get; set; }

        public void Handle() => IsHandled = true;
    }

    [Flags]
    internal enum KeyModifiers
    {
        None = 0,
        Shift = 1,
        Control = 2,
        Alt = 4
    }

    internal enum KeyCode
    {
        Q, C, R, F1, F5, Escape
    }
}