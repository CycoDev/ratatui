namespace CycoTui.Core.Style;

/// <summary>
/// Bit flags representing text styling modifiers. These correspond to terminal
/// attributes where supported. Some combinations (Bold + Dim) are mutually exclusive
/// and will be normalized by the style diff logic in later phases.
/// </summary>
[System.Flags]
public enum TextModifier
{
    None        = 0,
    Bold        = 1 << 0,
    Dim         = 1 << 1,
    Italic      = 1 << 2,
    Underline   = 1 << 3,
    Blink       = 1 << 4,
    Invert      = 1 << 5,
    Hidden      = 1 << 6,
    Strikethrough = 1 << 7
}
