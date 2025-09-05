## ratatui-widgets\src\borders.rs-ratatui-for-other-lang-impl-notes.md

- Borders are a bitflag set (TOP/RIGHT/BOTTOM/LEFT) — use a C# [Flags] enum for same semantics.
- BorderType is an enum mapping styles to Unicode box-drawing character sets — implement enum + mapping dictionary/struct of symbols.
- bitflags crate → use [Flags] enum + explicit bit values; provide static readonly combos (NONE, ALL).
- strum (Display/EnumString) → use DescriptionAttribute/EnumMember, System.Text.Json converters, or a small helper to parse/format enums.
- ratatui_core::symbols::border → port as static symbol sets (corners, horiz, vert) or embed as resources.
- Keep border visibility (which sides) separate from visual style (symbol set) for clean architecture.
- Terminal backend choices (NuGet): Spectre.Console (ANSI/VT, rich rendering), Terminal.Gui (higher-level TUI), or raw System.Console with VT support.
- Ensure Unicode: set Console.OutputEncoding = Encoding.UTF8 and enable VT processing on Windows (or rely on Spectre.Console).
- Character width matters (emoji/East Asian width) — use System.Text.Rune + a wcwidth/EastAsianWidth NuGet (e.g., wcwidth.net, EastAsianWidth.NET) to compute layout.
- Provide ASCII fallbacks for limited terminals.
- Test across Windows Terminal, PowerShell, cmd, macOS Terminal, and common Linux terminals.
- Quick NuGet candidates to research: Spectre.Console, Terminal.Gui, wcwidth.net / EastAsianWidth.NET, Ardalis.SmartEnum (optional for richer enums).

