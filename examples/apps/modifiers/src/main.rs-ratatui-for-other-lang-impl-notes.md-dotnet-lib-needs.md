## examples\apps\modifiers\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Need cross‑platform terminal control (raw mode, alternate screen, cursor, clear).
- Look for .NET libs that expose ANSI/VT sequences and raw I/O.
- Candidate libraries: Spectre.Console (rich styling/ANSI), Terminal.Gui (gui.cs) for widget/layout, NCurses bindings (e.g., NCurses.Core) for Unix-like behavior.
- Investigate System.Console for low‑level Console.ReadKey/KeyAvailable + P/Invoke for Windows Console API (enable VT processing).
- Must support color models: 8/16, 256, truecolor (RGB); ensure library reports terminal capabilities.
- Require input/event handling: nonblocking key events, modifiers, mouse support (Terminal.Gui covers mouse).
- Double‑buffered rendering or diffing output to minimize flicker — check library rendering model.
- Styling system: composition of fg/bg colors and modifiers (bold/italic/underline); verify ANSI SGR support.
- Layout system: grid/percentage/fixed sizing and nested layouts; Terminal.Gui has layout primitives.
- Terminal state management: alternate screen, restore on exit, handle exceptions/ProcessExit/CancelKeyPress.
- Performance: efficient buffering, minimal control sequence emission — assess profiling info or API for partial redraws.

