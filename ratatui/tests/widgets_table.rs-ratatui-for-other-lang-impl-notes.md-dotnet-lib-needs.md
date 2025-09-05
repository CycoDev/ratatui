## ratatui\tests\widgets_table.rs-ratatui-for-other-lang-impl-notes.md

- Goal: port Ratatui Table widget to .NET — research libraries that provide terminal control, buffered rendering, styling, unicode width handling, layout helpers, and testable backends.  
- Candidate high-level libraries to evaluate: Spectre.Console (rich rendering, markup, IAnsiConsole for testing), Terminal.Gui / gui.cs (view/layout system, widget-style API).  
- Low-level options: System.Console (built-in, cross-platform but limited), P/Invoke to ncurses or native term APIs only if needed.  
- Look for buffered rendering or an abstract "Console" interface you can implement/mock (Spectre.Console exposes IAnsiConsole which is testable).  
- Need cell buffer abstraction: research libraries that let you write to a virtual screen or capture output; otherwise implement an internal Buffer<Cell> and flush as ANSI sequences.  
- Styling: verify color, background, and modifiers support (bold/italic/underline); prefer libs that expose style composition/APIs.  
- Unicode & width: .NET: System.Text.Rune + StringInfo for grapheme clusters; research east-asian width handling libraries (to measure column cell widths and wrapping).  
- Layout & constraints: require support or implement column constraints (fixed length, percent, ratio, flexible); check Terminal.Gui for layout managers and Spectre.Console for table/column helpers.  
- Selection/highlight support: need APIs for custom rendering per-cell/row and for drawing highlight symbols/spaces — prefer libraries allowing per-cell styling when rendering.  
- Input/modes: raw input, key events, mouse support — evaluate Terminal.Gui (input rich) and confirm Spectre.Console options or supplement with low-level input handling.  
- Cross-platform terminal features: research .NET VT/ANSI support on Windows (Windows 10+ VT sequences), and ensure chosen lib handles them or provide fallback backends.  
- Testing approach: prioritize libraries with virtual/test backends or interfaces you can fake; otherwise create TestBackend that captures buffer and compare ASCII/expected output.  
- Performance: check library rendering approach (immediate vs. buffered) and ability to batch draws to minimize flicker.  
- Versioning & packaging: prefer actively maintained NuGet packages with stable APIs; check licenses (MIT/Apache) for compatibility.  
- Required extras: terminal size queries, cursor positioning, clearing regions, margin/spacing controls — ensure library exposes these or are implementable.

