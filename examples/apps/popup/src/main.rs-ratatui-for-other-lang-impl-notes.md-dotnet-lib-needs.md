## examples\apps\popup\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Goal: look for .NET libraries that provide cross-platform terminal backends, raw mode, alternate screen, cursor control, colors, input events, and allow a buffer/diff rendering model.
- Primary .NET libraries to evaluate: Terminal.Gui (gui.cs) and Spectre.Console.
- Terminal.Gui: higher-level TUI widgets, cross-platform, good for composable widgets/layout.
- Spectre.Console: rich styling, colors, live rendering primitives (useful for buffer diffs/partial updates).
- Low-level options: termbox-sharp (bindings to termbox), NCurses via P/Invoke/Mono.Posix, or direct P/Invoke to native APIs.
- Windows specifics: must handle enabling VT sequences (SetConsoleMode) for ANSI/TrueColor; fallback to Console API when needed.
- Unix specifics: use termios for raw mode (Mono.Posix or P/Invoke).
- Input: ensure libraries expose raw key events, mouse events, and non-blocking reads.
- Rendering model: implement a cell buffer with diffing; prefer libraries that expose raw drawing or "live" render hooks.
- Colors: check support for 16, 256, and TrueColor and conversions from library color enums.
- Unicode: need East Asian width and combining-character handling (use a Unicode width library).
- Terminal capabilities: detect TERM, capabilities, and fallbacks (no-256, no-truecolor).
- Lifecycle: ensure init/cleanup patterns (IDisposable/using) to restore terminal state on crashes.
- Layout: choose a library or implement a layout engine separate from widgets.
- Testing: verify behavior on Windows (ConHost, Windows Terminal), macOS, Linux; add CI matrix.
- Performance: prefer libraries with minimal I/O or ability to send only diffs/regions.

