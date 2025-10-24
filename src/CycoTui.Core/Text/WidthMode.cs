namespace CycoTui.Core.Text;

/// <summary>
/// Mode controlling treatment of ambiguous-width characters.
/// Standard: ambiguous treated as width 1.
/// EastAsian: ambiguous treated as width 2.
/// </summary>
public enum WidthMode
{
    Standard = 0,
    EastAsian = 1
}
