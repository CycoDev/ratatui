---
id: SPEC-WIDGET-003
title: Widget Implementation Specification
status: draft
date: 2023-11-28
---

# Widget Implementation Specification

## Overview

This specification defines the technical implementation requirements for the CycoTui widget system. It covers the interface definitions, rendering protocols, state management patterns, and implementation guidelines for creating widgets that integrate seamlessly with the framework.

## Scope

This specification covers:
- Core widget interface definitions (`IWidget`, `IStatefulWidget<TState>`)
- Widget rendering protocols and contracts
- State management patterns for stateful widgets
- Widget composition and hierarchy patterns
- Widget configuration patterns using enums and options
- Namespace organization and API design
- Documentation and testing requirements
- Utility components and helper functions for widget implementation

This specification does not cover:
- Specific widget implementations (covered in separate specifications)
- Backend-specific rendering details
- Layout algorithm implementation

## Requirements

### Core Widget Interface

#### IWidget Interface
- **Render Method**: `void Render(Rect area, Buffer buffer)`
- **Immutable Rendering**: Widgets should not modify their own state during rendering
- **Area Constraints**: Must handle zero-width/height areas gracefully
- **Buffer Safety**: Must not write outside the provided area bounds

#### IStatefulWidget<TState> Interface
- **Stateful Render**: `void Render(Rect area, Buffer buffer, TState state)`
- **State Management**: State object passed by reference, can be modified during rendering
- **State Validation**: Must handle null or invalid state gracefully

### Widget Configuration Patterns

#### Fluent Builder Pattern
- **Method Chaining**: All configuration methods return the widget instance
- **Immutable Updates**: Each method returns a new instance (value types) or modifies and returns self (reference types)
- **Must Use Attributes**: Configuration methods should be marked with appropriate attributes to prevent unused results
- **Generic Acceptance**: Methods should accept appropriate interfaces (e.g., `IConvertible` for styles)

#### Generic Construction
- **Flexible Input**: Constructors should accept `IEnumerable<T>` where items can be converted to required types
- **Type Conversion**: Use implicit operators or extension methods for common conversions
- **Collection Support**: Support arrays, lists, and LINQ expressions seamlessly

### Table Row Widget Requirements

Based on table row widget analysis:

#### Core Row Components
- **Cell Collection**: Vector of cells that make up the row content
- **Height Management**: Fixed height with content truncation when necessary
- **Margin Support**: Top and bottom margins for vertical spacing
- **Style Inheritance**: Row-level styling that combines with cell-level styles

#### Row Configuration Patterns
- **Fluent Builder API**: Method chaining for ergonomic configuration
- **Generic Construction**: Accept any iterator of cell-convertible items
- **Type Safety**: Use of `#[must_use]` equivalent for fluent methods
- **Style Composition**: Hierarchical styling with cell styles overriding row styles

#### Height and Layout Management
- **Fixed Height Model**: Row height independent of content height
- **Content Truncation**: Cell content truncated when exceeding row height
- **Margin Calculation**: Total height includes top/bottom margins

### Tabs Widget Requirements

Based on tabs widget analysis:

#### Core Tab Components
- **Title Collection**: List of tab titles as styled text lines
- **Selection Management**: Optional selected tab index with bounds checking
- **Divider Customization**: Configurable separator between tabs
- **Padding Control**: Independent left and right padding for each tab
- **Block Integration**: Optional block wrapper for borders and titles

#### Tab Configuration Patterns
- **Fluent API**: Method chaining for all configuration options
- **Generic Title Acceptance**: Accept any collection convertible to text lines
- **Style Hierarchy**: Overall style, highlight style for selected tab, and individual title styles
- **Unicode Awareness**: Proper width calculation for Unicode characters including CJK

#### Selection and Interaction
- **Optional Selection**: Support for no selected tab (None/null)
- **Bounds Checking**: Automatic validation of selection index against available tabs
- **Out-of-Bounds Handling**: Invalid selections result in no tab being highlighted
- **Dynamic Updates**: Selection and titles can be updated independently

#### Rendering Behavior
- **Width-Aware Layout**: Graceful handling of insufficient horizontal space
- **Early Termination**: Stop rendering when available width is exhausted
- **Overflow Handling**: Tabs that don't fit are not rendered rather than being clipped
- **Unicode Width**: Accurate width calculations for tab layout using proper Unicode width libraries

#### Width Calculation Support
- **Total Width Method**: Calculate total rendered width without actual rendering
- **CJK Support**: Separate width calculation method for CJK character handling
- **Component Breakdown**: Width calculation includes titles, dividers, and padding
- **Performance**: Width calculation should be efficient for layout planning
- **Overflow Handling**: Graceful handling of content that exceeds allocated space

#### Iterator Integration
- **FromIterator Implementation**: Seamless collection from various data sources
- **Generic Input Support**: Accept strings, cells, or any convertible type
- **Efficient Conversion**: Single-pass iteration with type conversion

### Scrollbar Widget Requirements

Based on scrollbar widget analysis:

#### Core Scrollbar Components
- **Orientation Support**: Vertical (left/right) and horizontal (top/bottom) positioning
- **Symbol Customization**: Configurable symbols for thumb, track, begin/end elements
- **Style Customization**: Independent styling for each scrollbar component
- **State Management**: Content length, position, and viewport tracking

#### Scrollbar Rendering Algorithm
- **Proportional Positioning**: Calculate thumb position based on content ratio
- **Minimum Thumb Size**: Ensure thumb is always at least 1 cell visible
- **Symbol Width Handling**: Account for multi-character symbols in calculations
- **Area Optimization**: Extract appropriate column/row based on orientation

#### State Management Patterns
- **ScrollbarState**: Separate state object with fluent configuration
- **Position Clamping**: Automatic bounds checking for scroll positions
- **Navigation Methods**: Built-in prev/next/first/last/scroll operations
- **Content Length Validation**: Handle zero-length content gracefully

### Data Visualization Widget Requirements

Based on sparkline widget analysis:

#### Sparkline Widget Components  
- **Data Flexibility**: Support multiple input formats (ulong, ulong?, SparklineBar objects)
- **Absent Value Handling**: Distinguished missing data from zero values with custom styling
- **Direction Control**: Left-to-right and right-to-left rendering orientation
- **Symbol Precision**: Unicode bar symbols with 8 sub-levels per character cell

