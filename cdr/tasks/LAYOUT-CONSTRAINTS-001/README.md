# Layout Constraint System

## Overview

This task involves implementing the constraint-based layout system for CycoTui, which provides flexible, responsive layouts for terminal UIs. Based on detailed analysis of `ratatui-core/src/layout/constraint.rs`, this task will create the core constraint types with the exact priority system and behavior from Ratatui.

The constraint system provides six distinct constraint types with a well-defined priority order, comprehensive factory methods for creating constraint collections, and robust calculation algorithms with overflow protection.

## Implementation Approach

Based on the analysis of `ratatui-core/src/layout/constraint.rs`, implement the constraint system as follows:

1. **Create the base Constraint class**:
   ```csharp
   public abstract class Constraint : IEquatable<Constraint>
   {
       // Factory methods for constraint types (priority order)
       public static Constraint Min(ushort min) => new MinConstraint(min);
       public static Constraint Max(ushort max) => new MaxConstraint(max);
       public static Constraint Length(ushort length) => new LengthConstraint(length);
       public static Constraint Percentage(ushort percentage) => new PercentageConstraint(percentage);
       public static Constraint Ratio(uint numerator, uint denominator) => new RatioConstraint(numerator, denominator);
       public static Constraint Fill(ushort factor = 1) => new FillConstraint(factor);
       
       // Collection factory methods (key convenience feature from Ratatui)
       public static Constraint[] FromLengths(params ushort[] lengths) => 
           lengths.Select(Length).ToArray();
       public static Constraint[] FromLengths(IEnumerable<ushort> lengths) => 
           lengths.Select(Length).ToArray();
           
       public static Constraint[] FromRatios(params (uint numerator, uint denominator)[] ratios) => 
           ratios.Select(r => Ratio(r.numerator, r.denominator)).ToArray();
       public static Constraint[] FromRatios(IEnumerable<(uint, uint)> ratios) => 
           ratios.Select(r => Ratio(r.Item1, r.Item2)).ToArray();
           
       public static Constraint[] FromPercentages(params ushort[] percentages) => 
           percentages.Select(Percentage).ToArray();
       public static Constraint[] FromPercentages(IEnumerable<ushort> percentages) => 
           percentages.Select(Percentage).ToArray();
           
       public static Constraint[] FromMins(params ushort[] mins) => 
           mins.Select(Min).ToArray();
       public static Constraint[] FromMins(IEnumerable<ushort> mins) => 
           mins.Select(Min).ToArray();
           
       public static Constraint[] FromMaxes(params ushort[] maxes) => 
           maxes.Select(Max).ToArray();
       public static Constraint[] FromMaxes(IEnumerable<ushort> maxes) => 
           maxes.Select(Max).ToArray();
           
       public static Constraint[] FromFills(params ushort[] factors) => 
           factors.Select(Fill).ToArray();
       public static Constraint[] FromFills(IEnumerable<ushort> factors) => 
           factors.Select(Fill).ToArray();
       
       // Implicit conversion from ushort (creates Length constraint)
       public static implicit operator Constraint(ushort length) => Length(length);
       
       // Apply constraint to available space (deprecated but needed for compatibility)
       [Obsolete("This method will be hidden in future versions")]
       public abstract ushort Apply(ushort length);
       
       // Equality and display
       public abstract bool Equals(Constraint other);
       public override abstract bool Equals(object obj);
       public override abstract int GetHashCode();
       public override abstract string ToString();
       
       // Default constraint
       public static Constraint Default => Percentage(100);
   }
   ```

