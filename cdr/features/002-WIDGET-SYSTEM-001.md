---
id: 002-WIDGET-SYSTEM-001
title: Widget System
status: draft
priority: high
date: 2023-11-28
---

# Widget System

## Overview

The widget system provides the core abstractions for creating and rendering UI elements in CycoTui. It defines fundamental interface types: stateless widgets (`IWidget`), stateful widgets (`IStatefulWidget<TState>`), and experimental reference-based widgets (`IWidgetRef`). This system enables polymorphic widget usage, supports both consuming and non-consuming rendering patterns, and maintains clear separation between widgets that manage state and those that don't.

## User Stories

- As a developer, I want to create custom widgets that can render to a buffer
- As a developer, I want to use both stateless and stateful widgets seamlessly
- As a developer, I want widgets to be composable so I can build complex UIs
- As a developer, I want clear interfaces that follow C# conventions
- As a developer, I want to render widgets multiple times without consuming them
- As a developer, I want to store collections of different widget types
- As a developer, I want to render strings directly as simple widgets
- As a developer, I want comprehensive documentation for widget development
- As a library maintainer, I want a clean separation between widget types
- As a library maintainer, I want the widget system to be extensible

## Core Requirements

## Core Requirements

### Widget Interface
- Support for stateless widgets through `IWidget` interface
- Support for stateful widgets through `IStatefulWidget<TState>` interface
- Reference-based widget rendering through `IWidgetRef` interface
- Clear rendering contracts with area and buffer parameters
- Polymorphic widget collections and usage patterns

### Border Configuration
- Bitflag-based border selection system (`Borders` enum)
- Multiple border styles through `BorderType` enumeration
- Support for combining border sides (top, right, bottom, left)
- Integration with symbol system for Unicode border characters
- Convenient syntax for specifying border combinations

### Widget Composition
- Hierarchical widget nesting and composition
- Container widgets that manage child widget rendering
- Area subdivision and clipping for child widgets
- Layout integration for proper widget positioning

### Widget Rendering Patterns
- **Delegation Pattern**: Stateless widgets delegate to stateful implementations with default state
- **Reference Rendering**: Support both value-type and reference-type widget rendering
- **Viewport Calculation**: Efficient algorithms for determining visible content within area bounds
- **State Synchronization**: Automatic state updates during rendering to maintain consistency
- **Performance Optimization**: Early termination and minimal allocation patterns for frequent re-renders
- **Error Resilience**: Graceful handling of edge cases (empty areas, out-of-bounds selections, overflow)

### Standard Widgets
- Block widget with border configuration and styling
- Text rendering widgets (Paragraph, Line, Span)
- Interactive widgets with state management
- Data visualization widgets (charts, gauges, tables)
- Table row widgets with flexible cell organization and styling
- Layout and container widgets
- Define `IWidget` interface for stateless widgets based on Ratatui's Widget trait pattern
- Define `IStatefulWidget<TState>` interface for widgets with state
- Support polymorphic widget usage through common base interface
- Provide clear contracts for rendering behavior
- Support immediate-mode rendering pattern (widgets consumed during rendering)
- Define experimental `IWidgetRef` and `IStatefulWidgetRef` interfaces for reference-based rendering
- Enable widget reusability and heterogeneous collections through reference interfaces
- Support both consuming and non-consuming widget patterns

### Standard Widgets
- Provide foundational container widgets that establish visual structure
- **Block Widget**: Core container widget providing borders, titles, and padding
  - Support multiple border types (plain, rounded, double, thick, custom)
  - Enable multiple titles with flexible positioning (top/bottom) and alignment
  - Provide configurable padding for internal spacing using Padding value type
    - Support uniform, horizontal, vertical, and proportional padding patterns
    - Proportional padding accounts for terminal character aspect ratio (2:1 horizontal:vertical)
    - Enable individual control of left, right, top, bottom padding values
  - Calculate inner content area automatically accounting for borders and padding
  - Support border merging strategies for seamless layouts
  - Layer styles appropriately (base style, border style, title style)
- **Paragraph Widget**: Text rendering with word wrapping and alignment
- **Clear Widget**: Utility widget for clearing areas (enables popup overlays)
- Support widget composition patterns where widgets accept optional Block parameters
- Provide consistent styling and configuration APIs across all widgets

### Widget Hierarchy and Organization
- Organize widgets by functional categories (Basic, Interactive, Visualization, Special)
- Support feature flagging for optional widgets (e.g., Calendar widget)
- Maintain namespace organization for discoverability
- Provide comprehensive widget documentation and examples

### String Widget Support
- Built-in widget rendering capability for string types
- Extension methods or implicit conversions for string → widget
- Automatic text truncation when content exceeds available area
- Default styling applied to string content

### Optional Widget Pattern
- Support for conditional widget rendering
- `OptionalWidget<T>` wrapper or nullable widget pattern
- Graceful handling of null/empty widgets (render nothing)
- Zero-cost abstraction for conditional UI elements

