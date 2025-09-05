## examples\concepts\state\src\bin\immutable-consuming.rs-ratatui-for-other-lang-impl-notes.md

- Research .NET TUI libraries: Terminal.Gui (gui.cs) for full widget system; Spectre.Console for rich output (not full TUI).
- Low-level backends: System.Console + ANSI sequences; P/Invoke Win32 Console API; ConPTY wrappers (search "ConPty .NET" / "ConPtySharp").
- ncurses/terminfo options: look for Ncurses.NET or P/Invoke bindings if targeting Unix terminfo features.
- VT/ANSI on Windows: enable ENABLE_VIRTUAL_TERMINAL_PROCESSING via SetConsoleMode (P/Invoke).
- Input handling: System.Console.ReadKey/KeyAvailable for basics; Terminal.Gui or custom P/Invoke for advanced keys/mouse.
- Mouse support: varies by backend—use Terminal.Gui or terminfo/ncurses mouse handling or Win32 console events.
- Color models: Spectre.Console supports 16/256/RGB; otherwise send ANSI sequences and detect capability (env/terminfo).
- Unicode and widths: use System.Globalization.StringInfo for grapheme clusters and an EastAsianWidth library for CJK widths.
- Buffer/diff approach: implement an offscreen grid, diff frames, and write minimal updates; use Span<T>/ArrayPool for perf.
- Layout & widgets: Terminal.Gui provides layout and stateful widgets; otherwise design trait/interface-based widgets.
- P/Invoke helper: use the PInvoke.nuget packages to simplify bindings to Win32/ConPTY/ncurses.
- Recommendation: try Terminal.Gui for quick port of widget patterns, Spectre.Console for rich rendering pieces, and implement a custom buffer+diff layer for Ratatui-like performance.

