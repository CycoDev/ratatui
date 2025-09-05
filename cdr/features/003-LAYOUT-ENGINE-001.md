---
id: 003-LAYOUT-ENGINE-001
title: Layout Engine
status: draft
priority: high
date: 2023-11-28
---

# Layout Engine

## Overview

The Layout Engine provides a powerful, constraint-based system for arranging UI elements within the terminal. It allows developers to create flexible, responsive layouts that adapt to terminal size changes while maintaining the desired visual structure.

Based on Ratatui's layout system, CycoTui's layout engine uses the Cassowary constraint solving algorithm to create layouts similar to CSS flexbox, providing a familiar yet powerful model for terminal UI development.

## User Stories

1. As a terminal UI developer, I want to split the screen into regions so that I can organize my content logically.
2. As a developer, I want to create responsive layouts that adapt to terminal size changes so my UI works across different terminal dimensions.
3. As a developer, I want to specify different types of constraints (fixed, percentage, min/max) so I can create the exact layout I need.
4. As a developer, I want to nest layouts to create complex UIs with hierarchy and organization.
5. As a developer, I want to specify alignment and spacing to create visually balanced layouts.
6. As a developer, I want layouts to be efficiently calculated so my UI remains responsive even with complex arrangements.
7. As a developer, I want a flexbox-like layout system so I can use familiar web layout concepts in the terminal.

## Core Requirements

### Coordinate Primitives

The layout system must provide fundamental coordinate and positioning types:

**Position Type**:
- Represents specific points in the terminal coordinate system
- Terminal coordinate system with origin at top-left (0, 0)
- X-axis increases rightward, Y-axis increases downward
- 16-bit coordinate range (0-65535) for maximum terminal compatibility
- Immutable value type with efficient copy semantics
- Conversion support for tuples and other coordinate representations
- Integration with other layout types (Rect, Size)

**Usage Patterns**:
- Cursor positioning within terminal applications
- Anchor points for widget placement
- Coordinate calculations in layout algorithms
- Hit testing and mouse interaction coordinates
- Scrolling and viewport management

### Spatial Operations and Iteration

The layout system must provide comprehensive spatial traversal capabilities:

**Rectangle Iteration**:
- Row-wise iteration: Traverse rectangle as horizontal strips
- Column-wise iteration: Traverse rectangle as vertical strips  
- Position iteration: Traverse all cell positions in row-major order
- Bidirectional support: Iterate forward or backward through rows/columns
- Performance optimization: Efficient iteration with minimal memory allocation

**Usage Patterns**:
- Rendering operations: Draw content row by row or column by column
- Hit testing: Check positions within rectangular areas
- Spatial algorithms: Process areas systematically for effects or calculations
- Content traversal: Navigate through text or UI content spatially

**Developer Interface**:
```csharp
// Iterate over rows (horizontal strips)
foreach (var row in area.Rows())
{
    // Process each row as a Rect with height=1
}

// Iterate over columns (vertical strips)  
foreach (var column in area.Columns())
{
    // Process each column as a Rect with width=1
}

// Iterate over all positions
foreach (var position in area.Positions())
{
    // Process each (x, y) coordinate
}

// Use LINQ operations
var centerRow = area.Rows().Skip(area.Height / 2).First();
var positions = area.Positions().Where(p => IsValid(p)).ToList();
```

### Margin and Spacing

The layout system must provide comprehensive margin and spacing capabilities:

- **Margin Definition**: Simple structure for specifying horizontal and vertical spacing around elements
- **Symmetric Spacing**: Horizontal margin applies equally to left and right, vertical margin to top and bottom
- **Layout Integration**: Margins work seamlessly with Layout calculations and Rect operations
- **Character Cell Units**: All margin values specified in terminal character cells
- **Zero Margin Support**: Default zero margins for elements that don't need spacing

Usage patterns:
- Creating padded content areas within larger regions
- Adding spacing between layout elements
- Creating visual separation in complex layouts
- Consistent spacing throughout the application

### Layout Constraints

The layout engine must provide a comprehensive constraint system with the following types and priority ordering:

**Constraint Types** (in priority order):
1. **Min Constraints**: Set minimum size requirements for layout elements
2. **Max Constraints**: Set maximum size limits for layout elements  
3. **Length Constraints**: Set fixed sizes for layout elements
4. **Percentage Constraints**: Set sizes as percentages of available space
5. **Ratio Constraints**: Set sizes as ratios of available space (supports fractional values like 1/3)
6. **Fill Constraints**: Proportionally fill remaining space after other constraints are applied

**Constraint Features**:
- Support for both individual constraint creation and collection creation methods
- Implicit conversion from integers to Length constraints for convenience
- Robust calculation with overflow protection and division-by-zero handling
- Performance-optimized calculation using appropriate data types

