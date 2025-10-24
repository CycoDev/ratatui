using System.Text;


namespace CycoTui.Core.Style;

using System.Text;

namespace CycoTui.Core.Style;

/// <summary>
/// Converts style transitions into ANSI escape sequences with normalization and minimal resets.
/// Features:
/// - Foreground/background set & reset (ESC[39m / ESC[49m)
/// - Underline color (SGR 58) when supported, or degraded to foreground
/// - Bold/Dim intensity normalization (ESC[22m)
/// - Modifier add/remove sequences (italic, underline, blink, invert, hidden, strikethrough)
/// </summary>
public static class StyleEmitter
{
    /// <summary>
    /// Emit ANSI sequences for transitioning from <paramref name="from"/> to <paramref name="to"/>.
    /// </summary>
    public static string Emit(Style from, Style to, bool supportsUnderlineColor = false, bool mapUnderlineToForeground = false)
    {
        var sb = new StringBuilder();
        EmitColorChanges(sb, from, to, supportsUnderlineColor, mapUnderlineToForeground);
        EmitModifierChanges(sb, from, to);
        return sb.ToString();
    }

    private static void EmitColorChanges(StringBuilder sb, Style from, Style to, bool supportsUnderlineColor, bool mapUnderlineToForeground)
    {
        // Foreground changes
        if (from.Foreground != to.Foreground)
        {
            if (to.Foreground.HasValue)
            {
                sb.Append(GetAnsiForeground(to.Foreground.Value));
            }
            else if (from.Foreground.HasValue)
            {
                sb.Append("\u001b[39m"); // reset foreground
            }
        }
        // Background changes
        if (from.Background != to.Background)
        {
            if (to.Background.HasValue)
            {
                sb.Append(GetAnsiBackground(to.Background.Value));
            }
            else if (from.Background.HasValue)
            {
                sb.Append("\u001b[49m"); // reset background
            }
        }
        // Underline color conditional
        if (from.UnderlineColor != to.UnderlineColor)
        {
            if (to.UnderlineColor.HasValue && supportsUnderlineColor)
            {
                sb.Append(GetAnsiUnderlineColor(to.UnderlineColor.Value));
            }
            else if (to.UnderlineColor.HasValue && mapUnderlineToForeground)
            {
                sb.Append(GetAnsiForeground(to.UnderlineColor.Value)); // degrade to foreground
            }
            else if (from.UnderlineColor.HasValue && supportsUnderlineColor && !to.UnderlineColor.HasValue)
            {
                // Reset underline color (fallback: full reset of underline color - often just underline off already handled by modifier removal)
                // Many terminals do not support explicit reset; rely on modifier removal.
            }
        }
    }

    private static void EmitModifierChanges(StringBuilder sb, Style from, Style to)
    {
        var fromMods = from.AddModifier;
        var toMods = to.AddModifier;
        // Compute removed & added
        var removed = fromMods & ~toMods;
        var added = toMods & ~fromMods;

        // Intensity normalization (Bold/Dim): if changing intensity, reset first
        bool intensityChanged = (fromMods.HasFlag(TextModifier.Bold) != toMods.HasFlag(TextModifier.Bold))
                                || (fromMods.HasFlag(TextModifier.Dim) != toMods.HasFlag(TextModifier.Dim));
        if (intensityChanged)
        {
            sb.Append(ModifierCodes.ResetBoldDim);
        }

        EmitRemovedModifiers(sb, removed);
        EmitAddModifiers(sb, added);
    }

    private static void EmitRemovedModifiers(StringBuilder sb, TextModifier removed)
    {
        if (removed == TextModifier.None) return;
        if (removed.HasFlag(TextModifier.Italic)) sb.Append(ModifierCodes.ResetItalic);
        if (removed.HasFlag(TextModifier.Underline)) sb.Append(ModifierCodes.ResetUnderline);
        if (removed.HasFlag(TextModifier.Blink)) sb.Append(ModifierCodes.ResetBlink);
        if (removed.HasFlag(TextModifier.Invert)) sb.Append(ModifierCodes.ResetInvert);
        if (removed.HasFlag(TextModifier.Hidden)) sb.Append(ModifierCodes.ResetHidden);
        if (removed.HasFlag(TextModifier.Strikethrough)) sb.Append(ModifierCodes.ResetStrikethrough);
        // Bold/Dim handled by intensity reset above.
    }

    }

    private static void EmitAddModifiers(StringBuilder sb, TextModifier added)
    {
        if (added == TextModifier.None) return;
        if (added.HasFlag(TextModifier.Bold)) sb.Append("\u001b[1m");
        if (added.HasFlag(TextModifier.Dim)) sb.Append("\u001b[2m");
        if (added.HasFlag(TextModifier.Italic)) sb.Append("\u001b[3m");
        if (added.HasFlag(TextModifier.Underline)) sb.Append("\u001b[4m");
        if (added.HasFlag(TextModifier.Blink)) sb.Append("\u001b[5m");
        if (added.HasFlag(TextModifier.Invert)) sb.Append("\u001b[7m");
        if (added.HasFlag(TextModifier.Hidden)) sb.Append("\u001b[8m");
        if (added.HasFlag(TextModifier.Strikethrough)) sb.Append("\u001b[9m");
    }

    private static string GetAnsiForeground(Color color)
    {
        return color.Kind switch
        {
            ColorKind.Ansi => $"\u001b[3{color.Index % 8}m", // basic; bright not yet handled
            ColorKind.Indexed => $"\u001b[38;5;{color.Index}m",
            ColorKind.Rgb => $"\u001b[38;2;{color.R};{color.G};{color.B}m",
            _ => string.Empty
        };
    }

    private static string GetAnsiBackground(Color color)
    {
        return color.Kind switch
        {
            ColorKind.Ansi => $"\u001b[4{color.Index % 8}m",
            ColorKind.Indexed => $"\u001b[48;5;{color.Index}m",
            ColorKind.Rgb => $"\u001b[48;2;{color.R};{color.G};{color.B}m",
            _ => string.Empty
        };
    }
}

    private static string GetAnsiUnderlineColor(Color color)
    {
        return color.Kind switch
        {
            ColorKind.Ansi => $"\u001b[58;5;{color.Index}m", // map basic ansi to indexed range
            ColorKind.Indexed => $"\u001b[58;5;{color.Index}m",
            ColorKind.Rgb => $"\u001b[58;2;{color.R};{color.G};{color.B}m",
            _ => string.Empty
        };
    }
