---
id: SPEC-LAYOUT-004
title: Layout Engine Specification
status: draft
date: 2023-11-28
---

# Layout Engine Specification

## Overview

The Layout Engine in CycoTui provides a flexible, constraint-based system for arranging UI elements in the terminal. Based on analysis of Ratatui's layout implementation, this specification defines how CycoTui will handle UI layouts across different terminal sizes and conditions.

The layout engine focuses on providing a powerful yet intuitive API for defining how UI elements should be sized and positioned, with support for nested layouts, different constraint types, and various alignment options.

## Scope

This specification covers:

1. Rectangle representation and operations
2. Constraint system for defining element sizes
3. Layout algorithm for arranging elements
4. Alignment and positioning utilities
5. Margin and spacing handling
6. Nesting support for complex layouts

## Requirements

## Requirements

### Core Layout Types
Based on the prelude analysis, the layout system must provide these essential types:

- **Rect**: Rectangle representation with position (x, y) and size (width, height)
- **Size**: Size representation for width and height values
- **Position**: Position representation for x and y coordinates
- **Constraint**: Flexible constraint system for defining element sizes
- **Direction**: Layout direction (Horizontal, Vertical)
- **Alignment**: Both horizontal and vertical alignment options
- **Margin**: Margin specification for spacing around elements
- **Padding**: Padding specification for spacing within elements (for widgets like Block)
- **Layout**: Core layout calculation engine

### Position Type Requirements
The Position type must provide:

**Core Properties**:
- `X: ushort` - Horizontal coordinate (0-based, increasing rightward)
- `Y: ushort` - Vertical coordinate (0-based, increasing downward)

**Construction Methods**:
- Constructor accepting x and y coordinates
- Static `Origin` property for position (0, 0)
- Conversion from tuple `(ushort, ushort)`
- Conversion from Rect (extracting top-left corner)

**Conversion Support**:
- Implicit conversion to/from `(ushort, ushort)` tuples
- Conversion from Rect types
- String representation for debugging

**Coordinate System**:
- Terminal coordinate system with origin at top-left
- X-axis increases rightward
- Y-axis increases downward
- 16-bit coordinate range (0-65535)

### Constraint System
The layout engine must implement a comprehensive constraint system:

**Constraint Types** (in order of priority):
1. **Length(u16)**: Fixed size in terminal cells
2. **Percentage(u16)**: Size as percentage of available space
3. **Ratio(u16, u16)**: Size as ratio (numerator/denominator) of available space
4. **Min(u16)**: Minimum size constraint
5. **Max(u16)**: Maximum size constraint
6. **Fill(u16)**: Proportionally fill remaining space

**Constraint Processing**:
- Strength-based priority system for conflict resolution using predefined strength values
- Division-by-zero protection in ratio calculations (uses max(1) for denominator)
- Graceful handling of constraint overflow/underflow scenarios
- Proportional distribution for Fill constraints based on scale factors

**Constraint Creation APIs**:
Based on analysis of `ratatui-macros/src/layout.rs`, CycoTui must provide ergonomic constraint creation APIs that match Ratatui's macro syntax:

```csharp
// Ratatui: constraint!(==50)
var constraint = Constraint.Length(50);
var constraint = 50.Fixed();  // Extension method

// Ratatui: constraint!(>=20)
var constraint = Constraint.Min(20);
var constraint = 20.Min();    // Extension method

// Ratatui: constraint!(<=100)
var constraint = Constraint.Max(100);
var constraint = 100.Max();   // Extension method

// Ratatui: constraint!(==30%)
var constraint = Constraint.Percentage(30);
var constraint = 30.Percent(); // Extension method

// Ratatui: constraint!(*=1)
var constraint = Constraint.Fill(1);
var constraint = 1.Fill();    // Extension method

// Ratatui: constraint!(==1/3)
var constraint = Constraint.Ratio(1, 3);
```

**Constraint Array Creation**:
```csharp
// Ratatui: constraints![==50, *=1, >=20]
var constraints = new Constraint[]
{
    Constraint.Length(50),
    Constraint.Fill(1),
    Constraint.Min(20)
};

// Alternative with extension methods
var constraints = new[]
{
    50.Fixed(),
    1.Fill(),
    20.Min()
};
```

