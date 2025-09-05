## ratatui-core\src\symbols\shade.rs-ratatui-for-other-lang-impl-notes.md

- Symbol set: EMPTY=" ", LIGHT="░", MEDIUM="▒", DARK="▓", FULL="█".
- Ensure UTF‑8: Console.OutputEncoding = Encoding.UTF8 (and set Console.InputEncoding if needed).
- On Windows enable VT/ANSI processing (SetConsoleMode -> ENABLE_VIRTUAL_TERMINAL_PROCESSING) for proper glyph & color handling.
- Prefer a terminal library that handles Unicode, colors, progress bars: Spectre.Console is a top .NET choice; Terminal.Gui for richer TUIs.
- For color capability detection and fallbacks use library facilities (Spectre.Console) or inspect TERM/Windows APIs.
- Character width: use a Unicode width utility (UnicodeWidth.NET / EastAsianWidth) to ensure 1 cell per glyph across terminals.
- Fallbacks: implement an ASCII fallback map (., #, @, *, etc.) and allow switching via config/auto‑detect.
- Glyph support detection: render a probe string and verify visible width/metrics at runtime.
- Fonts/terminal testing: validate on Windows Terminal, cmd.exe, PowerShell, iTerm2, common Linux emulators.
- Abstract symbol set behind an interface to allow Unicode/ASCII and platform-specific overrides.
- No special native libs required beyond enabling VT on Windows; pick Spectre.Console or Terminal.Gui + a width lib for most needs.

