## ratatui-widgets\src\reflow.rs-ratatui-for-other-lang-impl-notes.md

- Need grapheme-cluster segmentation: research System.Globalization.StringInfo/TextElementEnumerator and System.Text.Rune; also search for "grapheme cluster .NET" and libraries like ICU (icu-dotnet/ICU4NET) or GraphemeSplitter ports.
- Need Unicode visual width (wcwidth / East Asian width): look for "wcwidth .NET", UnicodeWidth.NET, EastAsianWidth.NET or use ICU APIs.
- Normalization: use String.Normalize (NFC/NFD) to stabilize combining sequences when needed.
- Handle NBSP (\u00A0), ZWSP (\u200B), ZWJ and other special codepoints explicitly in logic.
- Word-boundary breaking: .NET Regex with Unicode categories, or ICU BreakIterator for locale-aware boundaries.
- State-machine LineComposer: implement as IEnumerable/IEnumerator or a custom struct-based iterator to stream lines efficiently.
- Performance: use ArrayPool<T>, Span<T>/Memory<T>, and pooled buffers to reduce allocations.
- Alignment and padding: measure with Unicode width lib, then PadLeft/PadRight or custom padding using spans.
- Testing: use xUnit/NUnit and Unicode test vectors (GraphemeBreakTest.txt, EastAsianWidth data) plus CJK and RTL samples.
- Search keywords to guide library research: "TextElementEnumerator", "System.Text.Rune", "wcwidth .NET", "EastAsianWidth", "icu .net", "ArrayPool", "Grapheme cluster .NET".

