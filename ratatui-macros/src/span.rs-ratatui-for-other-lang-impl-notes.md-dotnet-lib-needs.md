## ratatui-macros\src\span.rs-ratatui-for-other-lang-impl-notes.md

- Core concept to port: Span = text + Style (color, modifiers) — find .NET libs offering textual spans/markup.
- Look first at Spectre.Console: rich TUI, nested markup, RGB/256/ANSI support.
- Terminal.Gui: higher-level windowed TUI (not ANSI-focused) — alternative backend.
- Simple color helpers: Pastel, Colorful.Console — useful for lightweight ANSI coloring.
- Styling model must support composable styles (inherit/override) and modifiers (bold/italic/underline).
- ANSI escape handling and color modes (16/256/RGB) — ensure library exposes or you can emit escapes.
- Windows: research enabling VT/ANSI (SetConsoleMode/ENABLE_VIRTUAL_TERMINAL_PROCESSING) or use ConPTY/Console API for older systems.
- Unicode grapheme clusters: use System.Globalization.StringInfo and System.Text.Rune for proper segmentation.
- Display width (wcwidth/East Asian Width): search for .NET ports of wcwidth or "EastAsianWidth" libraries.
- Interpolation: .NET has interpolated strings and string.Format — use for formatted spans.
- Terminal backend abstraction: design layer to switch between Spectre.Console, Terminal.Gui, or direct ANSI.
- Search .NET packages: "spectre.console", "pastel", "colorful.console", and ".NET wcwidth/EastAsianWidth" for concrete implementations.

