## examples\apps\demo2\src\tabs\email.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: implements TUI email tab with widgets (tabs, list, paragraph, scrollbar), selection, scrolling, layout and theming.
- Key .NET libraries to research: Terminal.Gui (gui.cs) for full widget/view system; Spectre.Console for rich text, colors and layout primitives.
- Unicode width/grapheme handling: System.Text.Rune + System.Globalization.StringInfo; look for a wcwidth/EastAsianWidth NuGet or port wcwidth for accurate column widths.
- Raw terminal/alternate screen: investigate P/Invoke to termios (Unix) and Win32 Console API, or use libraries that abstract this (Terminal.Gui handles low-level details).
- Input events: Terminal.Gui offers keyboard/mouse event model; Spectre.Console is more focused on output.
- Color support: research RGB vs ANSI support and capability detection; Spectre.Console handles fallbacks well.
- Layout engine: Terminal.Gui provides constraint/layout system; Spectre.Console offers Layout/Grid primitives for panels.
- Stateful widgets & scrolling: ensure list selection state persists between renders; Terminal.Gui has ListView/TextView with scrolling.
- Cross-platform testing: validate on Windows (Console/Windows Terminal) and Unix (terminals supporting ANSI/truecolor).
- Recommendation: start with Terminal.Gui for widget parity, add Spectre.Console where richer text/color or simpler output is needed, and add/port a reliable wcwidth implementation.

