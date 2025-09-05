## ratatui-core\src\layout\layout.rs-ratatui-for-other-lang-impl-notes.md

- Constraint solver (kasuari): look for .NET Cassowary/Kiwi implementations (Cassowary.NET, Kiwi/Cassowary ports) or other incremental linear constraint solvers that support strengths/priorities.
- HashMap (hashbrown): use System.Collections.Generic.Dictionary or System.Collections.Immutable.ImmutableDictionary for immutable collections.
- Iterator utilities (itertools): use LINQ and MoreLINQ for advanced iterator helpers.
- LRU cache (lru / optional feature): consider Microsoft.Extensions.Caching.Memory or third‑party LRU libs (LruCache.NET, SimpleLRU) for size/expiry eviction.
- Rc<[Rect]> semantics: use ImmutableArray<Rect>, ReadOnlyMemory<Rect> or shared references to arrays for zero-copy slices.
- Rect type: implement a simple struct (x,y,width,height) or reuse System.Drawing.Rectangle / System.Drawing.Common if acceptable cross‑platform.
- Floating‑point rounding consistency: use double and Math.Round with explicit MidpointRounding to match Rust behavior.
- Flex/spacing behaviors, percentages/ratios: implement as pure layout logic on top of solver or numeric routines.
- Prioritize solvers that support constraint priorities, variable bounds, and deterministic results across platforms.
- Also research .NET terminal UI integration (e.g., Terminal.Gui) for practical usage of the layout system.