**Collection Creation**: Convenient factory methods for creating constraint arrays:
- `Constraint.FromLengths()` - Create fixed-size layouts
- `Constraint.FromPercentages()` - Create percentage-based layouts  
- `Constraint.FromRatios()` - Create ratio-based layouts
- `Constraint.FromMins()` - Create minimum-size layouts
- `Constraint.FromMaxes()` - Create maximum-size layouts
- `Constraint.FromFills()` - Create proportional-fill layouts

The layout engine must support various constraint types:

1. **Fixed Length**: Exact size in terminal cells
2. **Percentage**: Size as a percentage of available space
3. **Ratio**: Size as a ratio of total available space
4. **Minimum**: Minimum size constraint
5. **Maximum**: Maximum size constraint
6. **Min/Max**: Combined minimum and maximum constraints
7. **Fill**: Use remaining available space

### Constraint Types

The constraint system must provide:

1. A `Constraint` abstraction with factory methods for different constraint types
2. A way to combine and resolve multiple constraints
3. Proper handling of conflicting constraints
4. Default/fallback behavior when constraints cannot be satisfied

### Layout Algorithm

The layout algorithm must:

1. Split areas according to specified constraints
2. Support both horizontal and vertical splitting
3. Distribute space efficiently
4. Handle overflow and underflow situations
5. Provide deterministic layout results

### Nesting Support

The layout engine must support:

1. Nested layouts with arbitrary depth
2. Consistent constraint resolution across nesting levels
3. Proper propagation of size changes through the layout hierarchy
4. Independent constraint systems for each nesting level

### Flex Distribution

The layout engine must provide flex distribution capabilities to control how excess space is distributed among layout segments:

**Flex Distribution Options**:
1. **Legacy**: Fills available space, putting excess into the last constraint of lowest priority (maintains backward compatibility)
2. **Start**: Aligns items to the start of the container (default)
3. **End**: Aligns items to the end of the container
4. **Center**: Centers items within the container
5. **SpaceBetween**: Distributes excess space between elements only
6. **SpaceEvenly**: Distributes excess space evenly including before first and after last
7. **SpaceAround**: Adds excess space around each element (half space at edges)

**Flex Features**:
- Mathematical distribution algorithms for each flex type
- Integration with constraint priority system for Legacy flex
- Proper handling of edge cases (single element, rounding)
- Performance-optimized space distribution calculations

### Positioning

The layout engine must support:

1. Alignment options (left, center, right, top, bottom)
2. Margin and spacing controls
3. Flex-like justification (space-between, space-around, etc.)
4. Relative positioning within parent containers

## Technical Strategy

The layout engine will be implemented using:

1. **Cassowary Constraint Solver**: Using Cassowary.NET or a similar library to provide the constraint solving algorithm
2. **Rect-Based Layout**: Using rectangles (Rect) as the fundamental unit of layout
3. **Factory-Based API**: Providing a simple, fluent API for creating and using layouts
4. **Caching Strategy**: Implementing performance optimizations through layout caching
5. **Flex Layout Model**: Providing a flexbox-like API for more advanced layouts

Key components:

1. **Rect**: A structure representing a rectangular area in the terminal
2. **Constraint**: An abstract class with implementations for different constraint types
3. **Layout**: A static class with methods for splitting areas according to constraints
4. **ILayout**: An interface for reusable layouts
5. **FlexLayout**: A more advanced layout implementation with flexbox-like capabilities

## Dependencies

1. **Cassowary Solver**: A .NET implementation of the Cassowary constraint algorithm
2. **Buffer System**: For rendering within the calculated layout areas
3. **Terminal Backend**: For getting the initial terminal size

## Implementation Tasks

1. **LAYOUT-RECT-001**: Implement rectangle and related geometric types
2. **LAYOUT-RECT-ITERATORS-001**: Implement rectangle iteration capabilities (rows, columns, positions)
3. **LAYOUT-CONSTRAINTS-001**: Implement constraint types and constraint solver integration
4. **LAYOUT-ALGORITHM-001**: Implement the layout calculation algorithm
5. **LAYOUT-FLEX-001**: Implement flex distribution system for space allocation
6. **LAYOUT-FLEX-ALGORITHMS-001**: Implement mathematical distribution algorithms for each flex type
7. **LAYOUT-CACHING-001**: Implement layout caching and performance optimizations
8. **LAYOUT-DEMO-001**: Create demonstration applications for layout capabilities

## Acceptance Criteria

The layout engine will be considered complete when:

1. All constraint types are implemented and working correctly
2. Layouts can be split both horizontally and vertically
3. Nested layouts work correctly at arbitrary depth
4. The API is consistent, intuitive, and well-documented
5. Layout calculations are efficient and optimized
6. Layouts adapt correctly to terminal size changes
7. Flexbox-like layouts with alignment and justification work correctly
8. All specified user stories are satisfied
9. Comprehensive tests verify layout functionality

## See Also

- [SPEC-LAYOUT-004](../specs/SPEC-LAYOUT-004.md): Layout engine specification
- [LAYOUT-CONSTRAINTS-001](../tasks/LAYOUT-CONSTRAINTS-001/README.md): Layout implementation task