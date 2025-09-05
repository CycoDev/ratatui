## ratatui-core\src\layout\rect.rs-ratatui-for-other-lang-impl-notes.md

- Rect = { x,y,width,height } using u16 → in .NET use ushort (System.UInt16) or int + validation.
- Origin: top-left (x rightwards, y downwards).
- Essential operations to implement: area, edges (left/right/top/bottom), intersection, union, contains(point).
- Transformations: apply margins (inner/outer), offset, clamp to another rect.
- Layout ops: split into sub-rects by constraints, center H/V, iterate rows/cols/cells.
- Iteration: expose IEnumerable for rows/cols/cells for efficient traversal.
- Immutability: return new Rects (use readonly struct/record) for composability and safety.
- Saturating arithmetic: implement via Math.Min/Max to avoid overflow (no built-in saturating ops).
- Performance: avoid allocations, consider inline/hot paths and optional caching.
- Constraint solving: Rust uses kasuari — in .NET research Facebook Yoga (YogaSharp), Flexbox engines, or implement a simple constraint solver yourself.
- Terminal backends to research in .NET: Spectre.Console, Terminal.Gui (gui-like TUI), SadConsole, System.Console, and ncurses wrappers/PInvoke.
- Backend abstraction: design an IBackend interface separating layout from terminal rendering (like Ratatui).
- Data type tradeoff: ushort matches terminals but limits max; int gives flexibility with extra checks.
- Minimal deps: core layout can be self-contained; only bring in layout/constraint and chosen backend libs.
- File is focused on geometry/layout concerns and mainly points to backend and constraint-solver libraries you should research for a .NET port.

