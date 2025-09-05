## xtask\src\commands\coverage.rs-ratatui-for-other-lang-impl-notes.md

- Note: coverage.rs describes a coverage tool (cargo llvm-cov) and LCOV output — that part is not relevant to runtime libraries for a .NET port.
- Primary need: a cross-platform terminal backend library (Crossterm equivalent) that supports raw mode, alternate screen, ANSI/VT sequences, colors, cursor control, mouse input, and resize events.
- Investigate Spectre.Console (rich console features) and Terminal.Gui (gui.cs) as high-level .NET TUI options.
- For low-level control, research ncurses wrappers (e.g., NcursesSharp) and direct P/Invoke to native APIs (Win32 Console on Windows, termios/ioctl on Unix).
- Ensure Windows support for ANSI/VT (enable Virtual Terminal Processing) or use Win32 APIs where necessary.
- Design a backend interface/abstraction layer so multiple .NET backends can be plugged in (same trait/pattern as Rust Backend).
- Use runtime OS detection (System.Runtime.InteropServices.RuntimeInformation) and/or multi-targeting for platform-specific code instead of Rust feature flags.
- Handle terminal init/restore carefully (enter/exit raw mode, restore cursor, clear alt screen) — critical for cross-platform correctness.
- Plan testing across platforms; separate unit vs integration tests and CI on Windows/macOS/Linux.
- Consider NuGet package versioning and conditional capabilities (polyfills/shims) to support multiple backend library versions.

