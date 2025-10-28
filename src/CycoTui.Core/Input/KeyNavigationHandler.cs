using System;

namespace CycoTui.Core.Input;

/// <summary>
/// Provides basic Tab/Shift+Tab navigation for a FocusManager.
/// </summary>
public sealed class KeyNavigationHandler
{
    private readonly FocusManager _focusManager;
    public KeyNavigationHandler(FocusManager fm) => _focusManager = fm;

    public bool Handle(InputEvent evt)
    {
        if (evt.Type != InputEventType.Key || !evt.Key.HasValue) return false;
        var key = evt.Key.Value;
        if (key.Code == KeyCode.Tab)
        {
            if ((key.Modifiers & KeyModifiers.Shift) != 0)
                _focusManager.Previous();
            else
                _focusManager.Next();
            return true;
        }
        return false;
    }
}
