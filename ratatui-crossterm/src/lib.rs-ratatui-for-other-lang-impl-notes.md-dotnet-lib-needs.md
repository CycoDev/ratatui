## ratatui-crossterm\src\lib.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries and primitives to match crossterm features (cross‑platform terminal control, colors, styles, raw input, alternate screen).
- Candidate high‑level libs: Spectre.Console (rich styling, colors), Terminal.Gui (gui.cs) (TUI framework with platform drivers).
- Candidate low‑level/libs-for-binding: NcursesSharp or P/Invoke to ncurses on Unix; on Windows P/Invoke to Console APIs (SetConsoleMode, WriteConsoleOutput*).
- Color & style support needed: ANSI 16/256, truecolor (RGB), attributes (bold/italic/underline/inverse). ensure libraries expose these or provide escape sequences.
- Windows: enable VT processing (SetConsoleMode ENABLE_VIRTUAL_TERMINAL_PROCESSING) or use WinAPI console functions for better control.
- ANSI escape sequences: ensure library or code can emit/parse sequences for cursor, clear, scroll regions, alternate screen.
- Raw mode & input: need to toggle raw/cooked mode, read bytes/keys without line buffering (Terminal.Gui or P/Invoke/termios on Unix).
- Terminal sizing & capability detection: read size, inspect TERM/COLORTERM, probe for truecolor/256 support (COLORTERM == "truecolor" or TERM contains "256color").
- Unicode & width handling: use System.Text.Rune + a wcwidth/grapheme splitter NuGet (or implement East Asian Width) to measure column widths for wide/combining chars.
- Buffering model: implement Cell, Buffer (2D), Style, Color, Modifier types and diff previous vs current buffer to minimize output.
- Rendering optimizations: track cursor position & current attributes, batch writes to a buffered StreamWriter (Console.OpenStandardOutput) and flush rarely.
- Style conversion layer: map your Color/Modifier models to library escape sequences or API types.
- Scrolling regions (optional): require terminal support via CSI sequences or WinAPI buffer scrolling; check chosen library for helpers.
- Performance: use StringBuilder/Spans, avoid per‑cell Console.Write calls, prefer single large writes.
- Testing matrix: verify behavior on Windows (old console vs Windows Terminal), macOS, Linux (various terminals), and WSL.
- When library features are missing: prefer P/Invoke for Windows console calls and termios/ncurses on Unix to fill gaps.
- Useful NuGet search keywords: "ansi terminal", "console raw mode", "ncurses", "wcwidth", "grapheme splitter", "virtual terminal processing".
- Short checklist when evaluating a .NET lib: raw mode, alt screen, cursor control, color modes (RGB/256/16), text attributes, input events, terminal size, cross‑platform consistency.

