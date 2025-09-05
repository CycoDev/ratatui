# Table Highlight Spacing Implementation

## Overview

Implement the HighlightSpacing enum and associated logic for controlling how table widgets allocate space for selection highlight symbols. This affects table layout consistency and provides user control over whether the table shifts when selection changes.

## Implementation Approach

Create an enum-based configuration system that allows users to control table highlight symbol spacing behavior:

1. **Define HighlightSpacing enum** with three behavior options
2. **Implement decision logic** using efficient const/bool methods
3. **Integrate with table layout calculation** to adjust column spacing
4. **Support string serialization** for configuration persistence
5. **Document layout implications** clearly for each option

## Key Challenges

- **Layout Shifting Behavior**: The `WhenSelected` option causes table layout to shift when selection changes, which may be jarring to users
- **Default Choice**: Ratatui defaults to `WhenSelected` but `Always` provides better UX consistency
- **String Parsing**: Need robust string parsing without `strum` crate automation
- **Performance**: Decision logic is called frequently during rendering, must be efficient

## Related Components

- Table widget implementation
- TableState for tracking selection
- Layout constraint system for column width calculation
- Style system for highlight styling

## Integration Points

- Table rendering pipeline for column width calculation
- Selection highlight symbol rendering logic
- Table configuration API for fluent builder pattern
- Serialization systems for configuration persistence

## Acceptance Criteria

- [ ] HighlightSpacing enum defined with three options (Always, WhenSelected, Never)
- [ ] Each enum option includes comprehensive XML documentation explaining behavior
- [ ] ShouldAdd(bool hasSelection) method provides efficient decision logic
- [ ] ToString() override and Parse() method support string serialization
- [ ] Table widget integrates spacing decision into layout calculation
- [ ] Fluent API supports highlight spacing configuration
- [ ] Unit tests verify all spacing behaviors work correctly
- [ ] Documentation includes layout implications and recommendations
- [ ] Performance testing shows minimal overhead for decision logic

## Implementation Notes

Consider defaulting to `Always` in C# version for better UX consistency, even though Ratatui defaults to `WhenSelected`. The shifting layout behavior of `WhenSelected` may be less acceptable in .NET desktop environments compared to terminal applications.

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [WIDGET-TABLE-001](../WIDGET-TABLE-001/README.md): Main table widget implementation task