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
                    while (m < input.Length && !char.IsLetter(input[m])) m++;
                    if (m < input.Length)
                    {
                        char final = input[m];
                        string paramsPart = input.Substring(start, m - start);
                        var (code, modifiers) = MapCsi(final, paramsPart);
                        yield return InputEvent.FromKey(new KeyEvent(code, null, modifiers));
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
}
