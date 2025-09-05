## ratatui-widgets\src\chart.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libs for terminal buffer, layout, styling, Unicode, and drawing primitives for charts (scatter/line/bar) in a TUI.
- Terminal rendering & high-level TUI: evaluate Spectre.Console and Terminal.Gui (gui.cs).
- ANSI/color modes (ANSI/256/RGB): Spectre.Console or libraries that emit ANSI sequences.
- Low-level buffered drawing/double-buffering: search for buffered console renderers or implement an off-screen cell buffer.
- Layout system (rect-based): Terminal.Gui has layout containers; otherwise implement simple rect math.
- Styling (fg/bg, attributes): Spectre.Console supports rich styles.
- Unicode and grapheme handling: use System.Text.Rune and search NuGet for "unicode width" / East Asian width utilities.
- Symbols (Braille, blocks): ensure terminal/font support; no special .NET lib required.
- Canvas / coordinate transform & primitives: look for plotting/canvas in Spectre or implement mapping from data→cell coords.
- Plot primitives / markers / legends: likely need custom implementation or a plotting library that targets console.
- Enum utilities (strum equivalent): use System.Enum, attributes, and LINQ.
- Itertools equivalents: use LINQ and System.Linq.Extensions.
- No_std concept: not applicable in .NET/runtime-managed environment.
- Backend abstraction (Windows vs Unix terminals): rely on cross-platform .NET runtime and chosen console lib.
- Performance: prefer libraries or designs that minimize per-frame allocations and support batched ANSI writes.