#### Sparkline Rendering Algorithm
- **Height Scaling**: Proportional scaling using `value * height * 8 / maxHeight` formula  
- **Symbol Mapping**: Map scaled heights (0-8) to appropriate Unicode bar symbols
- **Direction Calculation**: Position calculation based on rendering direction enum
- **Multi-Height Support**: Render across multiple rows with proper height distribution

#### Data Processing Patterns
- **Generic Input**: Accept `IEnumerable<T>` with conversion to internal SparklineBar format
- **Value Conversion**: Implicit operators for seamless data format conversion
- **Style Composition**: Combine widget-level and individual bar styling
- **Edge Case Handling**: Zero maximum values, empty datasets, oversized data

#### Unicode Width Requirements
- **Symbol Width Calculation**: Accurate character width for proper positioning
- **Multi-byte Character Support**: Handle Unicode symbols correctly
- **Fallback Mechanisms**: Graceful degradation for unsupported characters

### Half-Block Character Rendering
- Support for high-resolution graphics using Unicode half-block characters
- Efficient algorithm for converting 2x1 character pairs to single terminal cells
- Color mapping system for foreground/background assignment
- Pattern matching for character-to-symbol conversion (▀, ▄, █, space)
- Graceful handling of unmapped characters and edge cases

### Text Display and Formatting (Paragraph Widget)

Based on paragraph widget analysis:

#### Text Rendering Requirements
- **Multi-format Text Input**: Accept various input types (`string`, `Text`, `Line`, `Span`) with automatic conversion
- **Unicode Grapheme Support**: Proper handling of Unicode graphemes including wide characters and combining marks  
- **Styled Text Rendering**: Support for per-span styling with style composition and inheritance
- **Buffer Integration**: Direct rendering to cell buffer with efficient style application

#### Text Wrapping System
- **Word Wrapping**: Implement intelligent word-boundary wrapping with configurable behavior
- **Whitespace Handling**: Support for trimming vs. preserving leading whitespace during wrapping
- **Line Composition**: Abstract line composition through strategy pattern (`ILineComposer`)
- **Lazy Evaluation**: Efficient line generation without pre-computing entire document

#### Text Alignment
- **Multi-Alignment Support**: Left, Center, and Right alignment for both individual lines and paragraph-wide
- **Per-Line Override**: Allow individual lines to override paragraph alignment settings
- **Width Calculations**: Accurate text width measurement for proper alignment positioning

#### Scrolling Capabilities  
- **Bidirectional Scrolling**: Support both vertical (line-based) and horizontal (character-based) scrolling
- **Efficient Navigation**: Jump directly to scroll positions without processing intermediate content
- **Bounds Handling**: Graceful handling of scroll positions beyond content boundaries

#### Container Integration
- **Block Composition**: Seamless integration with Block widget for borders and titles
- **Style Inheritance**: Proper style composition between container, widget, and content styles
- **Space Calculation**: Account for block borders/padding in text area calculations

#### Performance Features
- **Line Count Prediction**: Calculate required lines for given width (unstable feature)
- **Width Calculation**: Determine minimum width needed to avoid wrapping (unstable feature)  
- **Incremental Rendering**: Avoid unnecessary work through smart skipping and caching

### Stateful Widget Implementation Pattern

Based on List widget rendering analysis:

- Implement both `IWidget` and `IStatefulWidget<TState>` for maximum flexibility
- Stateless rendering should create default state internally and delegate to stateful implementation
- Support both value-type and reference-type widget rendering through separate implementations
- Use delegate pattern: `Widget(T) -> StatefulWidget(T, DefaultState)`

#### List Widget Rendering Requirements
- **Viewport Calculation**: Implement efficient algorithm to determine visible items within area bounds
- **Scroll Padding**: Support padding around selected items to keep them away from viewport edges
- **Selection Highlighting**: Render highlight symbols and apply highlight styles conditionally
- **Direction Support**: Support both top-to-bottom and bottom-to-top rendering directions
- **Variable Item Heights**: Handle items with different heights in viewport calculations
- **State Management**: Maintain selection index, offset, and other state between renders

#### Rendering Performance Considerations
- Viewport calculation should be O(n) where n is items checked for visibility
- Early termination when height limits are reached
- Minimize allocations during frequent re-renders
- Efficient scroll padding algorithm that reduces padding when items don't fit

### Component Widget Pattern
- Support for widget components that wrap content with styling
- Examples: ListItem, TableCell, Button (wraps text with behavior)
- Pattern: Simple data structure with content field and style field
- Fluent API for configuration with method chaining
- Implicit conversion operators for convenient creation from basic types

## Requirements

### Widget Interface
- Define `IWidget` interface with `Render(Rect area, Buffer buffer)` method
- Support both value-type and reference-type widgets
- Enable polymorphic widget collections through common interface
- Support C#-idiomatic method naming conventions (PascalCase)

### Widget Delegation Pattern
- Support rendering for both owned and borrowed widget instances
- Implement delegation pattern where owned widgets delegate to borrowed implementation
- Pattern: `public void Render(Rect area, Buffer buffer) => this.Render(area, buffer);`
- Enables code reuse and consistent behavior across widget instances

### Zero-Sized Widgets
- Support utility widgets with no instance data (e.g., Clear widget)
- Implement as static classes or singleton patterns in C#
- Maintain same API surface as regular widgets
- Examples: Clear widget for area reset functionality

### Widget Interface Definition

#### Core Widget Interface
- **IWidget Interface**: Base interface for all stateless widgets
  - `void Render(Rect area, Buffer buffer)` method for rendering
  - Support for generic widget implementations
  - Immutable widget instances that can be reused

#### Stateful Widget Interface  
- **IStatefulWidget<TState> Interface**: Interface for widgets with mutable state
  - `void Render(Rect area, Buffer buffer, TState state)` method
  - State parameter passed by reference for mutation
  - State lifecycle management patterns

#### Widget Reference Patterns
- **IWidgetRef Interface**: For zero-copy widget rendering
  - `void RenderRef(Rect area, Buffer buffer)` for reference-based rendering
  - Memory-efficient patterns for large widget hierarchies

#### Complex Widget Examples
- **Chart Widget**: Demonstrates advanced composition patterns
  - Multiple sub-components (Axis, Dataset, Legend)
  - Complex layout calculation with constraint-based element visibility
  - Integration with Canvas system for data rendering
  - Fluent builder API with method chaining
  - Reference-based data storage for performance

