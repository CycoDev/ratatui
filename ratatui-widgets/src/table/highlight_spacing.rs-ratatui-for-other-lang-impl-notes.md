# Ratatui HighlightSpacing Implementation Notes

## Component Overview

`highlight_spacing.rs` implements the `HighlightSpacing` enum, which controls how space is allocated for selection indicators (highlight symbols) in UI components like tables and lists. This is a key component for handling selection visualizations consistently in terminal user interfaces.

## Core Functionality

The `HighlightSpacing` enum provides three options for managing the space allocated for selection indicators:

1. `Always` - Always reserve space for the selection indicator column, regardless of whether any item is currently selected.
2. `WhenSelected` (default) - Only allocate space for the selection indicator when an item is actually selected.
3. `Never` - Never allocate space for the selection indicator, effectively disabling the selection visualization.

The primary method is `should_add(has_selection: bool) -> bool`, which determines if the spacing should be added based on the enum variant and the current selection state.

## Dependencies and Integration

- Uses the [strum](https://github.com/Peternator7/strum) crate for deriving string conversion traits (`Display`, `EnumString`).
- Optional serialization/deserialization support via Serde (feature-gated with `feature = "serde"`).
- Used by UI components like `Table` and `List` to control selection indicator spacing.

## Cross-Platform Considerations

When implementing in another language:

1. **String Representation**: Implement string conversion functionality similar to Rust's `Display` and `FromStr` traits for configuration and serialization purposes.

2. **Terminal Width Handling**: Terminal character width calculation varies across platforms and font configurations. When adding or removing the highlight symbol column, the table/list width changes, which needs consistent handling.

3. **Default Values**: Match Rust's implementation by making `WhenSelected` the default, but consider documenting that `Always` provides a better user experience (as noted in the comments).

4. **Component Integration**: The spacing affects layout calculations in UI components. When a selection is made or cleared, components may need to recalculate their layout if using `WhenSelected`.

5. **Persistence**: Support for serialization/deserialization allows saving and loading user preferences.

## Usage Pattern

The typical usage pattern is:
1. UI components (Table, List) have a `highlight_spacing` property.
2. When rendering, they check `highlight_spacing.should_add(has_selection)` to determine if space should be allocated.
3. If true, they add the width of the highlight symbol to their layout calculations.
4. Users can configure this behavior using the `highlight_spacing()` builder method.

## Implementation Recommendations

1. Implement as an enum or equivalent in your target language.
2. Provide clear documentation about the behavior of each option.
3. Ensure the spacing calculation is consistent across all UI components that use selection indicators.
4. Consider the impact on layout stability and document the tradeoffs between the different options.