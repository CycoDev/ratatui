# TEXT-GRAPHEME-SEGMENTATION-001

Status: planned

## Overview
Implement Unicode grapheme cluster enumeration compliant with Unicode Standard Annex #29 to support accurate rendering, wrapping, truncation, and width calculation.

## Goals
- Provide `IGraphemeEnumerator` or static helper returning grapheme slices
- Handle ZWJ sequences, combining marks, emoji modifiers, variation selectors

## Scope
- Basic segmentation with .NET `StringInfo` baseline plus enhancements for ZWJ sequences
- Exclude custom line breaking logic (handled separately in wrapping)

## Implementation Notes
- Start with `StringInfo.GetTextElementEnumerator`
- Post-process for extended emoji sequences (skin tones, family ZWJ chains)
- Provide fast path for ASCII

## See Also
- SPEC-TEXT-001
- IMPLEMENTATION-DECISIONS-001 (#3 Unicode width infra prerequisite)

## Acceptance Criteria
- Returns identical boundaries for a curated test matrix vs reference expectations
- Correctly groups family emoji sequences as single grapheme

## Testing Approach
- Test dataset including: accented chars, emojis with modifiers, flags, ZWJ sequences
- Snapshot tests verifying enumerated graphemes