### Widget Configuration Patterns

Widget configuration should follow consistent patterns across the library:

#### Enum-Based Configuration
- Use enums for configuration options with clear semantic meaning
- Provide descriptive documentation for each option explaining behavior implications
- Include default values using appropriate C# patterns (default attribute or explicit initialization)
- Support string parsing for configuration serialization when needed
- Consider performance implications of configuration decisions

**Example: Table Highlight Spacing**
```csharp
public enum HighlightSpacing
{
    /// <summary>
    /// Always allocate space for selection symbol column.
    /// Table layout remains constant regardless of selection state.
    /// Recommended for consistent UI layout.
    /// </summary>
    Always,
    
    /// <summary>
    /// Only allocate space when a row is selected.
    /// Table layout shifts when selection changes.
    /// May cause jarring UI movement but saves space.
    /// </summary>
    WhenSelected, // Default in Ratatui, consider Always for C#
    
    /// <summary>
    /// Never allocate space for selection symbol.
    /// Highlight symbol will not be displayed.
    /// Use when highlight symbols are not desired.
    /// </summary>
    Never
}
```

#### Configuration Implementation Requirements
- Use efficient decision methods like `bool ShouldAdd(bool hasSelection)` for runtime logic
- Implement `ToString()` override for string serialization support
- Provide static `Parse(string)` method or use `Enum.Parse<T>()` for configuration loading
- Use `const` methods where possible for compile-time optimization
- Default to options that provide the most stable and predictable UI behavior
- Document layout implications clearly for options that cause UI shifting
- Consider backward compatibility when choosing defaults

### Fluent API Requirements
- **Builder Pattern Support**: Widgets should support fluent configuration
  - `#[MustUseReturnValue]` equivalent for method chaining
  - Immutable updates that return new widget instances
  - Validation of required properties during build
- **Style Integration**: Seamless integration with styling system
  - Implementation of `IStyled` interface for all styled widgets
  - Cascading style inheritance patterns
  - Style composition and override capabilities

### Border System Requirements

#### Border Configuration
- **Borders Flags Enum**: Bitflag enum for specifying visible borders
  - `Top`, `Right`, `Bottom`, `Left` individual flags
  - `All` and `None` convenience values
  - Support for flag combination using bitwise operations
  - Memory-efficient byte-based storage

#### Border Types
- **BorderType Enum**: Enumeration of available border styles
  - `Plain` (default) - single-line border using basic box-drawing characters
  - `Rounded` - border with rounded corners
  - `Double` - double-line border
  - `Thick` - thick single-line border
  - Multiple dashed variants (Light/Heavy with Double/Triple/Quadruple patterns)
  - `QuadrantInside`/`QuadrantOutside` - half-block character borders

#### Border Symbol Integration
- **Symbol Set Mapping**: Static mapping from BorderType to symbol collections
  - Compile-time or static readonly mapping to symbol sets
  - Integration with core symbol system
  - Unicode character support for all border styles

### Widget Composition

#### Simple Widget Pattern
- **Text-Based Widgets**: Widgets that delegate to Text widget for rendering
  - Single-responsibility widgets with minimal configuration
  - Static content widgets (logo, branding, simple displays)
  - Delegation pattern: convert widget data to Text and delegate rendering
  - Examples: Logo widget, status displays, simple branding elements

#### Widget Hierarchy
- **Composable Widgets**: Support for widgets containing other widgets
  - Clear parent-child relationships
  - Recursive rendering patterns
  - Area subdivision for child widgets

#### Container Patterns
- **Layout Integration**: Widgets must work with layout system
  - Accept Rect area for rendering bounds
  - Respect area constraints and clipping
  - Support for margin and padding calculations

### Special-Purpose Widget Requirements

#### Calendar Widget (Feature-Flagged)
- **Monthly<TDateStyler> Widget**: Display monthly calendar view with customizable date styling
  - Generic over `TDateStyler` where `TDateStyler : IDateStyler` for flexible date styling
  - Constructor: `Monthly(DateOnly displayDate, TDateStyler events)` 
  - Properties: display month, optional surrounding days, headers, default styling, optional Block container
  - Builder methods: `ShowSurrounding(Style)`, `ShowWeekdaysHeader(Style)`, `ShowMonthHeader(Style)`, `DefaultStyle(Style)`, `Block(Block)`
  - Automatic calendar layout generation starting from appropriate Sunday
  - Hierarchical style composition: default → surrounding → event-specific styling

- **IDateStyler Interface**: Strategy pattern for customizing calendar date appearance
  - Method: `Style GetStyle(DateOnly date)` - return styling for specific date
  - Enables flexible event highlighting and date-specific theming
  - Design for both simple HashMap-based and complex rule-based styling

- **CalendarEventStore**: Built-in HashMap-based `IDateStyler` implementation
  - Internal storage: `Dictionary<DateOnly, Style>` for date-to-style mappings
  - Builder methods: `Add(DateOnly date, Style style)`, `Today(Style style)` (using system clock)
  - Lookup method with default style fallback for unmapped dates
  - Optimized for common use cases with simple date highlighting

- **Layout Strategy**: Three-section vertical layout using constraint system
  - Optional month/year header (1 line when enabled)
  - Optional weekday abbreviations header (1 line when enabled)  
  - Calendar grid (fills remaining space, 6 weeks maximum)
  - Efficient date calculation using `TimeSpan` arithmetic for first-Sunday detection

- **Rendering Features**:
  - Unicode-aware date formatting with proper column alignment
  - Graceful handling of insufficient buffer space (no crashes)
  - Surrounding month days with optional display and distinct styling
  - Style patching for combining default, surrounding, and event styles
  - Background-only styling for empty surrounding day cells

- **C# Implementation Considerations**:
  - Use `DateOnly` (recommended) or `DateTime` for date handling instead of Rust `time::Date`
  - Replace Rust `Duration` with `TimeSpan` for date arithmetic
  - Generic constraint `where TDateStyler : IDateStyler` for compile-time type safety
  - Builder pattern with fluent API using immutable record/struct pattern
  - Extension methods for convenience: `CalendarEventStore.WithToday(Style)`, etc.
  - Feature flag using conditional compilation (`#if FEATURE_CALENDAR`)

