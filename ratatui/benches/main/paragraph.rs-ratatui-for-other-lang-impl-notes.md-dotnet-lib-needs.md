## ratatui\benches\main\paragraph.rs-ratatui-for-other-lang-impl-notes.md

- Goal: when porting, focus on Paragraph widget features (wrapping, alignment, scrolling, styled spans) and performance (large texts, buffering).
- Terminal backends to research in .NET: Spectre.Console (rich styling, ANSI), Terminal.Gui (gui.cs) for higher-level widgets; evaluate native Console + ConPTY/VT sequences for low-level control.
- Unicode and grapheme support: use System.Text.Rune and System.Globalization.StringInfo for grapheme clusters; find a NuGet for character display width (equivalent to unicode-width / East Asian Width).
- Styled spans: need per-grapheme/style rendering support (investigate Spectre.Console span/styled text APIs).
- Text wrapping/caching: look for or implement efficient word-wrapping algorithms and layout caching to avoid recomputing on scroll.
- Scrolling: avoid 16-bit limits — use 32/64-bit offsets; implement virtual scrolling for huge documents.
- Buffering/diffing: implement double-buffering and cell-diffing to only write changed cells (research Spectre.Console live rendering and patterns for partial redraw).
- Unicode width and emoji: ensure library handles wide characters, combining marks, and emoji correctly (test with East Asian and emoji cases).
- Performance concerns: measure wrapping cost, scrolling cost, and rendering of very large buffers; add benchmarks similar to the Rust file.
- Alignment and layout: implement alignment (left/center/right) and integration with border/title blocks; check Terminal.Gui/Spectre.Console positioning APIs.
- Cross-platform quirks: research Windows terminal limitations (colors, VT support), and differences across Linux/macOS terminals; consider abstraction layer for backends.
- Runtime dependencies to look up on NuGet: Spectre.Console, Terminal.Gui (gui.cs), plus candidate packages for Unicode width/eastasian-width or general Unicode utilities; otherwise plan to port/implement width logic.

If you want, I can produce a concise checklist of exact NuGet packages and APIs to evaluate next.

