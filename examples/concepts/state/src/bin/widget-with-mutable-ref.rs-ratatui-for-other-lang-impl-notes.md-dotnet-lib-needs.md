## examples\concepts\state\src\bin\widget-with-mutable-ref.rs-ratatui-for-other-lang-impl-notes.md

- Need terminal-backend abstraction that works on Windows (Console API) and Unix (VT/ANSI).
- Raw mode, alternate screen buffer, cursor control, and cleanup support.
- Nonblocking/evented input: keyboard, mouse, and terminal-resize events.
- In-memory buffer + efficient flush/diffing to avoid flicker.
- Color support: 16 / 256 / truecolor and color-capability detection.
- Correct Unicode/grapheme-cluster handling for wide chars and combining marks.
- Layout system: constraint-based splits, percentages, and resizing behavior.
- Widget rendering model: support consuming render calls and patterns to let widgets mutate shared state (refs, delegates, ref structs).
- State-sharing patterns: immutable, shared (reference-counted), or direct mutable refs — consider .NET equivalents (ref, ref struct, delegates, Concurrent structures).
- Performance: minimize syscalls, partial redraw, and frame-rate management.
- Cross-platform terminal feature detection (terminfo/termcap or built-in heuristics).
- Dotnet libraries/keywords to research: Spectre.Console, Terminal.Gui (gui.cs), low-level P/Invoke for Windows Console / libvterm or ncurses bindings, and libraries offering buffer/diff rendering.

