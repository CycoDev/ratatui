## examples\apps\widget-ref-container\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Target high-level .NET libraries: Spectre.Console (rich ANSI rendering, 256/truecolor, live updates) and Terminal.Gui (gui.cs) for widget/view-based TUI.
- Low-level console: System.Console for basics; use P/Invoke to Windows Console API and termios on Unix for raw mode when needed.
- ANSI/alternate-screen: Spectre.Console and direct ANSI sequences handle alt-screen, cursor, and style reliably cross‑platform.
- Color support: Spectre.Console exposes 16/256/truecolor; System.Console is limited to ConsoleColor (16).
- Input/events: Terminal.Gui for key/mouse events; for custom backends use Console.ReadKey, Console.KeyAvailable, and native APIs for advanced input.
- Buffer model: implement a cell grid (char + fg/bg + style). Spectre.Console rendering primitives can reduce work but custom buffer gives fine control.
- Layout engine: reuse Terminal.Gui's layout or implement a Rect-based splitter similar to Ratatui.
- Unicode/width: use System.Text.Rune, StringInfo, and a .NET wcwidth implementation (search for "wcwidth .NET" or implement East Asian width rules).
- Feature detection: check TERM, environment, and use capability probing (ANSI, color depth, unicode) at runtime.
- Mouse support: Terminal.Gui supports mouse; Windows-specific mouse requires WinAPI handling.
- Testing: Spectre.Console.Testing utilities help assert rendered output; useful for ported widget tests.
- Performance: batch updates, diff buffers, and use native console write APIs (Write/WriteAsync) to reduce flicker.
- Recommendation: start with Spectre.Console + Terminal.Gui exploration, then build a small backend abstraction (System.Console / PInvoke / ANSI) and a custom cell buffer + widget-ref interface to mirror Ratatui behavior.

