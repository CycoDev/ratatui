## ratatui-core\src\layout\size.rs-ratatui-for-other-lang-impl-notes.md

- Size is simply (width, height) as unsigned 16-bit — map to C# ushort / System.UInt16.
- Coordinates: origin (0,0) top-left; X right, Y down; units = character cells.
- Provide simple conversions (tuples/rect) and ToString as "WxH" — implement similarly.
- Layout engine uses a constraint solver (Cassowary) — search for "Cassowary.NET" or "kiwi" ports on NuGet for constraint-based layouts.
- Terminal backends needed: capabilities detection, cell drawing (colors/styles), input (keyboard/mouse), terminal modes (raw, alternate screen).
- Libraries to research in .NET:
  - Spectre.Console — rich ANSI rendering, colors, markup, terminal capability detection.
  - Terminal.Gui (gui.cs) — higher-level TUI widgets, layout, mouse/keyboard support.
  - NCurses bindings / P/Invoke termios & Win32 Console API — for low-level raw mode / alternate screen control.
  - Cassowary.NET (or equivalent) — for constraint solving.
- System.Console is limited (no raw mode/mouse) — expect to combine with P/Invoke or a library.
- No_std / core::fmt detail is Rust-specific — irrelevant for .NET portability.
- Implementation order: core types (Size/Position/Rect) → choose backend library → build layout + widgets.

