## examples\concepts\state\src\bin\refcell.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: demonstrates interior-mutability/stateful-widget pattern to persist mutable widget state across renders.
- .NET equivalence: C# reference types (GC) provide shared references; mutable objects suffice—no Rc/RefCell pattern required.
- Primary .NET libraries to research:
  - Terminal.Gui (gui.cs) — full TUI framework: views, layout, event loop, mouse support.
  - Spectre.Console — high-level rendering, ANSI/colour support, live renderables.
  - System.Console — base API for low-level I/O.
- Buffer/diffing: inspect Terminal.Gui renderer and Spectre.Console LiveRenderable; you may need a custom virtual buffer + diff algorithm for performance.
- Layout system: Terminal.Gui has Views/Layout; Spectre.Console has Layout/Columns primitives.
- Event handling: Terminal.Gui event loop; fallback to ConsoleKeyInfo for raw input.
- Cross-platform backend: rely on Terminal.Gui/Spectre.Console which abstract Windows Console API vs Unix ANSI; otherwise P/Invoke for native APIs.
- Colors & encoding: use Spectre.Console for 256/truecolor; ensure System.Text.Encoding.UTF8 and System.Text.Rune for wide/UTF-8 handling.
- Mouse support: Terminal.Gui supports mouse; Spectre.Console is limited.
- Concurrency/safety: use lock, Interlocked, or concurrent collections for shared state across threads.
- Terminal capability detection: check TERM env, Console.IsOutputRedirected, and Windows version/ANSI support.
- Performance note: minimize terminal writes; prefer buffer-based rendering + diffing as in Ratatui.

