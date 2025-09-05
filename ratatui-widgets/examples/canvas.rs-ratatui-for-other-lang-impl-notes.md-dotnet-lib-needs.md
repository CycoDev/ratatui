## ratatui-widgets\examples\canvas.rs-ratatui-for-other-lang-impl-notes.md

- Need a terminal backend abstraction: search for .NET libraries that provide cross-platform cursor control, raw mode, alternate screen, and style/colour output.
- Key .NET libraries/terms to evaluate: "Terminal.Gui (gui.cs)", "Spectre.Console", "ncurses .NET bindings", "termbox .NET binding", "ConPTY / CreatePseudoConsole .NET".
- Unicode support: must handle Braille U+2800–U+28FF and block chars U+2580–U+259F; verify font/terminal support (Windows Terminal, iTerm2, xterm).
- ANSI vs native APIs: on Windows check Virtual Terminal Processing and ConPTY; on Unix prefer ANSI/ncurses/backends.
- Color model: need RGB, 256-color, and 16-color fallbacks and conversions; check library support for truecolor.
- Event handling: keyboard and mouse input with non-blocking reads and key event abstractions.
- Alternate screen & raw mode: ability to enter/exit alt screen and disable line buffering.
- Efficient buffering: double-buffer or in-memory canvas to minimize terminal I/O; look for libraries exposing low-level write control.
- Glyph-level rendering: support composing characters per cell (braille 2x4, half-block 1x2, 1x1 char grid).
- Coordinate mapping: floating-point logical coords → terminal cells, custom bounds/viewport transforms.
- Drawing primitives needed: line (Bresenham), circle, rectangle, fill, point plotting — search for existing geometry/graphics helpers.
- Layering: ability to render multiple overlapping buffers/layers with z-order and color blending.
- Platform testing targets: conhost, Windows Terminal, macOS Terminal/iTerm2, common Linux terminals.
- Performance: minimize writes, batch ANSI sequences, prefer libraries with buffered rendering or raw access.

