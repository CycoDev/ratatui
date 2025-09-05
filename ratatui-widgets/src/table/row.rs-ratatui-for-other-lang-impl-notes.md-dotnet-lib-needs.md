## ratatui-widgets\src\table\row.rs-ratatui-for-other-lang-impl-notes.md

- Row is a container of Cell objects; port needs a table widget + per-cell styling support.
- Key .NET libraries to research: Spectre.Console (rich styling, tables, ANSI), Terminal.Gui (gui.cs) for fuller TUI features.
- Styling model: need composable styles (row style combined/overridden by cell style); check Spectre.Console.Style and merge behavior.
- Container/iteration: use IEnumerable<T> and LINQ; implement constructors accepting IEnumerable<Cell>.
- Trait features: map Rust traits to C# interfaces + extension methods for fluent APIs.
- Immutable builder pattern: use C# records with with-expressions or implement fluent builders that return new instances.
- Borrowed data (Cow): .NET strings are immutable; consider ReadOnlyMemory<char>/ReadOnlySpan<char> or string to avoid copies.
- Layout controls: implement height, top/bottom margins in layout engine; check Spectre.Console table row height handling.
- Style merging logic: ensure deterministic precedence (cell overrides row); implement utility to combine styles.
- Memory/perf: consider System.Memory, Span, and ArrayPool for high-throughput rendering.
- Tests: use xUnit/NUnit and test style inheritance, merging, and layout behavior.
- If you want minimal dependency: Spectre.Console covers most needs (tables, styles, rendering).

