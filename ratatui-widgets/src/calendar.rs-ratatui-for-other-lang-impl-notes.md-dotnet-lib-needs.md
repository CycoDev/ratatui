## ratatui-widgets\src\calendar.rs-ratatui-for-other-lang-impl-notes.md

- Date/time library: use NodaTime (recommended) or System.DateTime/DateOnly for date arithmetic, day-of-week, month boundaries.
- Immutable/date maps: System.Collections.Generic.Dictionary<DateOnly, TValue> or ImmutableDictionary for event styling storage.
- Terminal UI libraries: Spectre.Console (rich styling, Layout, IRenderable) or Terminal.Gui (ncurses-like Views/Layouts).
- Styling primitives: Spectre.Console provides Style/Color/Markup similar to Ratatui's Style; Terminal.Gui has ColorScheme.
- Layout system: Spectre.Console.Layout or Terminal.Gui.Grid/FrameView to implement calendar grid and headers.
- Buffer abstraction: implement a 2D cell buffer (char + Style) or use Spectre.Console render pipeline / IRenderable to draw offscreen.
- Unicode width/wcwidth: use Wcwidth.NET or EastAsianWidth.NET, or System.Text.Rune for grapheme handling to compute cell widths.
- Backend/cross-platform: .NET Console + Spectre.Console handles ANSI across Windows/macOS/Linux; ensure VT processing enabled on older Windows (SetConsoleMode) if needed.
- Color & capability detection: use Spectre.Console AnsiConsole.Profile or Console APIs to detect color support.
- Widget composability: map Ratatui Widget trait to classes implementing a Render/Draw method (IRenderable in Spectre.Console or View in Terminal.Gui).
- Performance: reuse date objects (NodaTime LocalDate), minimize buffer redraws, cache computed styles.
- Serialization/storage: use built-in JSON (System.Text.Json) for persisting event/style maps if needed.
- Recommendation: prefer Spectre.Console for terminal rendering + Layout + Style; use NodaTime for robust date logic and Wcwidth.NET for accurate Unicode cell widths.

