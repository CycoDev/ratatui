## ratatui-macros\src\lib.rs-ratatui-for-other-lang-impl-notes.md

- Spectre.Console — rich colors (16/256/RGB), tables, markup, styling APIs; strong starting point.
- Terminal.Gui (gui.cs) — higher-level cross-platform TUI with views, layout, input events.
- System.Console + P/Invoke (kernel32 SetConsoleMode) — low-level console control and enabling ANSI on Windows.
- System.Text.Rune and System.Globalization.StringInfo — built-in .NET Unicode / grapheme handling primitives.
- GraphemeSplitter (NuGet) — precise grapheme-cluster handling if needed.
- Unicode width / East Asian width libs (e.g., EastAsianWidth.NET, UnicodeWidth.NET) — measure column widths for CJK/emojis.
- Console.ReadKey / raw mode or Terminal.Gui input APIs — key handling choices.
- Console.SetCursorPosition / ForegroundColor / BackgroundColor or Spectre.Console APIs — cursor & color management.
- Buffering / dirty-render strategy — implement virtual cell buffer (no mainstream .NET library focused solely on this).
- Layout: no direct equivalent of Ratatui’s constraints; consider building a constraint engine or reuse Terminal.Gui/Spectre.Console layout facilities.
- Styling DSL: use builder patterns, fluent APIs, extension methods and interpolated strings to mimic macros.
- Cross-platform concerns: test ANSI support, color depth, terminal size detection, and input differences on Windows vs Unix.
- License and NuGet ecosystem: evaluate library licenses and maintenance before depending on them.

