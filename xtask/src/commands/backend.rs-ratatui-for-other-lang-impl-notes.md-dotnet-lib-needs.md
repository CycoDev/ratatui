## xtask\src\commands\backend.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries that provide low-level terminal backends equivalent to Crossterm/Termion/Termwiz.
- Needed backend features: alternate screen, raw mode, cursor visibility, buffer drawing, clearing.
- Input handling: key events, mouse, resize events, sync/async event loop.
- Terminal size detection and resize notifications.
- Color/style support and ANSI vs Windows console API compatibility.
- Cross-platform: Windows (Win32/ConHost/ConHost+VT), macOS, Linux (TTY/NCurses).
- Version/feature toggles: ability to pick/compile/use different backends per platform or runtime.
- Graceful restore on exit/panic: ensure terminal state restored.
- Testing hooks: run backend-specific tests and CI across OSes.
- .NET starting points to research: System.Console (built-in basics), Spectre.Console (rich output), Terminal.Gui (higher-level TUI), ncurses bindings or P/Invoke to native console APIs.
- Evaluate: which libraries expose raw-mode and alternate-screen primitives, cross-platform VT support, and event APIs.

