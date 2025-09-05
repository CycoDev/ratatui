# Source File Analysis: ratatui-widgets/src/paragraph.rs

## Basic Information

- **File Path**: ratatui-widgets/src/paragraph.rs
- **Component**: Widget Component
- **Analysis Date**: 2023-11-28

## Key Types and Interfaces

### **Paragraph<'a>**
- **Purpose**: A widget to display text with optional wrapping, alignment, and block styling
- **Key Properties**: 
  - `block: Option<Block<'a>>` - Optional border/title wrapper
  - `style: Style` - Widget-wide styling
  - `wrap: Option<Wrap>` - Text wrapping configuration
  - `text: Text<'a>` - The text content to display
  - `scroll: Position` - Scroll offset (y, x)
  - `alignment: Alignment` - Text alignment (Left, Center, Right)
- **Key Methods**:
  - `new<T: Into<Text<'a>>>(text)` - Constructor accepting any text type
  - `block(Block)` - Sets border/container
  - `style<S: Into<Style>>(style)` - Sets widget style
  - `wrap(Wrap)` - Configures text wrapping
  - `scroll((Vertical, Horizontal))` - Sets scroll offset
  - `alignment(Alignment)` - Sets text alignment
  - `left_aligned()`, `centered()`, `right_aligned()` - Convenience alignment methods
  - `line_count(width)` - Calculates rendered line count (unstable feature)
  - `line_width()` - Calculates minimum width needed (unstable feature)
- **Usage Pattern**: Fluent builder pattern with method chaining

### **Wrap**
- **Purpose**: Configuration for text wrapping behavior
- **Key Properties**:
  - `trim: bool` - Whether to trim leading whitespace when wrapping
- **Usage Pattern**: Simple configuration struct passed to `wrap()` method

## Core Behaviors

### **Text Rendering**
- **Description**: Renders text with support for styling, alignment, wrapping, and scrolling
- **Implementation Approach**: 
  - Uses composition pattern with line composers (WordWrapper, LineTruncator)
  - Applies styling through buffer operations
  - Handles Unicode graphemes correctly
- **Performance Considerations**: Efficient grapheme iteration, lazy line composition
- **Edge Cases**: Zero-width areas, out-of-bounds rendering, empty text, Unicode handling

### **Text Wrapping**
- **Description**: Wraps text across multiple lines when content exceeds available width
- **Implementation Approach**: 
  - WordWrapper for wrapped text with word boundaries
  - LineTruncator for non-wrapped text
  - Configurable whitespace trimming
- **Performance Considerations**: Iterative line composition, avoids pre-computing all lines
- **Edge Cases**: Very long words, whitespace-only lines, Unicode width calculations

### **Scrolling**
- **Description**: Supports vertical and horizontal scrolling through content
- **Implementation Approach**: 
  - Position-based offset (y, x coordinates)
  - Applied after wrapping and alignment calculations
  - Skip lines for vertical scroll, offset characters for horizontal
- **Performance Considerations**: Efficient skipping to scroll position
- **Edge Cases**: Scroll beyond content bounds, zero-width scrolling

### **Alignment**
- **Description**: Supports left, center, and right text alignment
- **Implementation Approach**: 
  - Calculated per-line during rendering
  - Uses `get_line_offset()` function for positioning
  - Accounts for line width vs. available width
- **Performance Considerations**: Simple arithmetic calculations per line
- **Edge Cases**: Content wider than available space, zero-width areas

## Platform-Specific Code

- **None**: This widget is platform-independent, relying on the buffer abstraction

## Dependencies

### **Internal Dependencies**
- `ratatui_core::buffer::Buffer` - Target rendering buffer
- `ratatui_core::layout::{Alignment, Position, Rect}` - Layout types
- `ratatui_core::style::{Style, Styled}` - Styling system
- `ratatui_core::text::{Line, StyledGrapheme, Text}` - Text types
- `ratatui_core::widgets::Widget` - Widget trait
- `crate::block::{Block, BlockExt}` - Container/border widget
- `crate::reflow::{LineComposer, LineTruncator, WordWrapper, WrappedLine}` - Text wrapping

