## ratatui\tests\widgets_paragraph.rs-ratatui-for-other-lang-impl-notes.md

- Core goals for port: Unicode width, double-buffered rendering/diffs, backend abstraction, wrapping/alignment, scrolling, styling.
- Unicode width: research .NET libraries or algorithms for East Asian/ambiguous widths (East Asian Width tables, Unicode grapheme clusters). Look for packages exposing character cell width.
- Text model: need Text/Line/Span abstractions with styles per span (foreground, background, attrs).
- Buffer: implement an in-memory screen buffer and a previous buffer to diff and emit minimal updates.
- Rendering: wrapping and alignment must count displayed cell widths (not codepoints).
- Double-width chars (CJK): ensure width logic treats them as 2 cells.
- Backends: plan for Console (System.Console + ANSI), Windows Console API (Win32) via P/Invoke, and third‑party libraries.
- Libraries to research: Spectre.Console (rich styling, not low‑level buffer diffs), Terminal.Gui (higher‑level UI), and lower‑level ANSI/Console wrappers.
- Styling/colors: ANSI sequences vs Windows Console APIs; check cross‑platform behavior.
- Input handling: Console.ReadKey and platform differences; consider native hooks for advanced input.
- Terminal size: System.Console.WindowWidth/Height and native APIs for robustness.
- Testing: build a TestBackend that renders to an in‑memory buffer for deterministic assertions.
- Scrolling: horizontal scrolling must account for cell widths.
- If you need specific .NET package names for Unicode width or low‑level terminal control, research "East Asian width .NET" and "ANSI terminal .NET" as next steps.

