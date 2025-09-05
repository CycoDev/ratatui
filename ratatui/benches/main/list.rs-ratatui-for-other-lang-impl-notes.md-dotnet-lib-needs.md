## ratatui\benches\main\list.rs-ratatui-for-other-lang-impl-notes.md

- Benchmarking: BenchmarkDotNet (equivalent to Criterion)
- Fake data generation: Bogus (equivalent to fakeit)
- Buffer/differential rendering model: no direct standard lib — evaluate Terminal.Gui (gui.cs) buffer APIs or implement a Buffer abstraction using Span<T>/arrays
- Layout/Rect primitives: Terminal.Gui provides layout; otherwise define Rect struct
- Widget toolkit (List, selection, scrolling): Terminal.Gui or build widgets atop Spectre.Console
- Rich styling & colors (8-bit/24-bit): Spectre.Console supports advanced colors and styles
- Low-level terminal backends: System.Console + ANSI; on Windows consider native Console APIs for legacy support
- Input handling (keys, resize, mouse): Terminal.Gui or use Console/termios wrappers
- Unicode/grapheme support: System.Text.Rune and set Console.OutputEncoding = UTF8
- Performance techniques: use Span<T>, pooled buffers, minimal allocations
- State management: keep separate state object for selection/scroll offset (mirror ListState)
- Testing frameworks: xUnit / NUnit
- Summary recommendation: evaluate Terminal.Gui for full widget set; Spectre.Console + custom buffer for a lightweight immediate-mode port; use BenchmarkDotNet + Bogus for benchmarks and fake content.