### Widget Interface
- **IWidget Interface**: Define core interface for stateless widgets inspired by Ratatui's Widget trait
  - `void Render(Rect area, Buffer buffer)` method for rendering
  - No state management responsibilities
  - Immutable during rendering
  - Support for both value types (structs) and reference types (classes)

- **IWidgetRef Interface (Experimental)**: Define interface for reference-based widget rendering
  - `void RenderRef(Rect area, Buffer buffer)` method for reusable widgets
  - Enables widgets to be stored and rendered multiple times
  - Supports heterogeneous widget collections via interface collections
  - Object-safe design for trait objects equivalent (List<IWidgetRef>)
  - Blanket implementations for common types (strings, optional widgets)
  - Currently experimental in Ratatui (feature flag: "widget-ref")
  - Gated behind experimental feature or separate assembly in C#

- **String Widget Support**: Built-in widget rendering for string types
  - Extension methods or implicit conversions for string types
  - Automatic text truncation when content exceeds area width
  - Default styling applied to string content

### Value Types and Supporting Structures

- **Padding Value Type**: Define padding structure for widget spacing
  - Four properties: Left, Right, Top, Bottom (all ushort or int)
  - Constructor methods for common patterns (Uniform, Horizontal, Vertical, Proportional)
  - Proportional constructor accounts for terminal character aspect ratio (2:1 horizontal:vertical)
  - Const/static factory methods for compile-time evaluation
  - Implements IEquatable<Padding>, IComparable<Padding>
  - Optional JSON serialization support via System.Text.Json attributes
  - Similar to WPF Thickness or WinForms Padding but optimized for terminal use

- **Optional Widget Pattern**: Support for nullable/optional widgets
  - `OptionalWidget<T>` wrapper or native nullable pattern
  - Graceful handling of null widgets (render nothing)
  - Zero-cost abstraction for conditional rendering

- **IStatefulWidget<TState> Interface**: Define interface for stateful widgets
  - `void Render(Rect area, Buffer buffer, ref TState state)` method
  - Generic state type parameter for type safety
  - State passed by reference for efficient updates

- **IStatefulWidgetRef<TState> Interface (Experimental)**: Reference-based stateful widgets
  - `void RenderRef(Rect area, Buffer buffer, ref TState state)` method
  - Combines reusability with state management
  - Supports unsized state types where possible (object base type or separate interfaces)
  - Automatic implementation for any IStatefulWidget<TState> through extension methods
  - Enables boxed widget scenarios: `IStatefulWidgetRef<TState>` collections
  - Higher-ranked trait bound equivalent: extension methods or adapter pattern
  - Experimental feature with same considerations as IWidgetRef

- **Common Base**: Both interfaces should derive from common base for polymorphism
  - Enable collections of mixed widget types
  - Support runtime type checking when needed

### Rendering Protocol
- **Area Parameter**: Widgets receive a `Rect` defining their rendering area
- **Buffer Parameter**: Widgets render directly to provided buffer
- **Clipping**: Widgets must respect area boundaries and not render outside
- **Efficiency**: Widgets should only modify buffer cells that need updating
- **Frame Extensions**: Extension methods on Frame for widget rendering
  - `RenderWidget<T>(T widget, Rect area)` for IWidget implementations
  - `RenderStatefulWidget<T, TState>(T widget, Rect area, ref TState state)` for stateful widgets
  - `RenderWidgetRef<T>(T widget, Rect area)` for experimental IWidgetRef (behind feature flag)
  - `RenderStatefulWidgetRef<T>(T widget, Rect area, ref TState state)` for experimental stateful refs

### State Management
- **External State**: State is managed outside the widget instance
- **Reference Passing**: State passed by reference to avoid copying
- **Type Safety**: Generic state types ensure compile-time type checking
- **Immutability**: Widget instances should be immutable during rendering

### Widget Lifecycle
- **Creation**: Widgets are typically created for each render cycle
- **Rendering**: Single render method call per widget per frame
- **Disposal**: No explicit disposal required (managed by GC)

### Widget Composition
- **Nested Rendering**: Widgets can render other widgets as part of their content
- **Area Subdivision**: Parent widgets calculate child widget areas
- **State Propagation**: Parent widgets manage child widget state appropriately
- **Builder Pattern**: Widgets should support fluent configuration APIs
  - Method chaining for property configuration
  - Immutable builder pattern where each method returns a new instance
  - Use of `#[Pure]` attribute or similar to indicate non-mutating methods
  - Support for generic type conversions for flexible APIs

#### Fluent API Design Patterns
Based on analysis of ListItem and similar components:
- **Immutable Builder Pattern**: All configuration methods return new instances
  - Use `[Pure]` attribute in C# to indicate non-mutating methods
  - Example: `ListItem.Style(Color.Red)` returns new ListItem with updated style
- **Conversion Support**: Implicit conversion operators for common types
  - Enable creation from primitive types: `ListItem item = "Hello World";`
  - Support multiple input types through constructor overloads
- **Method Chaining**: Support for fluent configuration chains
  - Example: `ListItem.New("text").Style(Color.Red).Align(Alignment.Center)`
- **Generic Content Acceptance**: Use constraints for flexible content input
  - Pattern: `T: Into<Text>` in Rust becomes multiple C# constructors/operators
- **Container Widgets**: Foundational widgets that wrap other widgets
  - Block widget pattern for providing borders, titles, and padding
  - Inner area calculation for content placement
  - Style layering (container style, then content style)
- **Border and Title System**: Support for visual framing
  - Configurable border types (plain, rounded, double, thick, etc.)
  - Multiple titles with positioning (top/bottom) and alignment
  - Border merging strategies for adjacent blocks
  - Padding support for internal spacing

## Technical Approach

