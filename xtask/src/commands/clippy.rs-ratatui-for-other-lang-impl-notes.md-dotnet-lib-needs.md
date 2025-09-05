## xtask\src\commands\clippy.rs-ratatui-for-other-lang-impl-notes.md

- Port need: a terminal-abstraction core plus platform-specific backends (same pattern as Ratatui).
- Essential capabilities to research in .NET: ANSI/VT support, fine-grained styling (underline, colors), cursor movement, scrolling regions, input handling, and alternate screen buffers.
- Primary .NET libraries to evaluate: Spectre.Console (ANSI, rich styling), Terminal.Gui (widget TUI), NCurses bindings for .NET (NcursesSharp / NCurses.NET) for Unix, System.Console as minimalist fallback.
- Windows-specific: plan for Win32 Console APIs via P/Invoke (or Windows Terminal-specific APIs) to get advanced features not exposed by System.Console.
- Ensure enabling Virtual Terminal Processing on Windows (for ANSI) when using ANSI-based libs.
- Backend strategy: implement core cross-platform library + separate NuGet backend adapters for Spectre.Console, Terminal.Gui, ncurses, and a Win32 backend.
- Feature toggles: use multi-targeting and runtime OS detection (RuntimeInformation/OSPlatform) or compile-time constants.
- Version compatibility: test multiple library versions and .NET target frameworks in CI (matrix builds).
- Testing: use xUnit/NUnit + integration tests on Linux/macOS/Windows runners; containerize Linux tests.
- Packaging: keep ratatui-core equivalent as platform-agnostic NuGet, backends as optional NuGets with feature flags.
- Focus your initial research on Spectre.Console, Terminal.Gui, NCurses .NET bindings, and strategies for Win32 console features.

