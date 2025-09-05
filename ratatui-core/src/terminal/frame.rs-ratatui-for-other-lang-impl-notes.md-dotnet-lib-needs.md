## ratatui-core\src\terminal\frame.rs-ratatui-for-other-lang-impl-notes.md

- Target .NET libraries: research Spectre.Console and Terminal.Gui (gui.cs) as primary candidates.
- Built-in: System.Console for basic cursor, color, and input APIs.
- ANSI/VT support: Spectre.Console (AnsiConsole) and Windows SetConsoleMode via P/Invoke for VT processing.
- Raw mode / alternate screen / mouse: look at Termios P/Invoke on Unix and kernel32 Console APIs on Windows; Terminal.Gui abstracts drivers.
- Widget system: Terminal.Gui provides composable widgets; study its driver/renderer separation.
- Double-buffering/diffing: Spectre.Console "Live" features and Terminal.Gui internal buffer rendering approaches.
- Unicode graphemes: System.Globalization.StringInfo and System.Text.Rune for code points; verify grapheme cluster libs on NuGet.
- Width calculations: search for wcwidth/EastAsianWidth ports on NuGet (wcwidth implementations for .NET).
- Color capability detection: Spectre.Console detects ANSI/color support; Windows Terminal support requires enabling VT.
- Performance: investigate Span<T>/Memory<T> usage patterns and efficient buffer grids in .NET.
- P/Invoke crates replacement: plan for cross-platform P/Invoke wrappers for low-level terminal ops.
- Recommendation: prototype with Spectre.Console for rich output and Terminal.Gui for full widget set; supplement with wcwidth and grapheme libs as needed.

