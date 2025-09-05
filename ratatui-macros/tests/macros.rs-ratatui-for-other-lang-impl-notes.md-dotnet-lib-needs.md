## ratatui-macros\tests\macros.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: Rust macros provide a DSL to produce layout Constraints; in .NET implement as builder API or C# Source Generator.
- Core types to model: Constraint (Min, Max, Length, Percentage, Ratio, Fill) and Rect (x,y,width,height → use ushort).
- Layout engine: Rust uses "kasuari" (linear constraint solver) — search for Cassowary/CSP libs for .NET (e.g., Cassowary.NET, Kiwi) or port kasuari.
- Simpler option: implement recursive space-division algorithm (no external lib).
- Terminal UI libraries to evaluate: Spectre.Console (rich rendering, cross-platform), Terminal.Gui (gui.cs) for TUI layouts.
- Rendering/backends: System.Console + Spectre.Console handles ANSI/VT; Terminal.Gui handles platform drivers.
- Compile-time ergonomics: C# Source Generators (Roslyn) to emulate macro conveniences.
- Parsing DSL (if runtime): use parser libs like Pidgin, Superpower, or Sprache.
- Geometry types: System.Drawing.Rectangle or custom struct with ushort to match u16.
- Caching: Microsoft.Extensions.Caching.Memory or custom cache for layout results.
- Cross-platform concerns: Console size APIs, Unicode/ cell aspect ratio, Windows vs Unix terminal differences—Spectre.Console helps abstract.
- Testing: xUnit or NUnit for unit tests.
- Targets: choose .NET Core/.NET 6+; for constrained environments consider .NET nanoFramework/.NET Standard.
- Performance: prefer structs, Span<T>, minimal allocations for hot layout code.

