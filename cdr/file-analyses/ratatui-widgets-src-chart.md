# File Analysis: ratatui-widgets/src/chart.rs

## Basic Information

- **File Path**: ratatui-widgets/src/chart.rs
- **Component**: Widget (Data Visualization)
- **Analysis Date**: 2023-11-28

## Key Types and Interfaces

### Axis<'a>
- **Purpose**: Represents an X or Y axis for the Chart widget with titles, bounds, labels, and styling
- **Key Properties**: 
  - `title: Option<Line<'a>>` - Optional axis title
  - `bounds: [f64; 2]` - Min/max bounds for data
  - `labels: Vec<Line<'a>>` - Labels for axis ticks
  - `style: Style` - Axis styling
  - `labels_alignment: Alignment` - Label alignment
- **Key Methods**: 
  - `title()` - Sets axis title
  - `bounds()` - Sets min/max bounds
  - `labels()` - Sets axis labels
  - `style()` - Sets axis styling
  - `labels_alignment()` - Sets label alignment
- **Usage Pattern**: Fluent API with builder pattern, consumed by Chart

### GraphType (enum)
- **Purpose**: Determines how datasets are rendered (scatter, line, or bar charts)
- **Values**: 
  - `Scatter` - Draw individual points (default)
  - `Line` - Draw lines connecting points
  - `Bar` - Draw bars from x-axis to points
- **Usage Pattern**: Used with Dataset to control rendering behavior

### LegendPosition (enum)
- **Purpose**: Controls legend placement within chart area
- **Values**: Top, TopRight (default), TopLeft, Left, Right, Bottom, BottomRight, BottomLeft
- **Key Methods**: `layout()` - Calculates legend rectangle based on position and constraints

### Dataset<'a>
- **Purpose**: Represents a data series with associated styling and rendering properties
- **Key Properties**:
  - `name: Option<Line<'a>>` - Dataset name for legend
  - `data: &'a [(f64, f64)]` - Reference to coordinate data
  - `marker: symbols::Marker` - Symbol for rendering points
  - `graph_type: GraphType` - How to render the dataset
  - `style: Style` - Dataset styling
- **Key Methods**:
  - `name()` - Sets dataset name
  - `data()` - Sets coordinate data
  - `marker()` - Sets rendering symbol
  - `graph_type()` - Sets rendering type
  - `style()` - Sets styling
- **Usage Pattern**: Fluent API, multiple datasets can be passed to Chart

### Chart<'a>
- **Purpose**: Main widget for plotting datasets in cartesian coordinate system
- **Key Properties**:
  - `block: Option<Block<'a>>` - Optional surrounding block
  - `x_axis: Axis<'a>` - X-axis configuration
  - `y_axis: Axis<'a>` - Y-axis configuration
  - `datasets: Vec<Dataset<'a>>` - Data series to plot
  - `style: Style` - Chart base style
  - `hidden_legend_constraints: (Constraint, Constraint)` - When to hide legend
  - `legend_position: Option<LegendPosition>` - Legend placement
- **Key Methods**:
  - `new()` - Creates chart with datasets
  - `block()` - Sets surrounding block
  - `x_axis()` / `y_axis()` - Sets axis configuration
  - `legend_position()` - Controls legend
  - `hidden_legend_constraints()` - Sets legend visibility rules

### ChartLayout (internal)
- **Purpose**: Internal layout container for chart components
- **Properties**: Positions for titles, labels, axes, legend area, and graph area

## Core Behaviors

### Chart Layout Calculation
- **Description**: Complex layout algorithm that calculates positions for all chart elements
- **Implementation Approach**: 
  - Starts from bottom-left, works up and right
  - Calculates space for labels, axes, titles
  - Handles legend positioning with constraints
  - Provides final graph area for data rendering
- **Performance Considerations**: Layout is calculated once per render
- **Edge Cases**: Handles very small areas by hiding elements; ensures minimum graph area

### Axis Label Rendering
- **Description**: Renders axis labels with proper alignment and spacing
- **Implementation Approach**:
  - X-axis: First label alignment depends on labels_alignment, middle labels centered, last label right-aligned
  - Y-axis: Labels distributed evenly along axis height
  - Handles label overflow and clipping
