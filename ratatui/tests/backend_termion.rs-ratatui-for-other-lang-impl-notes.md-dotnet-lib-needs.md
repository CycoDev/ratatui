## ratatui\tests\backend_termion.rs-ratatui-for-other-lang-impl-notes.md

- Implement a backend interface (IBackend) so multiple backends can be swapped.
- Differential rendering: track previous frame grid and send updates only for changed cells.
- Required terminal ops: cursor positioning (Goto), hide/show, clear, write text.
- Styling: foreground/background colors, text modifiers, and explicit reset commands.
- Terminal size: need rows/cols (pixel size optional).
- Platform handling: runtime or compile-time selection; Windows needs ANSI/VTP or native API.
- Windows note: enable Virtual Terminal Processing (SetConsoleMode) for ANSI escapes.
- Minimize cursor moves and batch writes for performance.
- Ensure cleanup on exit: show cursor, reset styles.
- Testing approach: capture output stream (StringWriter) to assert escape sequences and incremental updates.
- .NET libraries to research: Spectre.Console (ANSI, rich rendering), Terminal.Gui (higher-level TUI), ncurses/PDCurses .NET bindings (unix-like), and raw System.Console + enabling ANSI on Windows.
- Also look for libraries exposing low-level ANSI control if you need exact escape sequences rather than high-level widgets.

