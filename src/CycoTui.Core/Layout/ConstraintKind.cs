namespace CycoTui.Core.Layout;

/// <summary>
/// Kind of layout constraint applied to a segment.
/// </summary>
public enum ConstraintKind
{
    Length,
    Min,
    Max,
    Percentage,
    Ratio,
    Fill
}
