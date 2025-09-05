## xtask\src\commands\docs.rs-ratatui-for-other-lang-impl-notes.md

- Goal: port Ratatui split: platform-independent core + platform-specific backends + widgets.
- Research .NET terminal/backends for: raw buffer, terminal size, cursor, colors, styles, mouse/keys.
- Candidates for rendering & styling: Spectre.Console (rich text, colors, panels, tables).
- Candidates for widget systems & composable UI: Terminal.Gui (TUI widgets, layout, input, mouse).
- Low-level/advanced features: P/Invoke to Windows Console API / ConPTY; enable VT sequences on Windows.
- NCurses bindings: look for C# ncurses wrappers if targeting Unix C API behavior.
- Async input & event loop: investigate Terminal.Gui event model or implement custom async ReadKey with cancellation.
- Layout engine: implement Flexbox-like system or reuse Spectre.Console renderables; plan separate core library.
- Testing & docs: xUnit/NUnit for tests, DocFX for docs, snapshot/visual regression via terminal output diffs.
- Workspace mapping: separate projects/namespaces for core, widgets, backends, and helper macros/tools.

