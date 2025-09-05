## ratatui-core\src\symbols\line.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: defines Unicode box-drawing characters as core primitives (borders/lines), not widgets.
- .NET must support UTF-8 console output and correct Unicode glyph rendering.
- On Windows, enable VT/ANSI (SetConsoleMode) or target modern terminals (Windows Terminal).
- Candidate libraries to research: Spectre.Console (rich formatting, box work), Terminal.Gui (widget toolkit); also evaluate plain System.Console for low-level control.
- Prefer libraries that allow custom glyph sets / override border characters.
- Implement fallback mapping for terminals or fonts missing specific box-drawing glyphs.
- Verify font and terminal compatibility (glyph availability, double-width handling).
- Include automated rendering/snapshot tests across platforms and fonts.
- Keep core drawing primitives separate from higher-level widgets (match ratatui architecture).
- Minimize dependencies and target cross-platform .NET (net6+/netstandard) for broad compatibility.

