---
id: IMPLEMENTATION-DECISIONS-001
title: CycoTui Implementation Decisions
status: approved
date: 2025-10-23
---

# CycoTui Implementation Decisions

This document records the authoritative implementation choices derived from the Current Design Records (CDRs) and resolved ambiguity list. These serve as binding constraints for the first development phase and should be updated only through formal review (status changes or supersession).

## Change Control
- Modifications require review and status update (draft → review → approved).
- Superseded decisions must reference the new decision document ID.

## Summary Table
| # | Topic | Decision | Rationale |
|---|-------|----------|-----------|
| 1 | Layout Solver | Priority arithmetic (no solver) | Faster initial velocity; constraints are 1-D splits |
| 2 | Geometry Types | `int` for coords & sizes | Idiomatic .NET; allows intermediate negative math |
| 3 | Unicode Width | Internal implementation | Control accuracy; avoid dependency risk |
| 4 | Cell Symbol Storage | `string?` (null = space) | Simplicity; optimize later |
| 5 | Style Modifier Diff | Normalize intensity diff | Parity with Ratatui; minimal sequences |
| 6 | Underline Color | Capability-driven runtime | Stable API; graceful fallback |
| 7 | Experimental WidgetRef | Deferred | Reduce initial surface complexity |
| 8 | Builder Type Strategy | Small primitives = readonly struct; complex widgets = class | Performance + reduced copying |
| 9 | Error Modeling | Exceptions + TryX variants | Idiomatic; ergonomic |
| 10 | Diff Strategy | Provide grouped contiguous segments | Backend efficiency |
| 11 | Inline Viewport Overflow | Clamp height | Predictable; non-intrusive |
| 12 | MergeStrategy | Implement Replace + Preserve (Combine later) | Incremental complexity |
| 13 | Palette Packaging | Minimal in Core; advanced external package | Lean core footprint |
| 14 | External Color Converters | Optional extension packages | Avoid forced dependencies |
| 15 | Test Backend Scrollback | Configurable (default `ushort.MaxValue`) | Flexible memory use in tests |
| 16 | Logging Integration | `ILogger` abstractions | Consumer-controlled logging |
| 17 | Thread Safety | Not thread-safe; user synchronizes | Lower overhead; clarity |
| 18 | Feature Flags | Preprocessor + runtime toggles | Compile-time exclusion + dynamic behavior |
| 19 | Text Reflow Trim | Trim continuation lines by default | Cleaner wrapped output; configurable later |
| 20 | Masked Text | Simple (store original) | Deliver quickly; add secure variant later |
| 21 | Horizontal Scrolling API | `HorizontalOffset(ushort)` method | Explicit intent |
| 22 | Backend Clear API | `Clear(ClearType=All)` | Concise & flexible |
| 23 | Pixel Size | Expose; return (0,0) until supported | Future-proof interface |
| 24 | High-Res Canvas | Defer specialized abstraction | Focus core completeness |
| 25 | Animation Helper | User-managed loop only | Minimize scope |
| 26 | Async Input | Start blocking; add async stream later | Iterative evolution |
| 27 | README CI Behavior | Fail if outdated (manual update) | Explicit version control |
| 28 | #RRGGBBAA Parsing | Accept; ignore alpha | Flexible parsing; documented behavior |
| 29 | Ambiguous Width | `WidthMode` enum (Standard / EastAsian) | User configurability |
| 30 | Calendar Feature Flag | `FEATURE_CALENDAR` | Consistent naming |
| 31 | Backend Fallback Order | Preferred → Crossterm-like → Platform Native → Test (explicit only) | Predictable resolution |
| 32 | Optional Widget Pattern | Null allowed + `RenderIfNotNull` extension | Idiomatic C# |
| 33 | Buffer Resize Exposure | Internal only (Terminal handles) | Prevent misuse |
| 34 | Style Reset Post-Frame | Always emit full reset | Consistent terminal state |
| 35 | LineComposer Pooling | Defer pooling | Optimize after correctness |

## Detailed Decisions

### 1. Layout Solver
Implement a priority-based arithmetic layout engine without integrating a constraint solver. Provide internal extension points for future solver introduction.

### 2. Geometry Numeric Types
Use `int` for all geometry-related fields (Rect.X/Y/Width/Height, Position.X/Y). Validate non-negative public inputs; allow internal temporaries.

### 3. Unicode Width Implementation
Create an internal service (`IUnicodeWidthProvider`) with curated tables for: combining marks, zero-width, wide (East Asian), emoji sequences. Support mode switching via `WidthMode`.

### 4. Cell Symbol Storage
Use nullable `string` for cell symbol. Treat `null` as a single space for rendering and equality. Optimize later (e.g., small-string storage) if profiling warrants.

### 5. Style Modifier Diff Normalization
Implement intensity group normalization: reset intensity before applying bold/dim changes. Add common modifier diff rules to avoid redundant ANSI sequences.

### 6. Underline Color Capability
Expose `UnderlineColor` on `Style`. Backends advertise `SupportsUnderlineColor`. Fallback: use regular underline without color or map to foreground as needed.

### 7. Experimental WidgetRef Interfaces
Do not include `IWidgetRef` / `IStatefulWidgetRef<TState>` until the core widget model stabilizes. Document future addition path.

### 8. Builder Pattern Type Strategy
Use readonly structs for small immutable primitives (Style, Color, Position, Rect, Span, Line, Cell). Use classes for larger widgets (Block, Paragraph, List, Table, Tabs, Chart, Canvas) to prevent excessive copying in fluent chains.

