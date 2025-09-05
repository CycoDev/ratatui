## ratatui\benches\main\rect.rs-ratatui-for-other-lang-impl-notes.md

- Key concern: efficient Rect (x,y,width,height) iteration (rows, columns, positions).
- Terminal/backend libraries to evaluate: Spectre.Console (ANSI rendering, cross‑platform), Terminal.Gui (gui.cs) for higher‑level widgets.
- Ncurses bindings for .NET if you need curses semantics: NCursesSharp / PInvoke approaches.
- Console primitives: System.Console for size/input; Console.WindowWidth/Height and ConsoleKeyInfo for basic I/O.
- Performance primitives: Span<T>/ReadOnlySpan<T>, Memory<T>, ArrayPool<T> to avoid allocations when iterating/collecting positions.
- Prefer value types (structs) and struct enumerators to reduce GC pressure from iterators.
- Benchmarking: BenchmarkDotNet for microbenchmarks of iterators and allocations.
- Backend abstraction: design a driver interface (like Ratatui backends); check how Spectre.Console/Terminal.Gui separate rendering drivers.
- Windows specifics: research Win32 console APIs (GetConsoleScreenBufferInfo) or rely on Spectre.Console driver handling.
- Input/mouse mapping: Terminal.Gui and Spectre.Console handle higher‑level input events; investigate if you need raw coordinates.
- Tooling and tests: xUnit/NUnit and BenchmarkDotNet; use profiling/alloc tools in dotnet (dotnet-trace, dotnet-counters).
- Memory/collection patterns: support both lazy (IEnumerable/iterators) and eager (List/Array) usage; document expected sizes (16x16..255x255).

