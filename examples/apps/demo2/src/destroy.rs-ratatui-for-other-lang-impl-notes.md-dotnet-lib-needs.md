## examples\apps\demo2\src\destroy.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries to provide backend, buffer-based rendering, layout, widgets, events, and low‑level cell manipulation.
- Key .NET libraries to research: Terminal.Gui (gui.cs) and Spectre.Console (Ansi/markup), plus Konsole (NuGet) for lower-level control.
- For true low‑level terminal control, investigate P/Invoke to Windows Console APIs (EnableVirtualTerminalProcessing) and P/Invoke to termios/PDCurses on Unix.
- Color support: check ANSI/VT and 24‑bit (TrueColor) support in Windows 10+ and Unix terminals; how each lib exposes fg/bg and styles.
- Buffer/diffing: implement a virtual screen (cell = char + fg + bg + style) and diff algorithm; verify if libraries expose access to cell buffers or only high‑level rendering.
- Layout: look at Terminal.Gui’s layout system; if unavailable, plan constraint-based layout (percent/fixed/min/max).
- Widgets: evaluate built-in widgets in Terminal.Gui and Spectre.Console; verify support for custom widgets and stateful widgets.
- Event system: Console.ReadKey/KeyAvailable, Console.CancelKeyPress, Posix signal handling (or PosixSignalRegistration), and mouse support via libs or P/Invoke.
- Unicode/wide chars: use System.Text.Rune and grapheme cluster libraries; ensure correct column-width handling.
- Terminal modes: research raw mode handling (line buffering/echo) on Windows and Unix and how libraries manage restore on exit.
- Random/animation: use System.Random or System.Security.Cryptography.RandomNumberGenerator for effects.
- Cross‑platform testing: ensure behavior differences (Windows ConHost vs Windows Terminal vs Unix terminals) are covered by chosen libraries.

