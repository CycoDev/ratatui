# TEXT-HSCROLL-TRUNCATOR-001

Status: planned

## Overview
Add horizontal truncation / scrolling component used by Paragraph and future scrollable widgets to render a logical line segment offset by a horizontal scroll value.

## Goals
- Support `HorizontalOffset(ushort)` API
- Integrate with grapheme width calculations and multi-width characters

## Scope
- Input: sequence of StyledGrapheme, max width, horizontal offset
- Output: truncated span of graphemes + effective displayed width

## Implementation Notes
- Skip graphemes until cumulative width >= offset, then start rendering
- Preserve grapheme atomicity; never cut inside cluster
- Provide fallback for offset beyond line length (empty output)

## See Also
- IMPLEMENTATION-DECISIONS-001 (#21)
- SPEC-TEXT-001

## Acceptance Criteria
- Offsets produce expected visible substrings across wide + combining graphemes
- Performance acceptable for repeated calls (O(n) per line)

## Testing Approach
- Test lines containing emoji, combining marks, CJK; various offsets
