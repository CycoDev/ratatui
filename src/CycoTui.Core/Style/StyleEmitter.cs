using System.Text;

namespace CycoTui.Core.Style;

/// <summary>Converts style transitions into ANSI escape sequences.</summary>
public static class StyleEmitter
{
    /// <summary>Emit ANSI sequences representing transition from one style to another.</summary>
    public static string Emit(StyleType from, StyleType to, bool supportsUnderlineColor = false, bool mapUnderlineToForeground = false)
    {
        var sb = new StringBuilder();
        EmitColorChanges(sb, from, to, supportsUnderlineColor, mapUnderlineToForeground);
        EmitModifierChanges(sb, from, to);
        return sb.ToString();
    }

    private static void EmitColorChanges(StringBuilder sb, StyleType from, StyleType to, bool supportsUnderlineColor, bool mapUnderlineToForeground)
    {
        if (from.Foreground != to.Foreground)
        {
            if (to.Foreground.HasValue) sb.Append(GetAnsiForeground(to.Foreground.Value));
            else if (from.Foreground.HasValue) sb.Append("\u001b[39m");
        }
        if (from.Background != to.Background)
        {
            if (to.Background.HasValue) sb.Append(GetAnsiBackground(to.Background.Value));
            else if (from.Background.HasValue) sb.Append("\u001b[49m");
        }
        if (from.UnderlineColor != to.UnderlineColor)
        {
            if (to.UnderlineColor.HasValue && supportsUnderlineColor) sb.Append(GetAnsiUnderlineColor(to.UnderlineColor.Value));
            else if (to.UnderlineColor.HasValue && mapUnderlineToForeground) sb.Append(GetAnsiForeground(to.UnderlineColor.Value));
        }
    }

    private static void EmitModifierChanges(StringBuilder sb, StyleType from, StyleType to)
    {
        var fromMods = from.AddModifier;
        var toMods = to.AddModifier;
        var removed = fromMods & ~toMods;
        var added = toMods & ~fromMods;
        bool intensityChanged = (fromMods.HasFlag(TextModifier.Bold) != toMods.HasFlag(TextModifier.Bold)) ||
                                (fromMods.HasFlag(TextModifier.Dim) != toMods.HasFlag(TextModifier.Dim));
        if (intensityChanged) sb.Append(ModifierCodes.ResetBoldDim);
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
    }

    private static void EmitAddModifiers(StringBuilder sb, TextModifier added)
    {
        if (added == TextModifier.None) return;
        if (added.HasFlag(TextModifier.Bold)) sb.Append(ModifierCodes.SetBold);
        if (added.HasFlag(TextModifier.Dim)) sb.Append(ModifierCodes.SetDim);
        if (added.HasFlag(TextModifier.Italic)) sb.Append(ModifierCodes.SetItalic);
        if (added.HasFlag(TextModifier.Underline)) sb.Append(ModifierCodes.SetUnderline);
        if (added.HasFlag(TextModifier.Blink)) sb.Append(ModifierCodes.SetBlink);
        if (added.HasFlag(TextModifier.Invert)) sb.Append(ModifierCodes.SetInvert);
        if (added.HasFlag(TextModifier.Hidden)) sb.Append(ModifierCodes.SetHidden);
        if (added.HasFlag(TextModifier.Strikethrough)) sb.Append(ModifierCodes.SetStrikethrough);
    }

    private static string GetAnsiForeground(Color color) => color.Kind switch
    {
        ColorKind.Ansi => $"\u001b[3{color.Index % 8}m",
        ColorKind.Indexed => $"\u001b[38;5;{color.Index}m",
        ColorKind.Rgb => $"\u001b[38;2;{color.R};{color.G};{color.B}m",
        _ => string.Empty
    };

    private static string GetAnsiBackground(Color color) => color.Kind switch
    {
        ColorKind.Ansi => $"\u001b[4{color.Index % 8}m",
        ColorKind.Indexed => $"\u001b[48;5;{color.Index}m",
        ColorKind.Rgb => $"\u001b[48;2;{color.R};{color.G};{color.B}m",
        _ => string.Empty
    };

    private static string GetAnsiUnderlineColor(Color color) => color.Kind switch
    {
        ColorKind.Ansi => $"\u001b[58;5;{color.Index}m",
        ColorKind.Indexed => $"\u001b[58;5;{color.Index}m",
        ColorKind.Rgb => $"\u001b[58;2;{color.R};{color.G};{color.B}m",
        _ => string.Empty
    };
}