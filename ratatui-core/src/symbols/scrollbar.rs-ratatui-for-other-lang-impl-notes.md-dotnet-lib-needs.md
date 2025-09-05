## ratatui-core\src\symbols\scrollbar.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: defines Unicode characters for scrollbars and notes terminal/font pitfalls — relevant when choosing .NET terminal/console libraries.
- Rendering libraries to research: Spectre.Console (rich ANSI rendering), Terminal.Gui (gui.cs) for widgets/scrollbars.
- UTF-8 output: set Console.OutputEncoding = System.Text.Encoding.UTF8.
- ANSI/VT support on Windows: enable VT processing via SetConsoleMode (P/Invoke) or use .NET 5+ console defaults.
- Terminal size APIs: System.Console.WindowWidth/WindowHeight; for raw TTY accuracy use ioctl via Mono.Posix.NETStandard or P/Invoke to libc.
- Unicode width (double-width chars): use Wcwidth.NET or EastAsianWidth.NET, or implement wcwidth logic.
- Grapheme clusters and combining chars: use System.Globalization.StringInfo to iterate user-perceived characters.
- Normalization: apply System.Text.NormalizationForm (if comparing/width-checking).
- Fallbacks: implement ASCII fallback mapping if terminal/font lacks glyphs.
- Fonts/terminal compatibility: no .NET library fixes this—test on target terminals and fonts.
- Low-level console control (Windows): kernel32 GetConsoleScreenBufferInfo / SetConsoleCursorPosition via P/Invoke for fine-grained drawing.
- Input/mouse support: Terminal.Gui or handle Console.ReadKey/Read for basic input; for mouse you’ll need library support (gui.cs or custom ANSI mouse sequences).
- Recommendation: combine a rendering library (Spectre.Console or Terminal.Gui) + a wcwidth implementation + UTF‑8/VT setup for reliable scrollbar rendering.

