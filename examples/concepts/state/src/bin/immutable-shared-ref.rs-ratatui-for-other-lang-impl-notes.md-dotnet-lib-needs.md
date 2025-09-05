## examples\concepts\state\src\bin\immutable-shared-ref.rs-ratatui-for-other-lang-impl-notes.md

- Architecture: separate core (widgets, buffer) from backends (terminal I/O).
- Backend abstraction required: Windows Console API, ANSI terminals, or ncurses-like backends.
- Candidate .NET libraries to research: Terminal.Gui (gui.cs) for full TUI widgets; Spectre.Console for rich rendering/live updates; .NET ncurses bindings or direct P/Invoke to Windows Console for low-level control.
- Must support raw mode and alternate screen entry/restore.
- Non-blocking input and unified, cross-platform key/mouse event model.
- In-memory cell buffer with double-buffering and diff/partial updates.
- Widget pattern: immutable/stateless render method that takes a read-only state reference.
- Unicode/wide-grapheme support and correct cell-width calculations.
- Terminal capability and size detection across platforms.
- Batch writes to minimize I/O; expose APIs to write only changed cells.
- Ability to render to a buffer for unit testing (headless rendering).
- Licensing check: ensure library license fits your project.

If you want, I can fetch links and quick pros/cons for Terminal.Gui and Spectre.Console.

