# Widget Reference Interface Implementation

## Overview

Implement the `IWidgetRef` interface and related patterns that enable reference-based widget rendering. This allows widgets to be rendered multiple times without consuming them and supports heterogeneous widget collections.

## Implementation Approach

1. **Define IWidgetRef Interface**:
   - Create interface with `RenderRef(Rect area, Buffer buffer)` method
   - Design as object-safe interface for collections
   - Mark as experimental feature initially

2. **Implement Extension Methods**:
   - Create extension methods for string rendering as widgets
   - Implement optional widget rendering pattern
   - Provide automatic IWidgetRef implementation for IWidget types

3. **Collection Support**:
   - Enable `List<IWidgetRef>` for heterogeneous widget collections
   - Ensure interface works with LINQ operations
   - Support both value and reference type widgets

## Key Challenges

- **API Stability**: Ratatui marks this as unstable - monitor for changes
- **Performance**: Ensure reference-based rendering doesn't add overhead
- **Type System**: Design interface to work well with C# generics and constraints

## Related Components

- `CycoTui.Widgets.IWidget` - Base widget interface
- `CycoTui.Core.Buffer` - Rendering target
- `CycoTui.Layout.Rect` - Area specification

## Integration Points

- Widget implementations should optionally support IWidgetRef
- Frame rendering extensions should support widget references
- Collection handling in parent widgets

## Testing Approach

- Test reference vs consuming widget patterns
- Verify collection scenarios work correctly
- Benchmark performance vs regular widget rendering
- Test string widget extensions

## Acceptance Criteria

- [ ] IWidgetRef interface defined and documented
- [ ] String extension methods implemented and tested
- [ ] Optional widget pattern working
- [ ] Collections of mixed widget types render correctly
- [ ] Performance parity with consuming widgets
- [ ] Comprehensive unit tests written
- [ ] API marked as experimental appropriately

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md) - Widget implementation specification
- [002-WIDGET-SYSTEM-001](../../features/002-WIDGET-SYSTEM-001.md) - Widget system feature
- Source analysis: `cdr/file-analyses/ratatui-src-widgets-widget_ref.md`