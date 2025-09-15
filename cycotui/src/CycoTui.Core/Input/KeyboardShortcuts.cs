using System;
using System.Collections.Generic;
using System.Linq;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui.Core.Input
{
    /// <summary>
    /// Represents a keyboard shortcut with key and modifiers.
    /// </summary>
    public readonly struct KeyboardShortcut : IEquatable<KeyboardShortcut>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardShortcut"/> struct.
        /// </summary>
        /// <param name="key">The key for the shortcut.</param>
        /// <param name="modifiers">The modifier keys for the shortcut.</param>
        public KeyboardShortcut(Key key, KeyModifiers modifiers = KeyModifiers.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the key for this shortcut.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// Gets the modifier keys for this shortcut.
        /// </summary>
        public KeyModifiers Modifiers { get; }

        /// <summary>
        /// Creates a keyboard shortcut from a key event.
        /// </summary>
        /// <param name="keyEvent">The key event to convert.</param>
        /// <returns>The keyboard shortcut.</returns>
        public static KeyboardShortcut FromKeyEvent(KeyEvent keyEvent)
        {
            return new KeyboardShortcut(keyEvent.Key, keyEvent.Modifiers);
        }

        /// <summary>
        /// Checks if this shortcut matches a key event.
        /// </summary>
        /// <param name="keyEvent">The key event to check.</param>
        /// <returns>true if the shortcut matches; otherwise, false.</returns>
        public bool Matches(KeyEvent keyEvent)
        {
            return Key == keyEvent.Key && Modifiers == keyEvent.Modifiers;
        }

        /// <inheritdoc />
        public bool Equals(KeyboardShortcut other)
        {
            return Key == other.Key && Modifiers == other.Modifiers;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is KeyboardShortcut shortcut && Equals(shortcut);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Modifiers.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var modifierStr = Modifiers != KeyModifiers.None ? $"{Modifiers}+" : "";
            return $"{modifierStr}{Key}";
        }

        public static bool operator ==(KeyboardShortcut left, KeyboardShortcut right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(KeyboardShortcut left, KeyboardShortcut right)
        {
            return !left.Equals(right);
        }

        // Common shortcuts
        public static readonly KeyboardShortcut CtrlC = new KeyboardShortcut(Key.C, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlV = new KeyboardShortcut(Key.V, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlX = new KeyboardShortcut(Key.X, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlZ = new KeyboardShortcut(Key.Z, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlY = new KeyboardShortcut(Key.Y, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlA = new KeyboardShortcut(Key.A, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlS = new KeyboardShortcut(Key.S, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlO = new KeyboardShortcut(Key.O, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlN = new KeyboardShortcut(Key.N, KeyModifiers.Control);
        public static readonly KeyboardShortcut CtrlQ = new KeyboardShortcut(Key.Q, KeyModifiers.Control);
        public static readonly KeyboardShortcut AltF4 = new KeyboardShortcut(Key.F4, KeyModifiers.Alt);
        public static readonly KeyboardShortcut Escape = new KeyboardShortcut(Key.Escape);
        public static readonly KeyboardShortcut Enter = new KeyboardShortcut(Key.Enter);
        public static readonly KeyboardShortcut Tab = new KeyboardShortcut(Key.Tab);
        public static readonly KeyboardShortcut ShiftTab = new KeyboardShortcut(Key.Tab, KeyModifiers.Shift);
    }

    /// <summary>
    /// Interface for keyboard shortcut actions.
    /// </summary>
    public interface IShortcutAction
    {
        /// <summary>
        /// Executes the shortcut action.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <returns>true if the action was handled; otherwise, false.</returns>
        bool Execute(IShortcutContext context);

        /// <summary>
        /// Gets a description of this action.
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// Context information for shortcut execution.
    /// </summary>
    public interface IShortcutContext
    {
        /// <summary>
        /// Gets the original key event that triggered the shortcut.
        /// </summary>
        KeyEvent KeyEvent { get; }

        /// <summary>
        /// Gets additional context data.
        /// </summary>
        IReadOnlyDictionary<string, object> Data { get; }
    }

    /// <summary>
    /// Default implementation of shortcut context.
    /// </summary>
    public class ShortcutContext : IShortcutContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutContext"/> class.
        /// </summary>
        /// <param name="keyEvent">The key event that triggered the shortcut.</param>
        /// <param name="data">Additional context data.</param>
        public ShortcutContext(KeyEvent keyEvent, IReadOnlyDictionary<string, object>? data = null)
        {
            KeyEvent = keyEvent;
            Data = data ?? new Dictionary<string, object>();
        }

        /// <inheritdoc />
        public KeyEvent KeyEvent { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> Data { get; }
    }

    /// <summary>
    /// Simple action implementation using a delegate.
    /// </summary>
    public class DelegateShortcutAction : IShortcutAction
    {
        private readonly Func<IShortcutContext, bool> _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateShortcutAction"/> class.
        /// </summary>
        /// <param name="action">The action delegate.</param>
        /// <param name="description">The action description.</param>
        public DelegateShortcutAction(Func<IShortcutContext, bool> action, string description)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateShortcutAction"/> class with a simple action.
        /// </summary>
        /// <param name="action">The simple action delegate.</param>
        /// <param name="description">The action description.</param>
        public DelegateShortcutAction(Action action, string description)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _action = _ => { action(); return true; };
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public bool Execute(IShortcutContext context)
        {
            return _action(context);
        }
    }

    /// <summary>
    /// Manages keyboard shortcuts and their associated actions.
    /// </summary>
    public class KeyboardShortcutManager
    {
        private readonly Dictionary<KeyboardShortcut, List<IShortcutAction>> _shortcuts;
        private readonly Dictionary<string, KeyboardShortcut> _namedShortcuts;
        private readonly object _lockObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardShortcutManager"/> class.
        /// </summary>
        public KeyboardShortcutManager()
        {
            _shortcuts = new Dictionary<KeyboardShortcut, List<IShortcutAction>>();
            _namedShortcuts = new Dictionary<string, KeyboardShortcut>();
            _lockObject = new object();
        }

        /// <summary>
        /// Registers a keyboard shortcut with an action.
        /// </summary>
        /// <param name="shortcut">The keyboard shortcut.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="name">Optional name for the shortcut.</param>
        public void RegisterShortcut(KeyboardShortcut shortcut, IShortcutAction action, string? name = null)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            lock (_lockObject)
            {
                if (!_shortcuts.ContainsKey(shortcut))
                {
                    _shortcuts[shortcut] = new List<IShortcutAction>();
                }

                _shortcuts[shortcut].Add(action);

                if (!string.IsNullOrEmpty(name))
                {
                    _namedShortcuts[name] = shortcut;
                }
            }
        }

        /// <summary>
        /// Registers a keyboard shortcut with a delegate action.
        /// </summary>
        /// <param name="shortcut">The keyboard shortcut.</param>
        /// <param name="action">The action delegate to execute.</param>
        /// <param name="description">The action description.</param>
        /// <param name="name">Optional name for the shortcut.</param>
        public void RegisterShortcut(KeyboardShortcut shortcut, Func<IShortcutContext, bool> action, string description, string? name = null)
        {
            RegisterShortcut(shortcut, new DelegateShortcutAction(action, description), name);
        }

        /// <summary>
        /// Registers a keyboard shortcut with a simple action.
        /// </summary>
        /// <param name="shortcut">The keyboard shortcut.</param>
        /// <param name="action">The simple action to execute.</param>
        /// <param name="description">The action description.</param>
        /// <param name="name">Optional name for the shortcut.</param>
        public void RegisterShortcut(KeyboardShortcut shortcut, Action action, string description, string? name = null)
        {
            RegisterShortcut(shortcut, new DelegateShortcutAction(action, description), name);
        }

        /// <summary>
        /// Unregisters a specific action from a keyboard shortcut.
        /// </summary>
        /// <param name="shortcut">The keyboard shortcut.</param>
        /// <param name="action">The action to remove.</param>
        /// <returns>true if the action was removed; otherwise, false.</returns>
        public bool UnregisterShortcut(KeyboardShortcut shortcut, IShortcutAction action)
        {
            if (action == null)
                return false;

            lock (_lockObject)
            {
                if (_shortcuts.TryGetValue(shortcut, out var actions))
                {
                    var removed = actions.Remove(action);
                    if (actions.Count == 0)
                    {
                        _shortcuts.Remove(shortcut);
                    }
                    return removed;
                }
            }

            return false;
        }

        /// <summary>
        /// Unregisters all actions for a keyboard shortcut.
        /// </summary>
        /// <param name="shortcut">The keyboard shortcut.</param>
        /// <returns>true if any actions were removed; otherwise, false.</returns>
        public bool UnregisterShortcut(KeyboardShortcut shortcut)
        {
            lock (_lockObject)
            {
                var removed = _shortcuts.Remove(shortcut);

                // Remove named shortcuts pointing to this shortcut
                var namesToRemove = _namedShortcuts.Where(kvp => kvp.Value.Equals(shortcut)).Select(kvp => kvp.Key).ToList();
                foreach (var name in namesToRemove)
                {
                    _namedShortcuts.Remove(name);
                }

                return removed;
            }
        }

        /// <summary>
        /// Unregisters a shortcut by name.
        /// </summary>
        /// <param name="name">The name of the shortcut to remove.</param>
        /// <returns>true if the shortcut was removed; otherwise, false.</returns>
        public bool UnregisterShortcut(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            lock (_lockObject)
            {
                if (_namedShortcuts.TryGetValue(name, out var shortcut))
                {
                    _namedShortcuts.Remove(name);
                    return UnregisterShortcut(shortcut);
                }
            }

            return false;
        }

        /// <summary>
        /// Processes a key event and executes any matching shortcuts.
        /// </summary>
        /// <param name="keyEvent">The key event to process.</param>
        /// <param name="contextData">Additional context data.</param>
        /// <returns>true if any shortcut was executed; otherwise, false.</returns>
        public bool ProcessKeyEvent(KeyEvent keyEvent, IReadOnlyDictionary<string, object>? contextData = null)
        {
            var shortcut = KeyboardShortcut.FromKeyEvent(keyEvent);

            lock (_lockObject)
            {
                if (_shortcuts.TryGetValue(shortcut, out var actions))
                {
                    var context = new ShortcutContext(keyEvent, contextData);
                    var handled = false;

                    foreach (var action in actions.ToList()) // Copy to avoid modification during iteration
                    {
                        try
                        {
                            if (action.Execute(context))
                            {
                                handled = true;
                            }
                        }
                        catch (Exception)
                        {
                            // Log exception in a real implementation
                            continue;
                        }
                    }

                    return handled;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a shortcut by name.
        /// </summary>
        /// <param name="name">The name of the shortcut.</param>
        /// <returns>The keyboard shortcut if found; otherwise, null.</returns>
        public KeyboardShortcut? GetShortcut(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            lock (_lockObject)
            {
                return _namedShortcuts.TryGetValue(name, out var shortcut) ? (KeyboardShortcut?)shortcut : null;
            }
        }

        /// <summary>
        /// Gets all registered shortcuts.
        /// </summary>
        /// <returns>A collection of all registered shortcuts.</returns>
        public IReadOnlyCollection<KeyboardShortcut> GetAllShortcuts()
        {
            lock (_lockObject)
            {
                return _shortcuts.Keys.ToList();
            }
        }

        /// <summary>
        /// Gets all actions for a specific shortcut.
        /// </summary>
        /// <param name="shortcut">The keyboard shortcut.</param>
        /// <returns>A collection of actions for the shortcut.</returns>
        public IReadOnlyCollection<IShortcutAction> GetActions(KeyboardShortcut shortcut)
        {
            lock (_lockObject)
            {
                return _shortcuts.TryGetValue(shortcut, out var actions) ? (IReadOnlyCollection<IShortcutAction>)actions.ToList() : Array.Empty<IShortcutAction>();
            }
        }

        /// <summary>
        /// Clears all registered shortcuts.
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _shortcuts.Clear();
                _namedShortcuts.Clear();
            }
        }
    }
}