namespace CycoTui.Core.Style;

/// <summary>
/// Provides ANSI codes for setting and resetting text modifiers.
/// </summary>
internal static class ModifierCodes
{
    public const string SetBold = "\u001b[1m";
    public const string ResetBoldDim = "\u001b[22m"; // resets both bold and dim
    public const string SetDim = "\u001b[2m";
    public const string SetItalic = "\u001b[3m";
    public const string ResetItalic = "\u001b[23m";
    public const string SetUnderline = "\u001b[4m";
    public const string ResetUnderline = "\u001b[24m";
    public const string SetBlink = "\u001b[5m";
    public const string ResetBlink = "\u001b[25m";
    public const string SetInvert = "\u001b[7m";
    public const string ResetInvert = "\u001b[27m";
    public const string SetHidden = "\u001b[8m";
    public const string ResetHidden = "\u001b[28m";
    public const string SetStrikethrough = "\u001b[9m";
    public const string ResetStrikethrough = "\u001b[29m";
    public const string FullReset = "\u001b[0m";
}
