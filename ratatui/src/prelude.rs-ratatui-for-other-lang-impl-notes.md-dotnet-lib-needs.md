## ratatui\src\prelude.rs-ratatui-for-other-lang-impl-notes.md

- Research .NET terminal backends: System.Console, P/Invoke Win32 Console APIs, and termios via native interop.
- Investigate cross‑platform .NET libraries: Spectre.Console (rich rendering) and Terminal.Gui (gui.cs) for widget/layout concepts.
- Check ncurses/.so bindings for .NET (NcursesSharp or P/Invoke) if relying on ncurses features.
- Confirm ANSI/VT100 support on Windows (ENABLE_VIRTUAL_TERMINAL_PROCESSING via SetConsoleMode).
- Look for libraries or code for raw mode & alternate screen handling (enter/exit, cleanup on crash).
- Find Unicode / grapheme / East Asian width support (System.Text.Rune, Unicode width libs).
- Terminal capabilities detection: TERM env, Windows version, and color depth detection.
- Input handling libs: key codes, mouse events, and async input (Spectre.Console and Terminal.Gui approaches).
- Buffer/diffing: no standard — evaluate Spectre.Console rendering internals or plan custom diffing buffer.
- Layout primitives: grid/rect management — inspect Terminal.Gui for layout patterns to mirror.
- Styling/text rendering: color, attributes, spans; compare Spectre.Console APIs.
- Licensing and ecosystem maturity for chosen libraries.



