## examples\apps\input-form\src\main.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: map Ratatui components to .NET libraries you should research (backend, buffer/diffing, widgets, layout, styling, unicode, I/O, testing).
- Terminal backend (low-level): research enabling VT/ANSI on Windows via SetConsoleMode (PInvoke: Vanara.PInvoke or PInvoke.Win32) and using native ANSI on macOS/Linux.
- High-level TUI frameworks: Terminal.Gui (gui.cs) — full widget/layout system; Spectre.Console — rich text, colors, and ANSI helpers (good for styling and some rendering primitives).
- Buffer & diffing: no common drop-in — plan to implement a 2D cell buffer + diff algorithm; inspect Spectre.Console render pipeline or Terminal.Gui source for ideas.
- Widget & layout engines: Terminal.Gui already provides widgets, event loop and layout constraints; if you want Ratatui-style composable stateless widgets, you’ll likely implement your own on top of a low-level backend.
- Unicode grapheme segmentation: use System.Globalization.StringInfo (TextElements) for grapheme clusters.
- Character width (wcwidth / East Asian Width / emoji): research NuGet packages (e.g., wcwidth/EastAsianWidth ports) or embed a wcwidth implementation to correctly compute cell widths.
- Input & events: Terminal.Gui provides cross-platform input (keyboard, mouse). For custom backends, use Console.ReadKey for basic input and implement raw-mode reading via termios (Unix) and SetConsoleMode (Windows).
- Colors & styles: Spectre.Console supports 16/256/RGB and style modifiers; Terminal.Gui has a color palette system — evaluate both for feature fit.
- Alternate screen, raw mode, cursor control: rely on ANSI sequences plus Windows VT enablement; Spectre.Console exposes some helpers for cursor/alternate buffer.
- Cross-platform caveats: handle Windows VT enabling, UTF‑8 code page issues, and terminal capability differences (mouse, 256/RGB colors).
- Performance tips: batch writes, diff only changed cells, handle multi-width graphemes and zero-width characters carefully to avoid layout corruption.
- Testing & snapshots: use xUnit/NUnit + snapshot libraries like Verify (Verify.Xunit) or ApprovalTests for visual regression; build test backends or mocks for deterministic rendering tests.
- Recommended path: if you want minimal lift use Terminal.Gui for widgets/layout; use Spectre.Console for advanced styling/ANSI handling; implement buffer/diffing and precise unicode width handling yourself for faithful Ratatui semantics.

