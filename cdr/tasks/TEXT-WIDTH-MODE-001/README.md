# TEXT-WIDTH-MODE-001

Status: planned

## Overview
Integrate `WidthMode` configuration with layout measurement, truncation, and wrapping so ambiguous characters reflect selected width policy throughout rendering.

## Goals
- Propagate width provider to all measurement call sites (Tables, Paragraph, Layout constraints)
- Avoid recomputation explosion (cache ambiguous results if needed)

## Scope
- Adapt existing width utility to accept mode implicitly (singleton provider)
- Ensure diff invalidation handles mode changes (document requirement to re-render full frame after mode switch)

## Implementation Notes
- Provide `WidthService.Current` with active provider
- Document that switching width mode mid-frame is undefined

## See Also
- TEXT-AMBIGUOUS-WIDTH-POLICY-001
- SPEC-TEXT-001

## Acceptance Criteria
- Widgets reflect changed widths after mode change and redraw
- Tests covering table column width and paragraph wrapping under both modes

## Testing Approach
- Snapshot or string layout comparisons before/after mode change
