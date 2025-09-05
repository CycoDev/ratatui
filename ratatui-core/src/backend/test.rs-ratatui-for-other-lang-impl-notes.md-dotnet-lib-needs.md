## ratatui-core\src\backend\test.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: implement an in-memory, non-interactive terminal backend for tests (buffer grid + scrollback + cursor + styling).
- Grapheme segmentation: prefer System.Globalization.StringInfo / TextElementEnumerator for grapheme clusters; consider icu-dotnet or other ICU bindings if strict UAX#29 compliance is required.
- Unicode scalar access: System.Text.Rune for safe codepoint handling.
- Unicode display width (CJK, emoji, combining): .NET has no built-in terminal width API — search NuGet for EastAsian/Unicode width packages (e.g., packages named EastAsianWidth or UnicodeWidth) or implement width lookup per UAX#11.
- Styling model: represent per-cell foreground/background and decorations; use Spectre.Console types as a reference for colors/styles.
- Terminal rendering/test harness: Spectre.Console has a rendering model and a Spectre.Console.Testing Fake/ITestConsole useful for testing; evaluate Terminal.Gui for higher-level TUI widgets (but it’s not a test backend).
- Cursor/screen control: System.Console supports basic ops for real terminals; for test backend simulate cursor position & visibility in your buffer.
- Scrollback: implement ring/buffered list of lines with max capacity; ensure consistent width handling between visible buffer and scrollback.
- Serialization for test scenarios: use System.Text.Json or Newtonsoft.Json (choose based on needs).
- Testing frameworks: use xUnit or NUnit for assertions; consider creating helpers to assert buffer cells, graphemes, cursor state, and scrollback content.
- CRLF differences: normalize line endings when comparing across platforms.
- Performance/data structures: use 2D arrays or 1D arrays indexed by y*width+x and store grapheme string + style + width metadata.
- Dependency checklist to research on NuGet: grapheme/segmentation libraries, Unicode width/EastAsianWidth packages, Spectre.Console (and Spectre.Console.Testing), icu-dotnet (if needed), JSON libraries, and your chosen test framework.
- If you need terminal escape parsing/emulation, search for VT100/ANSI parsers (NuGet: vt100/terminal emulation packages).
- Prioritize: grapheme segmentation + Unicode width + a testable console abstraction (Spectre.Console.Testing or custom) — these are the most important libs to research first.

