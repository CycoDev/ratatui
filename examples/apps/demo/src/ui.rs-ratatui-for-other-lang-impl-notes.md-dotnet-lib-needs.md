## examples\apps\demo\src\ui.rs-ratatui-for-other-lang-impl-notes.md

- Need cross‑platform terminal backend (Windows + Unix) with raw mode, alternate screen, cursor control, and mouse capture.
- Look for .NET libs or native interop for raw TTY handling (termios via P/Invoke/Mono.Posix, Win32 Console APIs).
- Require color and style support (ANSI/truecolor detection and styled text APIs).
- Unicode and box/extended-graphics support (fallbacks when not available).
- Non‑blocking event loop with key/mouse events and tick timers (async/Task or event polling).
- Backend abstraction: separate low‑level terminal control from rendering logic.
- Rendering model: cell‑based double buffering or diffed frames to avoid flicker.
- Layout system: constraint‑based splits (fixed, percent, ratio) and nested layouts — likely need to implement or find a layout engine.
- Widget primitives to support: text, lists with selection, tables, charts (bars/lines/sparklines), gauges, canvas for custom drawing, tabs.
- Stateless widgets preferred; selection/state kept by application.
- Canvas/custom drawing needs coordinate mapping and low‑level cell painting.
- Consider existing .NET projects to evaluate: Spectre.Console (rich rendering, tables, charts), Terminal.Gui / gui.cs (higher-level TUI widgets), and P/Invoke or wrappers for low‑level terminal control.
- Expect native dependencies or P/Invoke for features missing in managed libraries (alternate screen, raw mode, mouse on Unix).
- Primary research targets: Spectre.Console, Terminal.Gui, and any small libraries/wrappers for raw terminal modes and ANSI truecolor support.

