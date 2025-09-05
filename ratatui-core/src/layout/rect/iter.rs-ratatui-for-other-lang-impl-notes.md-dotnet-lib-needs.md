## ratatui-core\src\layout\rect\iter.rs-ratatui-for-other-lang-impl-notes.md

- This file is pure geometry/iterator logic and does not require platform/terminal libraries in .NET.
- Prefer implementing your own Rect and Position value types (readonly struct) with ushort (System.UInt16) fields to match Rust semantics.
- For iteration surfaces, expose IEnumerable<Rect> / IEnumerable<Position> for idiomatic .NET use.
- To provide size hints/optimizations, implement IReadOnlyCollection<T> or expose a Count property (maps to Rust size_hint).
- Forward iteration: use C# iterator blocks (yield return) or a struct enumerator for zero-allocation enumeration.
- Bidirectional iteration: .NET IEnumerable is forward-only—either provide a Reverse() method (LINQ Reverse) or implement an indexable API (IReadOnlyList<T> / Count + indexer) so callers can iterate backward efficiently.
- Positions should be produced in row-major order (x increasing, then y increasing); document that behavior.
- Use int for loop counters internally to avoid ushort arithmetic overflow; cast to/from ushort only at API boundaries and validate ranges.
- Handle zero-sized rectangles by returning empty enumerables; ensure Count == 0.
- Guard against max-value edge cases (UInt16.MaxValue) — prefer checked arithmetic or validated ranges when computing extents.
- Testing: replicate iterator tests in .NET using xUnit/NUnit/MSTest + FluentAssertions; include forward, backward, meet-in-the-middle, zero-dimension, and boundary tests.
- Performance: consider struct enumerators and Span<T>/Memory<T> patterns if you need allocation-free loops; otherwise iterator blocks are simpler.
- No System.Drawing/System.Windows dependencies are required—keep implementation a small, portable geometry module.
- For concurrency/immutability, make Rect/Position immutable (readonly) and thread-safe by design.

