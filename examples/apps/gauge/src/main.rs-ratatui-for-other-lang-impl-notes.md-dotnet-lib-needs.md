## examples\apps\gauge\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Core model: immediate-mode UI with double-buffering + diffing — look for .NET libs that let you control buffering or expose a cell/grid.
- Terminal abstraction: need backends for Win32/Windows Console/VT100 and Unix (ncurses/ANSI).
- Buffer/Cell: cell = glyph + fg/bg + attrs; essential for minimal redraws.
- Widgets to support: blocks, paragraphs, lists, tables, gauges, charts.
- Layout: constraint-based/grid layout engine (absolute & relative sizing).
- Styling: must support 16/256/RGB colors and text attributes (bold/underline/reverse).
- Unicode: wide/double-width, combining chars, emoji — check library handling.
- Input/events: raw mode, key mapping, mouse, poll/timeout-based event loop.
- Platform quirks: Windows vs Unix color and Unicode behavior; Windows Terminal improvements matter.
- Error recovery: library or pattern to restore terminal state on crash.
- Performance: ability to batch writes, minimize cursor moves, and do diffs.
- Gauge example: need block/partial-block drawing (Unicode and ASCII fallbacks).
- .NET libraries to research first: Spectre.Console, Terminal.Gui (gui.cs), SadConsole; also search for ncurses P/Invoke wrappers or ANSI/WinAPI drivers.

