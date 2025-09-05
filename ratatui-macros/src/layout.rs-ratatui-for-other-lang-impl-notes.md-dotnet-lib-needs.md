## ratatui-macros\src\layout.rs-ratatui-for-other-lang-impl-notes.md

- Core goal: declarative layout constraints (fixed, percent, min, max, ratio, fill).
- Research .NET constraint-solver libraries (Cassowary/Kiwi/Cassowary.NET) for ratio/min/max/flexible sizing.
- Implement Constraint and Layout primitives: Constraint types, Layout engine for horizontal/vertical and nested layouts.
- Terminal size: research cross-platform terminal size/resize APIs (System.Console, Windows Console API, libuv/terminfo wrappers).
- ANSI/VT100 and Windows 10+ VT support libraries (Spectre.Console, Terminal.Gui) for rendering and control codes.
- Unicode width (CJK) libs/algorithms for .NET (East Asian Width, Rune-based width calculators).
- Rendering differences: research Windows Console vs Unix terminal behavior and fonts.
- Caching strategy: layout cache to avoid recomputation for complex UIs.
- DSL/builder approach: use fluent builders or expression DSLs to replace Rust macros.
- Performance: consider allocation-free patterns (Span, structs) for many nested layouts.
- Map ratatui_core::layout::Constraint/Layout responsibilities to .NET types/assemblies you create or reuse.
- Investigate existing .NET terminal UI libs (Spectre.Console, Terminal.Gui) for examples and interoperability.

