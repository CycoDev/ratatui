using System;

namespace CycoTui.Core.Style;

/// <summary>
/// Computes difference between two modifier sets.
/// Phase-1: simple added/removed calculation.
/// TODO (IMPLEMENTATION-DECISIONS-001 #5): add Bold/Dim intensity normalization logic and minimal ANSI sequence emission ordering.
/// </summary>
public readonly struct StyleDiff
{
    public TextModifier Added { get; }
    public TextModifier Removed { get; }

    public StyleDiff(TextModifier from, TextModifier to)
    {
        Added = to & ~from;
        Removed = from & ~to;
    }

    public bool IsEmpty => Added == TextModifier.None && Removed == TextModifier.None;
}
