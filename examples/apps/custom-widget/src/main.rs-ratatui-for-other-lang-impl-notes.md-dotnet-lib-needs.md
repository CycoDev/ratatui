## examples\apps\custom-widget\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Research Terminal.Gui (gui.cs) for a full widget/layout/evented TUI in .NET.
- Evaluate Spectre.Console for ANSI rendering, live updates, colors and partial "live" UI needs.
- Check ncurses bindings or wrappers for .NET if relying on terminfo/termcap features.
- Investigate enabling Windows Virtual Terminal (SetConsoleMode / PInvoke) for ANSI/VT support.
- Find or implement cross-platform raw-mode (termios on Unix, SetConsoleMode on Windows).
- Look for alternate-screen and mouse-capture support in libraries (Terminal.Gui supports mouse).
- Verify Unicode and wide-character (CJK) handling in chosen libraries.
- Confirm color capabilities: RGB, 256-color, and indexed color support.
- Ensure non-blocking event polling and resize event hooks are available.
- Prefer libraries with a buffer/double-buffer or "render diff" approach, or plan to implement one.
- Seek widget/state separation and composable layout primitives (constraint-based or flex-like).
- Plan a backend-abstraction layer to swap low-level terminal implementations if needed.
- If no library covers low-level needs, expect to P/Invoke termios/posix and Win32 console APIs.

