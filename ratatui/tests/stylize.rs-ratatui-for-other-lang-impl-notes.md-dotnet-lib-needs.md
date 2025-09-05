## ratatui\tests\stylize.rs-ratatui-for-other-lang-impl-notes.md

- Focus: stylize tests show requirements for terminal styling, buffer rendering, and virtual backends — all relevant when porting to .NET.
- Need cross-platform terminal backend: research Spectre.Console (rich styling, testing support) and Terminal.Gui (gui.cs) for higher-level widgets.
- Look for ANSI/VT support & Windows enablement (SetConsoleMode / VT processing).
- Virtual/test backend: Spectre.Console.Testing or ability to capture virtual console output for assertions.
- Buffer-based rendering + diffing: implement off-screen buffer and send only deltas; check Spectre.Console live rendering primitives.
- Styling API: fluent chains for colors, backgrounds, attributes — Spectre.Console already supports this.
- Unicode width/CJK handling: search for EastAsianWidth.NET or libraries exposing wcwidth; ensure correct grapheme/width handling.
- ANSI capabilities detection and fallbacks: terminal color level detection libraries or implement capability probing.
- Box-drawing and glyph fallbacks: handle Unicode vs ASCII fallbacks and font/terminal support.
- Test strategy: render to virtual buffer, build expected buffer (chars + styles), compare.
- Low-level alternatives: termbox-sharp, ncurses bindings if you need lower-level control.
- Windows specifics: consider Windows Console API differences vs VT100 and test on legacy consoles.

