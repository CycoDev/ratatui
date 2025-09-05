## ratatui-macros\tests\ui\fails.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: tests failing macro usages — not directly a deps list, but highlights runtime concerns when porting.
- Core pieces to research in .NET: layout engine, terminal backend, text/styling, input/event handling, buffer-based rendering, Unicode support.
- Terminal/backends: System.Console (builtin basics), Spectre.Console (rich text, ANSI, live rendering), Terminal.Gui (gui.cs) for full TUI widgets and mouse/events.
- Layout engine: consider Yoga.NET (Flexbox-style layout) for proportional/min/max/fill constraints.
- Styling/markup: Spectre.Console provides spans, colors, bold, markup parsing.
- Low-level POSIX/Windows: Mono.Posix.NETStandard for unix ioctl/raw mode; P/Invoke or Windows Console APIs for advanced Windows features.
- Mouse/input/raw mode: Terminal.Gui handles mouse; otherwise use Console.ReadKey, Console.KeyAvailable, and platform P/Invoke for raw modes.
- Terminal capability detection: Console APIs + Spectre.Console/ANSI detection for color/VT support.
- Buffering/diff rendering: implement a virtual buffer + diff apply (Terminal.Gui and Spectre.Console examples exist).
- Unicode/encoding: use UTF8 Encoding (Console.OutputEncoding), normalize grapheme clusters when measuring.
- Source-level ergonomics: replace Rust macros with C# fluent builders, extension methods or Roslyn source generators.
- Recommended approach: build platform-agnostic core (layout, widgets, spans), then backend adapters using Spectre.Console or Terminal.Gui; add P/Invoke backends only if needed.

