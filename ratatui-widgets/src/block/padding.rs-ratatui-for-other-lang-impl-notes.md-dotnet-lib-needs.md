## ratatui-widgets\src\block\padding.rs-ratatui-for-other-lang-impl-notes.md

- Data: Padding has four u16 fields: left, right, top, bottom → use ushort in .NET.
- API: multiple factories (new, uniform, horizontal, vertical, symmetric, proportional, per-side) → implement as static factory methods or overloaded constructors.
- Proportional: doubles horizontal padding to account for terminal cell aspect ratio — keep this behavior or make it configurable.
- Zero: provide a static readonly ZERO/default instance.
- Immutability: methods return new instances → use immutable record/readonly struct or make properties get-only.
- Const constructors: Rust const fn → in .NET use static readonly members; C# has no const methods.
- Underflow safety: subtracting padding from bounds must guard (use Math.Max/Clamp or checked arithmetic).
- Serialization feature: optional Serde → in .NET use System.Text.Json or Newtonsoft.Json (conditional attributes/flags if optional).
- No-std: irrelevant for .NET runtime; but keep minimal dependencies for small targets (.NET Nano/.NET Core trimmed).
- Testing: port tests to xUnit/NUnit/MSTest; include unit tests for each factory and edge cases.
- Rendering integration: ensure padding applied when computing inner rectangle and text placement.
- Libraries to research for terminal UI in .NET: Spectre.Console (rich terminal rendering), Terminal.Gui (gui.cs) for higher-level widgets, and potential ncurses/ANSI helpers for low-level control.

(These are the essential points to guide selecting .NET libraries and implementing equivalent behavior.)

