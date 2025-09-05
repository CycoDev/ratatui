## examples\apps\demo\src\crossterm.rs-ratatui-for-other-lang-impl-notes.md

- Research .NET TUI libraries: Spectre.Console (rich text, styling), Terminal.Gui (Gui.cs) for widgets/layouts.
- Look for ncurses wrappers (NcursesSharp) or use libncurses via P/Invoke on Unix.
- Low-level terminal control: System.Console + termios via Mono.Posix.NETStandard (raw mode, alternate screen) on Unix.
- Windows low-level: Win32 Console APIs (SetConsoleMode, ReadConsoleInput, WriteConsoleOutput) via P/Invoke; use Vanara.PInvoke or PInvoke.Windows.
- Enable VT/ANSI on Windows (ENABLE_VIRTUAL_TERMINAL_PROCESSING) and set UTF-8 code page.
- Input handling: Console.KeyAvailable/ReadKey for simple use; ReadConsoleInput or raw stdin parsing for nonblocking, special keys, mouse.
- Mouse support: Windows console events or terminal mouse reporting escape sequences + parser.
- Resize detection: poll Console.WindowWidth/Height or handle SIGWINCH via Mono.Posix.
- Buffering: implement double-buffer diffing and batched writes; check if libraries provide efficient rendering.
- Styling/colors: ANSI sequences or Spectre.Console features.
- Unicode: ensure Console.OutputEncoding = UTF8 and Windows CP set.
- Cross-platform design: separate backend abstraction for Windows vs Unix.
- Performance: minimize syscalls, batch output, efficient buffer compare.
- Useful helpers: Vanara, PInvoke.Windows, Mono.Posix, and existing high-level libraries to avoid reimplementing widgets.