**Layout Creation with Constraints**:
```csharp
// Ratatui: vertical![==1, *=1, >=3]
var layout = Layout.Vertical(
    1.Fixed(),
    1.Fill(),
    3.Min()
);

// Ratatui: horizontal![>=20, *=1, >=20]
var layout = Layout.Horizontal(
    20.Min(),
    1.Fill(),
    20.Min()
);
```

### Layout Algorithm
The layout engine must integrate with a Cassowary constraint solver:

**Core Algorithm**:
- Uses kasuari-compatible constraint solver for layout calculations
- Creates variables for each segment boundary (start/end pairs)
- Converts constraints to solver constraints with appropriate strengths
- Solves constraints and converts floating-point results to integer coordinates

**Implementation Requirements**:
- Floating-point precision management with multiplier (100.0) for accuracy
- Custom rounding function for cross-platform consistency
- Variable ordering constraints to maintain segment relationships
- Area constraints to ensure all segments fit within bounds

### Rectangle Management
Rectangle implementation must provide:

**Rect Structure**:
- Properties: `X`, `Y`, `Width`, `Height`
- Computed properties: `Left`, `Right`, `Top`, `Bottom`, `Area`
- Operations: `Inner(margin)`, intersection, union, contains tests
- Iteration support for enumerating cell positions

**Rectangle Iteration**:
Based on analysis of `ratatui-core/src/layout/rect/iter.rs`, the rectangle system must provide comprehensive iteration capabilities:

**Row Iterator**:
- Purpose: Iterate over horizontal strips (rows) within a rectangle
- Returns: Each row as a `Rect` with height=1 and full rectangle width
- Features: Bidirectional iteration (forward and backward)
- Performance: O(1) per iteration step with accurate size hints

**Column Iterator**:
- Purpose: Iterate over vertical strips (columns) within a rectangle  
- Returns: Each column as a `Rect` with width=1 and full rectangle height
- Features: Bidirectional iteration (forward and backward)
- Performance: O(1) per iteration step with accurate size hints

**Position Iterator**:
- Purpose: Iterate over individual cell positions within a rectangle
- Order: Row-major traversal (left-to-right, top-to-bottom)
- Returns: Each position as a `Position` struct
- Performance: O(1) per iteration step with accurate size hints

**C# Implementation Requirements**:
```csharp
public readonly struct Rect
{
    // ... other properties and methods ...
    
    /// <summary>
    /// Returns an enumerable that iterates over each row in the rectangle.
    /// Each row is returned as a Rect with height=1.
    /// </summary>
    public IEnumerable<Rect> Rows() => new RowIterator(this);
    
    /// <summary>
    /// Returns an enumerable that iterates over each column in the rectangle.
    /// Each column is returned as a Rect with width=1.
    /// </summary>
    public IEnumerable<Rect> Columns() => new ColumnIterator(this);
    
    /// <summary>
    /// Returns an enumerable that iterates over each position in the rectangle.
    /// Positions are returned in row-major order (left-to-right, top-to-bottom).
    /// </summary>
    public IEnumerable<Position> Positions() => new PositionIterator(this);
}

// Iterator implementations should support:
// - IEnumerable<T> for standard C# enumeration
// - Custom bidirectional iteration interfaces where appropriate
// - Accurate count properties for collection optimization
// - Efficient struct-based implementation to avoid allocations
```

**Iterator Design Considerations**:
- Use struct-based iterators to minimize memory allocations
- Provide accurate count estimates for collection pre-sizing
- Support LINQ operations through standard IEnumerable interface
- Consider custom interfaces for bidirectional iteration where beneficial
- Handle edge cases (zero-width/height rectangles) gracefully

### Nesting Support
The layout engine must support nested layouts:

**Nesting Features**:
- Arbitrary depth layout composition
- Independent constraint resolution per level
- Proper margin and spacing propagation
- Consistent coordinate system across nesting levels

### Margin and Spacing
The layout engine must provide comprehensive margin and spacing capabilities:

