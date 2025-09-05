## ratatui-core\src\style\palette_conversion.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: convert palette crate types (Srgb, LinSrgb) to Ratatui Color — port needs color-space transforms and terminal color mapping.
- Key needs in .NET: color-space conversions (linear<->sRGB), color type structs, nearest-palette mapping (ANSI/256/16), terminal truecolor detection, rendering backend integration.
- Colourful (NuGet) — color spaces and conversions (sRGB, linear RGB, Lab), useful for linear↔sRGB maths.
- ColorMine — many color models + distance metrics (helpful for nearest ANSI/256 mapping).
- SixLabors.ImageSharp — robust color structs and conversion utilities for pixel/color math.
- SkiaSharp — advanced color transforms and profiles if you need higher-fidelity conversions.
- System.Drawing.Common — basic Color struct; use only for simple tasks (limited color-space ops).
- Spectre.Console — terminal rendering with 24-bit color support and built-in detection/fallbacks; good backend candidate.
- Terminal.Gui — TUI framework; check its color model and truecolor support before using as backend.
- ANSI/256 mapping — likely implement xterm palette nearest-color algorithm or use ColorMine to pick nearest index.
- Truecolor detection patterns: COLORTERM=truecolor, TERM value, Windows Terminal capabilities; Spectre.Console can simplify this.
- Suggested starting stack: Colourful (conversions) + ImageSharp (color math) + Spectre.Console (terminal rendering/fallbacks).

