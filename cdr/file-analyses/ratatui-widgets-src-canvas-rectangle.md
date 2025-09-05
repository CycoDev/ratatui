# Source File Analysis: ratatui-widgets/src/canvas/rectangle.rs

## Basic Information

- **File Path**: `ratatui-widgets/src/canvas/rectangle.rs`
- **Component**: Widget Component (Canvas subcomponent)
- **Analysis Date**: 2023-11-28

## Key Types and Interfaces

### Rectangle struct
- **Purpose**: Represents a drawable rectangle shape for canvas widgets
- **Key Properties**:
  - `x: f64` - X position (bottom-left corner)
  - `y: f64` - Y position (bottom-left corner)  
  - `width: f64` - Rectangle width
  - `height: f64` - Rectangle height
  - `color: Color` - Rectangle border color
- **Key Methods**:
  - `new(x, y, width, height, color)` - Constructor (const fn)
- **Usage Pattern**: Used within Canvas paint functions to draw rectangular outlines

### Shape trait implementation
- **Purpose**: Allows Rectangle to be drawn on a Canvas
- **Key Methods**:
  - `draw(&self, painter: &mut Painter)` - Renders rectangle as 4 lines
- **Usage Pattern**: Called by Canvas during rendering process

## Core Behaviors

### Rectangle Drawing Algorithm
- **Description**: Renders rectangle by decomposing it into 4 line segments
- **Implementation Approach**: 
  - Creates array of 4 Line objects representing the rectangle's edges
  - Each line uses the same color as the rectangle
  - Lines form: left edge, top edge, right edge, bottom edge
- **Performance Considerations**: Efficient conversion to basic drawing primitives (lines)
- **Edge Cases**: Zero-width or zero-height rectangles will still draw correctly

### Coordinate System
- **Description**: Uses mathematical coordinate system (not terminal cell coordinates)
- **Implementation Approach**: 
  - Position specified from bottom-left corner
  - Floating-point coordinates for sub-cell precision
  - Relies on Canvas coordinate transformation
- **Special Handling**: Different from typical terminal UI coordinate systems

## Platform-Specific Code

- **None**: Rectangle implementation is platform-agnostic
- **Dependencies**: Relies on Canvas and underlying backend for actual rendering

## Dependencies

### Internal Dependencies
- `ratatui_core::style::Color` - Color type for styling
- `crate::canvas::{Line, Painter, Shape}` - Canvas drawing infrastructure

### External Dependencies
- **None**: No external crate dependencies

## Key Algorithms and Techniques

### Rectangle-to-Lines Decomposition
- **Purpose**: Convert rectangle to basic drawing primitives
- **Approach**: 
  - Calculate four corner coordinates
  - Create line segments connecting corners in order
  - Each line inherits rectangle's color
- **Complexity**: O(1) - constant time conversion
- **Optimizations**: Direct array initialization, no allocations

## C# Port Considerations

### Idiomatic Translations
- `struct Rectangle` → `public struct Rectangle` or `public class Rectangle`
- `const fn new()` → `public Rectangle(double x, double y, double width, double height, Color color)`
- `f64` → `double`
- `Color` → Custom Color type or System.Drawing.Color equivalent
- Array literals → Array initialization or span usage

### Potential Challenges
- **Const constructor**: C# doesn't have exact equivalent to Rust's `const fn`
- **Pattern matching on arrays**: May need different approach for line array handling
- **Trait implementation**: Will need interface implementation (IShape)

### .NET API Equivalents
- `f64` → `double`
- Array handling → `ReadOnlySpan<T>` or standard arrays
- Pattern matching → switch expressions or traditional conditionals

## Documentation Updates Needed

### Features
- **Canvas System Feature**: Add rectangle drawing capabilities
- **Shape System Feature**: Document rectangle as basic drawing primitive

### Specifications
- **SPEC-CANVAS-001**: Add rectangle drawing specification
- **SPEC-WIDGET-003**: Update widget specifications for canvas shapes

### Tasks
- **WIDGET-CANVAS-RECTANGLE-001**: Implement rectangle shape for canvas
- **WIDGET-CANVAS-SHAPES-001**: Implement basic canvas shapes (if not exists)

## Questions and Issues

### Performance Considerations
- **Context**: Rectangle creates 4 Line objects for each draw call
- **Potential Solutions**: Could optimize by drawing rectangle directly, but current approach maintains consistency with Shape trait design

### API Design Question
- **Context**: Rectangle positioned from bottom-left corner (mathematical convention)
- **Potential Solutions**: Consider if this is intuitive for C# developers or if top-left positioning would be more familiar

### Color Inheritance
- **Context**: All four lines inherit the same color from rectangle
- **Potential Solutions**: Could extend to support different colors per edge in future, but current design is simpler and covers most use cases