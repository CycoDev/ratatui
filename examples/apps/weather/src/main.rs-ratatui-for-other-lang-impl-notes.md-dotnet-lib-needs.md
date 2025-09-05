## examples\apps\weather\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Create a backend abstraction to isolate System.Console, Win32 VT APIs and Unix TTY specifics.
- Candidate .NET libs: Terminal.Gui (gui.cs) for widgets/layout, Spectre.Console for rich rendering; otherwise System.Console + P/Invoke or Ncurses wrappers (NcursesSharp).
- Raw mode: enable/disable line-buffering and echo (on Windows enable VT processing via SetConsoleMode).
- Alternate screen: use ANSI ESC[?1049h/1049l (requires VT on Windows).
- Non-blocking input: Console.KeyAvailable, async reads or background thread; normalize keys across platforms.
- Mouse: Terminal.Gui provides mouse support; raw input needed for other approaches.
- Terminal size: Console.WindowWidth/Height; for resize events poll or use native SIGWINCH handlers on Unix.
- Unicode: set Console.OutputEncoding = Encoding.UTF8 and ensure console supports UTF‑8.
- Colors: detect capabilities (TERM/COLORTERM), enable VT on Windows; Spectre.Console handles many cases.
- Double-buffering: implement off-screen buffer (chars+style), diff and write minimal updates.
- Layout & widgets: use Terminal.Gui or implement constraint-based layout engine.
- Terminal state cleanup: restore modes in finally/Dispose and on unhandled exceptions.
- Start by building the backend layer, then buffer system, then widgets/layout.

