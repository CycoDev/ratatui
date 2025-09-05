## examples\apps\color-explorer\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET equivalents for Ratatui concerns: terminal backends, raw/alternate-screen control, buffered/diffed rendering, layout engine, widgets, event input, color modes, and platform quirks.
- High-priority libraries to evaluate:
  - Spectre.Console — rich ANSI rendering, grids/layout primitives, supports 16/256/truecolor via ANSI sequences.
  - Terminal.Gui (gui.cs) — higher-level TUI framework with views, layout, events (more widget-oriented).
  - NCurses bindings for .NET (e.g., NCurses.Core / PInvoke wrappers) — unix-style terminal control.
  - Konsole (NuGet) and Colorful.Console — small helpers; check for buffering and ANSI support.
- Low-level APIs to research:
  - System.Console for basic I/O, Console.ReadKey/KeyAvailable, Console.OutputEncoding = UTF8.
  - Windows SetConsoleMode (P/Invoke kernel32) to enable ENABLE_VIRTUAL_TERMINAL_PROCESSING (VT sequences).
  - Using ANSI escape sequences directly (alternate screen, mouse, enable/disable cursor).
- Terminal setup / restoration: ensure alternate-screen, raw input mode, and cleanup even after exceptions — implement try/finally or use an IDisposable wrapper.
- Buffered rendering model: likely implement your own frame buffer + diffing if library doesn’t provide it; check if Spectre.Console or Konsole offer virtual buffers or efficient redraws.
- Layout engine: compare Terminal.Gui’s view-based layout and Spectre.Console’s grid/columns; decide if you need a custom layout system similar to Ratatui’s Constraints API.
- Widgets: Terminal.Gui provides many widgets; Spectre.Console has higher-level renderables (tables, panels). Choose based on desired API style (immediate-mode vs retained widgets).
- Event handling: evaluate library support for keyboard, mouse, and resize events; Terminal.Gui has event model, Spectre.Console is more render-focused and may need supplemental input handling.
- Color support: verify 16, 256, and 24-bit color support on target terminals; ensure you enable VT processing on Windows and provide graceful fallbacks.
- Mouse & resize: for full parity, ensure library or custom code can enable mouse reporting (Xterm sequences) and handle SIGWINCH/Console.WindowWidth changes.
- Unicode / wide-char: set UTF-8 output encoding, test wide grapheme widths and box-drawing characters; use System.Text.Rune (.NET 5+) for robust handling.
- Windows caveats: older consoles lack VT; require explicit enabling or use Windows Console API for advanced control — test on Windows Terminal, conhost, and PowerShell.
- Performance: plan frame throttling on resize, minimize Console I/O, and profile any text processing; prefer batched writes and diffs.
- Recommendation: prototype two approaches — Spectre.Console (ANSI-first, simpler rendering) and Terminal.Gui (widget/layout-heavy). If neither offers diffed-buffering, implement a small buffered renderer layer that writes minimal ANSI changes.

