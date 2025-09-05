## examples\apps\demo\src\termion.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET equivalents for raw terminal control, alternate screen, mouse, and event-driven input/tick model.
- Search for low-level terminal control libraries: termios (Unix) and Windows Console API P/Invoke wrappers.
- Enable VT/ANSI sequences on Windows (SetConsoleMode ENABLE_VIRTUAL_TERMINAL_PROCESSING).
- Look up Tmds.Terminal (low-level VT & raw mode) and ncurses bindings for .NET (NCurses.NET, P/Invoke wrappers).
- Evaluate high-level toolkits: Terminal.Gui (gui.cs) and Spectre.Console for rendering primitives and colors.
- Check mouse support availability in each library (capture, mapping to events).
- Investigate cross-platform alternatives: a backend abstraction that switches between WinAPI, termios/ANSI, or ncurses.
- Event model: research async vs threads for input reader + periodic tick (System.Threading.Thread, Task, System.Timers.Timer).
- Buffer-based drawing: verify libraries that allow off-screen buffer rendering and full-screen redraws.
- Unicode and color handling: confirm terminal/console encoding and 24-bit color support.
- Terminal resize and signals: ensure library exposes resize events or pollable size API.
- Cleanup patterns: ensure library or code can restore terminal state in finally/Dispose.
- Conditional compilation/runtime OS detection: use RuntimeInformation.IsOSPlatform or runtime identifiers (RIDs).
- Prioritize libraries that are actively maintained and cross-platform for easiest porting.

