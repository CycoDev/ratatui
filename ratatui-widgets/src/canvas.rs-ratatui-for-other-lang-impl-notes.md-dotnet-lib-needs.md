## ratatui-widgets\src\canvas.rs-ratatui-for-other-lang-impl-notes.md

- Target .NET libraries: Spectre.Console, Terminal.Gui (gui.cs), SadConsole, and Ncurses bindings (e.g., NCurses.Core) — evaluate for buffered terminal drawing, color, input and resize events.
- ANSI/VT handling on Windows: research libraries or samples that enable VT processing (or P/Invoke SetConsoleMode) — Spectre.Console handles this for you.
- Color support: check 8/256/24-bit color support in chosen lib (Spectre.Console supports truecolor; Terminal.Gui is more limited).
- Buffered output / off-screen buffer API: prefer libs offering efficient per-frame buffers or implement own char/attr buffer before flushing.
- Unicode handling: ensure Console.OutputEncoding = UTF8 and test braille/half-block glyph rendering; System.Text handles encoding but terminal must support glyphs.
- Braille & half-block rendering: no common C# library — plan to port mapping logic (bitmap → Unicode Braille codepoints; half-block foreground/background handling).
- Terminal capability detection: research terminfo/Ncurses bindings or environment heuristics (COLORTERM, TERM) to decide fallbacks.
- Low-level TTY control: Mono.Posix or P/Invoke for raw mode, cursor, and terminal size if needed.
- Input & resize events: prefer libraries that provide event-driven input and resize (Terminal.Gui or Spectre.Console ConsoleHost patterns).
- Cross-platform testing: include Windows Terminal, ConHost, common Linux terminals, macOS Terminal; fonts affect glyph availability.
- Project targets: .NET 6+ or .NET Standard for widest cross-platform support.
- Implementation notes: use interfaces for Grid/Shape, off-screen buffer, and provide multiple rendering backends (Braille, HalfBlock, Char) with fallbacks.

