## ratatui\benches\main\sparkline.rs-ratatui-for-other-lang-impl-notes.md

- Goal: find .NET libraries that cover terminal I/O, low-level control (raw mode, alternate screen), color modes (ANSI/256/truecolor), Unicode, and widget/TTY toolkits.
- Terminal abstraction / low-level control: System.Console is limited — look for libraries or P/Invoke helpers that expose SetConsoleMode (Windows) and termios (POSIX) for raw mode and enabling ANSI/VT sequences.
- ANSI/VT and Windows compatibility: ensure library can enable ENABLE_VIRTUAL_TERMINAL_PROCESSING on Windows or provide fallback emulation.
- Truecolor / 256-color support: prefer libraries that explicitly support 24‑bit colors and indexed palettes (Spectre.Console advertises rich color support).
- High-level TUI widgets/layout: evaluate Terminal.Gui (gui.cs) and Spectre.Console (panels, tables, progress); Terminal.Gui offers a widget/layout system similar to Ratatui’s widget approach.
- Cell-buffer / diffing capability: need a library or writable buffer abstraction that allows per-cell symbol + fg/bg + attributes and efficient diffing; if absent, plan to implement an in-memory buffer and minimal renderer.
- Low-level curses bindings: consider PDCurses / ncurses .NET wrappers (PDCurses.NET, NCursesSharp) if you need cell-level control and stable alternate-screen behavior across Unix/Windows (via PDCurses).
- Unicode support: ensure Console.OutputEncoding = Encoding.UTF8; on Windows verify code page, font, and terminal app Unicode behavior (Windows Terminal vs legacy cmd).
- Input handling / key sequences: seek libraries that normalize key/mouse events across platforms or be prepared to P/Invoke and translate terminal escape sequences yourself.
- Benchmarking: use BenchmarkDotNet as the equivalent of Criterion for performance tests.
- Random/test data: use System.Random or System.Security.Cryptography.RandomNumberGenerator for deterministic/non-deterministic test data.
- Testing strategy: look for libraries with test drivers or design a mock backend (in-memory terminal buffer) to run unit tests and benchmarks headless.
- Packaging & cross-platform testing: prefer .NET 6+/Core for cross-platform support; validate behavior on Windows Terminal, PowerShell, macOS Terminal, and common Linux terminals.
- Performance considerations: measure terminal I/O overhead — prefer libraries that minimize writes or allow batch/sequence writes; implement diffing at the application layer if library has no native support.
- Styling & attributes: confirm library supports bold/italic/underline and per-cell foreground/background; if not, normalize to ANSI attributes.
- Summary actionable shortlist to investigate first: Spectre.Console, Terminal.Gui, PDCurses.NET/NCurses wrappers, BenchmarkDotNet, and using direct P/Invoke for advanced console modes and raw input.

