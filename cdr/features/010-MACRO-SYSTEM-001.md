---
id: 010-MACRO-SYSTEM-001
title: Macro System and Convenience APIs
status: draft
priority: medium
date: 2023-11-28
---

# Macro System and Convenience APIs

## Overview

The macro system provides compile-time convenience APIs that reduce boilerplate when creating UI elements. This feature encompasses both direct equivalents to Ratatui's declarative macros and C#-idiomatic convenience methods.

## User Stories

1. **As a developer**, I want concise syntax for creating styled text elements without verbose constructor calls.
2. **As a developer**, I want shorthand notation for defining layout constraints and directions.
3. **As a developer**, I want simplified table row creation with automatic type conversion.
4. **As a developer**, I want compile-time code generation for repetitive UI patterns.
5. **As a library maintainer**, I want to provide both explicit APIs and convenient shortcuts.
6. **As a library maintainer**, I want development tools that ensure code quality and consistency.
7. **As a contributor**, I want automated code formatting and linting that catches issues early.

## Core Requirements

### Text Convenience APIs
Based on analysis of `ratatui-macros/src/span.rs` and `ratatui-macros/src/line.rs`, the text convenience APIs must provide:

**Span Creation Patterns**:
Based on the span! macro analysis, the span convenience APIs must provide:
- Simple text creation: `span!("text")` equivalent
- Formatted text creation: `span!("hello {}", name)` equivalent  
- Expression conversion: `span!(variable)` equivalent
- Styled text creation: `span!(style; "text")` equivalent
- Style-type flexibility: `span!(Color::Green; "text")` and `span!(Modifier::BOLD; "text")` equivalents
- Compile-time error prevention for invalid syntax patterns

**Required Span API Patterns**:
```csharp
// Ratatui: span!("literal") → CycoTui: Span.Raw("literal") or "literal".ToSpan()
// Ratatui: span!("hello {}", name) → CycoTui: Span.Raw($"hello {name}")
// Ratatui: span!(variable) → CycoTui: Span.Raw(variable.ToString()) or variable.ToSpan()
// Ratatui: span!(Color::Green; "text") → CycoTui: Span.Styled(Color.Green, "text") or "text".Green()
// Ratatui: span!(Modifier::BOLD; "text") → CycoTui: Span.Styled(Modifier.Bold, "text") or "text".Bold()
// Ratatui: span!(style; "text") → CycoTui: Span.Styled(style, "text") or "text".Styled(style)
```

**Alternative C# Approaches for Spans**:
```csharp
// Extension method approach for quick styling
var span = "hello world".Green().Bold();
var span = $"hello {name}".Red().Italic();

// Fluent builder approach
var span = Span.Builder()
    .WithText("hello world")
    .WithColor(Color.Green)
    .WithModifier(Modifier.Bold)
    .Build();

// Static factory methods (recommended for macro equivalent)
var span = Span.Raw("hello world");
var span = Span.Styled(Color.Green, "hello world");
var span = Span.Styled(Modifier.Bold, "hello world");

// String interpolation integration
var span = Span.Raw($"hello {name}");
var span = Span.Styled(Color.Green, $"hello {name}");
```

**Line Creation Patterns**:
- Empty line creation: `line![]` equivalent
- Multiple span collection: `line!["hello", "world"]` equivalent  
- Repeated span pattern: `line!["hello"; 3]` equivalent
- Automatic span conversion via `.into()` pattern
- Nested macro composition support

**Required Line API Patterns**:
```csharp
// Ratatui: line![] → CycoTui: Line.Empty() or new Line()
// Ratatui: line!["hello", "world"] → CycoTui: Line.From("hello", "world")
// Ratatui: line!["hello"; 3] → CycoTui: Line.Repeat("hello", 3)
// Ratatui: line![span1, span2] → CycoTui: Line.From(span1, span2)
```

**Alternative C# Approaches**:
```csharp
// Collection initializer approach (requires ICollection implementation)
var line = new Line { "hello", "world" };

// Extension method approach
var line = new[] { "hello", "world" }.ToLine();
var line = "hello".Repeat(3).ToLine();

// Builder pattern approach
var line = Line.Builder().Add("hello").Add("world").Build();

// Static factory method approach (recommended)
var line = Line.From("hello", "world");
var line = Line.Repeat("hello", 3);
var line = Line.Empty();
```

**Core Text Requirements**:
- **Span Creation**: Simple raw text spans and styled spans with compile-time safety
- **Format String Integration**: Support for string interpolation and format strings 
- **Style Type Flexibility**: Accept Color, Modifier, or full Style objects for styling
- **Automatic Conversion**: Support conversion from any object to span via ToString()
- **Error Prevention**: Design APIs to prevent common mistakes that would cause compile errors in Rust
- **Expression Support**: Handle both literal strings and variable expressions seamlessly
- Styled span creation with fluent syntax
- Line creation from multiple elements with automatic conversion
- Text creation from multiple lines with automatic conversion
- Support for style composition and color shortcuts
- String interpolation integration
- Automatic type conversion from strings to spans

### Layout Convenience APIs
Based on analysis of `ratatui-macros/src/layout.rs`, the layout convenience APIs must provide:

**Constraint Creation Shortcuts**:
- Support for all Ratatui constraint operators: `==`, `>=`, `<=`, `%`, `/`, `*=`
- Extension methods on integer types for fluent syntax
- Direct mapping to Constraint enum variants
- Type-safe parameter validation