**Margin Structure**:
Based on analysis of `ratatui-core/src/layout/margin.rs`, the margin system must provide:

```csharp
/// <summary>
/// Represents spacing around rectangular areas.
/// Defines horizontal and vertical spacing applied around a rectangular area.
/// </summary>
public readonly struct Margin : IEquatable<Margin>
{
    /// <summary>
    /// Horizontal spacing applied to both left and right sides (in character cells).
    /// </summary>
    public ushort Horizontal { get; }
    
    /// <summary>
    /// Vertical spacing applied to both top and bottom sides (in character cells).
    /// </summary>
    public ushort Vertical { get; }
    
    /// <summary>
    /// Creates a new margin with specified horizontal and vertical spacing.
    /// </summary>
    /// <param name="horizontal">Horizontal spacing for left and right</param>
    /// <param name="vertical">Vertical spacing for top and bottom</param>
    public Margin(ushort horizontal, ushort vertical)
    {
        Horizontal = horizontal;
        Vertical = vertical;
    }
    
    /// <summary>
    /// Creates a margin with zero spacing.
    /// </summary>
    public static Margin Zero => new Margin(0, 0);
    
    /// <summary>
    /// Creates a uniform margin with the same spacing on all sides.
    /// </summary>
    public static Margin Uniform(ushort spacing) => new Margin(spacing, spacing);
    
    // Equality and string representation
    public bool Equals(Margin other) => Horizontal == other.Horizontal && Vertical == other.Vertical;
    public override bool Equals(object obj) => obj is Margin other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Horizontal, Vertical);
    public override string ToString() => $"{Horizontal}x{Vertical}";
    
    public static bool operator ==(Margin left, Margin right) => left.Equals(right);
    public static bool operator !=(Margin left, Margin right) => !left.Equals(right);
}
```

**Margin Usage**:
- Used with `Layout` for adding space between layout boundaries and contents
- Used with `Rect.Inner()` and `Rect.Outer()` for creating padded areas
- Margin values represent character cells to add on each side
- For horizontal margin: space applied to both left and right
- For vertical margin: space applied to both top and bottom

**Spacing System**:
- `Spacing` enum with `Space(u16)` and `Overlap(u16)` variants
- Conversion from integers (positive → Space, negative → Overlap)
- Integration with flex distribution for segment gaps

**Flex Distribution** (space allocation strategies):
- **Legacy**: Backward-compatible stretching of last segment  
- **Start**: Align segments to container start
- **End**: Align segments to container end
- **Center**: Center segments within container
- **SpaceBetween**: Distribute space between segments only
- **SpaceAround**: Add space around segments (half space at edges)
- **SpaceEvenly**: Distribute space evenly including edges
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    
    // Derived properties
    public int Left => X;
    public int Top => Y;
    public int Right => X + Width;
    public int Bottom => Y + Height;
    public int Area => Width * Height;
    public bool IsEmpty => Width == 0 || Height == 0;
    
    // Constructors
    public Rect(int x, int y, int width, int height) => /* implementation */
    
    // Factory methods
    public static Rect FromSize(int width, int height) => new Rect(0, 0, width, height);
    
    // Geometric operations
    public bool Contains(Rect other) => /* implementation */
    public bool Contains(Position pos) => /* implementation */
    public bool Intersects(Rect other) => /* implementation */
    
    public Rect Intersection(Rect other) => /* implementation */
    public Rect Union(Rect other) => /* implementation */
    
    // Layout operations
    public Rect Inner(Margin margin) => /* implementation */
    public Rect Outer(Margin margin) => /* implementation */
    
    // Positioning operations
    public Rect At(Position pos) => /* implementation */
    public Rect WithPosition(Position pos) => /* implementation */
    public Rect WithSize(Size size) => /* implementation */
    public Rect WithWidth(int width) => /* implementation */
    public Rect WithHeight(int height) => /* implementation */
    
    // Centering
    public Rect CenterIn(Rect other) => /* implementation */
    public Rect CenterHorizontally() => /* implementation */
    public Rect CenterVertically() => /* implementation */
    
    // Iteration
    public IEnumerable<Position> Positions() => /* implementation */
    public IEnumerable<Rect> Rows() => /* implementation */
    public IEnumerable<Rect> Columns() => /* implementation */
    
    // Layout integration
    public Rect[] SplitHorizontally(params Constraint[] constraints) => /* implementation */
    public Rect[] SplitVertically(params Constraint[] constraints) => /* implementation */
    
    // Equality and comparison
    public override bool Equals(object obj) => /* implementation */
    public bool Equals(Rect other) => /* implementation */
    public override int GetHashCode() => /* implementation */
    public override string ToString() => /* implementation */
}