### Implementation Pattern
```csharp
namespace CycoTui.Widgets
{
    // Base interface for polymorphic widget collections
    public interface IWidgetBase
    {
        // Common properties or methods if needed
    }

    // Stateless widget interface (inspired by Ratatui's Widget trait)
    public interface IWidget : IWidgetBase
    {
        void Render(Rect area, Buffer buffer);
    }

    // Stateful widget interface with generic state
    public interface IStatefulWidget<TState> : IWidgetBase
    {
        void Render(Rect area, Buffer buffer, ref TState state);
    }

    // Experimental: Reference-based widget interfaces
    #if EXPERIMENTAL_WIDGET_REF
    public interface IWidgetRef : IWidgetBase
    {
        void RenderRef(Rect area, Buffer buffer);
    }

    public interface IStatefulWidgetRef<TState> : IWidgetBase
    {
        void RenderRef(Rect area, Buffer buffer, ref TState state);
    }
    #endif

    // Optional widget wrapper for conditional rendering
    public struct OptionalWidget<T> : IWidget where T : IWidget
    {
        private readonly T? _widget;
        
        public OptionalWidget(T? widget) => _widget = widget;
        
        public void Render(Rect area, Buffer buffer)
        {
            _widget?.Render(area, buffer);
        }
    }
}

#if EXPERIMENTAL_WIDGET_REF
// Extension methods providing automatic StatefulWidgetRef implementation
public static class StatefulWidgetRefExtensions
{
    public static void RenderRef<TWidget, TState>(this TWidget widget, Rect area, Buffer buffer, ref TState state)
        where TWidget : IStatefulWidget<TState>
    {
        widget.Render(area, buffer, ref state);
    }
}
#endif

// Extension methods for string widget support
public static class StringWidgetExtensions
{
    public static void RenderAsWidget(this string text, Rect area, Buffer buffer)
    {
        buffer.SetStringN(area.X, area.Y, text, (int)area.Width, Style.Default);
    }
}
```

### Base Classes and Interfaces
- **Interface-Only Design**: Prefer interfaces over abstract base classes
- **Minimal Hierarchy**: Keep inheritance hierarchy shallow
- **Extension Methods**: Use extension methods for common widget operations
- **Helper Classes**: Provide static helper classes for common patterns

### Widget Hierarchy
```
CycoTui.Widgets/
├── IWidgetBase (base interface)
├── IWidget (stateless widgets)
├── IStatefulWidget<TState> (stateful widgets)
├── Basic/
│   ├── Paragraph
│   ├── Block
│   ├── Clear
│   └── Borders
### Interactive Widgets

#### Table Widget Requirements
Based on Table widget analysis (`ratatui-widgets/src/table.rs`):

##### Core Table Components
- **Generic Table<'a> Structure**: Flexible data table with configurable columns, rows, headers, and footers
- **Row/Header/Footer Support**: Optional header and footer rows with separate styling from data rows
- **Column Width Management**: Flexible width constraints using Constraint system (Length, Percentage, Ratio, etc.)
- **Multi-Level Selection**: Support for row, column, and cell-level highlighting with style precedence
- **Scrolling Capabilities**: Efficient viewport management for large datasets with scroll state management

##### Table Cell Component
Based on analysis of `ratatui-widgets/src/table/cell.rs`:

- **Cell<'a> Structure**: Individual table cell containing styled text content
  - Contains `Text<'a>` content and `Style` for cell-level styling
  - Supports fluent builder pattern with consuming methods
  - Implements style composition: Cell style + Text content styles
  - Accepts any type convertible to `Text` for flexible content input
  - Internal rendering method applies cell style to entire area, then renders text content

- **C# Cell Implementation Pattern**:
  ```csharp
  public struct Cell : IStylable
  {
      private readonly Text _content;
      private readonly Style _style;

      public Cell(Text content) => (_content, _style) = (content, Style.Default);

      // Fluent builders with consuming semantics
      [Pure] public Cell Content<T>(T content) where T : IConvertible<Text> => 
          this with { _content = content.ToText() };
      
      [Pure] public Cell Style<S>(S style) where S : IConvertible<Style> => 
          this with { _style = style.ToStyle() };

      // Implicit conversions for ease of use
      public static implicit operator Cell(string content) => new Cell(content);
      public static implicit operator Cell(Text content) => new Cell(content);

      internal void Render(Rect area, Buffer buffer)
      {
          buffer.SetStyle(area, _style);  // Apply cell style to entire area
          _content.Render(area, buffer);  // Text renders with its own styles
      }
  }
  ```

- **Style Composition Behavior**: 
  - Cell style applies to entire cell area (background, borders if any)
  - Text content styles layer on top for content-specific styling
  - Text alignment handled through underlying Text alignment property
  - Style precedence: Cell style → Text style for content areas

##### Table Configuration System
- **Fluent Builder API**: Immutable builder pattern with consuming methods for configuration
- **Column Constraints**: Width constraints with validation (percentage totals must be <100%)
- **Spacing Control**: Configurable spacing between columns
- **Flexible Content**: Accept various input types through generic constraints and iterators
- **Block Integration**: Optional Block wrapper for borders and titles

##### Table State Management Pattern
- **TableState Class**: External state management for selection and scrolling
  - `Selected: int?` - index of selected row (null if none selected)
  - `SelectedColumn: int?` - index of selected column for multi-dimensional navigation
  - `Offset: int` - first visible row index for scrolling
  - Navigation methods with bounds checking and scroll position management
- **Stateful/Stateless Rendering**: Support both patterns with default state creation

##### Table Rendering Features
- **Efficient Viewport Calculation**: `visible_rows()` algorithm determines renderable content based on area
- **Style Precedence**: Row → Column → Cell highlighting with proper intersection calculations
- **Header/Footer Layout**: Separate area calculations for header, content, and footer sections
- **Selection Symbol**: Configurable highlight symbol with positioning and repeat options
- **Responsive Layout**: Handles area resizing and constraint recalculation

##### Implementation Patterns for C#
- **Generic Input Support**: Use `IEnumerable<T>` with conversion to internal Row format
- **Builder Method Chaining**: Consuming methods return new instances (record pattern)
- **State Management**: External state pattern with reference-based updates
- **Memory Efficiency**: Minimize allocations during frequent re-renders
- **Layout Integration**: Use constraint system for column width distribution

#### List Widget Requirements
- **List<TItem> Widget**: Scrollable list of items with optional selection
  - Generic over item type: `List<TItem>` where items implement `IConvertible<ListItem>`
  - Core properties: items collection, base style, highlight style and symbol
  - Display configuration: direction (TopToBottom/BottomToTop), highlight spacing behavior
  - State management: Works with `ListState` for selection and scrolling
  - Fluent builder API: all configuration methods support method chaining
  - Zero-copy rendering: supports both value and reference semantics

- **ListDirection Enum**: Controls list display orientation
  - `TopToBottom` (default) - first item at top, grows downward
  - `BottomToTop` - first item at bottom, grows upward
  - Affects scrolling behavior and item placement for short lists

- **ListItem Structure**: Individual list entry wrapper
  - Wraps `Text` content with optional per-item styling
  - Height calculated from line count, width from maximum line width
  - Style composition: ListItem style + Text style + List highlight style
  - Supports text alignment through underlying Text alignment

