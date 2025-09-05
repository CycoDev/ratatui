## xtask\src\commands\test_docs.rs-ratatui-for-other-lang-impl-notes.md

- Research .NET cross-platform console/terminal libraries: Spectre.Console, Terminal.Gui, and any VT/ANSI-focused libs.
- Investigate Windows-specific terminals (Windows Console/WinAPI + VT support) and enabling ANSI on Windows.
- Find libraries that provide alternate screen, raw mode, and terminal resize events.
- Look for input/event handling libs: keyboard, mouse, and async event streams.
- Check Unicode and wide-character support in candidate libraries.
- Verify color capabilities: ANSI 16/256, truecolor (RGB) support and fallbacks.
- Identify style attribute support (bold, italic, underline) and how to map to terminals.
- Explore modular architecture patterns in .NET (core + backend implementations) and conditional compilation (#if, multi-targeting).
- Plan feature-flag/version management: NuGet package versions and testing against multiple dependency versions.
- CI/testing: dotnet test across Windows/macOS/Linux, and include doc/example-snippet tests if possible.
- Map terminal capability detection APIs and fallbacks in .NET libs.
- Prefer libraries with active maintenance, cross-platform parity, and good docs/examples.

