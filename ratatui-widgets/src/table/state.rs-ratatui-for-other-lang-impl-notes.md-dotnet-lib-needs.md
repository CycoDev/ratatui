## ratatui-widgets\src\table\state.rs-ratatui-for-other-lang-impl-notes.md

- TableState is purely a data container: offset, selected row (Option), selected column (Option), and cell selection.
- No terminal or OS-specific code in this file — state only.
- Minimal dependencies in Rust: optional serde for serialization.
- Key behaviors to port: selection APIs (select, select_column, select_cell) and navigation (next/previous/first/last for rows and columns).
- Scrolling helpers: scroll_up/down_by and scroll_left/right_by (saturation/bounds-safe arithmetic).
- Boundary handling: methods prevent underflow/overflow; selection correction happens during rendering in Rust.
- Pattern: clear separation between state and rendering; maintain state across render cycles.
- Immutable vs mutable APIs: Rust offers both patterns — choose idiomatic .NET approach (mutable class or immutable record + fluent).
- Serialization: use System.Text.Json or Newtonsoft.Json if you need serde-like support.
- Unicode grapheme handling: use System.Globalization.StringInfo or a grapheme-segmentation library for correct cell width/selection.
- .NET TUI rendering libraries to research: Spectre.Console (rich rendering), Terminal.Gui / gui.cs (widgets), or build atop System.Console with custom rendering.
- Conclusion: this file is about state only; for porting, research .NET rendering/console libraries and grapheme support — no other native libraries are required.

