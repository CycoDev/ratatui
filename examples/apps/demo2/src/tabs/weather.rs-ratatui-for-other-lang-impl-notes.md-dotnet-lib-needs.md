## examples\apps\demo2\src\tabs\weather.rs-ratatui-for-other-lang-impl-notes.md

- Look for a .NET TUI toolkit with widget/render-to-buffer model: Terminal.Gui (gui.cs) or SadConsole.
- For high-level rendering, charts, gauges, and progress: Spectre.Console.
- For per-cell buffer + partial updates (diffing): SadConsole or Konsole.
- Layout system: Terminal.Gui has constraint-based layouts; Spectre.Console offers panels/trees/grids.
- Event loop, keyboard & mouse: Terminal.Gui (built-in).
- Truecolor/ANSI support: Spectre.Console (ANSI escapes); ensure Console.OutputEncoding = UTF8.
- Unicode/emoji: set UTF-8 and test terminal emulators.
- Color-space conversions (Okhsv/OKLab): use Colourful or ColorMine NuGet, or implement conversions.
- Color & drawing primitives: System.Drawing.Common or SkiaSharp for conversions if needed.
- Terminal backend abstraction: rely on .NET Core cross-platform System.Console + chosen library; P/Invoke Windows Console API only if low-level control needed.
- Charts/graphs primitives: Spectre.Console has bar charts; otherwise render custom with per-cell buffer (SadConsole).
- Performance: pick library supporting buffer diffs or implement cell diffing; cache layouts.
- Resizing: subscribe to Console.Window* events or use library hooks (Terminal.Gui handles resizing).

