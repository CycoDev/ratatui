## xtask\src\commands\check.rs-ratatui-for-other-lang-impl-notes.md

- Key Rust terminal backends to mirror in research: crossterm (cross-platform), termion (Unix-only), termwiz (cross-platform).
- Look for .NET equivalents that are cross‑platform vs Unix‑only (Windows console API vs POSIX/tty).
- The Rust tooling tests multiple versions of a backend (crossterm 0.28/0.29); plan to check compatibility across library versions in .NET.
- Implement a core abstraction layer (platform‑agnostic rendering, events, widgets) before binding to concrete .NET terminal libs.
- Create separate backend adapters for each terminal library you evaluate in .NET.
- Provide a feature/flag system or build-time configuration to enable/disable backends per platform.
- Use platform detection to choose/back off from incompatible backends (Rust uses cfg!(windows)); .NET has RuntimeInformation APIs.
- Ensure dependency management can tolerate multiple versions or optional features.
- Build a unified test matrix: OSes (Windows/macOS/Linux) × terminal backends × feature combinations.
- Conditionally skip or provide clear errors for incompatible backend/platform combinations.
- Prioritize cross‑platform libraries first, then platform‑specific ones for optimizations or native features.

