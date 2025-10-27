namespace CycoTui.Core.Input;

public enum InputEventType
{
    Key,
    Mouse,
    Resize,
    Focus
}

[System.Flags]
public enum KeyModifiers
{
    None = 0,
    Ctrl = 1 << 0,
    Alt  = 1 << 1,
    Shift = 1 << 2,
    Meta = 1 << 3
}

public enum KeyCode
{
    Unknown,
    Enter,
    Escape,
    Tab,
    Backspace,
    Delete,
    Home,
    End,
    PageUp,
    PageDown,
    ArrowUp,
    ArrowDown,
    ArrowLeft,
    ArrowRight,
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    Character // printable char/grapheme
}

public enum MouseEventKind { Move, ButtonDown, ButtonUp, ScrollUp, ScrollDown, Drag }
public enum MouseButton { None, Left, Middle, Right, X1, X2 }
