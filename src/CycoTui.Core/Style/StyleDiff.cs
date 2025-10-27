using System;

namespace CycoTui.Core.Style;

/// <summary>
/// Computes difference between two modifier sets.
/// Phase-1: simple added/removed calculation.
/// Intensity (Bold/Dim) normalization: if either Bold or Dim changes, reset both then apply new set to avoid conflicting states.
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
