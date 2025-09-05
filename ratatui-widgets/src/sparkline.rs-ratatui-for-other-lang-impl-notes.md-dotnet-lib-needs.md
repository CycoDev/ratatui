## ratatui-widgets\src\sparkline.rs-ratatui-for-other-lang-impl-notes.md

- Need a terminal drawing/buffer abstraction (per-cell buffer) — search Spectre.Console (Canvas/Render), Terminal.Gui (gui.cs) and raw ANSI libraries.
- Layout/Rect utilities for sizing/positioning widgets — Terminal.Gui provides Rect; otherwise implement simple Rect struct.
- Styling (foreground/background colors, attributes) with support for 8/16/256/RGB — Spectre.Console handles color modes; System.Console is limited.
- Unicode block characters and customizable symbol sets — no special lib needed; ensure font/terminal support.
- Unicode width/grapheme handling (CJK, emoji, variable widths) — search EastAsianWidth.NET, System.Text.Rune, and libraries for Unicode width.
- Right-to-left text and bidi handling for layout — look for .NET bidi/RTL libraries (e.g., NETBidi) or implement mirrored rendering.
- Color downgrading / detection of terminal capabilities — Spectre.Console can detect capabilities; otherwise query TERM/ANSICON.
- Scaling and value-to-height mapping logic — pure algorithmic code; port directly.
- Absent/missing-value representation with per-bar styling — simple nullable handling; use nullable types or option pattern.
- Widget interface/trait equivalent — define an IWidget with Render(Buffer, Rect, Style).
- Enum/string parsing utilities — use System.Enum and System.ComponentModel.TypeConverter or DescriptionAttribute.
- Testing: unit tests for rendering output into an in-memory buffer — use xUnit/NUnit and compare buffer cells.
- No_std concerns are irrelevant for .NET (managed runtime handles allocation).

