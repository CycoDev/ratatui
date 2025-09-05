## ratatui\benches\main\constraints.rs-ratatui-for-other-lang-impl-notes.md

- Core Rust deps to map to .NET: a constraint solver (kasuari), fast hash maps (hashbrown), iterator utilities (itertools), and an LRU cache (lru).
- Constraint solver: look for Cassowary/Kiwi implementations for .NET (e.g., Cassowary.NET / Kiwi.NET) or a general-purpose linear/constraint solver you can bind; these mirror kasuari’s role.
- Layout algorithm can also be implemented without a full solver; for Ratatui’s constraints (Fill, Length, Max, Min, Percentage, Ratio) a deterministic constraint solver or custom allocator math is sufficient.
- Iteration utilities: LINQ covers itertools functionality; consider MoreLINQ for extra helpers.
- Hash maps: use System.Collections.Generic.Dictionary<TKey,TValue> or ConcurrentDictionary for thread-safety (Dictionary is the usual replacement for hashbrown).
- LRU cache: consider existing NuGet packages like LruCache.Net or use Microsoft.Extensions.Caching.Memory with eviction policies or implement a small linked-list+dictionary LRU.
- Rect and geometry: define a lightweight Rect struct (use ushort/uint for terminal coords or int with bounds checks); System.Drawing.Rectangle exists but a custom struct avoids extra dependencies.
- Reference-counted slice return: use ReadOnlyMemory<Rect>, ReadOnlySpan<Rect> (when in-process), or ImmutableArray<Rect> to avoid copies while keeping safe ownership semantics.
- Float-to-int conversions: use double with explicit Math.Round/Math.Floor/Clamp and careful determinism; document precision and rounding behavior for reproducibility.
- Terminal coordinate types: keep widths/heights as small unsigned integers (ushort) or validated int; enforce top-left (0,0) convention.
- Platform backends: for rendering/backends investigate Terminal.Gui (gui.cs) and Spectre.Console; both handle cross-platform terminal I/O in .NET.
- Benchmarking: use BenchmarkDotNet to reproduce microbenchmarks across different terminal sizes (16x16, 64x64, 256x256).
- Performance/data structures: consider pooled arrays (ArrayPool<T>) and Span<T>/Memory<T> to minimize allocations during layout splits.
- Feature toggles: use configuration flags or conditional compilation to enable/disable layout caching like the Rust feature flag.
- Threading/memory: .NET’s GC and structs (value types) change allocation patterns — prefer struct Rect and pooled collections to match Rust’s avoidance of allocations.
- If you need a direct drop-in solver alternative and Cassowary/Kiwi are unsuitable, search for “constraint solver .NET” or “linear programming .NET” NuGet packages and evaluate for interactive/real-time layout performance.

