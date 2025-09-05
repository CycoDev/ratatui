## ratatui-widgets\examples\scrollbar.rs-ratatui-for-other-lang-impl-notes.md

- Goal: identify .NET libraries/features needed to port Ratatui widget layer (scrollbars, layouts, rendering).
- Terminal abstraction: research how to implement pluggable backends in .NET (interface for cursor, clear, size, styles).
- Well-known .NET TUI libraries to study: Spectre.Console and Terminal.Gui (gui.cs).
- ANSI/VT/escape handling: libraries that emit/parse ANSI sequences (Spectre.Console handles ANSI well).
- Windows pty/console: ConPTY wrappers or P/Invoke to Win32 Console API for modern Windows support.
- Unix raw mode: termios access via Mono.Posix or P/Invoke for enabling raw input/alternate screen.
- Alternate screen, hide cursor, raw mode: must be supported by chosen backend or by P/Invoke.
- Input handling: non-blocking key read, keycode normalization, mouse events—look for event loop helpers or implement with Console.KeyAvailable + P/Invoke.
- Color support: detect 8/16/256/RGB; check libraries that expose terminal capability detection.
- Unicode width/wcwidth: search for wcwidth / East Asian width .NET ports (for CJK width correctness).
- Double-buffering & diff rendering: look for virtual terminal buffer patterns or implement frame diffing to minimize writes.
- Decide: reuse Spectre.Console/Terminal.Gui for high-level widgets, implement low-level backend (P/Invoke) where needed.

