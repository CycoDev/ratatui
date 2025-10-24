# INPUT-EVENTS-001

Status: planned

## Overview
(Placeholder was not originally created—ensure base input event type definitions exist.) Define foundational input event data structures (keyboard, mouse, resize, focus) consumed by higher-level application loop.

## Goals
- Unified `TerminalEvent` discriminated union (enum + structs or record hierarchy)
- Normalized modifier representation

## Scope
- Event data contracts only; no OS polling

## Implementation Notes
- Consider using structs for high-frequency events (KeyEvent, MouseEvent)
- Provide modifier flags enum (Ctrl, Alt, Shift)

## See Also
- SPEC-BACKEND-001 (Event Handling section)
- INPUT-ASYNC-PIPELINE-PLANNING-001

## Acceptance Criteria
- Event structures compile and are referenced by subsequent input tasks
- Unit tests for modifier combination integrity

## Testing Approach
- Simple construction and serialization tests
