## examples\apps\demo2\src\colors.rs-ratatui-for-other-lang-impl-notes.md

- Goal: research .NET libraries/features to reproduce Ratatui’s terminal, rendering, color, and input model.
- Terminal backend abstraction: look for cross-platform terminal libraries and ability to swap backends (Windows vs Unix).
- Recommended .NET libs to evaluate: Spectre.Console, Terminal.Gui, SadConsole, and any ncurses bindings (e.g., NCursesSharp).
- Low-level VT/ANSI support: System.Console + P/Invoke for Windows SetConsoleMode(ENABLE_VIRTUAL_TERMINAL_PROCESSING) and Unix termios/ANSI escape sequences.
- Buffer-based rendering / diffing: search for libraries or examples that support double-buffering or virtual DOM (Spectre.Console’s live rendering features or implement a 2D Cell buffer).
- Cell model requirements: per-cell character, foreground/background color, style flags, optional underline color — must be supported or implementable.
- High-resolution “two‑pixels-per-char” trick: render top/bottom halves using '▀' with distinct fg/bg colors — confirm library supports independent fg/bg truecolor.
- Color space conversions: need perceptual color conversions (Oklab/OKHSV). Research .NET color libraries: Colourful (ColourfulLib), ColorMine, ImageSharp, SkiaSharp, and search specifically for “Oklab .NET”.
- Terminal color capability detection: must detect 16/256/truecolor and fall back; Spectre.Console already includes detection utilities to study.
- RGB-to-palette mapping: research algorithms/libraries or functions in Spectre.Console/Colourful for nearest-color mapping to 16/256 palettes.
- Unicode and grapheme support: use System.Text.Rune and System.Globalization.StringInfo for grapheme clusters and wide/combining characters.
- Terminal state management: alternate screen, raw mode, cursor visibility, and restoring state on exit — check Terminal.Gui and Spectre.Console for helpers or implement via P/Invoke.
- Input & events: keyboard, mouse, and resize handling — evaluate Terminal.Gui for higher-level events or use Console.KeyAvailable + low-level input processing for raw mode.
- Mouse reporting: verify terminal mouse protocol support and library support (xterm mouse mode) if mouse is required.
- Cross-platform quirks: research Windows VT enabling, terminfo/terminfo-like behavior on Unix, and consistent handling of colors and control codes.
- Performance: minimize writes by diffing buffers; research best practices in Spectre.Console or implement a change-tracking buffer.
- Layout & widget system: investigate Terminal.Gui for widget/layout primitives and Spectre.Console for composable renderables to mirror Ratatui’s widget trait.
- Useful search queries: “Spectre.Console truecolor”, “Terminal.Gui buffer rendering”, “Oklab .NET”, “enable virtual terminal processing C#”, “RGB to 256 color mapping .NET”.
- Implementation approach: pick a backend (Spectre.Console for rich rendering or Terminal.Gui for widgets), add/implement a Cell buffer, integrate a color-space library (or port Oklab conversions), and implement diffing + VT output.



