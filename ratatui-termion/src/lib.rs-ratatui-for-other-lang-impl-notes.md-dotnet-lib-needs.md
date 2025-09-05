## ratatui-termion\src\lib.rs-ratatui-for-other-lang-impl-notes.md

- Spectre.Console — high‑level, cross‑platform ANSI styling, RGB color support; good starting point for formatting and styles.
- Terminal.Gui — a .NET TUI framework (widget system); useful to study rendering and input models.
- System.Console — built‑in; limited (Windows needs VT enabled via SetConsoleMode); OK for simple output.
- Windows ConPTY / Win32 Console API — required for advanced Windows control (cursor, raw input, full VT features) via P/Invoke.
- Enable Virtual Terminal Processing on Windows (SetConsoleMode) to use ANSI sequences reliably.
- termios via P/Invoke (libc) on Unix — needed for raw input modes and finer keyboard handling.
- Direct ANSI escape sequences — simple, portable approach for cursor, clearing, scrolling regions when terminal supports VT.
- Color model mapping — plan mapping between ratatui colors/styles and .NET library enums (ensure 24‑bit support).
- Input handling — Console.ReadKey for basic, raw mode + polling/P/Invoke for mouse/complex input.
- Double/diff buffer rendering — implement to minimize writes; inspect Spectre.Console/Terminal.Gui for approaches.
- Scrolling regions and region scrolling — done via ANSI sequences; verify Windows VT support.
- Terminal size in characters via System.Console.WindowWidth/Height; pixel dimensions generally not available cross‑platform.
- Modular backend architecture — separate Unix/Windows backends and expose a common interface for selection/auto‑detect.
- Test on common terminals (Windows Terminal, CMD, PowerShell, xterm, iTerm2) and provide fallbacks for unsupported features.