// Helper types
public readonly struct Position
{
    public int X { get; }
    public int Y { get; }
    
    public Position(int x, int y) => (X, Y) = (x, y);
}

public readonly struct Size
{
    public int Width { get; }
    public int Height { get; }
    
    // Constructors and factory methods
    public Size(int width, int height) => (Width, Height) = (Math.Max(0, width), Math.Max(0, height));
    public static Size Zero => new Size(0, 0);
    
    // Conversion operators
    public static implicit operator Size((int width, int height) tuple) => new Size(tuple.width, tuple.height);
    
    // String representation
    public override string ToString() => $"{Width}x{Height}";
    
    // Equality and comparison
    public override bool Equals(object obj) => obj is Size other && Equals(other);
    public bool Equals(Size other) => Width == other.Width && Height == other.Height;
    public override int GetHashCode() => HashCode.Combine(Width, Height);
    
    // Operators
    public static bool operator ==(Size left, Size right) => left.Equals(right);
    public static bool operator !=(Size left, Size right) => !left.Equals(right);
}

public readonly struct Offset
{
    public int X { get; }
    public int Y { get; }
    
    public Offset(int x, int y) => (X, Y) = (x, y);
}
```

### Constraint System

Based on the analysis of `ratatui-core/src/layout/constraint.rs`, the constraint system must provide a comprehensive and flexible approach to element sizing with a well-defined priority system.

#### Constraint Types and Priority

Constraints are applied in the following priority order:
1. **Min** - Minimum size constraints
2. **Max** - Maximum size constraints  
3. **Length** - Fixed size constraints
4. **Percentage** - Proportional size based on percentage
5. **Ratio** - Proportional size based on ratios
6. **Fill** - Proportional fill of remaining space

```csharp
// Base constraint class
public abstract class Constraint
{
    // Factory methods for different constraint types
    public static Constraint Min(ushort min) => new MinConstraint(min);
    public static Constraint Max(ushort max) => new MaxConstraint(max);
    public static Constraint Length(ushort length) => new LengthConstraint(length);
    public static Constraint Percentage(ushort percentage) => new PercentageConstraint(percentage);
    public static Constraint Ratio(uint numerator, uint denominator) => new RatioConstraint(numerator, denominator);
    public static Constraint Fill(ushort factor = 1) => new FillConstraint(factor);
    
    // Collection factory methods
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
    
    // Apply constraint to available space (deprecated but needed for compatibility)
    [Obsolete("This method will be hidden in future versions")]
    public abstract ushort Apply(ushort length);
    
    // Convert from ushort (creates Length constraint)
    public static implicit operator Constraint(ushort length) => Length(length);
}

// Specific constraint implementations
internal sealed class MinConstraint : Constraint
{
    public ushort Value { get; }
    public MinConstraint(ushort value) => Value = value;
    public override ushort Apply(ushort length) => Math.Max(length, Value);
    public override string ToString() => $"Min({Value})";
}

internal sealed class MaxConstraint : Constraint  
{
    public ushort Value { get; }
    public MaxConstraint(ushort value) => Value = value;
    public override ushort Apply(ushort length) => Math.Min(length, Value);
    public override string ToString() => $"Max({Value})";
}

internal sealed class LengthConstraint : Constraint
{
    public ushort Value { get; }
    public LengthConstraint(ushort value) => Value = value;
    public override ushort Apply(ushort length) => Math.Min(length, Value);
    public override string ToString() => $"Length({Value})";
}