### Standard Widgets
- Implement core widget types based on Ratatui's widget catalog:
  - **BarChart**: displays multiple datasets as bars with optional grouping, supports both vertical and horizontal orientations, tick-based sub-cell precision rendering
  - **Block**: basic widget with borders, titles, and styles
  - **Calendar**: displays calendar month (feature-flagged)
  - **Canvas**: draws arbitrary shapes using drawing characters
  - **Chart**: displays datasets as lines or scatter graphs
  - **Clear**: clears areas for rendering over previous widgets
  - **Gauge** and **LineGauge**: display progress using block/line characters
  - **List**: displays selectable items
  - **Logo** and **Mascot**: display Ratatui branding elements (adapt for CycoTui)
  - **Paragraph**: displays styled and wrapped text
  - **Scrollbar**: displays scrollbar indicators with support for vertical/horizontal orientations, customizable symbols and styling, proportional thumb positioning, and integrated state management
  - **Sparkline**: displays single dataset as sparkline
  - **Table**: displays rows and columns with selection, headers, footers, multi-level highlighting (row/column/cell), flexible column width constraints, scrolling support, and fluent builder API
  - **Tabs**: displays horizontal tab bar with single tab selection, customizable dividers and padding, Unicode-aware width calculation, optional block wrapper, style hierarchy (overall/highlight/individual), selection bounds checking
- Advanced widget features from BarChart analysis:
  - Support for complex data visualization with multiple data series
  - Fluent builder pattern APIs for widget configuration
  - Smart space allocation algorithms for optimal rendering
  - Unicode symbol-based rendering for graphical elements
  - Sophisticated label management with overflow handling
  - Direction-aware rendering (vertical/horizontal layouts)
- Organize widgets into logical namespace hierarchy
- Ensure all widgets follow the same interface patterns
- Support both immediate-mode and retained-mode patterns
- Consider modular organization for optional widget sets

### Widget Composition
- Enable nesting widgets within other widgets
- Support widget hierarchies and parent-child relationships
- Allow widgets to render other widgets as part of their content

### Widget State
- Separate state management from widget instances for clear separation of concerns
- Support generic state types for compile-time type safety (`IStatefulWidget<TState>`)
- State passed by reference to avoid unnecessary copying and enable in-place updates
- Widget instances remain immutable during rendering while state can be modified
- Support for both sized and unsized state types where possible
- State persists between render cycles while widgets can be recreated each frame
- Enable complex state management patterns like scroll offsets and selection tracking

### Rendering Mechanism
- All widgets render to a buffer within a specified area
- Support efficient partial rendering and updates
- Enable widgets to query and modify buffer content

## Technical Strategy

- Use C# interfaces to define widget contracts (equivalent to Rust traits)
- Organize widgets in a clear namespace hierarchy (`CycoTui.Widgets`)
- Implement facade pattern for clean public API
- Separate interface definitions from implementations
- Use generic interfaces for type-safe state management

## Dependencies

- Buffer system (for rendering target)
- Layout system (for area calculations)
- Style system (for visual appearance)

## Implementation Tasks

- [`WIDGET-CLEAR-001`](../tasks/WIDGET-CLEAR-001/README.md): Implement Clear utility widget
- [`WIDGET-BASE-001`](../tasks/WIDGET-BASE-001/README.md): Create core widget interfaces
- [`WIDGET-BLOCK-001`](../tasks/WIDGET-BLOCK-001/README.md): Implement Block container widget
- [`WIDGET-LIBRARY-ORGANIZATION-001`](../tasks/WIDGET-LIBRARY-ORGANIZATION-001/README.md): Implement widget library organization structure
- [`WIDGET-STATEFUL-INTERFACE-001`](../tasks/WIDGET-STATEFUL-INTERFACE-001/README.md): Implement IStatefulWidgetRef interface
- [`WIDGET-STRING-001`](../tasks/WIDGET-STRING-001/README.md): Implement string widget support
- [`WIDGET-OPTIONAL-001`](../tasks/WIDGET-OPTIONAL-001/README.md): Implement optional widget pattern
- Create documentation standards for widgets
- Implement widget testing framework
- Implement automatic StatefulWidgetRef support for existing StatefulWidget implementations
- Design approach for handling unsized state types in C#

## Acceptance Criteria

- [ ] `IWidget` interface is defined with clear rendering contract
- [ ] `IStatefulWidget<TState>` interface supports generic state types
- [ ] Widget namespace organization follows C# conventions
- [ ] All public interfaces have comprehensive XML documentation
- [ ] Widget system supports composition and nesting
- [ ] Performance meets requirements for interactive applications

## See Also

- [SPEC-WIDGET-003](../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [001-BUFFER-MODEL-001](001-BUFFER-MODEL-001.md): Buffer system requirements
- [003-LAYOUT-ENGINE-001](003-LAYOUT-ENGINE-001.md): Layout system integration