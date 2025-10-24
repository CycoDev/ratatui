using System;

namespace CycoTui.Core.Layout;

/// <summary>
/// Represents a layout constraint influencing size calculation for a segment.
/// </summary>
/// <remarks>
/// Precedence order in distribution: Length -> Min -> Percentage -> Ratio -> Fill -> Max enforcement.
/// Ratio uses Value/Denominator as weight.
/// </remarks>
public readonly struct Constraint
{
    public ConstraintKind Kind { get; }
    public int Value { get; }          // used for Length, Min, Max, Percentage numerator, Fill order
    public int Denominator { get; }    // used for Ratio denominator

    private Constraint(ConstraintKind kind, int value, int denominator = 0)
    {
        Kind = kind;
        Value = value;
        Denominator = denominator;
    }

    public static Constraint Length(int value) => new(ConstraintKind.Length, Math.Max(0, value));
    public static Constraint Min(int value) => new(ConstraintKind.Min, Math.Max(0, value));
    public static Constraint Max(int value) => new(ConstraintKind.Max, Math.Max(0, value));
    public static Constraint Percentage(int percent) => new(ConstraintKind.Percentage, Math.Clamp(percent, 0, 100));
    public static Constraint Ratio(int numerator, int denominator) => new(ConstraintKind.Ratio, Math.Max(0, numerator), Math.Max(1, denominator));
    public static Constraint Fill(int order = 0) => new(ConstraintKind.Fill, Math.Max(0, order));

    public override string ToString() => Kind switch
    {
        ConstraintKind.Length => $"Length({Value})",
        ConstraintKind.Min => $"Min({Value})",
        ConstraintKind.Max => $"Max({Value})",
        ConstraintKind.Percentage => $"Percentage({Value}%)",
        ConstraintKind.Ratio => $"Ratio({Value}/{Denominator})",
        ConstraintKind.Fill => $"Fill(order={Value})",
        _ => "Unknown"
    };
}