internal sealed class PercentageConstraint : Constraint
{
    public ushort Value { get; }
    public PercentageConstraint(ushort value) => Value = value;
    public override ushort Apply(ushort length)
    {
        var percentage = Value / 100.0f;
        var result = percentage * length;
        return (ushort)Math.Min(result, length);
    }
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
        // Avoid division by zero by using 1 when denominator is 0
        // This results in 0/0 -> 0 and x/0 -> x for x != 0
        var percentage = (float)Numerator / Math.Max(1, Denominator);
        var result = percentage * length;
        return (ushort)Math.Min(result, length);
    }
    public override string ToString() => $"Ratio({Numerator}, {Denominator})";
}

internal sealed class FillConstraint : Constraint
{
    public ushort Factor { get; }
    public FillConstraint(ushort factor) => Factor = factor;
    public override ushort Apply(ushort length) => Math.Min(length, Factor);
    public override string ToString() => $"Fill({Factor})";
}
```

#### Constraint Behavior Details

**Min Constraint**: Sets the minimum size for an element. The element will be at least the specified size.

**Max Constraint**: Sets the maximum size for an element. The element will be at most the specified size.

**Length Constraint**: Sets a fixed size for an element. The element will be exactly the specified size (clamped to available space).

**Percentage Constraint**: Sets the size as a percentage of the total available space. Uses floating-point calculation for precision.

**Ratio Constraint**: Sets the size as a ratio of the total available space. Includes division-by-zero protection.

**Fill Constraint**: Sets the size to proportionally fill remaining space after other constraints are applied. Only expands into excess space.

#### Collection Creation Methods

The constraint system provides convenient factory methods for creating collections of constraints:

```csharp
// Create length constraints from array
var constraints = Constraint.FromLengths(10, 20, 10);

// Create centered layout using ratios
var constraints = Constraint.FromRatios((1, 4), (1, 2), (1, 4));
var constraints = Constraint.FromPercentages(25, 50, 25);

// Create layout with minimum sizes
var constraints = Constraint.FromMins(0, 100, 0);

// Create sidebar layout with maximum sizes
var constraints = Constraint.FromMaxes(30, 170);

// Create proportional fill layout
var constraints = Constraint.FromFills(1, 2, 1);
```

### Layout Algorithm

The layout algorithm must handle space distribution according to flex configuration and constraint priorities.

#### Flex Distribution System

Based on analysis of `ratatui-core/src/layout/flex.rs`, the layout engine must provide a comprehensive flex distribution system:

```csharp
/// <summary>
/// Defines how extra space is distributed among layout segments in a container.
/// Controls space distribution when layout constraints are met and excess space is available.
/// </summary>
public enum Flex
{
    /// <summary>
    /// Fills available space, putting excess into the last constraint of lowest priority.
    /// Maintains backward compatibility with ratatui behavior without Flex.
    /// Uses constraint priority system: Min, Max, Length, Percentage, Ratio, Fill.
    /// </summary>
    Legacy,
    
    /// <summary>
    /// Aligns items to the start of the container (default).
    /// Items are packed to the beginning with excess space at the end.
    /// </summary>
    Start,
    
    /// <summary>
    /// Aligns items to the end of the container.
    /// Items are packed to the end with excess space at the beginning.
    /// </summary>
    End,
    
    /// <summary>
    /// Centers items within the container.
    /// Equal space is distributed before and after the item group.
    /// </summary>
    Center,
    
    /// <summary>
    /// Distributes excess space between elements only.
    /// No space before first or after last element.
    /// </summary>
    SpaceBetween,
    
    /// <summary>
    /// Distributes excess space evenly including before first and after last.
    /// Equal spacing between all elements and container edges.
    /// </summary>
    SpaceEvenly,
    
