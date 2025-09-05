## examples\concepts\state\src\bin\mutable-widget.rs-ratatui-for-other-lang-impl-notes.md

- Need a terminal abstraction: raw mode, alternate screen, cursor control, colors, and input/event plumbing (resize, mouse, keyboard).
- On Windows explicitly research enabling VT sequences (SetConsoleMode ENABLE_VIRTUAL_TERMINAL_PROCESSING) and ConPTY APIs.
- Look for .NET libraries that expose ANSI/VT handling or wrap platform APIs.
- High-level TUI libraries to evaluate: Spectre.Console (advanced console rendering, styles, live updates), Terminal.Gui (gui.cs) (widget/view model inspired by curses).
- Low-level/Unix bindings: Ncurses.Core, Mono.Terminal — useful for terminfo/ncurses behavior.
- Event handling needs non-blocking input and mouse/resize support; check Console.KeyAvailable, ReadKeyAsync, and library support for mouse reporting (ANSI).
- Buffering/diffing: either use library features (Spectre.Console LiveRenderable) or implement an off-screen buffer + diff/paint algorithm.
- Widget system: require an interface for widgets, composition, and mutable-state rendering; Terminal.Gui already has widget abstractions.
- Unicode & width handling: use System.Text.Rune and search for wcwidth/EastAsianWidth.NET implementations or port wcwidth logic.
- Error/cleanup model: ensure try/finally or AppDomain.UnhandledException handlers to restore terminal state on crashes.
- Styling & colors: prefer libraries that emit ANSI sequences (Spectre.Console) or provide abstractions for foreground/background/styles.
- Cross-platform testing checklist: Windows Console, Windows Terminal, WSL, macOS Terminal, major Linux terminals.
- Useful search terms/nuget keywords: "dotnet ANSI terminal", "dotnet raw mode console", "dotnet alternate screen", "wcwidth .net", "ConPTY .NET", "Terminal.Gui", "Spectre.Console".
- Decide tradeoff: use high-level library (faster port, less control) vs low-level backend + custom buffer/widgets (closer to ratatui architecture).

