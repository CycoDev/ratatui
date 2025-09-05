## ratatui\tests\state_serde.rs-ratatui-for-other-lang-impl-notes.md

- Focus: widget state serialization tests (ListState, TableState, ScrollbarState) — relevant when choosing .NET serialization and state patterns.
- State fields: offset (usize) → use int/uint; selected/selected_column → Option<usize> → use int? (nullable int) or Nullable<int>.
- Serialization format used: JSON (platform-agnostic). .NET equivalents: System.Text.Json or Newtonsoft.Json.
- Serde derive with feature flag → in .NET implement conditional serialization via build flags or separate assemblies; use attributes like [JsonPropertyName], [JsonIgnore], [JsonConstructor].
- Fluent interface: builder-like methods (with_selected) — map to method chaining or immutable-with patterns in .NET.
- Backwards compatibility: tests deserialize older formats; in .NET use nullable fields, default values, [JsonExtensionData], and version fields when evolving schemas.
- Separate state from rendering logic — keep POCO state classes independent of terminal/backend implementations.
- Rendering tests use a TestBackend buffer — in .NET emulate with virtual console buffers or unit-testable renderers (e.g., mockable Spectre.Console, or a custom TestConsole).
- Scrollbar state includes content_length, position, viewport_content_length — ensure numeric ranges and clamping logic preserved.
- Tests validate visual equivalence before/after serialize-deserialize — include rendering assertions in .NET tests.
- Dependency checklist for .NET port: JSON library (System.Text.Json/Newtonsoft), terminal UI library (Spectre.Console, Terminal.Gui, or a custom renderer), unit-test framework (xUnit/NUnit), mocking for backend.
- Handle Rust usize semantics (platform dependent) — prefer fixed-size ints (int/long) in .NET to avoid cross-platform surprises.
- Recommendation: prototype state POCOs, serialize with System.Text.Json, add tests that render to a test buffer to confirm behavior parity.

