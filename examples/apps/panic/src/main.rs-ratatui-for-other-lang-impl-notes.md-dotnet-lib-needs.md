## examples\apps\panic\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Terminal state management: need raw mode, alternate screen, cursor control, and reliable restore on crash.
- Exception handling: research AppDomain.UnhandledException, TaskScheduler.UnobservedTaskException and using try/finally or using IDisposable patterns to guarantee restore.
- Windows terminal: enable VT/ANSI (SetConsoleMode), consider ConPTY for full features and UTF‑8 handling.
- Unix terminal: use termios (via P/Invoke or Mono.Posix) to set raw mode and mouse support.
- Backend abstraction: design an IBackend interface with platform-specific implementations (Win/ANSI/termios).
- Buffered drawing + diffing: implement an offscreen buffer and cell diff algorithm to minimize writes (Terminal.Gui and Spectre.Console ideas).
- Colors & styles: support 16/256/truecolor; map library color model to ANSI/Win sequences; graceful fallbacks.
- Libraries to evaluate: Spectre.Console (rich styled output, hex colors), Terminal.Gui (widget system, redraw logic), Colorful.Console (simple styling).
- Low‑level APIs: P/Invoke Win32 Console APIs (SetConsoleMode, WriteConsole), libc termios on Unix.
- Capability detection: check TERM, query terminal for truecolor/256 support; use RuntimeInformation for OS.
- Mouse & input: research raw input, escape sequences for mouse events, and multiline/UTF‑8 grapheme handling.
- Testing: test across Windows Terminal, cmd, PowerShell, ConHost, xterm, iTerm2, and minimal terminals for fallbacks.