#### List Widget Requirements
- **List<TItem> Widget**: Scrollable list of items with optional selection
  - Generic over item type: `List<TItem>` where items implement `IConvertible<ListItem>`
  - Core properties: items collection, base style, highlight style and symbol
  - Display configuration: direction (TopToBottom/BottomToTop), highlight spacing behavior
  - State management: Works with `ListState` for selection and scrolling
  - Fluent builder API: all configuration methods support method chaining
  - Zero-copy rendering: supports both value and reference semantics

- **ListDirection Enum**: Controls list display orientation
  - `TopToBottom` (default) - first item at top, grows downward
  - `BottomToTop` - first item at bottom, grows upward
  - Affects scrolling behavior and item placement for short lists

- **ListItem Structure**: Individual list entry wrapper
  - Wraps `Text` content with optional per-item styling
  - Height calculated from line count, width from maximum line width
  - Style composition: ListItem style + Text style + List highlight style
  - Supports text alignment through underlying Text alignment

- **ListState Class**: Manages selection and scrolling state
  - `Selected: int?` - index of selected item (null if none selected)
  - `Offset: int` - index of first visible item for scrolling
  - Navigation methods: `SelectNext()`, `SelectPrevious()`, `SelectFirst()`, `SelectLast()`
  - Scroll methods: `ScrollDownBy(amount)`, `ScrollUpBy(amount)`  
  - Fluent setters: `WithSelected(index)`, `WithOffset(offset)`
  - Designed for external state management pattern
  - Uses saturating arithmetic for safe navigation operations
  - Deferred bounds checking - navigation works before list size is known
  - Compatible with both mutable and immutable state management approaches

#### List Rendering Features
- **Selection Highlighting**: Configurable highlight style and symbol for selected items
  - `HighlightStyle` property for selected item appearance
  - `HighlightSymbol` (Line) for prefix symbol on selected items
  - `RepeatHighlightSymbol` boolean for multi-line item symbol behavior
- **Highlight Spacing Management**: Control when to allocate space for selection symbols
  - `HighlightSpacing.Always` - consistent layout, no shifting (recommended)
  - `HighlightSpacing.WhenSelected` - space only when item selected (legacy)
  - `HighlightSpacing.Never` - no symbol space allocation
- **Scroll Padding**: `ScrollPadding` property to maintain context around selection
- **Block Integration**: Optional `Block` wrapper for borders and titles
- **Style Inheritance**: Hierarchical styling from List → ListItem → Text → Highlight

#### List Implementation Pattern
```csharp
public struct List<TItem> : IWidget, IStatefulWidget<ListState>
{
    private readonly IList<ListItem> _items;
    private readonly Style _style;
    private readonly ListDirection _direction;
    private readonly Style _highlightStyle;
    private readonly Line? _highlightSymbol;
    private readonly bool _repeatHighlightSymbol;
    private readonly HighlightSpacing _highlightSpacing;
    private readonly ushort _scrollPadding;
    private readonly Block? _block;

    // Constructor accepting any IEnumerable<T> where T converts to ListItem
    public List(IEnumerable<TItem> items) where TItem : IConvertible<ListItem>
    {
        _items = items.Select(item => item.ToListItem()).ToList();
        // ... initialize defaults
    }

    // Fluent builder methods (all return new instance)
    [Pure] public List<TItem> Style<S>(S style) where S : IConvertible<Style>;
    [Pure] public List<TItem> HighlightStyle<S>(S style) where S : IConvertible<Style>;
    [Pure] public List<TItem> HighlightSymbol<L>(L symbol) where L : IConvertible<Line>;
    [Pure] public List<TItem> RepeatHighlightSymbol(bool repeat);
    [Pure] public List<TItem> HighlightSpacing(HighlightSpacing spacing);
    [Pure] public List<TItem> Direction(ListDirection direction);
    [Pure] public List<TItem> ScrollPadding(ushort padding);
    [Pure] public List<TItem> Block(Block block);

    // Both stateless and stateful rendering support
    public void Render(Rect area, Buffer buffer) => 
        Render(area, buffer, ref ListState.Default);
    
    public void Render(Rect area, Buffer buffer, ref ListState state)
    {
        // Rendering implementation with state management
    }

    // Collection interface support
    public int Count => _items.Count;
    public bool IsEmpty => _items.Count == 0;
}

public enum ListDirection
{
    TopToBottom,
    BottomToTop
}

public class ListState  
{
    public int? Selected { get; set; }
    public int Offset { get; set; }
    
    public static ListState Default => new ListState();
    
    public ListState WithSelected(int? selected) => new ListState { Selected = selected, Offset = Offset };
}

public enum HighlightSpacing
{
    Always,     // Recommended: consistent layout
    WhenSelected, // Legacy: layout shifts
    Never       // No symbol space
}
```

#### List Usage Patterns
```csharp
// Basic list with fluent configuration
var list = new List<string>(["Item 1", "Item 2", "Item 3"])
    .Style(Style.Default.Foreground(Color.White))
    .HighlightStyle(Style.Default.Background(Color.Blue))
    .HighlightSymbol(">> ")
    .RepeatHighlightSymbol(true)
    .Direction(ListDirection.TopToBottom)
    .Block(Block.Bordered().Title("My List"));

// Stateless rendering
list.Render(area, buffer);

// Stateful rendering with selection
var listState = new ListState { Selected = 1 };
list.Render(area, buffer, ref listState);

// Collection interface for dynamic lists
var dynamicList = new List<ListItem>(
    items.Where(predicate).Select(transform)
);
```

### Interactive/
│   ├── List
│   ├── Table
│   ├── Tabs
│   └── Scrollbar
├── Visualization/
│   ├── Chart
│   ├── BarChart
│   ├── Gauge
│   ├── LineGauge
│   ├── Sparkline
│   └── Canvas
├── Special/
│   ├── Calendar (feature-flagged)
│   ├── Logo
│   └── Mascot
└── Extensions/
    ├── StringWidget
    └── OptionalWidget<T>
```

### Builder Pattern for Widget Configuration

Widgets should support fluent configuration through immutable builder patterns:

```csharp
// Example widget using builder pattern
public struct Bar : IWidget
{
    private readonly ulong _value;
    private readonly Line? _label;
    private readonly Style _style;
    private readonly Style _valueStyle;
    private readonly string? _textValue;