- **Edge Cases**: Minimum 2 labels required; >3 labels have positioning issues (noted bug #334)

### Dataset Rendering via Canvas
- **Description**: Uses Canvas widget to render different graph types
- **Implementation Approach**:
  - Scatter: Renders individual points
  - Line: Connects consecutive points with lines
  - Bar: Draws vertical lines from x-axis to each point
  - Uses Canvas coordinate transformation
- **Performance Considerations**: Leverages Canvas widget's efficient rendering

### Legend Management
- **Description**: Automatically shows/hides legend based on space constraints
- **Implementation Approach**:
  - Calculates legend size based on dataset names
  - Applies hidden_legend_constraints to determine visibility
  - Positions legend according to LegendPosition
  - Only shows datasets with names
- **Edge Cases**: No legend if no named datasets; handles overlapping with titles

## Platform-Specific Code

- **None**: This widget is platform-agnostic, relies on underlying buffer and canvas systems

## Dependencies

### Internal Dependencies
- `ratatui_core::buffer::Buffer` - Rendering target
- `ratatui_core::layout::{Alignment, Constraint, Flex, Layout, Position, Rect}` - Layout system
- `ratatui_core::style::{Color, Style, Styled}` - Styling system
- `ratatui_core::symbols` - Drawing symbols
- `ratatui_core::text::Line` - Text rendering
- `ratatui_core::widgets::Widget` - Widget trait
- `crate::block::{Block, BlockExt}` - Container widget
- `crate::canvas::{Canvas, Line as CanvasLine, Points}` - Drawing primitives

### External Dependencies
- `alloc::vec::Vec` - Collections
- `core::cmp::max` - Utilities
- `strum::{Display, EnumString}` - Enum utilities

## Key Algorithms and Techniques

### Layout Algorithm
- **Purpose**: Efficiently calculates chart component positions
- **Approach**: Bottom-up, left-to-right space allocation
- **Complexity**: O(1) - fixed number of calculations
- **Optimizations**: Early returns for insufficient space

### Legend Constraint Evaluation
- **Purpose**: Determines legend visibility based on available space
- **Approach**: Uses Layout system to calculate maximum allowed space
- **Complexity**: O(n) where n is number of named datasets

### Axis Label Distribution
- **Purpose**: Evenly distributes labels along axis
- **Approach**: Mathematical division with special handling for first/last labels
- **Optimizations**: Avoids rendering labels outside visible area

## C# Port Considerations

### Idiomatic Translations
- **Fluent Builder Pattern**: Maps well to C# - use method chaining with return types
- **Enums with Display/Parse**: Use `[Display]` attributes and `ToString()` / `Parse()` methods
- **Lifetime parameters `<'a>`**: Use interfaces or concrete types, no lifetime management needed
- **`#[must_use]`**: Use `[MustUseReturnValue]` analyzer attribute or documentation
- **Const methods**: Use readonly properties or immutable builders

### Potential Challenges
- **Reference to data `&'a [(f64, f64)]`**: Use `IReadOnlyList<(double, double)>` or `ReadOnlySpan<(double, double)>`
- **Optional types**: Use nullable reference types and `Option<T>` pattern or nullable types
- **Pattern matching on enums**: Use switch expressions in C# 8+
- **Vector operations**: Use `List<T>` or `IList<T>` with LINQ for transformations

### .NET API Equivalents
- **`alloc::vec::Vec`** → `List<T>` or `IList<T>`
- **`core::cmp::max`** → `Math.Max()`
- **Strum Display/EnumString** → Custom attributes + reflection or source generators
- **Slice operations `windows(2)`** → LINQ `Zip()` with skip, or custom extension methods

## Documentation Updates Needed

### Features
- **006-DATA-VISUALIZATION-001.md**: Add comprehensive chart widget capabilities
- **002-WIDGET-SYSTEM-001.md**: Update with chart as complex widget example
- **005-STYLE-SYSTEM-001.md**: Include chart-specific styling patterns

### Specifications
- **SPEC-WIDGET-003.md**: Add Chart as example of complex stateless widget
- **SPEC-LAYOUT-004.md**: Include chart's complex layout algorithm as case study
- **SPEC-CANVAS-001.md**: Reference chart's usage of canvas for data rendering

### Tasks
- **WIDGET-CHART-001**: Create new task for implementing Chart widget
- **WIDGET-AXIS-001**: Create task for Axis implementation
- **WIDGET-DATASET-001**: Create task for Dataset implementation
- **LAYOUT-COMPLEX-001**: Create task for complex layout algorithms

## Questions and Issues

### Chart Layout Complexity
- **Context**: The layout algorithm is quite complex with many edge cases
- **Potential Solutions**: 
  - Break into smaller, testable functions
  - Create separate layout calculator class
  - Use composition over complex single method

### Label Positioning Bug
- **Context**: Known issue #334 with >3 labels positioning
- **Potential Solutions**: 
  - Research fix in latest Ratatui version
  - Implement improved label distribution algorithm
  - Add validation for label count

### Performance with Large Datasets
- **Context**: No apparent optimization for large datasets
- **Potential Solutions**:
  - Add data point culling for points outside bounds
  - Implement level-of-detail rendering
  - Consider virtualization for very large datasets