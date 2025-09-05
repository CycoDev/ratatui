## ratatui-core\src\widgets\stateful_widget.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: stateful widgets separate rendering logic from app-owned state (selection, scroll, etc.).  
- Rust libs referenced: unicode_segmentation, unicode_width, crossterm/termion/termwiz — map these when porting.  
- Unicode grapheme clusters: .NET options — System.Globalization.StringInfo or System.Text.Rune; third-party: GraphemeSplitter (NuGet).  
- Character display width (wcwidth/East Asian): look for UnicodeWidth.NET, WcWidth.NET, or implement wcwidth tables.  
- Terminal I/O / backend (cross-platform): consider Spectre.Console (rich rendering + ANSI), Terminal.Gui (ncurses-like TUI), or P/Invoke to native APIs / PDCurses/NCurses bindings for low-level control.  
- ANSI / styling support: Spectre.Console covers ANSI colors/styles; otherwise use Colorful.Console or write ANSI sequences.  
- Cell/buffer model: implement a cell buffer (char + style) or use Spectre.Console's render tree/Canvas if it fits.  
- Input handling / raw mode: Terminal.Gui or p/invoke termios (Unix) and Windows Console API for raw key events.  
- Sizing/layout primitives: check library support for rect/constraints; Terminal.Gui provides layout; otherwise implement simple Rect and layout engine.  
- Unsized/state types: .NET supports reference types and interfaces; use object/abstract base or generics to model widget State.  
- Ownership pattern: keep state in app code and pass mutable references (or mutable objects) into widget render calls.  
- Recommendation: evaluate Spectre.Console first for high-level rendering and ANSI support; use StringInfo + UnicodeWidth.NET for correct grapheme/width handling if precise alignment is required.

