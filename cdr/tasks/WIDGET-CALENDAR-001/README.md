# Calendar Widget Implementation

## Overview

Implement the Calendar widget (Monthly) for CycoTui, providing monthly calendar display functionality with customizable date styling. This widget supports optional headers, surrounding month days, and flexible event highlighting through the DateStyler pattern.

## Implementation Approach

### Core Components
1. **Monthly<TDateStyler> Widget**: Main calendar widget with generic date styler
2. **IDateStyler Interface**: Strategy pattern for date-specific styling
3. **CalendarEventStore**: Built-in HashMap-based date styler implementation
4. **Date Handling**: Integration with .NET date/time APIs

### Key Implementation Details

#### Monthly Widget Structure
```csharp
public struct Monthly<TDateStyler> : IWidget 
    where TDateStyler : IDateStyler
{
    private readonly DateOnly _displayDate;
    private readonly TDateStyler _events;
    private readonly Style? _showSurrounding;
    private readonly Style? _showWeekday; 
    private readonly Style? _showMonth;
    private readonly Style _defaultStyle;
    private readonly Block? _block;
}
```

#### Builder Pattern Implementation
- Fluent API methods: `ShowSurrounding()`, `ShowWeekdaysHeader()`, `ShowMonthHeader()`, `DefaultStyle()`, `Block()`
- Immutable builder pattern using record/struct with init properties
- Generic style parameter support: `<S>(S style) where S : IConvertible<Style>`

#### Calendar Layout Algorithm
1. Calculate first day of display month
2. Find preceding Sunday using DayOfWeek arithmetic
3. Generate 6-week grid (42 days maximum) or until next month
4. Apply hierarchical styling: default → surrounding → event-specific

### Integration Points

#### Date/Time Library Choice
- **Primary**: Use `DateOnly` (.NET 6+) for date handling
- **Fallback**: Use `DateTime.Date` for earlier .NET versions
- **Duration**: Use `TimeSpan` for date arithmetic instead of Rust Duration
- **Week Calculation**: Use `DayOfWeek` enumeration for weekday calculations

#### Layout System Integration
- Three-section vertical layout using existing constraint system
- Header sections use `Constraint.Length(1)` when enabled
- Calendar grid uses `Constraint.Fill(1)` for remaining space
- Proper area subdivision and clipping support

#### Style System Integration
- Style composition using existing `Style.Patch()` method
- Support for all style conversion patterns (`IConvertible<Style>`)
- Background-only styles for empty cells using `Style.Background()` 
- Hierarchical style precedence: default < surrounding < event

### Key Challenges

#### Date Arithmetic Complexity
- **Challenge**: Efficiently calculate calendar start date (preceding Sunday)
- **Solution**: Use `DayOfWeek` enumeration and modular arithmetic
- **Implementation**: `var offset = (7 - (int)firstDay.DayOfWeek) % 7;`

#### Unicode Date Formatting
- **Challenge**: Ensure proper column alignment for date display
- **Solution**: Use fixed-width format strings and Unicode-aware width calculation
- **Implementation**: `string.Format("{0,2}", date.Day)` with style application

#### Flexible DateStyler Pattern
- **Challenge**: Support both simple and complex date styling scenarios
- **Solution**: Interface-based design with multiple implementations
- **Implementation**: Generic constraint with built-in CalendarEventStore

#### Performance Optimization
- **Challenge**: Efficient rendering for calendar grids
- **Solution**: Pre-calculate date ranges, cache formatted strings, minimize allocations
- **Implementation**: Use `Span<char>` for date formatting, reuse style objects

### Related Components

#### Dependencies
- Buffer system for cell-based rendering
- Layout constraint system for area subdivision  
- Style system for appearance management
- Text/Line components for header rendering

#### Integration Files
- `ratatui-core/src/layout/constraint.rs` - Layout constraint patterns
- `ratatui-core/src/style/style.rs` - Style composition and patching
- `ratatui-core/src/text/line.rs` - Header text rendering
- `ratatui-widgets/src/block.rs` - Container block functionality

### Testing Approach

#### Unit Test Categories
1. **Date Calculation Tests**: Verify calendar start date calculation for various months
2. **Layout Tests**: Confirm proper area subdivision and constraint handling
3. **Style Application Tests**: Test hierarchical style composition and precedence
4. **Edge Case Tests**: Minimal buffer sizes, empty DateStyler, leap years
5. **Feature Flag Tests**: Conditional compilation behavior

#### Test Data Sets
- Various months with different starting weekdays
- Leap years and month boundary conditions
- Different buffer sizes including edge cases (1x1, 0x0)
- Multiple event dates with overlapping styles

#### Integration Tests  
- Full calendar rendering with all features enabled
- Block integration and inner area calculation
- Style system integration and composition
- Performance tests with large numbers of events

## Acceptance Criteria

- [ ] Monthly widget implements IWidget interface correctly
- [ ] IDateStyler interface supports flexible date styling patterns
- [ ] CalendarEventStore provides efficient HashMap-based styling
- [ ] Builder pattern supports fluent configuration API
- [ ] Layout properly handles optional headers and calendar grid
- [ ] Style composition works hierarchically (default → surrounding → events)
- [ ] Date arithmetic correctly calculates calendar boundaries
- [ ] Unicode formatting produces properly aligned date display
- [ ] Widget gracefully handles insufficient buffer space
- [ ] Feature flag integration allows conditional compilation
- [ ] Unit tests cover all major functionality and edge cases
- [ ] Performance meets expectations for typical calendar sizes
- [ ] Documentation includes usage examples and API reference

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [002-WIDGET-SYSTEM-001](../../features/002-WIDGET-SYSTEM-001.md): Widget system feature requirements
- [SPEC-LAYOUT-004](../../specs/SPEC-LAYOUT-004.md): Layout constraint system
- [SPEC-STYLE-005](../../specs/SPEC-STYLE-005.md): Style system specification