using System;
using System.Collections.Generic;

namespace CycoTui.Core.Input;

/// <summary>
/// Parses common ANSI escape sequences into KeyEvents (arrow keys, function keys, modifiers).
/// Simplified: does not cover full xterm protocol.
/// </summary>
public static class EscapeSequenceParser
{
    public static IEnumerable<InputEvent> Parse(string input)
    {
        if (string.IsNullOrEmpty(input)) yield break;
        int i = 0;
        while (i < input.Length)
        {
            char c = input[i];
            if (c == '\u001b') // ESC
            {
                // Look ahead for CSI: ESC [
                if (i + 1 < input.Length && input[i+1] == '[')
                {
                    int start = i + 2;
                    int m = start;
                    while (m < input.Length && !char.IsLetter(input[m]) && input[m] != '~' && input[m] != 'M' && input[m] != 'm') m++;
                    if (m < input.Length)
                    {
                        char final = input[m];
                        string paramsPart = input.Substring(start, m - start);
                        if (final == 'M' || final == 'm')
                        {
                            foreach (var mevt in ParseMouse(paramsPart, final)) yield return mevt;
                        }
                        else if (final == '~')
                        {
                            foreach (var evt in ParseTilde(paramsPart)) yield return evt;
                        }
                        else
                        {
                            var (code, modifiers) = MapCsi(final, paramsPart);
                            yield return InputEvent.FromKey(new KeyEvent(code, null, modifiers));
                        }
                        i = m + 1;
                        continue;
                    }
                }
                // ESC followed by printable => Alt+Char
                if (i + 1 < input.Length)
                {
                    char next = input[i+1];
                    yield return InputEvent.FromKey(new KeyEvent(KeyCode.Character, next, KeyModifiers.Alt));
                    i += 2;
                    continue;
                }
                i++;
            }
            else
            {
                // Regular character
                if (c == '\n' || c == '\r')
                {
                    yield return InputEvent.FromKey(new KeyEvent(KeyCode.Enter, null, KeyModifiers.None));
                }
                else if (c == '\t')
                {
                    yield return InputEvent.FromKey(new KeyEvent(KeyCode.Tab, null, KeyModifiers.None));
                }
                else
                {
                    yield return InputEvent.FromKey(new KeyEvent(KeyCode.Character, c, KeyModifiers.None));
                }
                i++;
            }
        }
    }

    private static (KeyCode code, KeyModifiers mods) MapCsi(char final, string paramPart)
    {
        // Arrow keys: A B C D
        KeyModifiers mods = KeyModifiers.None;
        KeyCode code = final switch
        {
            'A' => KeyCode.ArrowUp,
            'B' => KeyCode.ArrowDown,
            'C' => KeyCode.ArrowRight,
            'D' => KeyCode.ArrowLeft,
            _ => KeyCode.Unknown
        };
        if (string.IsNullOrEmpty(paramPart)) return (code, mods);
        // Parameters like 1;2A where second number encodes modifier (xterm style):
        // 2=Shift, 3=Alt, 4=Shift+Alt, 5=Ctrl, 6=Shift+Ctrl, 7=Alt+Ctrl, 8=Shift+Alt+Ctrl
        var parts = paramPart.Split(';');
        if (parts.Length >= 2 && int.TryParse(parts[1], out int modVal))
        {
            mods = modVal switch
            {
                2 => KeyModifiers.Shift,
                3 => KeyModifiers.Alt,
                4 => KeyModifiers.Shift | KeyModifiers.Alt,
                5 => KeyModifiers.Ctrl,
                6 => KeyModifiers.Ctrl | KeyModifiers.Shift,
                7 => KeyModifiers.Ctrl | KeyModifiers.Alt,
                8 => KeyModifiers.Ctrl | KeyModifiers.Alt | KeyModifiers.Shift,
                _ => KeyModifiers.None
            };
        }
        return (code, mods);
    }
    private static IEnumerable<InputEvent> ParseTilde(string paramPart)
    {
        if (!int.TryParse(paramPart, out var code)) yield break;
        KeyCode kc = code switch
        {
            1 => KeyCode.Home,
            4 => KeyCode.End,
            5 => KeyCode.PageUp,
            6 => KeyCode.PageDown,
            3 => KeyCode.Delete,
            11 => KeyCode.F1,
            12 => KeyCode.F2,
            13 => KeyCode.F3,
            14 => KeyCode.F4,
            15 => KeyCode.F5,
            17 => KeyCode.F6,
            18 => KeyCode.F7,
            19 => KeyCode.F8,
            20 => KeyCode.F9,
            21 => KeyCode.F10,
            23 => KeyCode.F11,
            24 => KeyCode.F12,
            _ => KeyCode.Unknown
        };
        yield return InputEvent.FromKey(new KeyEvent(kc, null, KeyModifiers.None));
    }

    private static IEnumerable<InputEvent> ParseMouse(string paramPart, char final)
    {
        if (!paramPart.StartsWith("<")) yield break;
        var body = paramPart.Substring(1);
        var parts = body.Split(';');
        if (parts.Length < 3) yield break;
        if (!int.TryParse(parts[0], out var b) || !int.TryParse(parts[1], out var x) || !int.TryParse(parts[2], out var y)) yield break;
        var kind = final == 'M' ? MouseEventKind.ButtonDown : MouseEventKind.ButtonUp;
        MouseButton btn = b switch
        {
            0 => MouseButton.Left,
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
            _ => MouseButton.None
        };
        if (b >= 64 && b <= 65) kind = b == 64 ? MouseEventKind.ScrollUp : MouseEventKind.ScrollDown;
        yield return InputEvent.FromMouse(new MouseEvent(x - 1, y - 1, kind, btn, KeyModifiers.None));
    }

}
