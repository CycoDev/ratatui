## ratatui-widgets\src\canvas\points.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: point-drawing on a character-cell canvas using colors and high-resolution Unicode (Braille/blocks).
- Need terminal abstraction with raw mode & per-cell control (cross-platform).
  - Investigate: Spectre.Console, Terminal.Gui (gui.cs) for higher-level; otherwise P/Invoke Windows Console API + ncurses bindings for low-level.
- Per-cell painting and direct buffer updates (not just line output).
  - Investigate libraries or implement your own terminal buffer + Write/SetCursor APIs.
- Color support (256/truecolor fallback and graceful degrade).
  - Investigate: Spectre.Console, Colorful.Console, and System.ConsoleCapabilities via P/Invoke.
- Unicode handling (Braille, block characters, grapheme widths).
  - Use System.Text.Rune and System.Globalization.StringInfo; validate terminal font/encoding.
- Terminal resize and input/events.
  - Terminal.Gui and Spectre.Console provide resize hooks; otherwise subscribe to Console.CancelKeyPress and P/Invoke SIGWINCH.
- Coordinate transforms and bounds checking.
  - Use System.Numerics (Vector types) and Span/Memory for performance.
- Floating->cell rounding and precision control — implement same logic as get_point.
- Memory/performance for large point sets — use arrays/Span<T>, avoid per-point allocations.
- Braille/block rendering logic usually implemented in-app; no ready .NET lib expected—plan to port mapping logic.
- Summary: focus research on Spectre.Console and Terminal.Gui for high-level features, plus P/Invoke (ncurses/Windows Console) for low-level control; rely on built-in System.Text and System.Numerics for Unicode/math.

