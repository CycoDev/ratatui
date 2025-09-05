## xtask\src\commands\format.rs-ratatui-for-other-lang-impl-notes.md

- Architecture: separate core, widgets, backends — map to separate .NET projects/libraries.
- Terminal/backends: evaluate Spectre.Console (rich rendering, ANSI), Terminal.Gui (gui-style TUI), and lower-level System.Console + native/terminfo bindings for advanced control.
- Input/event model: Terminal.Gui provides event loop; otherwise use System.Console.ReadKey / KeyAvailable or a library that exposes async key/mouse events.
- Unicode & widths: rely on System.Text.Rune + System.Globalization.StringInfo; search NuGet for "unicode width" or "EastAsianWidth" helpers for display width handling.
- Layout & rendering primitives: Spectre.Console supports panels/tables; for custom buffered drawing implement a double-buffer with Span/Memory for performance.
- Cross-platform terminal features: ensure ANSI support (Windows 10+), consider supporting different backends per OS.
- CLI parsing: use System.CommandLine (modern) or CommandLineParser.
- Process execution (formatters/externals): System.Diagnostics.Process.
- Code formatting tool equivalent: dotnet format.
- TOML handling/formatting: Tomlyn (NuGet) or Tomlet.
- Error/context reporting: use exceptions + logging libraries (Serilog) or Sentry for diagnostics.
- Testing & CI: xUnit/NUnit + GitHub Actions for multi-OS runs.
- Build/tooling: use SDK-style solution, NuGet for dependencies, separate projects for core/widgets/backends.

