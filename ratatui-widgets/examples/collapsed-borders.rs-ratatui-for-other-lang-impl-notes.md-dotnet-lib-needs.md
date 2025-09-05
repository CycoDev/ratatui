## ratatui-widgets\examples\collapsed-borders.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libs that provide raw-mode terminal control, alternate screen, per-cell drawing and non-blocking input to reimplement ratatui features (collapsed borders, immediate-mode UI).
- High-level TUI to try: Terminal.Gui (gui.cs) — view/layout/event model, cross-platform.
- Rich-ANSI renderer: Spectre.Console — good for colors/ANSI; may need custom loop for immediate-mode widgets.
- Low-level: System.Console + P/Invoke to Windows Console API (Windows) and termios/ANSI sequences (Unix) for raw mode & alternate screen.
- ncurses bindings for .NET (ncurses-sharp or P/Invoke) — cell buffer control and established border glyphs.
- Verify library exposes a cell buffer or layered rendering (needed for border merging and overlap).
- Input/event loop: need non-blocking key events and arrow-key handling; Terminal.Gui provides this out of the box.
- Layout engine: look for overlap/absolute positioning or implement a small layout that supports Spacing::Overlap(1).
- Border styles: must support custom glyphs and attributes (thick vs thin border, colored border).
- Unicode & color: test 256/truecolor and box-drawing glyph support across target terminals.
- If no single lib fits, combine low-level terminal control (raw + alternate screen) with a custom immediate-mode widget renderer and event loop.

