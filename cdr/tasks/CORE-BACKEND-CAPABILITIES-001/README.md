# CORE-BACKEND-CAPABILITIES-001

Status: planned

## Overview
Implement a capability detection system for terminal backends (color depth, underline color, scrolling regions, mouse support, Unicode width reliability) and expose it via a `BackendCapabilities` struct or record.

## Goals
- Centralized capability probing per backend
- Single immutable snapshot used by higher layers (Style application, Widgets)
- Optional logging summary at debug level

## Scope
Detect: color levels (None, Ansi16, Ansi256, TrueColor), underline color support, mouse support, scrolling region support, size query support.

## Implementation Notes
- Add `BackendCapabilities Capabilities { get; }` to backend interface or wrapper
- Runtime detection strategies differ Windows vs Unix (e.g., check env vars like COLORTERM)
- Provide helper `SupportsTrueColor` boolean

## See Also
- SPEC-BACKEND-001
- IMPLEMENTATION-DECISIONS-001 (#6, #18)

## Acceptance Criteria
- Each backend populates `BackendCapabilities` on initialization
- Test backend returns full capability set configurable in constructor
- Style system queries underline color capability before emitting sequence

## Testing Approach
- Mock / test backend asserting passed capability snapshot to terminal
- Unit test toggling underline capability affects output

## Related Components
- ITerminalBackend
- Style emission logic
