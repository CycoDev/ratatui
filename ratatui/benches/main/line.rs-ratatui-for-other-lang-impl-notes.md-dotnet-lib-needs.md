## ratatui\benches\main\line.rs-ratatui-for-other-lang-impl-notes.md

- Unicode width: research System.Text.Rune and System.Globalization.StringInfo for grapheme/text-element work; look for .NET ports of the wcwidth/EastAsianWidth algorithms.
- Grapheme-aware truncation: use StringInfo.ParseCombiningCharacters or dedicated grapheme-split libraries for correct truncation at grapheme boundaries.
- Terminal backends/abstraction: evaluate Spectre.Console and Terminal.Gui (gui.cs); also consider plain System.Console with ANSI + Windows VT handling.
- ANSI / style encoding: Spectre.Console and Colorful.Console can map style to terminal codes; ensure support for 8/16/256/truecolor.
- Buffered rendering model: implement a cell buffer (Rect + cell array) using Span/Memory and ArrayPool for perf; compare with Terminal.Gui rendering internals.
- Alignment behavior: need grapheme-aware width measurements for left/center/right and truncation semantics.
- Emoji / East Asian width: verify handling via wcwidth tables or EastAsianWidth data to compute visual cell width.
- Terminal capability detection: inspect TERM, terminfo, and Windows VT flags; consider abstractions for differing capabilities.
- Performance: use Span<T>, Memory<T>, ArrayPool<T>, and BenchmarkDotNet for microbenchmarks.
- Styling metadata: design a Style struct (fg/bg/colors, attrs) and mapping layer to backend-specific codes.
- Widget API: mirror Rect/Widget concepts—Terminal.Gui is a useful reference for widget/layout patterns.
- Recommendation: prioritize Unicode width/truncation libs and a mature terminal backend (Spectre.Console or Terminal.Gui) as first research targets.

