## ratatui-macros\src\line.rs-ratatui-for-other-lang-impl-notes.md

- Spectre.Console — primary .NET library to research for rich text, spans, styles, rendering and buffering.
- Terminal.Gui — if you need higher-level TUI widgets (windows, layouts) beyond line/span rendering.
- System.Console + P/Invoke SetConsoleMode (ENABLE_VIRTUAL_TERMINAL_PROCESSING) — to enable ANSI on Windows.
- ConPTY / ConPty.NET wrappers — advanced Windows terminal backend for modern consoles.
- Wcwidth.NET or UnicodeWidth libraries — compute terminal column width for Unicode/East Asian chars.
- System.Text.Rune and System.Globalization.StringInfo — handle grapheme clusters and code points in .NET.
- ANSI escape sequences support — rely on them on Unix/macOS; detect and degrade on limited terminals.
- Spectre.Console.Rendering types — study for Span/Line/Text modeling and builder/fluent APIs.
- Use Span<T>/Memory<T>, StringBuilder and batched writes for performance-critical rendering.
- Terminal capability detection — TERM env, Console.IsOutputRedirected, OperatingSystem APIs.
- Color helper libs (Colorful.Console/Pastel) — optional simpler color abstractions.
- Testing approach — capture terminal output, run CI on Windows/macOS/Linux, include visual regression checks.

