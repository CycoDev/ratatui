## xtask\src\commands.rs-ratatui-for-other-lang-impl-notes.md

- Goal: research .NET libraries to replicate Ratatui’s backend/xtask responsibilities (terminal backends, CLI, logging, process running, error reporting).
- Terminal UI libraries to evaluate: Terminal.Gui (gui.cs) and Spectre.Console (rich output; truecolor/ANSI support).
- Cross-platform terminal abstractions: look for libraries or patterns that wrap Windows Console API + Unix termios/ANSI (no single de facto .NET equivalent to Crossterm; expect some P/Invoke).
- Windows low-level API: research SetConsoleMode / ReadConsoleInput via P/Invoke / Windows SDK.
- Unix low-level API: research termios via P/Invoke for raw mode, alternate screen, mouse capture, and async input.
- ncurses option: evaluate .NET ncurses bindings (NuGet) as a Unix backend alternative.
- ANSI VT support: ensure chosen library supports VT100/ANSI sequences, 24-bit color, alternate screen, and mouse reporting.
- Unicode/grapheme handling: use System.Text.Rune and System.Globalization.StringInfo for grapheme clusters and wide characters.
- Raw mode & terminal size: check support or plan to implement via P/Invoke to termios/Windows APIs.
- Mouse input: confirm mouse reporting support (Terminal.Gui provides mouse handling; Spectre.Console less so).
- CLI and xtask tooling analogs: System.CommandLine or CommandLineParser for CLI tasks.
- Error reporting & diagnostics: compare Serilog / Microsoft.Extensions.Logging and rich error-reporting libraries (exceptions + structured logs).
- Process execution: System.Diagnostics.Process replaces duct for running shell commands.
- Feature flags & platform checks: use conditional compilation and RuntimeInformation.IsOSPlatform for OS-specific code paths; multi-target with SDK-style projects.
- Testing & CI: create test backends, unit + integration tests, run on GitHub Actions/Azure Pipelines across platforms.
- Packaging & modularity: split into NuGet packages for core, backends, widgets to mirror crate separation.
- Recommendation: prototype with Terminal.Gui for full TUI features and Spectre.Console for rich rendering/ANSI handling; implement a small POSIX/Windows abstraction layer (termios + SetConsoleMode) where libraries lack features.

