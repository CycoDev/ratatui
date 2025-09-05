## ratatui-core\src\text\span.rs-ratatui-for-other-lang-impl-notes.md

- Need grapheme-cluster segmentation: use System.Globalization.StringInfo (GetTextElementEnumerator) or ICU bindings (Microsoft.ICU4CLR) for full Unicode correctness.
- Need visual width (wcwidth / East Asian Width): search NuGet for "wcwidth" or "EastAsianWidth" ports; otherwise implement Unicode EastAsianWidth table.
- Terminal styling & ANSI handling: consider Spectre.Console (rich styling, ANSI, width helpers) for rendering primitives.
- Widget / buffering frameworks: evaluate Terminal.Gui (gui.cs) or build a custom buffer using System.Console + ANSI escapes.
- Windows compatibility: ensure use of Virtual Terminal sequences or libraries (Spectre.Console handles fallbacks).
- Zero-width / combining marks: append combining marks to previous grapheme cell (requires grapheme-aware logic).
- Multi-column characters: rely on wcwidth results to occupy/clear multiple buffer cells.
- String ownership: .NET strings are immutable; use ReadOnlySpan<char>/ReadOnlyMemory<byte>/string to avoid copies.
- Style composition: map Ratatui Style to Spectre.Console style model or custom struct with inheritance/patching.
- Testing: include wide Unicode vectors (emoji, combining marks, CJK), and test across Windows/macOS/Linux terminals.
- Useful search terms: "grapheme cluster .NET", "wcwidth .NET", "East Asian Width .NET", "ICU .NET", "Spectre.Console", "Terminal.Gui".

