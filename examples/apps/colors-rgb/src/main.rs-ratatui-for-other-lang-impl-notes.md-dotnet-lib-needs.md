## examples\apps\colors-rgb\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Core need: a terminal backend abstraction (draw, cursor, size, clear, events).
- Look for .NET libs that expose VT/ANSI sequences and Windows Console modes.
- Candidate libraries: Spectre.Console (rich output, 24-bit colors, ANSI), Terminal.Gui (gui.cs) (widget/layout/event system).
- For low-level control: P/Invoke SetConsoleMode/SetConsoleOutputCP on Windows and termios on Unix (or find wrappers).
- Buffered rendering: implement a screen buffer (cell = char + style) and diff before writing ANSI.
- Color support: detect/translate 16/256/24-bit; Spectre.Console helps mapping.
- Unicode/grapheme: use System.Text.Rune and System.Globalization.StringInfo; add a wcwidth port for column widths.
- Input/events: need non-blocking key polling (Console.KeyAvailable, async reads, or library event loop).
- Alternate screen & raw mode: enable virtual terminal processing on Windows; termios on Unix.
- Layout: evaluate Terminal.Gui layout engine or implement flexbox-like constraints.
- Widgets: Terminal.Gui provides many widgets; Spectre.Console has renderables but not full stateful widgets.
- Terminal capability detection: TERM env, Windows build/version, check support for true color.
- Windows-specific quirks: older consoles lack VT; consider fallback strategies.
- Start by choosing backend library (Spectre.Console or Terminal.Gui) then implement core buffer + widget layer.

