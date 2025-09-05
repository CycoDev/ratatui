## xtask\src\commands\rdme.rs-ratatui-for-other-lang-impl-notes.md

- Keep architecture: split platform-agnostic core (rendering, layout, widgets) from platform-specific backends — implement as separate NuGet packages.
- Research Spectre.Console (rich ANSI rendering, colors, styling) as the primary cross-platform backend.
- Research Terminal.Gui only if you want a higher-level TUI widget toolkit (heavier, opinionated).
- For low-level Unix terminal control (raw mode, termios): Mono.Posix.NETStandard or P/Invoke to libc (termios).
- For Windows console and ConPTY: Vanara.PInvoke or other Win32/ConPTY PInvoke wrappers (or ConPty.NET) to access advanced features.
- Consider ncurses bindings (NCurses.Core / NCursesSharp) if targeting traditional Unix terminals and ncurses ecosystem.
- Unicode and grapheme/width handling: use System.Text.Rune + System.Globalization.StringInfo and research UnicodeWidth.NET / EastAsianWidth.NET for cell width calculations.
- For enabling ANSI/VT on Windows: either use Spectre.Console or call SetConsoleMode to enable ENABLE_VIRTUAL_TERMINAL_PROCESSING.
- For input handling (raw key events, async): System.Console.ReadKey/KeyAvailable for simple cases; use P/Invoke termios/Win32 for true raw mode and control sequences.
- For buffering and rendering pipelines: consider System.IO.Pipelines or System.Threading.Channels for efficient diffed rendering.
- For terminal size and cursor control: Console APIs are basic; prefer native calls (ioctl on Unix, GetConsoleScreenBufferInfo on Windows) for reliability.
- Use MSBuild multi-targeting + RuntimeInformation.IsOSPlatform and conditional PackageReferences to provide multiple backend implementations and let users choose.
- Detect color/capabilities at runtime (ANSI level) — research libraries or implement detection heuristics.
- Suggested minimal path: implement core logic in pure .NET, provide a Spectre.Console backend for fast cross-platform parity, add P/Invoke-based backends for advanced platform-specific features.

