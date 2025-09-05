## examples\apps\advanced-widget-impl\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Need: cross-platform terminal control (cursor, clear, colors, styles, size).
- Research .NET: System.Console (basic), Spectre.Console (rich styling, ANSI, truecolor).
- For higher-level UI widgets/layouts: Terminal.Gui (gui-like), Spectre.Console rendering primitives.
- Windows special handling: P/Invoke to Win32 Console API or rely on ANSI support in modern Windows+Spectre.Console.
- Unix: ANSI escape sequences via standard output or libraries above.
- Buffering/diffing: implement cell-based buffer + diff algorithm (no well-known .NET drop-in).
- Unicode handling: System.Text.Rune, StringInfo, and a wcwidth implementation (search WcWidth.NET).
- Wide/combining char support: test EastAsianWidth rules and combining marks.
- Event handling: ConsoleKeyInfo for keys; Terminal.Gui or P/Invoke/termios for mouse & resize.
- Styling: research Spectre.Console for colors, modifiers, and underline color support.
- Layout system: consider Terminal.Gui constraints or implement constraint-based layout.
- Widget model: support by-value, &ref, &mut equivalents via interfaces/ownership patterns in .NET.
- Recommendation: prototype using Spectre.Console + Terminal.Gui for coverage; implement custom buffer/diff and Unicode width logic.

