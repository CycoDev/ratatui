# TEXT-AMBIGUOUS-WIDTH-POLICY-001

Status: planned

## Overview
Implement `WidthMode` (Standard vs EastAsian) to determine width of ambiguous-width characters (e.g., Greek letters, box elements in some locales).

## Goals
- Global configuration accessible to layout and text subsystems
- Switch affects width calculations without requiring restart

## Scope
- Mode enumeration and central provider update
- Ambiguous set table integration (characters default width 1 or 2 depending on mode)

## Implementation Notes
- Provide `TextConfiguration.SetWidthMode(WidthMode mode)` that rebuilds or swaps ambiguous lookup
- Thread safety not guaranteed (document single-thread config before rendering loop)

## See Also
- IMPLEMENTATION-DECISIONS-001 (#29)
- SPEC-TEXT-001

## Acceptance Criteria
- Changing mode affects subsequent width computations
- Unit tests demonstrate difference for sample ambiguous characters

## Testing Approach
- Verify widths for set {·, Ω, ─} under both modes