2. **Implement specific constraint types with exact Ratatui behavior**:
   ```csharp
   internal sealed class MinConstraint : Constraint
   {
       public ushort Value { get; }
       
       public MinConstraint(ushort value) => Value = value;
       
       public override ushort Apply(ushort length) => Math.Max(length, Value);
       
       public override bool Equals(Constraint other) => other is MinConstraint min && min.Value == Value;
       public override bool Equals(object obj) => obj is MinConstraint other && Equals(other);
       public override int GetHashCode() => HashCode.Combine(nameof(MinConstraint), Value);
       public override string ToString() => $"Min({Value})";
   }

   internal sealed class MaxConstraint : Constraint  
   {
       public ushort Value { get; }
       
       public MaxConstraint(ushort value) => Value = value;
       
       public override ushort Apply(ushort length) => Math.Min(length, Value);
       
       public override bool Equals(Constraint other) => other is MaxConstraint max && max.Value == Value;
       public override bool Equals(object obj) => obj is MaxConstraint other && Equals(other);
       public override int GetHashCode() => HashCode.Combine(nameof(MaxConstraint), Value);
       public override string ToString() => $"Max({Value})";
   }

   internal sealed class LengthConstraint : Constraint
   {
       public ushort Value { get; }
       
       public LengthConstraint(ushort value) => Value = value;
       
       public override ushort Apply(ushort length) => Math.Min(length, Value);
       
       public override bool Equals(Constraint other) => other is LengthConstraint len && len.Value == Value;
       public override bool Equals(object obj) => obj is LengthConstraint other && Equals(other);
       public override int GetHashCode() => HashCode.Combine(nameof(LengthConstraint), Value);
       public override string ToString() => $"Length({Value})";
   }

   internal sealed class PercentageConstraint : Constraint
   {
       public ushort Value { get; }
       
       public PercentageConstraint(ushort value) => Value = value;
       
       public override ushort Apply(ushort length)
       {
           // Match Ratatui's exact calculation: f32::from(p) / 100.0 * f32::from(length)
           var percentage = Value / 100.0f;
           var result = percentage * length;
           return (ushort)Math.Min(result, length);
       }
       
       public override bool Equals(Constraint other) => other is PercentageConstraint pct && pct.Value == Value;
       public override bool Equals(object obj) => obj is PercentageConstraint other && Equals(other);
       public override int GetHashCode() => HashCode.Combine(nameof(PercentageConstraint), Value);
       public override string ToString() => $"Percentage({Value})";
   }

   internal sealed class RatioConstraint : Constraint
   {
       public uint Numerator { get; }
       public uint Denominator { get; }
       
       public RatioConstraint(uint numerator, uint denominator) => 
           (Numerator, Denominator) = (numerator, denominator);
       
       public override ushort Apply(ushort length)
       {
           // Match Ratatui's exact calculation with division-by-zero protection
           // Avoid division by zero by using 1 when denominator is 0
           // This results in 0/0 -> 0 and x/0 -> x for x != 0
           var percentage = (float)Numerator / Math.Max(1u, Denominator);
           var result = percentage * length;
           return (ushort)Math.Min(result, length);
       }
       
       public override bool Equals(Constraint other) => 
           other is RatioConstraint ratio && ratio.Numerator == Numerator && ratio.Denominator == Denominator;
       public override bool Equals(object obj) => obj is RatioConstraint other && Equals(other);
       public override int GetHashCode() => HashCode.Combine(nameof(RatioConstraint), Numerator, Denominator);
       public override string ToString() => $"Ratio({Numerator}, {Denominator})";
   }

   internal sealed class FillConstraint : Constraint
   {
       public ushort Factor { get; }
       
       public FillConstraint(ushort factor) => Factor = factor;
       
       public override ushort Apply(ushort length) => Math.Min(length, Factor);
       
       public override bool Equals(Constraint other) => other is FillConstraint fill && fill.Factor == Factor;
       public override bool Equals(object obj) => obj is FillConstraint other && Equals(other);
       public override int GetHashCode() => HashCode.Combine(nameof(FillConstraint), Factor);
       public override string ToString() => $"Fill({Factor})";
   }
   ```

