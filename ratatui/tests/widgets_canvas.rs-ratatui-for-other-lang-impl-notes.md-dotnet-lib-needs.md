## ratatui\tests\widgets_canvas.rs-ratatui-for-other-lang-impl-notes.md

- Research cross-platform terminal libraries: Spectre.Console, Terminal.Gui, and raw System.Console (ANSI support).
- Check Spectre.Console.Testing or similar for a virtual/fake console buffer for unit tests.
- Look for ANSI/VT100 emulation libraries (for color/escape parsing and testing).
- Verify 24-bit and ANSI color support on Windows (ENABLE_VIRTUAL_TERMINAL_PROCESSING) and Unix.
- Unicode handling: System.Text.Rune, StringInfo, grapheme/width libs (East Asian width).
- Braille glyph support: Unicode block U+2800–U+28FF rendering in terminals.
- Investigate libraries for high-resolution terminal grids (braille/half-block rendering helpers).
- Buffer model: seek libraries offering cell buffers (char + style) or plan to implement one.
- Style system: color, bold/italic, background; map to Spectre.Console or ANSI attributes.
- Text measurement and alignment utilities (monospace width, combining chars).
- Testing approach: create TestBackend abstraction to capture buffer and assert cells.
- Layering & z-order: design or find rendering pipeline helpers in chosen library.

