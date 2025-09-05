## ratatui-core\src\text\text.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: lists runtime concerns and deps to replace when porting Ratatui text rendering to .NET.
- Grapheme segmentation: .NET APIs—System.Globalization.StringInfo / TextElementEnumerator; NuGet: GraphemeSplitter for robust grapheme clusters.
- Codepoint iteration: System.Text.Rune for Unicode scalar handling.
- Display width (wcwidth/East Asian): look for NuGet ports (e.g., EastAsianWidth or WcWidth implementations) or implement the wcwidth algorithm.
- Style model: use Spectre.Console (rich style model, RGB), or implement simple Style struct (fg/bg/attrs) with merging.
- Buffer abstraction: implement a 2D cell buffer (char/grapheme, style) or reuse Terminal.Gui/Spectre.Console renderables if suitable.
- Terminal backends: Spectre.Console and Terminal.Gui (gui.cs) for high-level; System.Console + ANSI sequences for low-level.
- Windows VT handling: enable Virtual Terminal Processing via SetConsoleMode (P/Invoke) or rely on modern Windows Terminal/.NET behavior.
- Color detection & modes: detect terminal capabilities (ANSI/8/256/24-bit); Spectre.Console already handles this.
- Style cascading: implement Text→Line→Span merge semantics as in Ratatui.
- Testing: create test vectors for wide chars, combining marks, emoji, and bidirectional edge cases.
- Recommendation: start with Spectre.Console + GraphemeSplitter + a wcwidth/EastAsianWidth NuGet as the minimal stack.