**Supported Constraint Patterns**:
```csharp
// Ratatui: constraint!(==50) → CycoTui: 50.Fixed()
// Ratatui: constraint!(>=20) → CycoTui: 20.Min()  
// Ratatui: constraint!(<=100) → CycoTui: 100.Max()
// Ratatui: constraint!(==30%) → CycoTui: 30.Percent()
// Ratatui: constraint!(*=1) → CycoTui: 1.Fill()
// Ratatui: constraint!(==1/3) → CycoTui: ConstraintBuilder.Ratio(1, 3)
```

**Constraint Array Creation**:
- Support comma-separated constraint lists
- Support semicolon repetition syntax (via overloads)
- Collection initializer syntax compatibility
- Mixed constraint type support

**Layout Creation Shortcuts**:
```csharp
// Ratatui: vertical![==1, *=1, >=3] 
// CycoTui: Layout.Vertical(1.Fixed(), 1.Fill(), 3.Min())

// Ratatui: horizontal![>=20, *=1, >=20]
// CycoTui: Layout.Horizontal(20.Min(), 1.Fill(), 20.Min())
```

**Advanced Features**:
- Constraint array repetition patterns
- Complex constraint expressions
- Direction-specific layout builders
- Nested layout composition support

### Table Convenience APIs
Based on analysis of `ratatui-macros/src/row.rs`, the table convenience APIs must provide:

**Row Creation Patterns**:
- Empty row creation: `row![]` equivalent
- Multiple cell collection: `row!["cell1", "cell2"]` equivalent
- Repeated cell pattern: `row!["cell"; count]` equivalent
- Automatic cell conversion via `Cell::from()` pattern
- Nested content support (Text, Line, Span within cells)

**Required Row API Patterns**:
```csharp
// Ratatui: row![] → CycoTui: Row.Empty() or new Row()
// Ratatui: row!["hello", "world"] → CycoTui: Row.From("hello", "world")
// Ratatui: row!["hello"; 3] → CycoTui: Row.Repeat("hello", 3)
// Ratatui: row![cell1, cell2] → CycoTui: Row.From(cell1, cell2)
```

**Alternative C# Approaches**:
```csharp
// Collection initializer approach (requires ICollection implementation)
var row = new Row { "hello", "world" };

// Extension method approach
var row = new[] { "hello", "world" }.ToRow();
var row = "hello".Repeat(3).ToRow();

// Builder pattern approach
var row = Row.Builder().Add("hello").Add("world").Build();

// Static factory method approach (recommended)
var row = Row.From("hello", "world");
var row = Row.Repeat("hello", 3);
var row = Row.Empty();
```

**Core Row Requirements**:
- Row creation from heterogeneous elements
- Automatic cell conversion from various types (string, Text, Line, Span)
- Support for nested content within cells
- Flexible cell repetition patterns
- Type-safe parameter validation
- Optimized single-allocation construction

### C# Implementation Strategy
- Fluent builder APIs as primary approach
- Extension methods for enhanced convenience
- Source generators for advanced compile-time scenarios
- Method overloads for different parameter types

## Technical Strategy

### Fluent API Design
- Use method chaining for style composition
- Implement implicit conversions where appropriate
- Provide both generic and typed overloads
- Support C# collection initializer syntax

### Builder Pattern Implementation
- Create builder classes for complex types
- Implement fluent interfaces with method chaining
- Support both mutable and immutable builder patterns
- Provide validation at build time

### Extension Method Approach
- Extend core types with convenience methods
- Use static extension classes for organization
- Implement operator overloads where meaningful
- Support LINQ-style method chaining

## Dependencies

- Text System (006-TEXT-SYSTEM-001)
- Layout Engine (003-LAYOUT-ENGINE-001)
- Widget System (002-WIDGET-SYSTEM-001)
- Style System (005-STYLE-SYSTEM-001)

## Implementation Tasks

- MACRO-SOURCE-GENERATORS-001: Implement source generators for advanced scenarios
- MACRO-BUILDER-PATTERNS-001: Create fluent builder APIs
- MACRO-EXTENSION-METHODS-001: Implement convenience extension methods
- MACRO-CONSTRAINT-DSL-001: Create constraint definition DSL
- MACRO-TEXT-BUILDERS-001: Implement text convenience builders

## Acceptance Criteria

- [ ] Styled text creation is significantly more concise than explicit constructors
- [ ] Layout constraint definition supports symbolic notation
- [ ] Table row creation accepts heterogeneous content types
- [ ] All convenience APIs maintain type safety
- [ ] Performance is equivalent to explicit constructor calls
- [ ] IntelliSense and tooling support is excellent
- [ ] Documentation includes migration examples from verbose syntax

## Development and Build Tools

### Code Quality Integration
Based on analysis of Ratatui's clippy integration, CycoTui should include comprehensive development tooling:

**Linting and Analysis**:
- Static code analysis with consistent error handling
- Multiple package analysis with feature-specific validation
- Automated code fixes for common issues
- Integration with CI/CD pipelines for quality gates

**Build Tool Patterns**:
- CLI tools for development workflow (similar to xtask)
- Feature flag management for testing different configurations
- Platform-specific build validation
- Comprehensive error reporting with actionable feedback

### Development CLI
Provide development tools that mirror Ratatui's xtask functionality:

```csharp
// Example usage patterns
CycoTui.DevCli check --all-features     // Validate all feature combinations
CycoTui.DevCli lint --fix               // Run code analysis with auto-fixes
CycoTui.DevCli format                   // Apply consistent code formatting
CycoTui.DevCli test --backend Crossterm // Test specific backend
```

**Integration with Build System**:
- MSBuild targets for quality validation
- Pre-commit hooks for code quality
- Automated formatting and style enforcement
- Documentation generation and validation

## See Also

- SPEC-API-DESIGN-001: API design principles
- SPEC-TEXT-001: Text system specification
- SPEC-LAYOUT-004: Layout engine specification
- 006-TEXT-SYSTEM-001: Text system feature