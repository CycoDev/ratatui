## ratatui\tests\widgets_block.rs-ratatui-for-other-lang-impl-notes.md

- Research cross-platform terminal libraries: Spectre.Console (rich ANSI, colors), Terminal.Gui (gui-like), .NET System.Console (basic).
- Look for libraries that expose alternate screen & raw mode (ANSI escape support / ConPTY on Windows).
- Find Windows-specific options: ConPTY, Windows Console API via P/Invoke, or libraries that wrap them.
- Input/event handling: Console.ReadKey/ConsoleKeyInfo, but prefer libraries with async key/mouse/resize events.
- ANSI/VT100 support: ability to write raw escape sequences and capture mouse/resize events.
- Unicode/grapheme support: System.Text.Rune, System.Globalization.StringInfo, and NuGet GraphemeSplitter.
- Character width (wcwidth): search for WcWidth.NET or EastAsianWidth implementations.
- Box-drawing/Unicode support: ensure library preserves codepoints and supports wide characters/emojis.
- Color models: 16/256/RGB support; confirm library supports truecolor.
- Buffer abstraction: ability to render into an in-memory buffer before writing to terminal (for testing).
- Test facilities: virtual/ test backends or ways to capture ANSI output for assertions (Spectre.Console testing helpers).
- Performance: batched writes / minimal terminal I/O APIs.
- Modular backend design: choose libraries that allow swapping raw output backend (ANSI vs Windows API).
- NuGet packages to search: Spectre.Console, Terminal.Gui, WcWidth.NET, GraphemeSplitter, ConPtySharp.
- Prioritize cross-platform maintenance and active community/support.

