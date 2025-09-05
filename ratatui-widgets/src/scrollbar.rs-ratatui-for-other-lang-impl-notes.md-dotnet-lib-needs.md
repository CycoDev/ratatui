## ratatui-widgets\src\scrollbar.rs-ratatui-for-other-lang-impl-notes.md

- Terminal UI frameworks to evaluate: Terminal.Gui (gui.cs) and Spectre.Console — check widget model, layout and rendering APIs. SadConsole if you need game-style cell control.
- Low-level cell/buffer model: look for APIs that let you write a 2D cell buffer (Terminal.Gui ConsoleDriver/Buffer, Spectre.Console Canvas / IAnsiConsole) or plan to implement one.
- Layout primitives: research each library's Rect/Size/Measure equivalents for mapping scrollbar area to characters.
- Styling and colors: Spectre.Console has rich style/ANSI support; otherwise use System.Console with ANSI escape sequences or the UI lib’s style API.
- Unicode grapheme clusters: System.Globalization.StringInfo and TextElementEnumerator; System.Text.Rune for codepoints.
- Display width (monospace/East Asian Width): search NuGet for “Unicode width” / “EastAsianWidth” implementations (e.g., UnicodeWidth.NET / EastAsianWidth packages) or implement UAX#11 rules.
- Box-drawing / symbol rendering: verify terminal UTF‑8 and font support; Spectre.Console handles UTF‑8 well — provide ASCII fallbacks.
- Stateful widget pattern: map StatefulWidget to a simple state class (position, content length, viewport) held alongside the widget implementation.
- Proportional sizing/math: use System.Math (double) for thumb size/position calculations; handle rounding to integer cells.
- Terminal capability detection: consider libraries or checks for ANSI support and truecolor; fallback behavior if limited.
- Cross-platform testing targets: Windows (Windows Terminal, PowerShell), macOS Terminal/iTerm2, Linux (xterm, kitty, Alacritty).
- Fallback strategies: plan configurable symbol sets and minimal-UTF8/ASCII modes for constrained terminals.

