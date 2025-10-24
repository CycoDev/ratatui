using System;

namespace CycoTui.Core.Style;

/// <summary>
/// Represents a composed style: optional foreground/background/underline colors and modifier additions/removals.
/// This is a minimal phase-1 version; underline color capability will be respected once backends surface it.
/// </summary>
public readonly struct Style : IEquatable<Style>
{
    public Color? Foreground { get; }
    public Color? Background { get; }
    public Color? UnderlineColor { get; } // may be ignored if backend does not support
    public TextModifier AddModifier { get; }
    public TextModifier SubModifier { get; }

    private Style(Color? fg, Color? bg, Color? ul, TextModifier add, TextModifier sub)
    {
        Foreground = fg;
        Background = bg;
        UnderlineColor = ul;
        AddModifier = add;
        SubModifier = sub;
    }

    public static Style Empty => new(null, null, null, TextModifier.None, TextModifier.None);

    public Style WithForeground(Color color) => new(color, Background, UnderlineColor, AddModifier, SubModifier);
    public Style WithBackground(Color color) => new(Foreground, color, UnderlineColor, AddModifier, SubModifier);
    public Style WithUnderlineColor(Color color) => new(Foreground, Background, color, AddModifier, SubModifier);

    public Style Add(TextModifier modifier) => new(Foreground, Background, UnderlineColor, AddModifier | modifier, SubModifier & ~modifier);
    public Style Remove(TextModifier modifier) => new(Foreground, Background, UnderlineColor, AddModifier & ~modifier, SubModifier | modifier);

    public Style Patch(Style other)
    {
        // Patch semantics: non-null colors override, modifiers OR into add/remove sets
        return new(
            other.Foreground ?? Foreground,
            other.Background ?? Background,
            other.UnderlineColor ?? UnderlineColor,
            AddModifier | other.AddModifier,
            SubModifier | other.SubModifier
        );
    }

    public bool Equals(Style other) =>
        Foreground == other.Foreground &&
        Background == other.Background &&
        UnderlineColor == other.UnderlineColor &&
        AddModifier == other.AddModifier &&
        SubModifier == other.SubModifier;

    public override bool Equals(object? obj) => obj is Style s && Equals(s);
    public override int GetHashCode() => HashCode.Combine(Foreground, Background, UnderlineColor, AddModifier, SubModifier);
    public static bool operator ==(Style left, Style right) => left.Equals(right);
    public static bool operator !=(Style left, Style right) => !left.Equals(right);

    public override string ToString() => $"Style(Fg={Foreground},Bg={Background},Ul={UnderlineColor},+={AddModifier},-={SubModifier})";
}

    /// <summary>
    /// INTERNAL: Terminal integration hook for style transition emission.
    /// </summary>
    internal static string StyleEmitterIntegration(Style from, Style to, bool supportsUnderlineColor = false, bool mapUnderlineToForeground = false)
        => StyleEmitter.Emit(from, to, supportsUnderlineColor, mapUnderlineToForeground);