    public Bar(ulong value)
    {
        _value = value;
        _label = null;
        _style = Style.Default;
        _valueStyle = Style.Default;
        _textValue = null;
    }

    public static Bar WithLabel<T>(T label, ulong value) where T : IConvertible<Line>
    {
        return new Bar(value) { _label = label.ToLine() };
    }

    [Pure] 
    public Bar Label<T>(T label) where T : IConvertible<Line> => this with { _label = label.ToLine() };

    [Pure]
    public Bar Style<S>(S style) where S : IConvertible<Style> => this with { _style = style.ToStyle() };

    [Pure]
    public Bar ValueStyle<S>(S style) where S : IConvertible<Style> => this with { _valueStyle = style.ToStyle() };

    [Pure]
    public Bar TextValue<T>(T textValue) where T : IConvertible<string> => this with { _textValue = textValue.ToString() };

    public void Render(Rect area, Buffer buffer)
    {
        // Implementation using the configured properties
    }
}

// BarGroup example with multiple construction patterns
public struct BarGroup : IWidget  
{
    private readonly Line? _label;
    private readonly IList<Bar> _bars;

    public BarGroup(IEnumerable<Bar> bars)
    {
        _bars = bars.ToList();
        _label = null;
    }

    public static BarGroup WithLabel<T>(T label, IEnumerable<Bar> bars) where T : IConvertible<Line>
    {
        return new BarGroup(bars) { _label = label.ToLine() };
    }

    [Pure]
    public BarGroup Label<T>(T label) where T : IConvertible<Line> => this with { _label = label.ToLine() };

    [Pure] 
    public BarGroup Bars(IEnumerable<Bar> bars) => this with { _bars = bars.ToList() };

    // Multiple constructor overloads for different input patterns
    public static implicit operator BarGroup((string label, ulong value)[] tuples)
    {
        return new BarGroup(tuples.Select(t => Bar.WithLabel(t.label, t.value)));
    }

    public ulong? Max => _bars.Any() ? _bars.Max(b => b.Value) : null;

    public void Render(Rect area, Buffer buffer)
    {
        // Render bars and optional label with alignment
        RenderLabel(buffer, area, Style.Default);
        // ... bar rendering logic
    }

    private void RenderLabel(Buffer buffer, Rect area, Style defaultStyle)
    {
        if (_label == null) return;

        var width = (ushort)_label.Width;
        var labelArea = _label.Alignment switch
        {
            Alignment.Center => area with { 
                X = (ushort)(area.X + (area.Width - width) / 2), 
                Width = width 
            },
            Alignment.Right => area with { 
                X = (ushort)(area.X + area.Width - width), 
                Width = width 
            },
            _ => area with { Width = width }
        };
        
        buffer.SetStyle(labelArea, defaultStyle);
        _label.Render(labelArea, buffer);
    }
}

// Usage examples:
var barGroup1 = new BarGroup([
    Bar.WithLabel("Sales", 250),
    Bar.WithLabel("Marketing", 180)
]).Label("Q1 Results");

var barGroup2 = BarGroup.WithLabel("Q2 Results", [
    Bar.WithLabel("Sales", 320),
    Bar.WithLabel("Marketing", 210)
]);

// Using implicit conversion from tuples
BarGroup barGroup3 = new[] { ("Sales", 250ul), ("Marketing", 180ul) };
```

### Alignment System for Widget Labels

Widgets that display labels should implement consistent alignment behavior:

```csharp
public enum Alignment
{
    Left,
    Center, 
    Right
}

// Common alignment calculation utility
public static class AlignmentHelper
{
    public static Rect CalculateAlignedArea(Rect container, ushort contentWidth, Alignment alignment)
    {
        return alignment switch
        {
            Alignment.Center => container with { 
                X = (ushort)(container.X + Math.Max(0, (container.Width - contentWidth) / 2)), 
                Width = Math.Min(contentWidth, container.Width)
            },
            Alignment.Right => container with { 
                X = (ushort)(container.X + Math.Max(0, container.Width - contentWidth)), 
                Width = Math.Min(contentWidth, container.Width)
            },
            _ => container with { Width = Math.Min(contentWidth, container.Width) }
        };
    }
}
```

### Assembly Organization
- **Single Assembly Approach**: All widgets in one CycoTui.Widgets assembly
- **Namespace Organization**: Widgets organized by functional groups within namespaces
- **Feature Flags**: Optional widgets controlled via conditional compilation or separate packages
- **Compatibility**: Target .NET Standard for broad framework compatibility
- **Dependencies**: Minimize external dependencies beyond core CycoTui assemblies

### Example Implementation

#### Paragraph Widget
```csharp
public struct Paragraph : IWidget
{
    private readonly Text _text;
    private readonly Style _style;

    public Paragraph(Text text, Style style = default)
    {
        _text = text;
        _style = style;
    }

    public void Render(Rect area, Buffer buffer)
    {
        // Implementation details...
        for (int y = 0; y < area.Height && y < _text.Lines.Count; y++)
        {
            buffer.SetString(area.X, area.Y + y, _text.Lines[y], _style);
        }
    }
}
```

#### Block Widget (Container/Foundational Widget)
```csharp
public struct Block : IWidget
{
    private readonly List<(TitlePosition?, Line)> _titles;
    private readonly Style _titlesStyle;
    private readonly Alignment _titlesAlignment;
    private readonly TitlePosition _titlesPosition;
    private readonly Borders _borders;
    private readonly Style _borderStyle;
    private readonly BorderSet _borderSet;
    private readonly Style _style;
    private readonly Padding _padding;
    private readonly MergeStrategy _mergeBorders;

    public Block()
    {
        _titles = new List<(TitlePosition?, Line)>();
        _titlesStyle = Style.Default;
        _titlesAlignment = Alignment.Left;
        _titlesPosition = TitlePosition.Top;
        _borders = Borders.None;
        _borderStyle = Style.Default;
        _borderSet = BorderType.Plain.ToBorderSet();
        _style = Style.Default;
        _padding = Padding.Zero;
        _mergeBorders = MergeStrategy.Replace;
    }

    // Factory methods
    public static Block New() => new Block();
    public static Block Bordered() => new Block() with { _borders = Borders.All };

