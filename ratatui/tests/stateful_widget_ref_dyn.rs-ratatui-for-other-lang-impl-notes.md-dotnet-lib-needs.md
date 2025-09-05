## ratatui\tests\stateful_widget_ref_dyn.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries to replicate Ratatui features (terminal rendering, widget system, unicode handling, input/backends, buffering).
- Terminal UI libraries to evaluate: Terminal.Gui (gui.cs), Spectre.Console, SadConsole.
- Rendering/diffing: check Spectre.Console rendering pipeline and virtual console abstractions.
- Unicode grapheme clusters: System.Globalization.StringInfo; System.Text.Rune for codepoints.
- Display width / East Asian widths: search NuGet for "Unicode width", "EastAsianWidth", or "UnicodeWidth" packages.
- ANSI/VT support on Windows: enable Virtual Terminal Processing via Win32 APIs or use libraries that handle it.
- Low-level backends: consider P/Invoke to Windows Console API or ncurses bindings for Unix.
- Input (keyboard/mouse): Terminal.Gui or Spectre.Console higher-level handlers; otherwise Console.ReadKey + native mouse via P/Invoke.
- Dynamic widget typing: use interfaces + object/dynamic; downcast with "as" or pattern matching.
- Stateful widgets: model state objects (mutable refs) — .NET uses mutable fields, optional locking (lock/Interlocked) for concurrency.
- Interior mutability: not needed idiomatically; use private mutable state or Concurrent structures if shared.
- Search keywords: "terminal UI .NET", "ANSI console .NET", "unicode grapheme .NET", "unicode width .NET", "Terminal.Gui", "Spectre.Console".

