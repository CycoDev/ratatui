# SETUP-LOGGING-INTEGRATION-001

Status: planned

## Overview
Implement initial logging infrastructure for CycoTui using Microsoft.Extensions.Logging abstractions. Provide a consistent way for core, backend, and future widget layers to emit diagnostic information without hard‑wiring to Console output.

## Goals
- Introduce `ILogger` usage patterns (dependency injection style or factory)
- Provide a default no-op logger when none supplied
- Define logging categories (Core/Backend/Rendering/Layout/Widget/Input)
- Ensure early tasks can record diagnostics (e.g., clamped inline viewport height)

## Scope
In scope:
- Logger acquisition pattern (factory delegate or static LogManager)
- Minimal extension helpers (e.g., `.LogCapabilitySummary()`)
- Integration points in backend creation and terminal initialization
Out of scope:
- Structured event tracing (future performance phase)
- External log provider configuration (left to host application)

## Implementation Notes
- Add an internal `ILoggingContext` wrapper that holds an `ILoggerFactory` and exposes typed loggers
- Terminal<TBackend> accepts optional `ILoggerFactory`; fall back to `NullLoggerFactory.Instance`
- Avoid logging in tight per-cell loops (diff application); only coarse events
- Use `LoggerMessage.Define` for hot path structured messages if necessary later

## Key Challenges
- Avoid premature allocation in performance paths
- Keep logging optional to not burden minimal consumers

## See Also
- IMPLEMENTATION-DECISIONS-001 (#16 Logging Integration)
- SPEC-BACKEND-001 (capability detection points)
- SPEC-TERMINAL-007 (frame lifecycle events)

## Acceptance Criteria
- Terminal can be constructed with and without a logger factory
- Backends log a single capability summary at initialization (debug level)
- No direct `Console.WriteLine` calls added in core or backend code after this task
- Unit test verifies no-op fallback does not throw

## Testing Approach
- Inject a custom test logger capturing messages; assert expected categories
- Ensure absence of logger does not produce output or exceptions

## Related Components
- Terminal initialization path
- Backend factory / selection logic

## Performance Considerations
- Ensure zero allocations when logger is disabled / debug not enabled
