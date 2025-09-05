## ratatui-widgets\src\canvas\rectangle.rs-ratatui-for-other-lang-impl-notes.md

- Core concepts to map: Painter (cell/grid drawing), Shape trait, Line primitive, Color model.
- Need low-level API that can write to a terminal cell buffer (position, char/marker, foreground/background).
- Must support multiple "markers": braille, full-block, half-block, dot — research Unicode support in .NET terminals.
- Color levels to support: 8/16, 256, and truecolor (RGB) — check library color capabilities.
- Terminal capability detection (Unicode, color depth, fallback) is required.
- Coordinate transform: Ratatui uses bottom-left origin for shapes — plan world→screen mapping (most terminals are top-left).
- Line drawing & clipping (modified Bresenham + saturation) should be implemented in library or in your port.
- Grid abstraction with z-order/layers and batch rendering to minimize writes.
- Performance: batch/flush model, minimize per-cell Console calls.
- Windows specifics: consider Windows Console API / Windows Terminal differences and CRLF behavior.
- macOS/Linux: ANSI escape support; ensure chosen lib handles positioning and colors consistently.
- Libraries to evaluate in .NET: Spectre.Console (rich ANSI, colors), gui.cs/Terminal.Gui (widgets, may not expose raw cell buffer), direct System.Console + P/Invoke (Windows Console API) for lowest-level control.
- For ncurses-style approach: look for .NET ncurses bindings if targeting Unix terminals.
- Unicode/encoding: ensure Console.OutputEncoding/terminal encoding supports braille and block characters.
- Testing: verify rendering with different markers, zero-size/bounds, and minimal buffers.

(If you want, I can shortlist specific .NET packages and quick pros/cons for each.)