    // Builder pattern methods
    [Pure] public Block Title<T>(T title) where T : IConvertible<Line> => 
        this with { _titles = _titles.Append((null, title.ToLine())).ToList() };
    
    [Pure] public Block TitleTop<T>(T title) where T : IConvertible<Line> => 
        this with { _titles = _titles.Append((TitlePosition.Top, title.ToLine())).ToList() };
    
    [Pure] public Block TitleBottom<T>(T title) where T : IConvertible<Line> => 
        this with { _titles = _titles.Append((TitlePosition.Bottom, title.ToLine())).ToList() };
    
    [Pure] public Block Borders(Borders borders) => this with { _borders = borders };
    [Pure] public Block BorderStyle<S>(S style) where S : IConvertible<Style> => this with { _borderStyle = style.ToStyle() };
    [Pure] public Block BorderType(BorderType borderType) => this with { _borderSet = borderType.ToBorderSet() };
    [Pure] public Block Style<S>(S style) where S : IConvertible<Style> => this with { _style = style.ToStyle() };
    [Pure] public Block Padding(Padding padding) => this with { _padding = padding };
    [Pure] public Block MergeBorders(MergeStrategy strategy) => this with { _mergeBorders = strategy };

    // Core functionality - calculate inner area
    public Rect Inner(Rect area)
    {
        var inner = area;
        
        // Account for borders
        if (_borders.HasFlag(Borders.Left))
        {
            inner = inner with { X = Math.Min((ushort)(inner.X + 1), inner.Right), Width = (ushort)Math.Max(0, inner.Width - 1) };
        }
        if (_borders.HasFlag(Borders.Top) || HasTitleAtPosition(TitlePosition.Top))
        {
            inner = inner with { Y = Math.Min((ushort)(inner.Y + 1), inner.Bottom), Height = (ushort)Math.Max(0, inner.Height - 1) };
        }
        if (_borders.HasFlag(Borders.Right))
        {
            inner = inner with { Width = (ushort)Math.Max(0, inner.Width - 1) };
        }
        if (_borders.HasFlag(Borders.Bottom) || HasTitleAtPosition(TitlePosition.Bottom))
        {
            inner = inner with { Height = (ushort)Math.Max(0, inner.Height - 1) };
        }

        // Account for padding
        inner = inner with 
        { 
            X = (ushort)(inner.X + _padding.Left),
            Y = (ushort)(inner.Y + _padding.Top),
            Width = (ushort)Math.Max(0, inner.Width - _padding.Left - _padding.Right),
            Height = (ushort)Math.Max(0, inner.Height - _padding.Top - _padding.Bottom)
        };

        return inner;
    }

    public void Render(Rect area, Buffer buffer)
    {
        var clippedArea = area.Intersection(buffer.Area);
        if (clippedArea.IsEmpty) return;

        // Apply base style
        buffer.SetStyle(clippedArea, _style);
        
        // Render borders
        RenderBorders(clippedArea, buffer);
        
        // Render titles
        RenderTitles(clippedArea, buffer);
    }

    private void RenderBorders(Rect area, Buffer buffer)
    {
        RenderSides(area, buffer);
        RenderCorners(area, buffer);
    }

    private void RenderSides(Rect area, Buffer buffer)
    {
        // Complex border rendering logic with merge strategy support
        // Implementation would handle border character placement and merging
    }

    private void RenderCorners(Rect area, Buffer buffer)
    {
        // Corner rendering logic
    }

    private void RenderTitles(Rect area, Buffer buffer)
    {
        // Title positioning and rendering logic
        // Handle multiple titles, alignment, and spacing
    }

    private bool HasTitleAtPosition(TitlePosition position) =>
        _titles.Any(t => t.Item1?.GetValueOrDefault(_titlesPosition) == position);
}

public enum TitlePosition
{
    Top,
    Bottom
}

[Flags]
public enum Borders
{
    None = 0,
    Top = 1,
    Right = 2,
    Bottom = 4,
    Left = 8,
    All = Top | Right | Bottom | Left
}

public enum BorderType
{
    Plain,
    Rounded,
    Double,
    Thick
}

public struct Padding
{
    public ushort Top { get; init; }
    public ushort Right { get; init; }
    public ushort Bottom { get; init; }
    public ushort Left { get; init; }

    public static Padding Zero => new Padding();
    public static Padding Uniform(ushort value) => new Padding { Top = value, Right = value, Bottom = value, Left = value };
}

public enum MergeStrategy
{
    Replace,
    Exact,
    Fuzzy
}
```

## Examples

### Stateless Widget Example
```csharp
var paragraph = new Paragraph("Hello, World!")
    .Style(Style.Default.Foreground(Color.Blue));

paragraph.Render(area, buffer);
```

### Stateful Widget Example
```csharp
var listState = new ListState { Selected = 2 };
var list = new List(items);

list.Render(area, buffer, ref listState);
```

### Progress/Gauge Widget Example
```csharp
// Basic gauge with percentage
var gauge = new Gauge()
    .Percent(75)
    .GaugeStyle(Style.Default.Foreground(Color.Green))
    .UseUnicode(true);

gauge.Render(area, buffer);

// Line gauge with custom symbols
var lineGauge = new LineGauge()
    .Ratio(0.6)
    .FilledSymbol("█")
    .UnfilledSymbol("░")
    .FilledStyle(Style.Default.Foreground(Color.Blue))
    .Label("Download Progress");

lineGauge.Render(area, buffer);
```

## See Also

### Utility Components

#### Mathematical Polyfills (Optional)
- **Purpose**: Provide floating-point math operations for specialized environments
- **C# Consideration**: .NET has comprehensive Math library, polyfills likely unnecessary
- **Implementation Options**:
  - Skip entirely (recommended) - use `System.Math` directly
  - Provide as extension methods for API completeness
  - Implement only if embedded .NET scenarios require them
- **API Design**: If implemented, use static extension methods on `double`
  ```csharp
  public static class DoublePolyfillExtensions
  {
      public static double MulAdd(this double value, double a, double b) => Math.FusedMultiplyAdd(value, a, b);
      // Other methods map directly to System.Math equivalents
  }
  ```

- [002-WIDGET-SYSTEM-001](../features/002-WIDGET-SYSTEM-001.md): Widget system feature requirements
- [SPEC-BUFFER-002](SPEC-BUFFER-002.md): Buffer implementation details
- [SPEC-LAYOUT-004](SPEC-LAYOUT-004.md): Layout system integration