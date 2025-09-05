## ratatui-widgets\examples\sparkline.rs-ratatui-for-other-lang-impl-notes.md

- Primary .NET libraries to evaluate: Spectre.Console (high-level rendering, colors, live updates) and Terminal.Gui (gui.cs) for full TUI + input/mouse/resize.
- For low-level control: System.Console plus P/Invoke to Win32 Console APIs (SetConsoleMode) and POSIX termios for raw mode on Unix.
- Need alternate-screen & raw-mode support (ANSI alt-screen 1049h or native APIs).
- Ensure UTF-8 output: Console.OutputEncoding = Encoding.UTF8; verify font/terminal support for blocks ▁▂▃▄▅▆▇█.
- Enable VT100 processing on Windows (ENABLE_VIRTUAL_TERMINAL_PROCESSING) when using ANSI escapes.
- Seek libraries or implement: virtual buffer/grid of Cells (char + style) to diff and minimize terminal writes.
- Look for or implement backend abstraction layer to swap consoles/backends.
- Event handling: non-blocking key/mouse/resize (Terminal.Gui provides; otherwise Console.KeyAvailable, async reads, or native events).
- Mouse support requires enabling mouse reporting (ANSI) or Win32 mouse events.
- Styling: verify 256-color/truecolor support; Spectre.Console exposes wide color features.
- Widget concerns to implement in .NET: scaling to width/height, per-bar styling, handling null/absent values, multi-line rendering.
- Test extensively on Windows (cmd/PowerShell/Windows Terminal), macOS, and Linux terminals.

