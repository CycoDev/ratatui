## ratatui-core\src\text\grapheme.rs-ratatui-for-other-lang-impl-notes.md

- Need Unicode grapheme-cluster segmentation: research System.Globalization.StringInfo, GraphemeSplitter (NuGet), and ICU.NET.
- Need Unicode display width (wcwidth/East Asian Width): search for wcwidth ports, EastAsianWidth.NET, or UnicodeWidth.NET.
- Must detect zero-width and non‑breaking spaces (U+200B, U+00A0) at grapheme level.
- Terminal abstraction & styling: evaluate Spectre.Console (ANSI + Windows), Terminal.Gui (full TUI), and plain System.Console + ANSI shim.
- Color/format capability detection: find libs or APIs that report color depth (none, 16, 256, truecolor) and fallbacks.
- Windows terminal quirks: research enabling Virtual Terminal Processing and differences vs Unix pty.
- RTL and shaping: investigate Bidi.NET and HarfBuzzSharp for complex scripts.
- Memory model: plan for Span<string>/ReadOnlySpan<char>, pooling; Rust immutable-ref patterns don’t map directly.
- Testing needs: libraries should allow testing of widths, grapheme segmentation, and rendering across terminals.
- Core dependencies to look up: grapheme segmentation, width calculation, ANSI/style emitter, terminal capability detector.

