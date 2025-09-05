## ratatui-widgets\src\polyfills.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: pure-Rust fallbacks for floating-point ops used in no_std contexts (embedded/constrained).
- Key ops to cover in .NET: mul_add (FMA), round, floor, sin, cos.
- Precision: micromath trigonometric polyfills accept ~0.002 max error — fine for UI rendering.
- First check: System.Math / System.MathF on your target .NET runtime (full .NET, .NET Core, Mono) — they usually provide sin/cos/floor/round.
- FMA support: verify if runtime exposes fused multiply-add (or use System.Runtime.Intrinsics for hardware FMA); otherwise implement software polyfill.
- Candidate .NET libraries: MathNet.Numerics (feature-rich/heavy) — prefer built-ins or intrinsics for small footprint.
- Constrained runtimes to research: .NET nanoFramework, .NET Micro Framework, Unity, Blazor WASM — may lack full math/intrinsics.
- Conditional builds: use #if TFMs (NET7_0, NETSTANDARD, etc.) to select runtime vs polyfill implementations.
- Testing: create unit tests comparing to System.Math reference with explicit error tolerances like micromath.
- Memory/perf: avoid allocations, prefer stack-based math, AOT-friendly code, and minimal dependencies for embedded use.
- Port option: port micromath algorithms to C# if no suitable lightweight library exists; preserve documented error bounds.
- Prioritize these for widgets: Canvas, Gauge, Scrollbar — trig and FMA correctness affect rendering geometry.

