## ratatui-core\src\buffer.rs-ratatui-for-other-lang-impl-notes.md

- Grapheme clustering: .NET built-in System.Globalization.StringInfo; NuGet option GraphemeSplitter for precise grapheme segmentation.
- Width calculation (CJK/emoji/combining): look for wcwidth implementations (e.g., WcWidth.NET) or EastAsianWidth.NET equivalents.
- Zero-width/combining mark handling: rely on grapheme splitter + width lib.
- Terminal control / ANSI & styles: Spectre.Console (ANSI, colors, styles); also research Colorful.Console and direct System.Console with VT support.
- High-level TUI frameworks: Terminal.Gui (gui/layout) and Spectre.Console.Rendering (widgets).
- Underline color / advanced style features: check Spectre.Console capabilities; may need platform-specific fallbacks.
- Memory optimization: use System.Buffers, ArrayPool<T>, Span<T>/Memory<T>; investigate Utf8String/other compact string libs.
- Diffing/minimal updates: no standard; consider DiffPlex or implement bespoke buffer diff algorithm.
- Coordinate/map utilities: implement efficient 2D->1D index math with Span-backed storage.
- Cross-platform VT/ANSI support: ensure Windows VT processing enabled or use wrappers.
- Recommendation: separate buffer logic from renderer; implement robust Unicode support first.
- Prioritize researching: GraphemeSplitter, WcWidth.NET/EastAsianWidth.NET, Spectre.Console, Terminal.Gui, System.Buffers.

