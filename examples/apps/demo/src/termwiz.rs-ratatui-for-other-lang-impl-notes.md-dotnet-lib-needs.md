## examples\apps\demo\src\termwiz.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libs that provide raw-mode terminal control, alternate screen, input (keys/mouse), ANSI/VT support, and efficient buffered drawing.
- High-level TUI frameworks: Terminal.Gui (gui.cs) — widgets/layouts, cross-platform.
- Rich rendering/color: Spectre.Console — ANSI styling and advanced console output.
- Windows PTY: search for ConPTY/PTY wrappers for .NET (ConPtySharp, Pty.Net, ConPTY.NET).
- Unix termios/terminfo: Ncurses wrappers (NcursesSharp) or P/Invoke termios for raw mode and capability detection.
- Low-level console: System.Console + P/Invoke (termios on Unix, Win32 console APIs on Windows).
- VT/ANSI detection: libraries or terminfo bindings to detect terminal capabilities and fallbacks.
- Input normalization: ensure library exposes normalized key codes and mouse events (Terminal.Gui does).
- Alternate screen & cursor control: verify support for switching screens and showing/hiding cursor.
- Buffered/diff rendering: look for double-buffering or implement frame diffing to minimize updates.
- Resize handling: library must expose resize events or allow polling Console.Window size.
- Unicode & codepage: check Windows VT mode & UTF-8 support; provide ASCII fallbacks.
- Cleanup on crash: library should allow guaranteed terminal state restore (finally/Dispose patterns).
- Event loop/tick: async loops with poll timeout or timer integration for ticks.
- Performance: prefer libs optimized for large displays and minimal terminal writes.

