## ratatui-core\src\buffer\cell.rs-ratatui-for-other-lang-impl-notes.md

- Target .NET libraries: research Spectre.Console and Terminal.Gui (gui.cs) first — they cover ANSI styling, colors, modifiers, and higher-level widgets.
- Unicode grapheme clusters: prefer built-in System.Globalization.StringInfo for text elements; also search NuGet for "grapheme splitter" / "grapheme segmentation" for more complete clustering.
- Character width (wcwidth/East Asian width): search NuGet for "wcwidth .NET" and "EastAsianWidth .NET" (or implement standard wcwidth table).
- Zero‑width / combining characters: ensure grapheme segmentation + width logic (wcwidth) are combined.
- Small-string / memory optimization: use Span<char>, Memory<char>, ArrayPool<char>, and ValueStringBuilder patterns rather than allocating many strings.
- CompactString analogue: no direct SSO string in .NET — use pooled buffers or ReadOnlyMemory<char> slices.
- ANSI / terminal escape mapping: Spectre.Console handles mapping; otherwise use System.Console + manual ANSI sequences.
- Windows compatibility: ensure Windows 10+ virtual terminal enabled or use libraries that abstract Windows console differences.
- Buffer diffing: implement IEquatable<T>, custom GetHashCode, and canonicalize empty vs space cells when comparing.
- Styling primitives: model Color, Modifier, Style types similar to Rust; Spectre.Console types can guide API/semantics.
- Box‑drawing merge/collapse: likely custom implementation — search for "box drawing merge algorithm" and reuse Unicode box-drawing tables.
- Optional features (underline color): gate by capability flags or config options.
- Performance profiling: use BenchmarkDotNet and System.Diagnostics to measure memory/CPU of cell buffer approaches.
- Interop considerations: prefer using spans and pooled buffers to keep per‑cell allocations minimal.

