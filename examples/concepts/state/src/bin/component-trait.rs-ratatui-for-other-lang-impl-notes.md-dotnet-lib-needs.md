## examples\concepts\state\src\bin\component-trait.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries that provide cross-platform terminal control, input events, ANSI/colour, alternate screen, raw mode, mouse, and unicode support.
- High-level TUI widget/layout library to research: Terminal.Gui (gui.cs) — widgets, layouts, event loop.
- Rich rendering / ANSI features: Spectre.Console — colors (16/256/RGB), ANSI, live updates.
- Low-level terminal control: P/Invoke Win32 Console API (Windows) and termios via Mono.Posix.NETStandard (Unix/macOS) for raw mode/alternate buffer.
- Curses/PDCurses wrappers (PDCurses.NET, NCursesSharp) for mature cross-platform primitives.
- Mouse & complex input: verify library support (Terminal.Gui and some curses wrappers provide mouse events); Console.ReadKey is limited.
- Alternate screen & raw mode: ensure chosen lib exposes enter/exit alternate buffer and raw input toggles.
- Unicode/wide-char: use System.Text.Rune (.NET Core+) and check terminal font/encoding behavior.
- Event handling: async input loop + normalized key/mouse events across platforms.
- Double-buffering / diffing: look for libraries or patterns (offscreen buffer, minimal writes) or implement on top of Spectre.Console/Terminal.Gui.
- Logging/error handling: ensure terminal restoration on crash (AppDomain.UnhandledException hook).
- Performance: measure terminal I/O, prefer buffered writes and batched updates.
- Consider license/maintenance of bindings (P/Invoke layers) and cross-platform CI for Windows/macOS/Linux tests.

