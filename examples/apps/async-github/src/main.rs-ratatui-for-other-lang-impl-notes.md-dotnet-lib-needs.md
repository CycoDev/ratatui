## examples\apps\async-github\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries/features to replicate Ratatui layers: backend, buffer/diff, widgets, layout, input, async.
- Backend: need cross-platform terminal control (raw mode, alternate screen, cursor, colors). On Windows consider ConPTY/SetConsoleMode (P/Invoke) or use libraries that wrap them.
- Candidate .NET libraries: Terminal.Gui (gui.cs) for higher-level TUI; Spectre.Console for rich rendering and live updates; both are primary starting points.
- Low-level control: use System.Console plus P/Invoke to termios (Unix) and Windows Console API/ConPTY for raw mode and finer input.
- Buffering/diffing: implement a cell buffer and compute diffs or use Spectre.Console Live/Ansi rendering for partial updates.
- Widgets/layout: Terminal.Gui provides widgets/layout; otherwise implement a composable widget system and flexbox-like layout.
- Input handling: need raw key events, mouse sequences, multi-byte/escape sequences; Terminal.Gui handles many, or implement via low-level reads.
- Unicode: handle grapheme clusters and East Asian widths — use System.Globalization.StringInfo and libraries for EastAsianWidth.
- Async: share state via thread-safe types (ConcurrentQueue, ConcurrentDictionary, or immutable + Interlocked/Volatile) and use async/await for background tasks.
- Testing: create a test backend that records draw calls for unit tests.
- Performance: batch terminal commands, minimize writes, and cache layout; Spectre.Console helps with batching.
- Cross-platform quirks: test on Windows terminals (ConHost, Windows Terminal), macOS, Linux terminals for color/resize/mouse behavior.
- Mouse & resize: ensure chosen library supports mouse and subscribes to resize events (Terminal.Gui and Spectre.Console have varying support).

