## ratatui-core\src\style\color.rs-ratatui-for-other-lang-impl-notes.md

- Look for .NET terminal UI libs that already handle ANSI/truecolor/256-color and fallbacks: Spectre.Console (best fit), Pastel, Colorful.Console.
- Spectre.Console: truecolor, 256-color, rich styling, abstracts platform quirks — top candidate.
- Pastel: simple string coloring with truecolor support and fallbacks.
- Colorful.Console: easy color output, but check truecolor/256 support.
- Terminal capability detection: RuntimeInformation + TERM env on Unix, Windows enable VT via SetConsoleMode; Spectre.Console already manages this.
- Color parsing: hex (#RRGGBB), named colors, numeric indices — use SixLabors.ImageSharp or Colourful for parsing and color structs.
- Color conversions (HSL, HSLuv): Colourful, ColorMine, or Hsluv.Net (HSLuv implementation).
- Palette / nearest-index mapping (24-bit → 256/16): Colourful or implement nearest-color using deltaE.
- Serialization: System.Text.Json or Newtonsoft.Json for config/serde equivalents.
- Cross-platform caveats: Windows console old versions lack truecolor — plan fallbacks.
- Underline-color: rare in terminals; verify backend support (Spectre.Console docs).
- Testing: validate on Windows 10+, macOS Terminal/iTerm2, and common Linux terminals.
- Performance: prefer lightweight structs (RGB+kind) and avoid heavy deps unless needed.

