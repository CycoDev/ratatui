## ratatui-widgets\src\barchart\bar_group.rs-ratatui-for-other-lang-impl-notes.md

- Core .NET libs to research: System.Console, System.Text.Rune, System.Globalization.StringInfo, System.Memory/Span<T>.
- Terminal frameworks: Spectre.Console (rich styling, truecolor, IAnsiConsole), Terminal.Gui (gui.cs) for layout/views.
- ANSI/VT support: how Windows console handles VT sequences; libraries that emit 24-bit ANSI (Spectre.Console).
- Unicode grapheme clustering: System.Globalization.StringInfo / TextElementEnumerator; System.Text.Rune for codepoints.
- Display width / East Asian width: search for EastAsianWidth.NET or implement width table; no built-in exact equivalent to unicode_width crate.
- Virtual buffer/diffing: look at Spectre.Console internals or implement a Buffer<Cell> and diff to console using ANSI.
- Layout primitives: implement Rect and constraint-based layout or reuse Terminal.Gui panels/layout.
- Styling model: map Rust Style to Spectre.Console styles or own struct with fg/bg/attrs and ANSI emitter.
- Performance: use Span<T>, pooled arrays (ArrayPool<T>), minimize allocations during render.
- API patterns: use fluent/builder patterns common in .NET for widget configuration.
- Backend abstraction: define IBackend (size, write buffer, color capabilities, resize events) similar to IAnsiConsole.
- If you need low-level terminal control, research libs: Spectre.Console, gui.cs, and any smaller VT/ANSI emitters.

(Only items directly relevant to choosing .NET libraries and APIs for porting.)

