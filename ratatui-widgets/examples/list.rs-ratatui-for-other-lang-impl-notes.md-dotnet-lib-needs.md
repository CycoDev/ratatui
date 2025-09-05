## ratatui-widgets\examples\list.rs-ratatui-for-other-lang-impl-notes.md

- Target .NET subsystems to research: terminal control (raw mode, alternate screen), input events (keyboard/mouse), buffered rendering/diffing, Unicode width, color/style models, layout system.
- High-level .NET TUI libraries to evaluate first: Terminal.Gui (gui.cs) and Spectre.Console.
- Spectre.Console: rich styling, 256/RGB color, render primitives — good for mapping Ratatui styles.
- Terminal.Gui: complete TUI framework with layout, widgets, and event loop — good reference for layout/state.
- Low-level terminal control: Windows Console APIs / ConPTY (P/Invoke) for Windows; ncurses/terminfo via P/Invoke on Unix.
- Alternate screen, raw mode, cursor control, and mouse require either P/Invoke wrappers or using existing libs above.
- Unicode/wide-char handling: use System.Text.Rune/StringInfo plus search for .NET wcwidth / EastAsianWidth implementations.
- Buffer & diffing: likely implement in-library; check Spectre.Console internals for efficient rendering ideas.
- Backend abstraction: design an IBackend interface (size, draw buffer, cursor, styles, flush) with platform-specific implementations.
- Event handling: map Console.ReadKey to raw-mode input; for mouse/advanced keys use terminfo/ConPTY or high-level libs.
- Color mapping: System.Console limited — prefer Spectre.Console or custom mapping to 256/RGB terminals.
- Research existing .NET wrappers for ncurses/terminfo and ConPTY examples for robust cross-platform behavior.

