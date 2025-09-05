## ratatui-widgets\src\table\highlight_spacing.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: small enum (Always / WhenSelected (default) / Never) that controls whether a column of selection/highlight symbols is reserved in table/list layouts.
- Enum representation: .NET can use standard enums + ToString()/Enum.TryParse; for custom string names research EnumMemberAttribute or a TypeConverter.
- String<>Enum helpers: consider Enums.NET or Ardalis.SmartEnum for richer parsing/display behaviors.
- Serialization: use System.Text.Json (JsonStringEnumConverter) or Newtonsoft.Json (StringEnumConverter); both support serializing enums as strings and honoring attributes.
- Optional feature gating (serde in Rust): map to optional NuGet dependency or conditional compile symbols (#if) in .NET libraries or provide a separate serializer-using API surface.
- Terminal/TUI libraries to evaluate: Spectre.Console (rich tables, colors, layout), Terminal.Gui (gui.cs) for higher-level widgets — research how each handles dynamic layout and selection indicators.
- Unicode/terminal width: need a wcwidth/grapheme-cluster solution for accurate column width when adding/removing the highlight column — research .NET libraries implementing wcwidth or use System.Text.Rune + existing wcwidth ports.
- Layout updates: ensure chosen TUI library supports recalculating layout on state change (selection toggles) or provides hooks to reflow table widths.
- Human-readable enum names: consider Humanizer or EnumMember + serializer converters for user-facing configuration strings.
- Persistence of preference: store HighlightSpacing in appsettings.json or user config via the chosen JSON serializer and configuration API (Microsoft.Extensions.Configuration).
- Cross-platform terminal behavior: verify Spectre.Console/Terminal.Gui behavior on Windows/macOS/Linux and test East Asian width, combining marks, emoji width differences.
- Testing: use xUnit/NUnit + snapshot or console-integration tests to validate layout when highlight column is present/absent.
- Logging/diagnostics: Microsoft.Extensions.Logging to log layout decisions (helpful when width differences appear across terminals).
- Recommendation: focus research on Spectre.Console and Terminal.Gui first, plus a .NET wcwidth/grapheme-width solution and JSON enum-string converters; Enums.NET and Humanizer are useful secondary libraries.

