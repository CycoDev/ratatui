---
id: IMPLEMENTATION-DECISIONS-ADDENDUM-002
title: Phase 2 Completion Addendum
status: approved
date: 2025-10-23
---

# Phase 2 Completion Addendum

This document records enhancements applied to fully complete Phase 2 (Core) beyond the minimal baseline described in IMPLEMENTATION-DECISIONS-001.

## Enhancements
1. Underline Color Integration
   - StyleEmitter now emits SGR 58 (underline color) when backend capability present.
   - Fallback maps underline color to foreground color when capability absent (explicit degradation path).

2. Grapheme Transmission Support
   - CellData upgraded to store full grapheme strings rather than single char.
   - Terminal still emits only first UTF-16 unit for cell symbol in CellUpdate (backend refinement future task).

3. Buffer Reuse Optimization (Foundation)
   - Terminal uses two buffers; current and previous swapped and cleared (Buffer.Clear) instead of reallocating each frame.

4. Style Grouping Preparation
   - StyleState introduced; style transitions applied per segment run. Further grouping by identical style sequences planned.

5. Underline Color Emission Strategy
   - Capability flag: BackendCapabilities.SupportsUnderlineColor.
   - MapUnderlineToForeground flag used by Terminal when capability absent (temporary until user-configurable).

6. Rudimentary ZWJ Grapheme Merging
   - GraphemeEnumerator.EnumerateWithZwj merges sequences containing U+200D.
   - Family emoji and chained sequences treated as single grapheme (basic heuristic; not full spec compliance).

7. TestBackend Scrollback & Raw Sequences
   - Captures per-frame cell updates (Frames) and raw ANSI sequences (RawSequences).
   - Reset detection via ESC[0m; future classification of sequences planned.

8. Color Reset Sequences
   - StyleEmitter emits ESC[39m and ESC[49m when foreground/background removed.

## Deferred Items (Backlog)
- Full East Asian Width Table and complete emoji width coverage.
- Advanced grapheme ZWJ and skin tone sequence normalization.
- Style run coalescing across segment boundaries (current per segment).
- Backend direct multi-grapheme rendering (emit full grapheme per CellUpdate).
- Scroll region operations and scrollback persistence semantics.
- Underline color explicit reset sequence (many terminals lack support).

## Rationale
These enhancements reduce future refactoring costs for layout and widget phases by establishing stable style transition and grapheme handling semantics. Deferred items are primarily fidelity and performance optimizations that do not alter public API shapes.

## Next Steps
- Proceed to Phase 3 (Layout) constraints and algorithm implementation using stable buffer/style primitives.
- Schedule fidelity backlog for future optimization milestones.

---
Status: approved
