## ratatui-widgets\examples\line_gauge.rs-ratatui-for-other-lang-impl-notes.md

- Goal: when porting Ratatui to .NET, focus on libraries for terminal control, buffered rendering, widgets/layout, input/events, Unicode width, and color/terminal capability detection.
- Terminal control + ANSI: Spectre.Console (rich ANSI, truecolor, cross-platform).
- Full TUI widget/layout: Terminal.Gui (gui.cs) — widgets, layouts, event loop, mouse, resize.
- Progress/gauge primitives: Spectre.Console has Progress; Terminal.Gui supports custom widgets.
- Low-level raw mode & input: System.Console (ReadKey/KeyAvailable) or Terminal.Gui for robust event handling.
- Buffered/diffed rendering: use Terminal.Gui’s renderer or implement off-screen buffer + ANSI flush.
- Unicode/grapheme/width: System.Text.Rune + EastAsianWidth.NET or Unicode width libraries from NuGet.
- Color fallbacks: Spectre.Console handles truecolor → 256 → 16-color; ensure Windows VT enabled.
- Terminal capability detection: inspect TERM, use terminfo/TermInfo.NET or Spectre.Console profiles.
- Mouse support & resize events: Terminal.Gui provides both; otherwise capture via ANSI sequences.
- Backend abstraction: wrap Console differences (enable VT on Windows) rather than many OS-specific backends.
- Recommendation: evaluate Spectre.Console for styling/colors and Terminal.Gui for widget/layout; combine if needed.

