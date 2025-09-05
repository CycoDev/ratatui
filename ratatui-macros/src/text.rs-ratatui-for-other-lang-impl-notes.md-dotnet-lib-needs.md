## ratatui-macros\src\text.rs-ratatui-for-other-lang-impl-notes.md

- Target .NET libraries for TUI rendering: Spectre.Console (rich styling/markup), Terminal.Gui (gui.cs) for higher-level widgets.
- Low-level terminal control: System.Console + Win32 Console API (via P/Invoke) or enable ANSI (VT) sequences on Windows 10+.
- ANSI escape/VT handling libraries or helpers for cross-platform control-sequences.
- Unicode/grapheme handling: System.Text.Rune, System.Globalization.StringInfo; search for GraphemeSplitter/.NET ports or ICU.NET for robust grapheme clusters.
- Unicode width/East Asian width: find a .NET equivalent (unicode-width/EastAsianWidth ports).
- Styling model: look for span/line/text abstractions or implement with builder/DSL patterns (no macros).
- Terminal capability detection: terminfo/terminfo-sharp or feature-detection helpers.
- Cross-platform backends: compare behavior on Windows, macOS, Linux (Spectre.Console abstracts many differences).
- Performance/memory: consider immutable value types vs GC-managed objects and pooling.
- Text composition/testing: port text/span/line unit tests to validate style inheritance and grapheme behavior.

