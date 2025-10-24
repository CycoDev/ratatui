# BUFFER-DIFF-SEGMENTS-001

Status: planned

## Overview
Enhance buffer diffing to aggregate contiguous horizontal cell updates into diff segments for efficient backend rendering (reducing cursor movement & escape sequences).

## Goals
- Produce `IEnumerable<DiffSegment>` from two buffers
- Preserve ability to fall back to per-cell enumeration

## Scope
Segment definition: (Row, StartX, CellSpan) where CellSpan contains ordered cells. Only group when y constant and x strictly increments by 1.

## Implementation Notes
- After core cell diff loop, perform single pass grouping
- Avoid allocations by using ArrayPool for temporary segment buffers
- Provide extension method `.AsSegments()` or dedicated diff function

## See Also
- IMPLEMENTATION-DECISIONS-001 (#10 Diff Strategy)
- SPEC-BUFFER-002

## Acceptance Criteria
- For a line of N changed contiguous cells, emit exactly one segment
- Non-contiguous or skip-flagged cells start a new segment
- Unit tests cover: single cell, fully changed line, alternating changed/unchanged, multi-line changes

## Testing Approach
- Construct synthetic previous/current buffers triggering patterns
- Validate segment count and content integrity

## Performance Considerations
- O(n) additional pass; minimal allocations
