## ratatui-widgets\src\canvas\circle.rs-ratatui-for-other-lang-impl-notes.md

- Primary needs when porting to .NET: terminal I/O & capability detection, Unicode glyph rendering (braille/blocks), color/ANSI control, trig/math support, and allocation-free/perf techniques.
- High-level terminal libraries to evaluate: Spectre.Console (ANSI, colors, richer output) and Terminal.Gui (full-screen TUI widgets) — Spectre.Console is best for ANSI and color control; Terminal.Gui is higher-level and not geared to pixel glyph rendering.
- Low-level/legacy bindings: consider Ncurses/.NET bindings (NcursesSharp, NCurses.Core) for terminfo-backed behavior on Unix; on Windows use Win32 Console APIs via P/Invoke when you need features Spectre.Console doesn’t expose.
- Unicode/encoding: ensure Console.OutputEncoding = Encoding.UTF8; .NET System.Text.Rune supports working with Unicode code points (braille block range U+2800).
- Braille/half-block rendering: no mainstream .NET library for terminal “pixel” rendering — implement mapping to Unicode braille (⢀..⣿) and block characters yourself and render strings via Console or Spectre.Console.
- ANSI/VT on Windows: enable virtual terminal processing with SetConsoleMode (ENABLE_VIRTUAL_TERMINAL_PROCESSING) via P/Invoke if not using a library that already does it.
- Color support: use Spectre.Console for SGR sequences and 24-bit color; otherwise emit ANSI sequences manually for truecolor/fallbacks.
- Math/trig: .NET provides System.Math and MathF (sin/cos, etc.). If targeting constrained runtimes (nanoFramework, embedded), plan to supply small polyfills for sin/cos/mul_add or implement approximations.
- Precision/ops: MulAdd (fused multiply-add) may not exist — implement a*b + c when needed; use float (MathF) for performance when acceptable.
- Text cell width and grapheme handling: use System.Text.Rune and implement or reuse a wcwidth/wide-character library (there are NuGet packages for wcwidth behavior) to compute cell widths and clipping correctly.
- Performance & allocations: use Span<T>, stackalloc, ArrayPool<T>, structs and pooled buffers to avoid heap allocations in the hot rendering path.
- Painter abstraction: create an IPainter and multiple backends (BraillePainter, HalfBlockPainter, CharGridPainter) so you can switch rendering strategy based on terminal capabilities.
- Capability detection: inspect TERM env var, test Console.OutputEncoding, and detect Windows/Unix; fallback heuristics: ability to print braille glyphs + font support, or presence of 24-bit color/ANSI sequences.
- Testing strategy: capture Console output (or writer abstraction), normalize ANSI/Unicode, and create separate expected outputs per rendering mode (braille/half/block).
- Terminal control & cursor: use Console.SetCursorPosition for simple cases; for full control or performance batching, write raw ANSI CSI sequences or use Spectre.Console APIs.
- When targeting embedded/.NET runtimes without System.Math or Console, treat this file as guidance for capabilities to reimplement (math polyfills, a minimal painter writing to a framebuffer or device), otherwise rely on core .NET APIs + Spectre.Console/Ncurses bindings.

