## ratatui-core\src\style\palette.rs-ratatui-for-other-lang-impl-notes.md

- Core requirement: a Color type supporting named ANSI, 24-bit RGB, and 8-bit indexed (256) representations — research .NET types/libraries that expose/manipulate these formats.
- Terminal UI libraries (handle rendering, detection, and some fallbacks): look up Spectre.Console and Terminal.Gui (gui.cs) for TrueColor support and backend behavior.
- Truecolor/terminal capability detection: search for existing detection in Spectre.Console or code snippets using TERM, COLORTERM, Windows build APIs; you may need to implement detection & fallback logic.
- RGB fallback/quantization: need algorithms to map 24-bit RGB to nearest 256/ANSI color — search “color quantization .NET”, “nearest 256-color .NET”, and libraries that offer palette reduction (ImageSharp has quantizers).
- Color conversions (HSL, HSLuv, HSLuv↔RGB): search for .NET ports of HSLuv (keywords: HSLuv .NET, Hsluv.NET) and general color-conversion libraries like ColorMine for multiple color spaces.
- Hex/string parsing and named CSS colors: System.Drawing.ColorTranslator.FromHtml and other small utilities; verify cross-platform behavior (System.Drawing.Common caveats on Linux) or use lightweight parsers.
- Nearest-color/delta metrics: for accurate fallback choose libraries offering Delta E or RGB distance (ColorMine supports color distance calculations).
- Serialization/deserialization: use System.Text.Json or Newtonsoft.Json; plan for backward-compatible formats if persisting palettes.
- Palette data structure: static grouped palettes (Material, Tailwind) — simple JSON or static classes/structs suffice; no heavy dependency required.
- Optional heavy graphics libraries for advanced conversions: ImageSharp or SkiaSharp if you need robust, high-performance color manipulation or image-based quantization.
- Console color APIs and cross-platform caveats: research Console, Windows-specific APIs, and how Spectre.Console abstracts those differences.
- Suggested NuGet search keywords: “Spectre.Console”, “Terminal.Gui”, “HSLuv .NET”, “ColorMine”, “ImageSharp”, “SkiaSharp”, “color quantization”, “console truecolor detection”, “ANSI 256 color .NET”.
- Implementation notes to guide library choice: favor libraries that (a) explicitly state truecolor/256-color support, (b) offer color-space conversions or allow plugging in conversion code, and (c) work cross-platform or are easily abstracted.

