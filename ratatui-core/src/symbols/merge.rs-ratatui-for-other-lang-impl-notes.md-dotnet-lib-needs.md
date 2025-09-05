## ratatui-core\src\symbols\merge.rs-ratatui-for-other-lang-impl-notes.md

- Goal: implement merging of Unicode box‑drawing characters where borders intersect; porting needs efficient Unicode handling + symbol maps.
- Unicode handling: research System.Text.Rune, System.Text.Encoding, and System.Globalization for robust Unicode/scalar handling in .NET.
- Box‑drawing mapping: plan static lookup tables (ReadOnlySpan<char>/ReadOnlyMemory<char>, static arrays, or ImmutableDictionary<Rune,byte/flags>).
- Small-string/compact storage: investigate ValueStringBuilder, string.Create, System.Memory/Span<T>, and ArrayPool<T> for low‑allocation short string storage.
- no_std equivalent: use Span/Memory + stackalloc, avoid GC allocations where possible; consider System.Buffers and ArrayPool for constrained scenarios.
- Error types: Rust's thiserror ≈ custom Exception types, or use Result-like libs (CSharpFunctionalExtensions, OneOf, LanguageExt) for non‑exception error handling.
- Macro/codegen replacement: use Roslyn Source Generators or a build‑time tool to generate large mapping tables instead of Rust macros.
- Fuzzy matching: research fuzzy string/character matching libs (FuzzySharp, FuzzyStrings, SimMetrics.NET) or implement a domain‑specific heuristic for box‑drawing approximations.
- Optimized lookups: compare switch expressions, dictionaries, and direct-index arrays (indexed by bitmask/enum) for speed; consider ReadOnlySpan<byte> switch tables.
- Terminal/renderer integration: evaluate Spectre.Console, Terminal.Gui, and .NET Console behavior for rendering box characters and terminal compatibility.
- Cross‑platform rendering caveat: terminals vary; test on Windows console/Windows Terminal, Linux terminals, macOS Terminal/iTerm2.
- Allocation minimization patterns: use readonly struct wrappers, Span/Rune, and avoid boxing; consider pooling and stack allocation where safe.
- Testing & fixtures: port existing symbol matrix tests; verify visual output in multiple terminal emulators and fonts.
- Performance profiling: use BenchmarkDotNet to compare lookup strategies and memory behavior.
- If you need a direct library mapping: there is no single .NET library that implements box‑drawing merge logic—expect to reimplement mappings + heuristics, using the above building blocks.

