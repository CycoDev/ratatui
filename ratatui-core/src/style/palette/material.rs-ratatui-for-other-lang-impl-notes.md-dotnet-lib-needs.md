## ratatui-core\src\style\palette\material.rs-ratatui-for-other-lang-impl-notes.md

- Spectre.Console (NuGet): high-level ANSI/truecolor support, 256-color, rich styling — primary candidate.
- Pastel (NuGet): lightweight truecolor string extensions (ANSI escape generation).
- Colorful.Console (NuGet): easy colored output; verify 24-bit support on targets.
- Terminal.Gui (NuGet): full-screen TUI framework (not ANSI-based); alternative approach.
- Ncurses/PDCurses bindings (NCursesSharp/SharpNCurses): terminfo-based capability detection for Unix.
- Windows VT handling: enable ENABLE_VIRTUAL_TERMINAL_PROCESSING via SetConsoleMode (P/Invoke) for ANSI on older consoles.
- 256-color & ANSI handling: libraries or implement conversion to ANSI 38/48 sequences.
- Color types: use SixLabors.ImageSharp (Rgba32) or System.Drawing.Color (be aware System.Drawing cross-platform limits).
- Hex<->RGB utilities: trivial to implement; ImageSharp can help if you want built-ins.
- Terminal capability detection: check TERM env var, use terminfo or ncurses bindings; Spectre.Console abstracts much of this.
- Fallback mapping: need nearest-ansi/256 algorithm if truecolor unsupported; Spectre.Console offers strategies.
- Serialization: System.Text.Json or Newtonsoft.Json for saving/loading palettes.
- Licensing check: ensure NuGet libs license compatible with ratatui port.
- Implementation note: palette constants need no external deps; focus libraries around output, detection, and conversions.
- Start research: Spectre.Console docs, Pastel, ImageSharp, P/Invoke SetConsoleMode, ncurses .NET bindings.

