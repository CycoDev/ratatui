namespace CycoTui.Core.Input;

public readonly record struct KeyEvent(KeyCode Code, char? Char, KeyModifiers Modifiers);
public readonly record struct MouseEvent(int X, int Y, MouseEventKind Kind, MouseButton Button, KeyModifiers Modifiers);
public readonly record struct ResizeEvent(int Width, int Height);
public readonly record struct FocusEvent(object? OldFocus, object? NewFocus);

public readonly struct InputEvent
{
    public InputEventType Type { get; }
    public KeyEvent? Key { get; }
    public MouseEvent? Mouse { get; }
    public ResizeEvent? Resize { get; }
    public FocusEvent? Focus { get; }

    private InputEvent(InputEventType type, KeyEvent? key = null, MouseEvent? mouse = null, ResizeEvent? resize = null, FocusEvent? focus = null)
    {
        Type = type; Key = key; Mouse = mouse; Resize = resize; Focus = focus;
    }

    public static InputEvent FromKey(KeyEvent e) => new(InputEventType.Key, key: e);
    public static InputEvent FromMouse(MouseEvent e) => new(InputEventType.Mouse, mouse: e);
    public static InputEvent FromResize(ResizeEvent e) => new(InputEventType.Resize, resize: e);
    public static InputEvent FromFocus(FocusEvent e) => new(InputEventType.Focus, focus: e);
}
