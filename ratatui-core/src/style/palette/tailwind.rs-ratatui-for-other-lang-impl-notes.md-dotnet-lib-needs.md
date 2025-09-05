## ratatui-core\src\style\palette\tailwind.rs-ratatui-for-other-lang-impl-notes.md

- Goal: research .NET libraries to reproduce Ratatui’s color abstraction + backend handling.
- Key color models to support: 16 ANSI (ConsoleColor), 256-indexed, 24-bit RGB.
- Libraries to evaluate: Spectre.Console (rich ANSI/truecolor, high-level rendering), Terminal.Gui (TUI framework), Colorful.Console, Pastel (string coloring).
- Spectre.Console provides ANSI/truecolor handling and abstracts capabilities—good first port target.
- System.Console only exposes 16 ConsoleColor values; for 256/24-bit emit ANSI escapes or use a lib.
- On Windows you may need SetConsoleMode ENABLE_VIRTUAL_TERMINAL_PROCESSING (P/Invoke) for ANSI.
- Terminal capability detection: TERM/COLORTERM env vars, isatty, Windows version/VT support.
- Implement a Color enum/struct (Ansi16, Indexed256, Rgb) and converters (hex↔RGB, ANSI escapes).
- Fallbacks: map RGB→256→16 when truecolor unsupported.
- Serialization/parsing: use System.Text.Json or Newtonsoft.Json for color configs.
- Per-cell buffer/diffing: Spectre.Console lacks low-level cell buffer; you may need a custom buffer layer.
- Test targets: Windows Terminal, cmd/PowerShell, macOS Terminal, common Linux terminals.
- If you need low-level Windows APIs, research Windows Console API via P/Invoke.

