## ratatui-core\src\layout\direction.rs-ratatui-for-other-lang-impl-notes.md

- Core concept: a tiny enum with two variants — Horizontal and Vertical; implement as a plain C# enum.
- String conversions: use Enum.ToString() and Enum.TryParse<T>(..., ignoreCase: true); for nicer names use [Display(Name="…")] or Humanizer.
- JSON (de)serialization: System.Text.Json + JsonStringEnumConverter or Newtonsoft.Json + StringEnumConverter.
- "strum" analogs: use attributes (Display) and small extension methods for parsing/display; no special crate needed.
- Keep core free of terminal APIs (portable library targeting netstandard2.0/.NET 6+); implement terminal adapters separately.
- Terminal backends to evaluate: Spectre.Console (feature-rich, cross-platform), Terminal.Gui (gui.cs, curses-like), and System.Console for minimal control.
- Low-level terminal control: research native/OS bindings only if you need full control (p/invoke, terminfo, or libraries that wrap ANSI/crossterm-like behavior).
- Constraint solver: look for Cassowary ports on NuGet (e.g., Cassowary.NET / CassowarySharp / Kiwi ports) for constraint-based layout support.
- Coordinate system: adopt top-left origin (0,0), x→right, y→down; use System.Drawing.Point/PointF or a small custom struct for no-GDI builds.
- Feature flags & optional dependencies: implement via multi-targeting and conditional compilation (MSBuild TargetFrameworks + CompileConstants) and separate NuGet packages for adapters.
- Serialization/config: support string enums for readable configs; provide custom JsonConverters if you need alternative names.
- Testing and portability: keep layout logic in pure, side-effect-free classes so it can run on CI and in environments without terminal access.



