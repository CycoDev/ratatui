# TERMINAL-STYLE-RESET-001

Status: planned

## Overview
Ensure each completed frame emission ends with a full ANSI style reset (foreground, background, modifiers) to keep terminal state consistent for subsequent shell or user output.

## Goals
- Append reset sequence only once per frame (if any draw occurred)
- Guarantee consistent baseline for next frame

## Scope
- Modify Terminal flush / draw sequence
- Provide configuration hook if future customization needed (not now)

## Implementation Notes
- Track whether any cell updates were emitted; if none, optionally skip reset
- Standard sequences: `ESC[39m ESC[49m ESC[0m` (order verified)

## See Also
- IMPLEMENTATION-DECISIONS-001 (#34 Style Reset)
- SPEC-TERMINAL-007

## Acceptance Criteria
- Frames with updates end with reset sequences
- Frames without updates do not duplicate resets unnecessarily
- Test verifying last bytes of backend output are reset codes

## Testing Approach
- Use TestBackend capturing raw write buffer
- Compare outputs for changed vs unchanged frame
