## examples\concepts\state\src\bin\immutable-function.rs-ratatui-for-other-lang-impl-notes.md

- Goal: port Ratatui to .NET — research libraries that provide terminal primitives and higher‑level TUI features.
- Key primitives to find: raw mode (termios/Win32), alternate screen, enable VT/ANSI, cursor control, colors, buffered drawing, efficient diff/blit.
- Input/events: async keyboard, mouse, resize events, and platform differences.
- Backends: ability to implement pluggable backends (Windows vs Unix).
- Layout & widgets: constraint/percentage layout support or ability to build one.
- Buffering: offscreen buffer/diffing support or fast terminal writes.
- Cross-platform: ensure Unix termios (Mono.Posix.NETStandard) and Win32 Console APIs (P/Invoke) are accessible.
- ANSI/VT support: Windows 10+ virtual terminal processing flag.
- Candidate .NET libraries to evaluate: Spectre.Console (rich rendering, ANSI), Terminal.Gui / gui.cs (higher‑level TUI), NCurses wrappers (NcursesSharp/NCurses.Core) for Unix compatibility.
- Consider combining low-level P/Invoke (termios/Win32) for exact control with a higher-level rendering lib for widgets.