3. **Add extension methods for constraint checking (matching Ratatui's EnumIs)**:
   ```csharp
   public static class ConstraintExtensions
   {
       public static bool IsMin(this Constraint constraint) => constraint is MinConstraint;
       public static bool IsMax(this Constraint constraint) => constraint is MaxConstraint;
       public static bool IsLength(this Constraint constraint) => constraint is LengthConstraint;
       public static bool IsPercentage(this Constraint constraint) => constraint is PercentageConstraint;
       public static bool IsRatio(this Constraint constraint) => constraint is RatioConstraint;
       public static bool IsFill(this Constraint constraint) => constraint is FillConstraint;
   }
   ```

4. **Implement comprehensive unit tests matching Ratatui's test suite**:
   ```csharp
   [Test]
   public void ConstraintApplyTests()
   {
       // Test percentage calculations
       Assert.AreEqual(0, Constraint.Percentage(0).Apply(100));
       Assert.AreEqual(50, Constraint.Percentage(50).Apply(100));
       Assert.AreEqual(100, Constraint.Percentage(100).Apply(100));
       Assert.AreEqual(100, Constraint.Percentage(200).Apply(100)); // Clamped
       
       // Test ratio calculations with division-by-zero protection
       Assert.AreEqual(0, Constraint.Ratio(0, 0).Apply(100)); // 0/0 -> 0
       Assert.AreEqual(100, Constraint.Ratio(1, 0).Apply(100)); // 1/0 -> 100% of length
       Assert.AreEqual(0, Constraint.Ratio(0, 1).Apply(100));
       Assert.AreEqual(50, Constraint.Ratio(1, 2).Apply(100));
       
       // Test other constraint types
       Assert.AreEqual(50, Constraint.Length(50).Apply(100));
       Assert.AreEqual(100, Constraint.Length(200).Apply(100)); // Clamped
       Assert.AreEqual(50, Constraint.Max(50).Apply(100));
       Assert.AreEqual(200, Constraint.Min(200).Apply(100));
   }
   
   [Test]
   public void ConstraintFactoryMethodTests()
   {
       var expectedLengths = new[] { Constraint.Length(1), Constraint.Length(2), Constraint.Length(3) };
       CollectionAssert.AreEqual(expectedLengths, Constraint.FromLengths(1, 2, 3));
       
       var expectedRatios = new[] { Constraint.Ratio(1, 4), Constraint.Ratio(1, 2), Constraint.Ratio(1, 4) };
       CollectionAssert.AreEqual(expectedRatios, Constraint.FromRatios((1, 4), (1, 2), (1, 4)));
       
       // Test other factory methods...
   }
   ```

3. **Create the layout engine**:
   ```csharp
   public static class Layout
   {
       // Split a rectangle into chunks along specified direction
       public static Rect[] Split(Rect area, Direction direction, Constraint[] constraints)
       {
           // Initialize solver
           var solver = new CassowaryNET.Solver();
           
           // Create variables for each chunk
           var variables = new List<Variable>();
           for (int i = 0; i < constraints.Length; i++)
           {
               variables.Add(new Variable($"chunk_{i}"));
           }
           
           // Add constraints to solver
           // ... implementation
           
           // Solve and extract results
           solver.UpdateVariables();
           
           // Convert results to Rect[]
           // ... implementation
           
           return chunks;
       }
       
       // Split with spacing between chunks
       public static Rect[] SplitWithSpacing(Rect area, Direction direction, 
                                           int spacing, Constraint[] constraints)
       {
           // Similar to Split but with spacing
           // ... implementation
       }
       
       // Create a reusable layout
       public static ILayout Create(Direction direction, Constraint[] constraints)
           => new LayoutImpl(direction, constraints);
   }
   
   public interface ILayout
   {
       Rect[] Split(Rect area);
       Rect[] SplitWithSpacing(Rect area, int spacing);
   }
   
   public class LayoutImpl : ILayout
   {
       private readonly Direction _direction;
       private readonly Constraint[] _constraints;
       
       public LayoutImpl(Direction direction, Constraint[] constraints)
       {
           _direction = direction;
           _constraints = constraints;
       }
       
       public Rect[] Split(Rect area) => Layout.Split(area, _direction, _constraints);
       
       public Rect[] SplitWithSpacing(Rect area, int spacing)
           => Layout.SplitWithSpacing(area, _direction, spacing, _constraints);
   }
   ```

4. **Implement Flex layout**:
   ```csharp
   public enum FlexDirection { Row, RowReverse, Column, ColumnReverse }
   public enum FlexJustify { FlexStart, FlexEnd, Center, SpaceBetween, SpaceAround, SpaceEvenly }
   public enum FlexAlign { FlexStart, FlexEnd, Center, Stretch, Baseline }
   
   public class FlexLayout : ILayout
   {
       private readonly FlexDirection _direction;
       private readonly FlexJustify _justify;
       private readonly FlexAlign _align;
       private readonly Constraint[] _constraints;
       
       public FlexLayout(FlexDirection direction, FlexJustify justify, 
                        FlexAlign align, Constraint[] constraints)
       {
           _direction = direction;
           _justify = justify;
           _align = align;
           _constraints = constraints;
       }
       
       public Rect[] Split(Rect area) => /* implementation */
       
       public Rect[] SplitWithSpacing(Rect area, int spacing) => /* implementation */
   }
   ```

5. **Integrate with Cassowary solver**:
   - Integrate with Cassowary.NET or Kiwi.NET library
   - Create a wrapper to simplify constraint creation
   - Translate high-level constraints to solver constraints
   - Extract results from solver solution

6. **Add caching for performance**:
   ```csharp
   public class CachedLayout : ILayout
   {
       private readonly ILayout _inner;
       private readonly Dictionary<Rect, Rect[]> _cache = new();
       private readonly Dictionary<(Rect, int), Rect[]> _spacingCache = new();
       
       public CachedLayout(ILayout inner) => _inner = inner;
       
       public Rect[] Split(Rect area)
       {
           if (_cache.TryGetValue(area, out var result))
               return result;
               
           var chunks = _inner.Split(area);
           _cache[area] = chunks;
           return chunks;
       }
       
       public Rect[] SplitWithSpacing(Rect area, int spacing)
       {
           var key = (area, spacing);
           if (_spacingCache.TryGetValue(key, out var result))
               return result;
               
           var chunks = _inner.SplitWithSpacing(area, spacing);
           _spacingCache[key] = chunks;
           return chunks;
       }
   }
   ```

## Key Challenges

1. **Cassowary Solver Integration**:
   - Finding or implementing a suitable C# Cassowary solver
   - Mapping high-level constraints to solver constraints
   - Handling solver errors and edge cases

2. **Layout Algorithm Complexity**:
   - Implementing spacing and alignment correctly
   - Handling edge cases (zero size, too many constraints)
   - Optimizing solver performance

3. **Flex Layout Implementation**:
   - Implementing CSS flexbox-like behavior
   - Correctly handling different flex directions
   - Managing alignment and justification

4. **Caching Strategy**:
   - Determining when to cache layout results
   - Handling cache invalidation
   - Memory usage considerations

## Performance Considerations

1. **Minimize Solver Invocations**:
   - Cache layout results when possible
   - Avoid recalculating layouts for identical areas

2. **Optimize Common Cases**:
   - Implement fast paths for simple layouts
   - Use direct calculation for single-item layouts

3. **Memory Efficiency**:
   - Use value types (structs) for small objects
   - Avoid unnecessary allocations
   - Consider object pooling for frequent calculations

4. **Benchmark and Profile**:
   - Measure layout performance in various scenarios
   - Identify and optimize hotspots

## Related Components

- **Buffer System**: Will use Rects for rendering
- **Widget System**: Will use layouts for positioning
- **Terminal Backend**: Will provide the overall terminal size

## Testing Approach

1. **Unit Tests**:
   - Test each constraint type individually
   - Test layout calculations with various constraints
   - Test edge cases (zero size, one item, etc.)

2. **Visual Tests**:
   - Create visual representations of layouts
   - Verify alignment and spacing behavior
   - Test nested layouts

3. **Property-Based Tests**:
   - Generate random constraints and areas
   - Verify layout properties (all space used, no overlaps)
   - Test layout stability

## Acceptance Criteria

1. All core layout types (Rect, Size, Position, Margin) are implemented
2. All constraint types (Length, Percentage, Ratio, Min, Max, Fill) are implemented
3. The Layout static class with Split methods is implemented
4. The ILayout interface and implementations are complete
5. Flex layout with direction, justify, and align options is implemented
6. Layout caching for performance is implemented
7. Layout calculations are correct for all constraint combinations
8. Nested layouts work correctly
9. Performance is acceptable for typical use cases
10. All tests pass

## See Also

- [SPEC-LAYOUT-004](../../specs/SPEC-LAYOUT-004.md): Layout engine specification
- [003-LAYOUT-ENGINE-001](../../features/003-LAYOUT-ENGINE-001.md): Layout engine feature