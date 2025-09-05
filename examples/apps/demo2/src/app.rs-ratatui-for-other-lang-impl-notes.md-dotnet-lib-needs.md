## examples\apps\demo2\src\app.rs-ratatui-for-other-lang-impl-notes.md

- Need a cross‑platform terminal abstraction: search for ".NET cross‑platform terminal libraries".
- Candidates to investigate: Spectre.Console, Terminal.Gui (gui/curses style), and ANSI + Windows Console API (P/Invoke).
- Low‑level control: must support raw mode and alternate screen; search "alternate screen buffer .NET" and "SetConsoleMode ENABLE_VIRTUAL_TERMINAL_INPUT".
- Input handling: non‑blocking keyboard, mouse, modifiers, and resize events — look at Console.KeyAvailable, Terminal.Gui mouse API, and libraries exposing raw input.
- Unicode width/metrics: search "wcwidth .NET", "East Asian Width .NET", and System.Text.Rune / grapheme cluster libraries.
- Color capability detection: look for 16/256/RGB support in Spectre.Console or ANSI detection libraries.
- Buffering/double‑buffering: need virtual screen buffers and diff/redraw support — search "terminal virtual buffer .NET" or "partial redraw terminal .NET".
- Layout & widgets: need constraint/nested layout and composable widgets — Terminal.Gui and Spectre.Console renderables/grid are relevant.
- Event loop: implement async event loop with timeouts using Task, CancellationToken, and Channels.
- Styling/theme system: look for libraries supporting colors, attributes, and centralized themes (Spectre.Console).
- Platform specifics: research Windows Console APIs (kernel32) vs UNIX ttys and ncurses bindings for .NET.
- Mapping of Rust deps: Crossterm → Spectre.Console/ANSI+WinAPI; unicode‑width → wcwidth/.NET Unicode libs; itertools → LINQ; strum → Enums.NET or System.Enum utilities.
- Ensure cleanup on exit/crash: search patterns for restoring console state in .NET (try/finally, CancelKeyPress).
- Performance keywords to research: "terminal diffing", "partial redraw", "layout caching .NET".

