## ratatui-core\src\terminal\viewport.rs-ratatui-for-other-lang-impl-notes.md

- Viewport API: Fullscreen, Inline(height), Fixed(Rect) — need layout primitives and variant support.
- Rect: x,y,width,height — essential for layout calculations.
- Coordinate system: (0,0) top-left, x→right, y→down.
- Backend abstraction: separate platform-specific terminal I/O from rendering logic.
- Windows backend: research Windows Console API access via P/Invoke or .NET wrappers.
- Unix backend: termios + ANSI escape sequences; ensure .NET can toggle raw mode and emit ANSI.
- Double-buffering: maintain current/previous frame buffers and diff to minimize updates.
- Unicode & wide chars: handle grapheme clusters and East Asian widths (use .NET Rune APIs / Unicode libs).
- Terminal capabilities: detect colors, size, unicode support, mouse events; query at backend level.
- Raw mode & alternate screen: must support toggling raw input and alternate buffer.
- Candidate .NET libraries to research: Terminal.Gui (gui.cs), Spectre.Console, Tmds.Terminal, ncurses wrappers (if needed).
- Interop: expect some P/Invoke/native bindings for low-level features not covered by libraries.
- Practical approach: prototype with Spectre.Console/Terminal.Gui for high-level, implement a Tmds.Terminal or P/Invoke backend to match ratatui semantics.

