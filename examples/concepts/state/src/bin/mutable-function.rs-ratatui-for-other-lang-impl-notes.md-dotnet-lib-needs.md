## examples\concepts\state\src\bin\mutable-function.rs-ratatui-for-other-lang-impl-notes.md

- Goal: map Ratatui concerns to .NET libraries (terminal control, input, rendering, buffering, widgets).
- Cross-platform default: consider Spectre.Console (rich ANSI rendering, styling, UTF‑8) as primary backend.
- Higher-level widget/windowing: Terminal.Gui (gui.cs) for full TUI widgets and layouts.
- Low-level tty control (raw mode, termios): use Mono.Posix / P/Invoke to termios on Unix and Windows Console API (Kernel32) on Windows.
- Alternate screen buffer: emit ANSI sequences ("\x1b[?1049h"/"\x1b[?1049l") or use library support if provided.
- Non-blocking input/event loop: Console.KeyAvailable / async reads, or rely on library event systems (Terminal.Gui/Spectre.Console live updates).
- UTF-8/wide chars: set Console.OutputEncoding = UTF8 and on Windows call SetConsoleOutputCP(65001) if needed.
- Double-buffering / diffing: implement a frame buffer and minimal diff renderer or leverage Spectre.Console/Terminal.Gui internals.
- Styling and cell model: Spectre.Console provides style primitives; build a cell abstraction if porting Ratatui APIs.
- Cleanup/rescue on crash: use try/finally, AppDomain.ProcessExit, and unhandled exception handlers to restore terminal state.
- Logging/error reporting: use Serilog or Microsoft.Extensions.Logging for rich diagnostics (analogous to color_eyre).
- Backend abstraction: design an interface to swap Spectre.Console, Terminal.Gui, or native backends.
- Recommendation: prototype with Spectre.Console for simplicity, add Terminal.Gui or native backends for advanced widget needs.

