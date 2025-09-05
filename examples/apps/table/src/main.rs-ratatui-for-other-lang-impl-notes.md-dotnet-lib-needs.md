## examples\apps\table\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Goal: identify the runtime/utility libraries and features you must research in .NET to port Ratatui (Rust TUI) behavior.

- Backend abstraction: need a pluggable terminal backend API (draw, cursor, clear, size, raw mode, alternate screen). Research how to implement per-platform backends in .NET (Windows vs Unix).

- Windows console API: investigate P/Invoke to Windows Console (ReadConsoleInput, SetConsoleMode, WriteConsoleOutput) or higher-level wrappers.

- Unix terminal control: research termios/tty control via Mono.Posix or P/Invoke to libc for raw mode and alternate screen.

- ANSI / truecolor / 256-color support: find .NET libraries that emit and parse ANSI escape sequences and support RGB/256-color with fallbacks (Spectre.Console is a strong candidate).

- High-level rendering libraries: evaluate Spectre.Console and Terminal.Gui for widget/layout capabilities and event handling; compare feature fit (Spectre.Console = rich text & ANSI; Terminal.Gui = immediate-mode, higher-level widgets).

- Low-level ncurses alternatives: research NcursesSharp or Mono.Terminal if you need a curses-style backend on Unix.

- Unicode width and grapheme handling: use System.Text.Rune and check libraries like Wcwidth.NET or any .NET port of wcwidth for accurate column width of Unicode, emoji and combining marks.

- Color conversion/palettes: research ColorMine or Colourful (C# libs) for palette and color conversions; check if Spectre.Console covers needed color features.

- Input/event handling: need keyboard (incl. modifiers) and mouse capture cross-platform. Compare how Spectre.Console, Terminal.Gui, or direct Console APIs deliver key & mouse events.

- Double-buffered rendering and partial updates: plan a buffer diffing strategy; research whether Spectre.Console or Terminal.Gui exposes APIs for partial redraws or if you must implement your own back-buffer.

- Stateful widgets: ensure chosen UI lib or your framework can hold widget state (selection, scroll offset) and allow custom renderers for components like tables and scrollbars.

- Layout system: implement or reuse a layout algorithm (rect splitting, constraints). Check Terminal.Gui for layout helpers; otherwise implement your own.

- Alternate screen & cleanup: verify library support for entering alternate screen and guaranteed cleanup (restore normal mode on exit or crash).

- Event loop & threading: map an input/tick/event loop model to .NET (Tasks, async/await); check examples in target libraries.

- Testing/platform quirks: research terminal capabilities on Windows (ConHost vs Windows Terminal), WSL, and common Unix terminals; plan fallbacks for limited terminals.

- Minimal dependency set: if you want low-level control, combine: P/Invoke (Windows/Unix), Wcwidth.NET, Colourful/ColorMine, and your own rendering + buffering; otherwise prefer Spectre.Console or Terminal.Gui and augment where they lack features.

- Prioritize research: Spectre.Console, Terminal.Gui, Wcwidth.NET, Mono.Posix (termios), Windows Console P/Invoke, Colourful/ColorMine, NcursesSharp/Mono.Terminal.

