## examples\apps\inline\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Goal: map Ratatui features to .NET libraries/APIs you must research.
- Terminal control (ANSI + cursor, colors): System.Console + enable VT via SetConsoleMode (P/Invoke) or use Spectre.Console.
- Alternate screen / raw mode: use ANSI alt-buffer sequences or P/Invoke to Win32 Console API; on Unix consider Mono.Posix termios or libc via P/Invoke.
- Cross-platform high-level TUI libs: evaluate Spectre.Console and Terminal.Gui (gui.cs).
- Low-level ncurses-style option: look for Ncurses.NET / P/Invoke ncurses bindings.
- Non-blocking input/events: use async Console.ReadKey in Task, or libraries like System.Threading.Channels, TPL Dataflow, or Rx.NET for event streams.
- Threading & channels: System.Threading.Channels maps well to Rust channels for worker events.
- Rendering & widgets: Terminal.Gui provides widget/layout system; Spectre.Console offers live updates and progress widgets.
- Buffering/diffing: implement double-buffer or use library features; investigate Spectre.Console "LiveDisplay" semantics.
- Unicode / wide chars: use System.Text.Rune and StringInfo; test East Asian width behavior.
- Inline viewport (fixed-height region + insertion above): requires cursor positioning, ANSI insert-line (ESC[Ln]) and scrolling support; ensure VT enabled on Windows.
- Panic/restore handling: ensure try/finally or AppDomain.UnhandledException to restore console modes.
- Recommendation: start with Spectre.Console (rich control) and Terminal.Gui for full widget/layout; fallback to low-level P/Invoke for missing raw-mode/VT details.

