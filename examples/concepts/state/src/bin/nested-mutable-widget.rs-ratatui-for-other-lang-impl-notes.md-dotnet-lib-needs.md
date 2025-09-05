## examples\concepts\state\src\bin\nested-mutable-widget.rs-ratatui-for-other-lang-impl-notes.md

- Goal: port Ratatui pattern where parent/child widgets each have mutable state and can render themselves.
- Look for .NET TUI frameworks that support widget hierarchies: Terminal.Gui (gui.cs) and Spectre.Console.
- Terminal backend equivalents to research: Windows Console API (P/Invoke), Unix termios, and any pty wrappers used by Terminal.Gui/Spectre.Console.
- Buffer-based rendering/double-buffering: check Terminal.Gui’s VirtualScreen and Spectre.Console renderables for intermediate buffer + partial redraw support.
- Event/input normalization (keyboard, mouse): Terminal.Gui provides cross-platform event model; evaluate its behavior on Windows vs Unix.
- Unicode and grapheme width: use System.Text.Rune and libraries like UnicodeWidth.NET / wcwidth ports for accurate column width.
- Colors/attributes and ANSI handling: Spectre.Console has robust ANSI and Windows support; verify capabilities for attributes and color modes.
- Mouse support: confirm Terminal.Gui or chosen lib exposes mouse events consistently.
- Layout system: prefer libs with container/layout primitives (Terminal.Gui has views/containers).
- Custom rendering API: need a Widget interface like Render(Rect, Buffer) that can mutate widget state — check extensibility points of chosen lib.
- Error/display tooling: for prettified errors consider Spectre.Console or logging libraries (Serilog) with colored output.
- Performance: evaluate library support for minimal redraw, efficient buffer ops, and large-screen performance.
- Cross-platform caveats: verify Windows console behavior, terminal capabilities (colors, attributes), and input differences.
- Recommendation: start prototyping with Terminal.Gui for full widget model; use Spectre.Console if you need rich formatted output and custom renderables.

