# INPUT-ASYNC-PIPELINE-PLANNING-001

Status: planned

## Overview
Design the evolution path from blocking input retrieval to an async streaming model (e.g., `IAsyncEnumerable<TerminalEvent>` or channels) without breaking existing synchronous APIs.

## Goals
- Define abstraction boundary for event source
- Identify backpressure & cancellation strategy
- Draft migration guidelines for consumers

## Scope
- Architectural design doc only (no implementation yet)

## Implementation Notes
- Consider: dedicated background thread feeding Channel<TerminalEvent>
- Cancellation via `CancellationToken`
- Preserve `ReadEvents()` as thin wrapper consuming from channel in blocking mode

## See Also
- IMPLEMENTATION-DECISIONS-001 (#26)
- SPEC-BACKEND-001 (input portion)

## Acceptance Criteria
- Document enumerates chosen async pattern, lifecycle, disposal model
- Risks and open questions captured

## Testing Approach
- N/A (planning only)