### **External Dependencies**
- `unicode_width::UnicodeWidthStr` - Unicode character width calculations

## Key Algorithms and Techniques

### **Line Composition**
- **Purpose**: Converts text into rendered lines with proper wrapping and alignment
- **Approach**: Strategy pattern with LineComposer trait, different implementations for wrapped vs. non-wrapped text
- **Complexity**: O(n) where n is text length
- **Optimizations**: Lazy evaluation, early termination for scrolling

### **Grapheme Rendering**
- **Purpose**: Correctly handles Unicode characters including wide characters and combining marks
- **Approach**: Iterates through styled graphemes, accounts for display width
- **Complexity**: O(g) where g is grapheme count
- **Optimizations**: Direct buffer manipulation, width caching

### **Alignment Calculation**
- **Purpose**: Positions text within available width based on alignment setting
- **Approach**: Simple offset calculation based on line width vs. area width
- **Complexity**: O(1) per line
- **Optimizations**: Const function, saturating arithmetic

## C# Port Considerations

### **Idiomatic Translations**
- `Option<T>` → `T?` (nullable reference types)
- `impl Widget for Paragraph` → `class Paragraph : IWidget`
- Lifetime parameters `<'a>` → Not needed in C# (GC managed)
- `const fn` → `const` or `static readonly` where appropriate
- `#[must_use]` → Consider `[Pure]` attribute or analyzer rules
- Fluent builder pattern → Keep same pattern, very C# idiomatic

### **Potential Challenges**
- Unicode width calculations - need equivalent to `unicode_width` crate
- Grapheme cluster handling - use `System.Globalization.StringInfo`
- Pattern matching on alignment - use switch expressions
- Const generics for type safety - use generic constraints where possible
- Zero-cost abstractions - accept some performance overhead for C# idioms

### **.NET API Equivalents**
- `unicode_width::UnicodeWidthStr` → Custom implementation or NuGet package
- Iterator patterns → `IEnumerable<T>` and LINQ
- `Into<T>` trait → Implicit conversion operators
- `Clone` trait → `ICloneable` or copy constructors
- Pattern matching → Switch expressions or if-else chains

## Documentation Updates Needed

### **Features**
- `002-WIDGET-SYSTEM-001.md` - Add paragraph widget capabilities
- `006-TEXT-SYSTEM-001.md` - Update with paragraph text handling requirements
- Create new feature document for text formatting and display

### **Specifications**
- `SPEC-WIDGET-003.md` - Add paragraph widget implementation details
- `SPEC-TEXT-001.md` - Update with text rendering and wrapping requirements
- Create specification for Unicode handling and text measurement

### **Tasks**
- Create `WIDGET-PARAGRAPH-001` task for implementing paragraph widget
- Create `TEXT-WRAPPING-001` task for text wrapping implementation
- Create `TEXT-ALIGNMENT-001` task for text alignment implementation
- Create `UNICODE-WIDTH-001` task for Unicode width calculation implementation

## Questions and Issues

### **Unicode Width Library**
- **Context**: Paragraph relies heavily on accurate Unicode width calculations
- **Potential Solutions**: 
  - Port unicode-width library to C#
  - Find existing .NET Unicode width library
  - Use System.Globalization APIs where possible

### **Text Wrapping Performance**
- **Context**: Need to ensure text wrapping performs well with large text blocks
- **Potential Solutions**: 
  - Implement lazy evaluation similar to Rust version
  - Consider caching strategies for repeated renders
  - Profile different approaches in C#

### **Builder Pattern Ergonomics**
- **Context**: Maintain fluent API while following C# conventions
- **Potential Solutions**: 
  - Use method chaining with `return this`
  - Consider records for immutable variants
  - Evaluate expression builder patterns