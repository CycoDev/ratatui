## ratatui-core\src\text.rs-ratatui-for-other-lang-impl-notes.md

- Grapheme segmentation: research System.Globalization.StringInfo / TextElementEnumerator and NuGet ports (e.g., "GraphemeSplitter").
- Unicode scalar support: System.Text.Rune (for iterating code points).
- Display width (wcwidth / East Asian width): search NuGet for "wcwidth" or "EastAsianWidth" ports.
- Zero-width & combining handling: validate with StringInfo/Rune + width lib.
- Emoji and multi-width support: test with width lib and Rune.
- Terminal backend & ANSI: Spectre.Console (rich styling, RGB, 256-color, ANSI); also consider direct System.Console + Windows VT enable.
- Full TUI frameworks: Terminal.Gui (gui-like) or use Spectre.Console for rich output.
- Color handling: Spectre.Console (RGB/ANSI) or Pastel/Colorful.Console for simpler needs.
- Style model: implement hierarchical style merging (Text → Line → Span) using structs/records.
- Memory: use ReadOnlyMemory<char>/Span<char>/Rune to avoid allocations.
- Testing: xUnit/NUnit for zero-width, multi-width, truncation, alignment.
- Terminal capability/terminfo: check Spectre.Console or native terminfo bindings if needed.

