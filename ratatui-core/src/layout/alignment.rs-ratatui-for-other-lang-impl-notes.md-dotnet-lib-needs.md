## ratatui-core\src\layout\alignment.rs-ratatui-for-other-lang-impl-notes.md

- Enums to port: HorizontalAlignment (Left, Center, Right; default Left) and VerticalAlignment (Top, Center, Bottom; default Top).
- Provide a backward-compat type alias or compatibility layer if prior API existed.
- Enum string conversion: use Enum.ToString()/Enum.Parse plus System.Text.Json's JsonStringEnumConverter or Newtonsoft.Json StringEnumConverter for (de)serialization.
- strum Display/EnumString equivalents: use EnumMember attributes or custom TypeConverter / extension methods for display names and parsing.
- Unicode-aware width/truncation is critical: use System.Globalization.StringInfo, System.Text.Rune and consider an East Asian width library (e.g., EastAsianWidth.NET) or implement Unicode width table.
- Grapheme cluster handling: use StringInfo or a Grapheme-splitting library to avoid breaking combined characters.
- Terminal backend abstraction: define an IConsoleBackend and provide implementations using System.Console + ANSI, Spectre.Console, or Terminal.Gui.
- Spectre.Console: good for ANSI rendering, alignment helpers, and richer features (MIT).
- Terminal.Gui: offers higher-level TUI widgets if you need widget layout support (MIT).
- Raw mode/alternate screen/mouse capture: may require P/Invoke on Windows or enabling VT processing; Spectre.Console/Terminal.Gui handle many cases.
- Target net6+/netstandard2.1 for widest cross-platform support; .NET has no direct no_std analog.
- Prefer well-maintained, cross-platform libraries (Spectre.Console, Terminal.Gui, System.Text.Json) and verify license compatibility.

