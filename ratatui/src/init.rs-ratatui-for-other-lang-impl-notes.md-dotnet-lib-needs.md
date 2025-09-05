## ratatui\src\init.rs-ratatui-for-other-lang-impl-notes.md

- Core needs: enter/exit raw mode, alternate screen buffer, restore on exit/panic/signal.
- Windows: use Win32 Console API (SetConsoleMode) or enable VT processing; P/Invoke via System.Runtime.InteropServices.
- Unix: use termios (tcgetattr/tcsetattr) — Mono.Posix.NETStandard or P/Invoke libc.
- Alternate screen: emit ANSI ESC [?1049h / ?1049l or use library that wraps it.
- ANSI handling: Spectre.Console supports rich ANSI rendering and can simplify colors/cursor.
- High-level TUI: Terminal.Gui (gui.cs) provides a native TUI framework (different approach than ANSI).
- ncurses option: P/Invoke wrappers or PDCurses/NCursesSharp for a curses-style backend.
- Signal/cleanup hooks: Console.CancelKeyPress, AppDomain.ProcessExit, AppDomain.CurrentDomain.UnhandledException.
- Backend abstraction: implement a platform-agnostic layer (like CrosstermBackend) to swap implementations.
- Low-level input: for immediate key reads you’ll need raw-mode + direct Console.OpenStandardInput read or native calls.
- Recommendation: combine Spectre.Console (ANSI output) + P/Invoke/Mono.Posix for reliable raw-mode control and cleanup.

