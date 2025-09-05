## ratatui-core\src\style.rs-ratatui-for-other-lang-impl-notes.md

- Core needs: colors (16 named, 256 indexed, 24-bit RGB), modifiers (bold/italic/underline/etc.), combined Style object.
- .NET patterns: use [Flags] enums for modifiers and extension methods for "stylize" ergonomics.
- ANSI: implement sequences directly (30–37/90–97, 40–47/100–107; 38;5;N / 48;5;N; 38;2;R;G;B / 48;2;R;G;B).
- Underline color is non‑standard (codes 58/59) — expect limited support.
- Terminal capability detection: check TERM/COLORTERM, terminfo where available, Windows version; enable VT processing on Windows via SetConsoleMode if needed.
- Backend strategy: abstract translation to escape sequences or adopt an existing backend library.
- Recommended .NET libraries to research: Spectre.Console (rich ANSI & 24‑bit support), Terminal.Gui (higher‑level TUI), Colorful.Console, Konsole, and any terminfo wrappers.
- Color conversions/palettes: ColorMine or HSLuv.NET for HSL/HSLuv support.
- Serialization: System.Text.Json or Newtonsoft.Json for style persistence.
- Windows legacy: implement VT enablement via P/Invoke; fallback to simpler palettes if no truecolor.
- Performance: batch style changes to minimize escape output and always reset state after use.
- Testing: validate across common terminals (Windows Terminal, ConEmu, Linux terminals, macOS Terminal) and fall back gracefully for unsupported features.

