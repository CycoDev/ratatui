## ratatui-widgets\src\gauge.rs-ratatui-for-other-lang-impl-notes.md

- Goal: port Gauge and LineGauge (buffered terminal widgets, styled filled/unfilled bar, centered labels, Unicode sub-cell precision).
- Evaluate high-level TUI libraries: Spectre.Console (rich rendering, styles, progress, Layout/Renderable model).
- Evaluate ncurses-like UI: Terminal.Gui (layout, widgets, panels).
- Low-level output: System.Console for direct writes; prefer libraries that handle ANSI/TTY detection.
- Off-screen buffer: need a 2D cell+style buffer or use Spectre.Console’s render model; otherwise implement custom.
- Unicode width/grapheme support: use System.Text.Rune + System.Globalization.StringInfo; search for “wcwidth .NET” implementations for East-Asian/combining width.
- Partial-cell rendering: use Unicode block elements; verify terminal font/encoding support.
- Styling/colors/attributes: Spectre.Console (best), Colorful.Console or Pastel for simpler use.
- Layout system: prefer libraries with Layout/Rect abstractions (Spectre.Console/Terminal.Gui) or implement a simple layout engine.
- Terminal capability detection: System.Runtime.InteropServices.RuntimeInformation + Console.IsOutputRedirected; Spectre.Console handles capability probing.
- Fluent API: implement builder-pattern in C# for configuration chaining.
- Recommendation: start with Spectre.Console for most features; use Terminal.Gui if you need windowed TUI layout semantics.

