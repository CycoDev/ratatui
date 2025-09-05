## ratatui-widgets\src\canvas\map.rs-ratatui-for-other-lang-impl-notes.md

- Goal: world-map canvas for terminal UIs — look for .NET libs that offer canvas/drawing abstractions.
- Spectre.Console: primary candidate for rich ANSI colors, advanced console rendering, and drawing primitives.
- Terminal.Gui (gui.cs): full TUI toolkit with custom drawing controls for higher-level widgets.
- System.Console + P/Invoke (SetConsoleMode): low-level approach; needed for precise ANSI/Windows behavior.
- Windows Console API vs ANSI: research enabling Virtual Terminal Processing on Windows when using ANSI.
- Unicode/Braille: .NET supports Unicode; implement Braille mapping (U+2800–U+28FF) yourself — no common library.
- Character grids: implement Braille/HalfBlock grids in code; test rendering in target terminals.
- Color support: Spectre.Console or Colorful.Console for portable color and styling.
- Terminal capability detection: inspect TERM env, consider terminfo bindings (e.g., NcursesSharp/TermInfoSharp) if needed.
- Terminal size: Console.WindowWidth/WindowHeight (watch cross-platform caveats).
- Coordinate data: embed arrays as resources or static files (.resx, embedded resource).
- Coordinate transforms: use System.Numerics or simple math utilities to map lon/lat → terminal coords.
- Enum/string utilities: Enum.GetName, DescriptionAttribute, or Humanizer for display names.
- Unicode encoding/normalization: System.Text.Encoding and string normalization checks.
- Testing: xUnit/NUnit + snapshot/approval testing for terminal output (ApprovalTests.NET).
- no_std concept: irrelevant in .NET — ignore Rust polyfill concerns.

