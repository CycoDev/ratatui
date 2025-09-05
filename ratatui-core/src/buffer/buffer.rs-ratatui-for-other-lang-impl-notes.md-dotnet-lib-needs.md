## ratatui-core\src\buffer\buffer.rs-ratatui-for-other-lang-impl-notes.md

- Need grapheme-cluster support (UTF-8 graphemes): .NET options: System.Globalization.StringInfo / TextElementEnumerator or NuGet GraphemeSplitter.
- Need display width (wcwidth / East Asian Width, emojis, CJK): search NuGet "wcwidth", "EastAsianWidth", or implement Unicode EastAsianWidth tables.
- Must handle zero-width & combining characters and control chars: use Char/UnicodeCategory checks + normalization (String.Normalize).
- Need robust Unicode API: System.Text.Rune and System.Globalization.
- Terminal ANSI/RGB/indexed color support: Spectre.Console (rich), Colorful.Console, or write ANSI escapes directly.
- Terminal capabilities & size detection cross-platform: Console.WindowWidth/Height + Spectre.Console / Terminal.Gui for richer features.
- Efficient small-string storage: use string pooling, StringSegment, Span<char>, ArrayPool, or MemoryMarshal techniques.
- Low-level buffer: 1D array of cells; use struct Cell with string/slice, fg/bg, modifiers, skip flag.
- Text modifiers (bold/italic/underline): represent as flags; map to ANSI sequences (Spectre.Console can help).
- Diffing/minimal updates: no ready .NET lib; port algorithm. Ensure correct handling of multi-width graphemes.
- Performance tools: System.Buffers.ArrayPool, Span<T>, memory pooling, and avoid allocations in hot paths.
- Search keywords for NuGet: "GraphemeSplitter", "wcwidth", "EastAsianWidth", "Spectre.Console", "Terminal.Gui", "StringSegment".

