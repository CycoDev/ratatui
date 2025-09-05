## examples\concepts\state\src\lib.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries and API patterns to replicate Ratatui's cross-platform terminal UI.
- Key candidates to evaluate: Spectre.Console, Terminal.Gui (gui.cs), Tmds.Terminal.
- Native/low-level building blocks: System.Console (limitations), Mono.Posix.NETStandard (termios), ncurses-sharp bindings.
- Windows specifics: P/Invoke SetConsoleMode / ENABLE_VIRTUAL_TERMINAL_PROCESSING to enable VT sequences.
- Unix specifics: termios for raw mode, TERM/terminfo for capability detection.
- Raw mode & alternate screen: research libraries or use termios + ANSI (SMcup/SMcup) sequences.
- Event handling: non-blocking input, normalize key codes (arrows, function keys, Esc/q).
- Unicode: ensure Console.OutputEncoding = UTF8 and Windows UTF-8 / VT support.
- Drawing model: implement double-buffering + diffing or use library that minimizes redraws.
- Terminal capabilities: detect colors, styles, truecolor vs limited palettes.
- Resize handling: Console.WindowSize, SIGWINCH or equivalent cross-platform notifications.
- Architecture: keep core UI platform-agnostic and provide pluggable backends for System.Console/WinAPI/termios.

