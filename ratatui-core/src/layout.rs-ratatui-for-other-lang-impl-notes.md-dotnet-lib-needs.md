## ratatui-core\src\layout.rs-ratatui-for-other-lang-impl-notes.md

- Key native dependency to replicate: a Cassowary constraint solver (kasuari in Rust).  
- Search NuGet for .NET Cassowary ports: "Cassowary.NET", "CassowarySharp", "Kiwi" (or broadly "cassowary" / "constraint solver"). Verify support for constraint strengths/priorities.  
- LRU cache (optional): use MemoryCache or NuGet packages like "LruCache" / "LruCache.Net".  
- HashMap: use System.Collections.Generic.Dictionary or ConcurrentDictionary (no external lib needed).  
- Iterator utilities: LINQ + MoreLINQ (NuGet) cover itertools features.  
- Enum helpers: Enums.NET or custom extensions replace strum.  
- Thread-local cache: ThreadLocal<T> or AsyncLocal<T> for per-thread LRU.  
- Numeric types: use ushort for coordinates (u16 equivalent); use double for internal floats and explicit rounding rules.  
- Layout structs: implement Rect/Size/Position as lightweight structs (Span/readonly struct for perf).  
- Alignment, Margin, Spacing, Direction, Flex: plain enums/structs; use existing layout idioms.  
- Performance: consider pooled collections, struct layouts, and System.Numerics where helpful.  
- Testing: use xUnit/NUnit and property-based tests for constraint combos.  
- Verify no terminal API dependency in layout module—only geometry and solver libraries are needed.

