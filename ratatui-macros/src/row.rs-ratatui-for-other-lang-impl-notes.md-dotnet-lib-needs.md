## ratatui-macros\src\row.rs-ratatui-for-other-lang-impl-notes.md

- Goal: identify .NET libs to implement Row/Cell/Text, styling, terminal backend, and Unicode/display width.
- Use Spectre.Console as primary research target: styled text, tables, ANSI/VT support, color modes (16/256/truecolor).
- Evaluate Terminal.Gui (gui.cs) for higher-level TUI widgets and event/mouse support.
- Check ncurses .NET bindings (or P/Invoke libncurses) for Unix-native behavior (raw mode, alternate screen).
- For Windows low-level control, research P/Invoke to Win32 Console APIs (SetConsoleMode, mouse) or rely on VT via Spectre.Console.
- Unicode/grapheme handling: System.Globalization.StringInfo and System.Text.Rune for graphemes/codepoints.
- Display-width libraries: search for UnicodeWidth.NET / wcwidth ports to handle CJK and emoji widths.
- Input handling/raw mode: Console.OpenStandardInput, ReadKey, or libraries that expose raw terminal input and mouse events.
- Color capability detection: ensure chosen lib can detect/handle 16/256/truecolor and fallbacks.
- Styling parity: confirm bold/italic/underline support differences across terminals.
- Architecture: design a backend abstraction layer to swap Spectre.Console, Terminal.Gui, ncurses or WinAPI implementations.
- API ergonomics: implement fluent/builder patterns or helper methods to mimic Rust macro convenience (Row/Cell factories).
- Test across Windows (native & WSL), macOS, Linux terminals for ANSI support and width rendering.

