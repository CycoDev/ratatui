## ratatui-widgets\src\table.rs-ratatui-for-other-lang-impl-notes.md

- Goal: map Ratatui components to .NET libraries when porting a terminal Table widget.
- Buffer/cell grid: Terminal.Gui (gui.cs) ConsoleDriver / View drawing APIs; Ncurses.NET bindings for lower-level cell buffers.
- High-level rendering: Spectre.Console (IRenderable, Ansi output) for styled output and tables.
- Layout/constraints: Spectre.Console Layout features or Terminal.Gui View positioning; otherwise implement custom constraint solver.
- Styles/colors/attributes: Spectre.Console (ANSI), Terminal.Gui (Theme/Colors).
- Unicode width & grapheme handling: System.Text.Rune + StringInfo, GraphemeSplitter (NuGet), UnicodeWidth.NET (NuGet).
- Widget/state management: Terminal.Gui View/Control model maps to StatefulWidget; Spectre.Console has render pipeline but less stateful.
- Alternate screen/raw mode/mouse: Terminal.Gui (uses curses drivers), Spectre.Console supports alternate screen and input handling.
- Cell-based rendering API: Terminal.Gui ConsoleDriver exposes per-cell operations; prefer for pixel-like control.
- Iterator utilities: use LINQ and Span<T>/Memory<T> for performance; no itertools needed.
- Memory model: .NET managed; use Span/Memory for low allocations.
- Recommendations to research first: Spectre.Console, Terminal.Gui (gui.cs), GraphemeSplitter, UnicodeWidth.NET, Ncurses.NET.

