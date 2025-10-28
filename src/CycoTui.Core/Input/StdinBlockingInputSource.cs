using System;
using System.Threading;

namespace CycoTui.Core.Input;

/// <summary>
/// Blocking stdin source using Console.ReadKey. Simplified; escape sequences read naïvely.
/// </summary>
public sealed class StdinBlockingInputSource : IBlockingInputSource, IDisposable
{
    private readonly bool _intercept;
    public StdinBlockingInputSource(bool intercept = true) => _intercept = intercept;
    public void Dispose() { }

    public bool TryReadBlocking(out InputEvent evt, CancellationToken token)
    {
        while (!Console.KeyAvailable)
        {
            if (token.IsCancellationRequested)
            {
                evt = default;
                return false;
            }
            Thread.Sleep(5);
        }
        var keyInfo = Console.ReadKey(_intercept);
        evt = MapConsoleKey(keyInfo);
        // If ESC and more keys rapidly follow, attempt simple sequence accumulation (non-blocking peek).
        if (keyInfo.Key == ConsoleKey.Escape && Console.KeyAvailable)
        {
            var seq = "\u001b";
            while (Console.KeyAvailable)
            {
                var k = Console.ReadKey(_intercept);
                seq += k.KeyChar;
                if (!Console.KeyAvailable) break;
            }
            foreach (var e in EscapeSequenceParser.Parse(seq)) { evt = e; break; }
        }
        return true;
    }

    private static InputEvent MapConsoleKey(ConsoleKeyInfo info)
    {
        KeyModifiers mods = KeyModifiers.None;
        if ((info.Modifiers & ConsoleModifiers.Control) != 0) mods |= KeyModifiers.Ctrl;
        if ((info.Modifiers & ConsoleModifiers.Alt) != 0) mods |= KeyModifiers.Alt;
        if ((info.Modifiers & ConsoleModifiers.Shift) != 0) mods |= KeyModifiers.Shift;
        KeyCode code = info.Key switch
        {
            ConsoleKey.Enter => KeyCode.Enter,
            ConsoleKey.Tab => KeyCode.Tab,
            ConsoleKey.Backspace => KeyCode.Backspace,
            ConsoleKey.Delete => KeyCode.Delete,
            ConsoleKey.Home => KeyCode.Home,
            ConsoleKey.End => KeyCode.End,
            ConsoleKey.PageUp => KeyCode.PageUp,
            ConsoleKey.PageDown => KeyCode.PageDown,
            ConsoleKey.UpArrow => KeyCode.ArrowUp,
            ConsoleKey.DownArrow => KeyCode.ArrowDown,
            ConsoleKey.LeftArrow => KeyCode.ArrowLeft,
            ConsoleKey.RightArrow => KeyCode.ArrowRight,
            ConsoleKey.F1 => KeyCode.F1,
            ConsoleKey.F2 => KeyCode.F2,
            ConsoleKey.F3 => KeyCode.F3,
            ConsoleKey.F4 => KeyCode.F4,
            ConsoleKey.F5 => KeyCode.F5,
            ConsoleKey.F6 => KeyCode.F6,
            ConsoleKey.F7 => KeyCode.F7,
            ConsoleKey.F8 => KeyCode.F8,
            ConsoleKey.F9 => KeyCode.F9,
            ConsoleKey.F10 => KeyCode.F10,
            ConsoleKey.F11 => KeyCode.F11,
            ConsoleKey.F12 => KeyCode.F12,
            _ => KeyCode.Character
        };
        char? ch = code == KeyCode.Character ? info.KeyChar : null;
        return InputEvent.FromKey(new KeyEvent(code, ch, mods));
    }
}
