## ratatui-core\src\layout\flex.rs-ratatui-for-other-lang-impl-notes.md

- Target .NET areas: terminal rendering, integer-based layout math, enum/union types, caching, testing.
- Terminal UI libraries to evaluate: Terminal.Gui (gui.cs) and Spectre.Console (rendering/layout primitives).
- Consider F# for native discriminated unions; in C#, represent Flex variants with records + pattern matching or OneOf/LanguageExt.
- Serialization: System.Text.Json or Newtonsoft.Json if persistence needed.
- Use ushort/int for coordinates; be careful with integer division and rounding strategies.
- Constraint types to implement: Min, Max, Length, Percentage, Ratio, Fill.
- Layout primitives: Rect, Direction (horizontal/vertical), and a split algorithm implementing flex strategies.
- Caching: Microsoft.Extensions.Caching.Memory or ConcurrentDictionary; consider ArrayPool<T> for allocations.
- Performance: use Span<T>, pooled buffers, and avoid floating-point where possible.
- Testing: xUnit or NUnit + comprehensive combinatorial tests for constraints and flex modes.
- Docs: preserve ASCII diagrams in markdown for behavior clarity.
- Edge cases: zero-size areas, insufficient space, and legacy allocation behavior.

