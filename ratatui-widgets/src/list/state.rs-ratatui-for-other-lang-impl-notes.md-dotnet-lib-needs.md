## ratatui-widgets\src\list\state.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: stores list UI state only — offset (first visible index) and selected (optional index).
- Fields to implement: offset (integer) and selected (nullable integer, e.g. int?).
- Index type: Rust usize varies by platform; in .NET prefer int (System.Int32) for indexes unless you need >2^31 items (then use long).
- Navigation API to provide: select_first, select_last, select(index), select_next, select_previous, clear_selection.
- Scrolling API to provide: scroll_up_by(n), scroll_down_by(n) with saturating behavior at bounds.
- Safe arithmetic: use Math.Clamp / Math.Min / Math.Max or checked/guarded operations to avoid overflow/underflow.
- State semantics: lightweight value (two integers) — can be a struct, but prefer a small mutable class to avoid mutable struct pitfalls.
- Trait/behavior mapping: implement ToString, Equals/GetHashCode, copy constructor or ICloneable, and a parameterless default ctor.
- Serialization: optional — use System.Text.Json or Newtonsoft.Json for feature-gated (de)serialization.
- Separation of concerns: keep state independent of rendering; state stored/updated by app logic, renderer reads it.
- Integration pattern: implement a StatefulWidget-like interface — pass state into render methods; renderer uses offset to compute visible slice.
- Event handling: use .NET events, delegates, or reactive libraries (Reactive Extensions / ReactiveUI) to propagate UI actions to state.
- Threading: if updates may come from other threads, add synchronization (lock or Interlocked) around state changes.
- Terminal UI libraries to research in .NET: Terminal.Gui (gui.cs) for full TUI widgets; Spectre.Console for rich console rendering; consider ncurses bindings only if needed.
- Other notes: no platform-specific dependencies in the Rust code, so porting state logic itself requires no special native libraries.

