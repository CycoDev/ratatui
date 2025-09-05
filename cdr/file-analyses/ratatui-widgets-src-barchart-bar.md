# Source File Analysis: ratatui-widgets/src/barchart/bar.rs

## Basic Information

- **File Path**: `ratatui-widgets/src/barchart/bar.rs`
- **Component**: Widget (Data Visualization)
- **Analysis Date**: 2023-11-28

## Key Types and Interfaces

### `Bar<'a>` struct
- **Purpose**: Represents a single bar in a bar chart widget
- **Key Properties**:
  - `value: u64` - Numeric value to display
  - `label: Option<Line<'a>>` - Optional label below/in the bar
  - `style: Style` - Style for the bar itself
  - `value_style: Style` - Style for the value text
  - `text_value: Option<String>` - Optional custom text instead of numeric value
- **Key Methods**:
  - `new(value: u64)` - Creates bar with value only
  - `with_label<T: Into<Line<'a>>>(label: T, value: u64)` - Creates bar with label and value
  - `value(value: u64)` - Sets the numeric value (builder pattern)
  - `label<T: Into<Line<'a>>>(label: T)` - Sets the label (builder pattern)
  - `style<S: Into<Style>>(style: S)` - Sets bar style (builder pattern)
  - `value_style<S: Into<Style>>(style: S)` - Sets value text style (builder pattern)
  - `text_value<T: Into<String>>(text_value: T)` - Sets custom display text (builder pattern)
- **Usage Pattern**: Builder pattern with fluent API

### Rendering Methods (Internal)
- **`render_value_with_different_styles`**: Renders value text with overflow handling
- **`render_value`**: Renders centered value with space constraints
- **`render_label`**: Renders centered label

## Core Behaviors

### **Builder Pattern Implementation**
- **Description**: Fluent API allowing method chaining for configuration
- **Implementation Approach**: Each setter method takes `self` by value and returns modified `Self`
- **Performance Considerations**: Uses `#[must_use]` attribute to prevent accidental discarding
- **Edge Cases**: All setters are optional, providing sensible defaults

### **Value Rendering with Overflow**
- **Description**: Handles cases where value text is longer than available bar space
- **Implementation Approach**: Splits text at character boundaries, applies different styles to parts
- **Performance Considerations**: Uses `char_indices()` for proper UTF-8 handling
- **Edge Cases**: Handles empty text, exact-width text, and overflow scenarios

### **Text Width Calculation**
- **Description**: Uses unicode-width crate for proper text width calculation
- **Implementation Approach**: Accounts for wide characters and Unicode complexities
- **Performance Considerations**: Width calculation affects centering and overflow decisions
- **Edge Cases**: Handles zero-width characters and wide Unicode characters

### **Style Composition**
- **Description**: Combines default styles with specific bar/value styles using patch method
- **Implementation Approach**: Uses Style::patch() for non-destructive style merging
- **Performance Considerations**: Minimal overhead for style composition
- **Edge Cases**: Handles missing styles gracefully with defaults

## Platform-Specific Code

- **None**: This file is platform-agnostic

## Dependencies

### Internal Dependencies
- `ratatui_core::buffer::Buffer` - For rendering to terminal buffer
- `ratatui_core::layout::Rect` - For area/position management
- `ratatui_core::style::{Style, Styled}` - For styling system
- `ratatui_core::text::Line` - For text content representation
- `ratatui_core::widgets::Widget` - For widget trait (imported but not directly used in this file)

### External Dependencies
- `unicode_width::UnicodeWidthStr` - For accurate text width calculation
- `alloc::string::{String, ToString}` - For string operations (no-std compatible)

## Key Algorithms and Techniques

### **Text Overflow Handling Algorithm**
- **Purpose**: Safely split text at character boundaries when it exceeds available space
- **Approach**: Uses `char_indices()` iterator to find valid split points
- **Complexity**: O(n) where n is the length of text up to split point
- **Optimizations**: Early termination when finding split point

### **Centering Algorithm**
- **Purpose**: Center text within available space
- **Approach**: Calculate offset as `(available_width - text_width) / 2`
- **Complexity**: O(1) arithmetic operation
- **Optimizations**: Uses bit shifting (`>> 1`) instead of division by 2

## C# Port Considerations

### Idiomatic Translations
- `Bar<'a>` → `Bar` (no lifetimes needed in C#)
- `Option<T>` → `T?` (nullable reference types)
- `Into<T>` trait → Generic constraints with implicit conversions
- `#[must_use]` → `[Pure]` attribute or analyzer rules
- Builder pattern → Fluent interface pattern (common in C#)

### Potential Challenges
- **Unicode width calculation**: Need to find or implement equivalent to `unicode-width` crate
- **Character boundary splitting**: C# string handling with proper Unicode support
- **Style patching**: Implement similar non-destructive style merging
- **Memory allocation**: Builder pattern in C# typically creates new objects vs. Rust's move semantics

### .NET API Equivalents
- `unicode_width::UnicodeWidthStr` → Custom implementation or third-party library
- `alloc::string::String` → `System.String`
- `char_indices()` → `StringInfo.ParseCombiningCharacters()` or custom enumeration
- Pattern matching → C# pattern matching or if/else chains

## Documentation Updates Needed

### Features
- **006-DATA-VISUALIZATION-001.md**: Add bar chart component requirements
- **002-WIDGET-SYSTEM-001.md**: Include builder pattern and fluent API requirements

### Specifications
- **SPEC-WIDGET-003.md**: Add widget builder pattern specification
- **SPEC-TEXT-001.md**: Include Unicode width handling requirements
- **SPEC-STYLE-005.md**: Add style composition and patching requirements

### Tasks
- **TEXT-UNICODE-WIDTH-001**: Create task for Unicode width calculation implementation
- **WIDGET-BARCHART-001**: Update with Bar component implementation details
- **STYLE-COMPOSITION-001**: Create task for style patching system

## Questions and Issues

### **Unicode Width Library**
- **Context**: Need equivalent to Rust's `unicode-width` crate for accurate text width calculation
- **Potential Solutions**: 
  - Find existing .NET Unicode width library
  - Port unicode-width algorithms to C#
  - Use ICU.NET library if available
  - Implement simplified version for common cases

### **Memory Allocation in Builder Pattern**
- **Context**: Rust's move semantics allow zero-allocation builder pattern; C# typically allocates
- **Potential Solutions**:
  - Accept allocation overhead for idiomatic C# code
  - Use `ref struct` for stack allocation (complex)
  - Implement mutable builder pattern
  - Use factory pattern with parameter objects