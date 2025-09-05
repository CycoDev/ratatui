## examples\apps\calendar-explorer\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Target .NET libs to investigate: Spectre.Console (ANSI rendering, colors, live updates) and Terminal.Gui (gui.cs) for widgets/events.
- Low-level backends: P/Invoke to Windows Console API (enable VT), PDCurses or ncurses .NET bindings for Unix-y drivers.
- Rendering model: implement buffer+diff yourself or use Spectre.Console LiveRender features.
- Layout: look at Cassowary.NET for constraint-based layouts.
- Widgets: Terminal.Gui supplies many standard widgets; Spectre.Console focuses on rich text and panels.
- Color handling: Spectre.Console supports 16/256/truecolor; implement fallbacks & conversions as needed.
- Input normalization: Terminal.Gui provides cross-platform key/mouse events; otherwise use Console.ReadKey + P/Invoke.
- Terminal states: alternate screen, raw mode via ANSI sequences or SetConsoleMode; ensure restore on exit.
- Unicode/wide chars: use System.Text.Rune and UnicodeWidth.NET for grapheme/east-asian width handling.
- Signal/cleanup: handle AppDomain.ProcessExit and Console.CancelKeyPress for state restoration.
- Platform detection: check TERM and Windows VT capability; fall back to simpler modes.
- If you need constraint layout + immediate-mode rendering + buffer diffing, expect to combine libraries (Spectre.Console + Cassowary.NET) and add a custom buffer layer.

