## ratatui-core\src\symbols\marker.rs-ratatui-for-other-lang-impl-notes.md

- Research Spectre.Console for rich Unicode, braille/box chars and color/ANSI support.
- Look into Terminal.Gui for higher-level TUI widgets.
- Ensure Console.OutputEncoding = UTF8 and test braille/block glyphs.
- On Windows, enable VT processing (SetConsoleMode) for ANSI sequences.
- Consider ncurses/termcap bindings (NCurses.NET) for Unix portability.
- Use System.Enum.Parse / TryParse or DescriptionAttribute for enum↔string.
- Handle per-cell coloring: Spectre.Console supports foreground/background mixing.
- No special lib needed for braille mapping; implement bit-to-codepoint conversion.
- Detect terminal size via Console.WindowWidth/Height; aspect-ratio requires heuristic.
- Use System.Buffers, Span<T>, StringBuilder to reduce allocations for performance.
- Provide fallbacks (dot/block) if glyphs unsupported or fonts render poorly.

