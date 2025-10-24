namespace CycoTui.Core.Text;

/// <summary>
/// Represents text whose visual representation is masked but original content retained.
/// Phase-2 simple implementation; does not secure memory.
/// </summary>
public sealed class MaskedText
{
    public string Original { get; }
    public char MaskChar { get; }

    public MaskedText(string original, char maskChar = '*')
    {
        Original = original ?? string.Empty;
        MaskChar = maskChar;
    }

    public string Render() => new string(MaskChar, Original.Length);
}
