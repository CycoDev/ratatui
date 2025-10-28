using System;
using System.Collections.Generic;

namespace CycoTui.Core.Input;

/// <summary>
/// Simple key binding service mapping KeyEvents to handlers.
/// </summary>
public sealed class KeyBindingService
{
    private readonly Dictionary<(KeyCode code, KeyModifiers mods), Action> _bindings = new();

    public void Bind(KeyCode code, KeyModifiers mods, Action handler)
    {
        _bindings[(code, mods)] = handler;
    }

    public bool TryHandle(InputEvent evt)
    {
        if (evt.Type != InputEventType.Key || !evt.Key.HasValue) return false;
        var k = evt.Key.Value;
        if (_bindings.TryGetValue((k.Code, k.Modifiers), out var action))
        {
            action();
            return true;
        }
        return false;
    }
}
