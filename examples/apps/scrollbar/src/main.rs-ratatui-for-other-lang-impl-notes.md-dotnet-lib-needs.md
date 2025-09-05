## examples\apps\scrollbar\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Backend abstraction: research Terminal.Gui (Gui.cs) and Spectre.Console as primary .NET options; also look for ncurses/pdcurses bindings via P/Invoke.
- Raw mode: on Windows use SetConsoleMode (kernel32), on Unix use termios (Mono.Posix or P/Invoke).
- Unicode: ensure Console.OutputEncoding = UTF8 and test wide/glyph widths (System.Text.Rune).
- Color: verify 16/256/truecolor support; Spectre.Console advertises rich color handling.
- Input events: Terminal.Gui provides normalized key/mouse events; Console.ReadKey is limited.
- Mouse support: Terminal.Gui supports mouse; check Spectre.Console for limited mouse features.
- Buffer-based rendering: prefer libraries with double-buffering or diff-based redraw (Terminal.Gui, Spectre.Console live rendering).
- Widgets & layout: Terminal.Gui has retained widgets/layout; for immediate-mode look for any “tui” .NET projects or build custom widget layer.
- Performance: minimize writes, batch flushes, and prefer libraries with efficient diffing.
- Platform gaps: compare Windows vs Unix feature parity and nuget maintenance/activity.
- Interop needs: plan P/Invoke for low-level terminal ops if library gaps exist.

