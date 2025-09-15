using System;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Layout
{
    /// <summary>
    /// Represents a layout constraint that defines how space should be allocated.
    /// Constraints can be absolute lengths, percentages, ratios, or min/max bounds.
    /// </summary>
    public readonly struct Constraint : IEquatable<Constraint>
    {
        /// <summary>
        /// Gets the type of this constraint.
        /// </summary>
        public ConstraintType Type { get; }

        /// <summary>
        /// Gets the numeric value associated with this constraint.
        /// The meaning depends on the constraint type.
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint"/> struct.
        /// </summary>
        /// <param name="type">The constraint type.</param>
        /// <param name="value">The constraint value.</param>
        private Constraint(ConstraintType type, float value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Creates a constraint that allocates a fixed number of units.
        /// </summary>
        /// <param name="length">The fixed length in terminal cells.</param>
        /// <returns>A length constraint.</returns>
        public static Constraint Length(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");
            return new Constraint(ConstraintType.Length, length);
        }

        /// <summary>
        /// Creates a constraint that allocates a percentage of available space.
        /// </summary>
        /// <param name="percentage">The percentage (0-100).</param>
        /// <returns>A percentage constraint.</returns>
        public static Constraint Percentage(float percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100.");
            return new Constraint(ConstraintType.Percentage, percentage);
        }

        /// <summary>
        /// Creates a constraint that allocates space based on a ratio relative to other ratio constraints.
        /// </summary>
        /// <param name="ratio">The ratio value (must be positive).</param>
        /// <returns>A ratio constraint.</returns>
        public static Constraint Ratio(float ratio)
        {
            if (ratio <= 0)
                throw new ArgumentOutOfRangeException(nameof(ratio), "Ratio must be positive.");
            return new Constraint(ConstraintType.Ratio, ratio);
        }

        /// <summary>
        /// Creates a constraint that ensures a minimum number of units.
        /// </summary>
        /// <param name="min">The minimum length in terminal cells.</param>
        /// <returns>A minimum constraint.</returns>
        public static Constraint Min(int min)
        {
            if (min < 0)
                throw new ArgumentOutOfRangeException(nameof(min), "Minimum cannot be negative.");
            return new Constraint(ConstraintType.Min, min);
        }

        /// <summary>
        /// Creates a constraint that limits to a maximum number of units.
        /// </summary>
        /// <param name="max">The maximum length in terminal cells.</param>
        /// <returns>A maximum constraint.</returns>
        public static Constraint Max(int max)
        {
            if (max < 0)
                throw new ArgumentOutOfRangeException(nameof(max), "Maximum cannot be negative.");
            return new Constraint(ConstraintType.Max, max);
        }

        /// <summary>
        /// Creates a constraint that fills available space (equivalent to Ratio(1)).
        /// </summary>
        /// <returns>A fill constraint.</returns>
        public static Constraint Fill() => Ratio(1.0f);

        /// <summary>
        /// Applies this constraint to calculate the actual size given available space and context.
        /// </summary>
        /// <param name="availableSpace">The total available space.</param>
        /// <param name="totalRatio">The total ratio for ratio constraints (used for ratio calculations).</param>
        /// <returns>The calculated size for this constraint.</returns>
        internal int Apply(int availableSpace, float totalRatio = 1.0f)
        {
            return Type switch
            {
                ConstraintType.Length => (int)Value,
                ConstraintType.Percentage => (int)(availableSpace * Value / 100.0f),
                ConstraintType.Ratio => totalRatio > 0 ? (int)(availableSpace * Value / totalRatio) : 0,
                ConstraintType.Min => Math.Max((int)Value, 0),
                ConstraintType.Max => Math.Min((int)Value, availableSpace),
                _ => 0
            };
        }

        /// <summary>
        /// Gets a value indicating whether this constraint has a fixed size (Length, Min, Max).
        /// </summary>
        public bool IsFixed => Type == ConstraintType.Length || Type == ConstraintType.Min || Type == ConstraintType.Max;

        /// <summary>
        /// Gets a value indicating whether this constraint is flexible (Percentage, Ratio).
        /// </summary>
        public bool IsFlexible => Type == ConstraintType.Percentage || Type == ConstraintType.Ratio;

        /// <inheritdoc />
        public bool Equals(Constraint other) => Type == other.Type && Math.Abs(Value - other.Value) < 0.001f;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Constraint other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Type, Value);
#else
            HashCode.Combine(Type, Value);
#endif

        /// <inheritdoc />
        public override string ToString() => Type switch
        {
            ConstraintType.Length => $"Length({Value})",
            ConstraintType.Percentage => $"Percentage({Value}%)",
            ConstraintType.Ratio => $"Ratio({Value})",
            ConstraintType.Min => $"Min({Value})",
            ConstraintType.Max => $"Max({Value})",
            _ => "Unknown"
        };

        /// <summary>
        /// Determines whether two constraints are equal.
        /// </summary>
        public static bool operator ==(Constraint left, Constraint right) => left.Equals(right);

        /// <summary>
        /// Determines whether two constraints are not equal.
        /// </summary>
        public static bool operator !=(Constraint left, Constraint right) => !(left == right);
    }

    /// <summary>
    /// Specifies the type of layout constraint.
    /// </summary>
    public enum ConstraintType
    {
        /// <summary>
        /// A fixed length in terminal cells.
        /// </summary>
        Length,

        /// <summary>
        /// A percentage of available space (0-100).
        /// </summary>
        Percentage,

        /// <summary>
        /// A ratio relative to other ratio constraints.
        /// </summary>
        Ratio,

        /// <summary>
        /// A minimum size constraint.
        /// </summary>
        Min,

        /// <summary>
        /// A maximum size constraint.
        /// </summary>
        Max
    }
}