## examples\apps\mouse-drawing\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Look for cross‑platform terminal backends (abstract Windows Console vs ANSI/VT on macOS/Linux).
- Must support enabling/disabling raw mode (no line buffering/echo).
- Must allow enabling alternate screen buffer.
- Must support enabling/disabling mouse capture and reporting (click/drag events).
- Unicode cell rendering and full-width character support.
- Foreground/background color models: RGB, 256, ANSI indexed.
- Efficient cell/grid rendering with diffing or double‑buffering.
- Non‑blocking event loop and event multiplexing (keyboard + mouse + timers).
- APIs to control cursor position, hide/show cursor, and clear regions.
- Ability to query terminal size and handle resize events.
- Robust cleanup on exit/crash and signal handling hooks.
- Platform-specific needs: on Windows ensure VT mode (Console API) support or wrapper.
- Search keywords: ".NET terminal gui", "ANSI/VT100 .NET", "console raw mode .NET", "mouse capture terminal .NET".
- Also plan to port Bresenham line drawing and cell-based rendering logic.

