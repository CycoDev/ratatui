## ratatui\tests\widgets_barchart.rs-ratatui-for-other-lang-impl-notes.md

- Key .NET libs to research: Spectre.Console (advanced ANSI rendering, colors), Terminal.Gui (gui.cs, higher-level TUI), ncurses bindings (NcursesSharp/ManagedNcurses) and native APIs via P/Invoke (Win32 Console, termios).
- Implement a Backend abstraction (draw, cursor, clear, size) so you can swap Spectre.Console/Terminal.Gui/native backends.
- Buffer-based rendering: use an in-memory grid of cells (char + style) for widgets and a TestBackend for unit tests.
- ANSI/VT support: enable Windows VT processing (SetConsoleMode ENABLE_VIRTUAL_TERMINAL_PROCESSING) and rely on ANSI on Unix.
- Color/style support: verify 16/256/truecolor support in chosen lib (Spectre.Console supports 256/truecolor).
- Cursor control, alternate screen, and clear: available via ANSI or Win32 APIs; confirm support in candidate libraries.
- Input/raw mode: Research Console.ReadKey limitations; use P/Invoke termios (Unix) or SetConsoleMode (Windows) for raw byte input if needed; Terminal.Gui handles input for you.
- Terminal size detection: Console.WindowWidth/Height, fallback to ioctl/TIOCGWINSZ via P/Invoke on Unix for reliability.
- Unicode & glyph widths: use System.Text.Rune, StringInfo and consider an East-Asian-width library to compute column widths for block characters.
- Partial-block rendering: ensure terminal/font supports block characters (█ ▇ ▆ ▅ ▄ …); no special lib but test across terminals.
- Testing strategy: create an in-memory TestBackend to assert cells and styles (mimic Ratatui tests).
- Performance/implementation notes: use spans/arrays, minimize allocations, and expose builder-style widget configuration.