### 9. Error Modeling
Primary methods throw exceptions. Provide `TryX` pattern (e.g., `TryInit(out Terminal term)` or `TryDraw`) returning `bool` or a small Result type where context matters.

### 10. Diff Strategy
Internally compute cell-level differences. Expose aggregated horizontal contiguous segments (`DiffSegment`) to backends for efficient output emission.

### 11. Inline Viewport Overflow Handling
If requested inline height exceeds available space, clamp down to remaining lines. Log diagnostic via `ILogger` (debug level). No scrolling or fallback to fullscreen.

### 12. MergeStrategy Semantics
Define strategies: Replace, Preserve (keep existing non-space), Combine (future). Implement Replace + Preserve now. Combine later with Unicode line-drawing merge logic.

### 13. Palette Packaging
Core ships ANSI + grayscale utilities only. Advanced palettes (Material, Tailwind, custom sets) reside in a separate extension package (e.g., `CycoTui.Style.Palettes`).

### 14. External Color Converters
Converters for System.Drawing, ImageSharp, SkiaSharp, etc., provided in optional extension packages to avoid mandatory dependencies.

### 15. Test Backend Scrollback
`TestBackend` constructor accepts optional scrollback limit parameter; default is `ushort.MaxValue`. Provide assertion helpers for scrollback content.

### 16. Logging Integration
Adopt `Microsoft.Extensions.Logging` abstractions in public surface. Accept `ILogger` injection or factory; default to a no-op logger if none provided.

### 17. Thread Safety
Document Terminal and Backend instances as not thread-safe. Responsibility rests with consumers to serialize access. Avoid internal locking overhead.

### 18. Feature Flag Mechanism
Combine compile-time symbols (`FEATURE_CALENDAR`, etc.) with runtime capability toggles (e.g., `BackendCapabilities`). Build excludes code when flag missing; runtime toggles refine behavior.

### 19. Text Reflow Trim Behavior
Default word-wrap trims leading whitespace on continuation lines. Provide configuration later to disable trimming or apply alternative policies.

### 20. Masked Text Security Model
`MaskedText` stores original string; mask only affects display. Future secure variant may use char[] with zeroization on disposal.

### 21. Horizontal Scrolling API Naming
Expose `HorizontalOffset(ushort offset)` on truncation / reflow components for horizontal scroll positioning.

### 22. Backend Clear API
Single method signature: `void Clear(ClearType type = ClearType.All)`. Eliminates multiple specialized methods while covering all cases.

### 23. Pixel Dimension Support
Expose `WindowSize` with `ColumnsRows` and `Pixels`. Return `(0,0)` for pixels unless backend can supply real values. Document limitation.

### 24. High-Resolution Canvas Abstraction
Defer specialized Braille/HalfBlock canvases until core Canvas widget stabilizes. Early complexity avoided.

### 25. Animation Helper API
No built-in scheduling or frame timing helpers initially. Users manage their own render loop with `Draw` calls.

### 26. Async Input Handling Evolution
Start with blocking `ReadEvents()` API. Plan upgrade to `IAsyncEnumerable<TerminalEvent>` or channel-based streaming after core stability.

### 27. README CI Behavior
CI validation fails if auto-generated READMEs are outdated. Developers update manually to maintain clear change history.

### 28. Hex Color Parsing (#RRGGBBAA)
Parse 8-digit hex; ignore alpha component. Document that translucency is unsupported. Accept both uppercase and lowercase.

### 29. Ambiguous Width Handling
Introduce `WidthMode` enum: `Standard` treats ambiguous as width 1; `EastAsian` treats ambiguous as width 2. Configurable globally.

### 30. Calendar Widget Feature Flag
Guard calendar implementation behind `FEATURE_CALENDAR`. Disabled by default for initial milestone.

### 31. Backend Fallback Order
Resolution sequence: Preferred backend → Crossterm-like generic backend → Platform-native backend (Windows/Unix) → Test backend only if explicitly requested (not automatic fallback). Ensures predictable behavior.

### 32. Optional Widget Pattern
Allow `null` for optional widgets. Provide extension `RenderIfNotNull(this IWidget? widget, Frame frame, Rect area)` or area-based variant. No wrapper type required.

### 33. Buffer Resize Exposure
Buffer resizing is internal, triggered by Terminal autoresize. No public resize method to avoid inconsistent external mutations.

### 34. Style Reset After Frame
Always emit a full reset (foreground, background, attributes) at end of frame to ensure terminal state consistency for subsequent output.

### 35. LineComposer Pooling
Initial implementation avoids pooling for simplicity. Profile later; if needed, introduce pooling with ArrayPool and object pooling utilities.

## Review & Supersession
Any future change requires:
1. New decision document with incremented ID and status lifecycle.
2. Cross-reference to this document’s ID (IMPLEMENTATION-DECISIONS-001).
3. Justification including performance or correctness motivation.

## Next Implementation Targets (Derived)
1. Core primitives (Rect, Position, Size, Margin, Padding, Color, Style).
2. Unicode width service with `WidthMode`.
3. Buffer + Cell + diff (with contiguous segment grouping).
4. ITerminalBackend + TestBackend + Capability model.
5. Terminal + Frame (Fullscreen viewport first; Inline clamping logic).
6. Text hierarchy (Span, Line, Text) + Reflow (trim default).
7. Logging integration and basic diagnostics.

---
>Status: approved. This document is the active reference for implementation decisions.
