## ratatui-core\src\symbols\border.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: defines border/box-drawing and block characters; concerns are Unicode rendering, widths, fallbacks and terminal capability detection.
- Research .NET terminal rendering libraries: Spectre.Console, Terminal.Gui (gui.cs) — see how they expose drawing primitives and Unicode handling.
- Investigate ncurses bindings or terminfo access on .NET (P/Invoke or packages) for capability detection on Unix.
- On Windows, plan to enable ANSI/VT sequences (SetConsoleMode / EnableVirtualTerminalProcessing) or use libraries that abstract this.
- Need a Unicode display-width solution (East Asian Width / wide char handling) — search for ".NET Unicode display width" or EAW implementations.
- Look for libraries that expose low-level terminal control (cursor, colors, alternate buffers) if needed beyond high-level UIs.
- Find or implement constants for box-drawing and block elements; ensure mapping to fallback ASCII sets.
- Consider libraries that normalize Unicode across platforms/fonts or provide fallback glyphs.
- Testing: pick libraries that allow rendering-to-string for unit tests (compare expected border strings).
- Configuration: prefer libs that allow runtime selection of character sets and ASCII safe mode.
- Terminals to test against: Windows Terminal, cmd/PowerShell, iTerm2, GNOME Terminal — ensure chosen .NET libs behave consistently.
- If you need terminfo/termcap, plan P/Invoke to libtinfo or use existing .NET wrappers.

