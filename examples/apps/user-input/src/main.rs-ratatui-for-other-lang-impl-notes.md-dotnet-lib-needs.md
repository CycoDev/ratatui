## examples\apps\user-input\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Goal: port Ratatui rendering/layout/widget model to .NET; separate rendering from input/backends.
- Research Spectre.Console (ANSI rendering, colors, tables, layouts) as a primary rendering helper.
- Research Terminal.Gui (gui.cs) for full-screen TUI widgets, input, mouse, resize events.
- Use System.Console for simple apps; consider P/Invoke for advanced control.
- For Windows raw mode & VT: P/Invoke SetConsoleMode/ENABLE_VIRTUAL_TERMINAL_PROCESSING.
- For Unix raw mode: P/Invoke termios (tcgetattr/tcsetattr) or use libterm libraries.
- Unicode handling: use System.Text.Rune and proper grapheme/width measurement libraries.
- Double-buffering/cell buffer: likely custom implementation; check Spectre.Console internals for diffing strategies.
- Alternate screen, cursor, styling: implement via ANSI VT sequences or use Spectre.Console helpers.
- Mouse input & advanced events: Terminal.Gui supports this; otherwise enable terminal mouse reporting and parse sequences.
- Layout system: build composable layout (flex-like) or adapt Spectre.Console layout primitives.
- Design a Backend abstraction interface to encapsulate Console/Spectre/terminal specifics for portability.