    /// <summary>
    /// Adds excess space around each element.
    /// Edge spacing is half of between-element spacing.
    /// </summary>
    SpaceAround
}
```

#### Flex Distribution Algorithms

Each flex type requires specific mathematical distribution:

**Legacy Flex**: 
- Follow constraint priority system
- Distribute excess to lowest priority constraints last
- Maintain compatibility with existing layouts

**Alignment Flex (Start, End, Center)**:
- Calculate total required space for all constraints
- Distribute remaining space according to alignment
- Simple arithmetic for positioning

**Distribution Flex (SpaceBetween, SpaceEvenly, SpaceAround)**:
- Calculate gaps between/around elements
- Handle edge cases (single element, rounding)
- Ensure even distribution of excess space

```csharp
public static class FlexDistribution
{
    /// <summary>
    /// Distributes space according to flex strategy.
    /// </summary>
    public static int[] DistributeSpace(int totalSpace, int[] constraintSizes, Flex flex)
    {
        return flex switch
        {
            Flex.Legacy => DistributeLegacy(totalSpace, constraintSizes),
            Flex.Start => DistributeStart(totalSpace, constraintSizes),
            Flex.End => DistributeEnd(totalSpace, constraintSizes),
            Flex.Center => DistributeCenter(totalSpace, constraintSizes),
            Flex.SpaceBetween => DistributeSpaceBetween(totalSpace, constraintSizes),
            Flex.SpaceEvenly => DistributeSpaceEvenly(totalSpace, constraintSizes),
            Flex.SpaceAround => DistributeSpaceAround(totalSpace, constraintSizes),
            _ => throw new ArgumentOutOfRangeException(nameof(flex))
        };
    }
    
    private static int[] DistributeLegacy(int totalSpace, int[] constraintSizes) { /* implementation */ }
    private static int[] DistributeStart(int totalSpace, int[] constraintSizes) { /* implementation */ }
    private static int[] DistributeEnd(int totalSpace, int[] constraintSizes) { /* implementation */ }
    private static int[] DistributeCenter(int totalSpace, int[] constraintSizes) { /* implementation */ }
    private static int[] DistributeSpaceBetween(int totalSpace, int[] constraintSizes) { /* implementation */ }
    private static int[] DistributeSpaceEvenly(int totalSpace, int[] constraintSizes) { /* implementation */ }
    private static int[] DistributeSpaceAround(int totalSpace, int[] constraintSizes) { /* implementation */ }
}
```

### Nesting Support

### Alignment and Spacing

## Technical Approach

### Implementation Strategy

The layout engine will be implemented as:

1. **Core Components**:
   - `Rect` struct for rectangle representation
   - `Constraint` enum for size constraints
   - `Layout` class for layout operations
   - `Direction` enum for layout direction
   - `Alignment` enum for element alignment
   - `Margin` struct for spacing

2. **Type Hierarchy**:
   - Value types (structs) for geometric primitives
   - Enums for discrete options
   - Static methods for layout operations

3. **Layout Process**:
   - Define constraints for elements
   - Apply layout to parent rectangle
   - Get resulting child rectangles
   - Apply widgets to rectangles

### Algorithm Details

### Optimization Techniques

## Examples

### Rectangle Operations

```csharp
// Creating rectangles
var rect = new Rect(10, 5, 30, 20);
var empty = Rect.Empty;
var fromSize = Rect.FromSize(40, 25);

// Geometric operations
var intersection = rect.Intersection(new Rect(5, 10, 20, 20));
var union = rect.Union(new Rect(5, 10, 20, 20));
var contains = rect.Contains(new Position(15, 10));

// Layout operations
var inner = rect.Inner(new Margin(1, 1, 1, 1));
var outer = rect.Outer(new Margin(2, 2, 2, 2));

// Positioning
var moved = rect.At(new Position(0, 0));
var centered = rect.CenterIn(new Rect(0, 0, 80, 40));

// Iteration
foreach (var position in rect.Positions())
{
    // Do something at each position
}

foreach (var row in rect.Rows())
{
    // Process each row
}
```

## See Also

- [VISION-CORE-001.md](../vision/VISION-CORE-001.md): Core vision
- [VISION-TECH-002.md](../vision/VISION-TECH-002.md): Technical vision
- [003-LAYOUT-ENGINE-001.md](../features/003-LAYOUT-ENGINE-001.md): Layout engine feature
- [LAYOUT-CONSTRAINTS-001](../tasks/LAYOUT-CONSTRAINTS-001/README.md): Layout implementation task