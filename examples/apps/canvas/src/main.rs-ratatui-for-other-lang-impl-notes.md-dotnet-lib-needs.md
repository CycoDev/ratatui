## examples\apps\canvas\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Need a backend abstraction: research .NET libraries that give low-level terminal control (System.Console limits; search Terminal.Gui (gui.cs), Spectre.Console, Ncurses bindings, Mono.Posix for termios, and P/Invoke for Windows Console API).
- Raw mode: ability to disable canonical input and echo (termios on Unix; SetConsoleMode on Windows).
- Alternate screen & mouse capture: ensure library can enter/exit alternate buffer and enable mouse reporting (ANSI/DEC sequences or platform API).
- Windows VT support: must enable ENABLE_VIRTUAL_TERMINAL_PROCESSING via SetConsoleMode for ANSI sequences.
- Event handling: keyboard, mouse, resize, and polling/timeouts (look for libraries exposing nonblocking input + resize events).
- Buffering/diffing: need virtual buffer (current/previous) and diff algorithm to minimize output — research libraries or implement.
- Truecolor & ANSI color support: verify terminal capability; Spectre.Console supports ANSI — check 24-bit support.
- Unicode/grapheme and width handling: use System.Globalization.StringInfo and lookup East Asian width libs for column widths.
- Braille/half-block drawing and glyph composition: ensure Unicode support and font/terminal glyph behavior.
- Character cell semantics: handle wide characters, combining marks, and surrogate pairs correctly.
- Panic/exit cleanup: ensure deterministic restore of terminal state on crashes (try/catch + AppDomain.UnhandledException).
- Implementation order: terminal abstraction → buffer system → event loop → layout/widgets → higher-level Canvas features.

