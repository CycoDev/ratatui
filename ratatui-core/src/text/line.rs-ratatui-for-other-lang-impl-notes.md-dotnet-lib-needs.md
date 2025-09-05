## ratatui-core\src\text\line.rs-ratatui-for-other-lang-impl-notes.md

- Need Unicode display-width lib (wcwidth / EastAsianWidth) for alignment/truncation.
- Need grapheme-cluster segmentation for safe truncation (Unicode grapheme cluster algorithm).
- .NET primitives to research: System.Globalization.StringInfo (TextElementEnumerator) and System.Text.Rune.
- Consider ICU bindings (icu-dotnet) if you need full Unicode algorithms.
- Look for NuGet packages implementing wcwidth / EastAsianWidth (search "wcwidth", "east asian width").
- Terminal TUI/backends: Spectre.Console (rich styling) and Terminal.Gui (gui widgets) are primary options.
- Style system: choose a library that supports composing styles per span (colors, bold/italic).
- Buffer abstraction: implement or use a 2D cell buffer if low-level drawing is needed (Terminal.Gui has higher-level widgets).
- Emoji & ZWJ sequences: ensure grapheme + width libs handle multi-codepoint emojis and flag sequences.
- CJK handling: must treat double-width characters correctly via EastAsianWidth.
- Windows terminals: verify VT100/ANSI support or fallback to Win32 Console APIs; test Windows differences.
- Performance: minimize allocations, cache width measurements for repeated text.
- Truncation/alignment logic must operate on display width, not codepoints/char counts.
- Test coverage: zero-width chars, combining marks, multi-codepoint emojis, CJK, alignment, style inheritance.

