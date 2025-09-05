## ratatui-core\src\layout\constraint.rs-ratatui-for-other-lang-impl-notes.md

- Core: six Constraint kinds to model: Min, Max, Length, Percentage, Ratio, Fill — preserve same API surface.
- Types: Rust u16 → C# ushort; u32 → uint. Keep integer ranges in mind.
- Collections: alloc::vec::Vec → System.Collections.Generic.List<T>.
- Display: core::fmt::Display → override ToString().
- Enum/sum-type: Rust enum → C# options: class/record hierarchy, discriminated-union library (OneOf, LanguageExt, SharpUnion) or manual pattern matching.
- Serialization: serde optional → System.Text.Json (custom converters) or Newtonsoft.Json for easy polymorphic support.
- Floating math: use double and explicit rounding; pick a consistent MidpointRounding (e.g., AwayFromZero) to match Rust behavior.
- Division-by-zero: guard denominators; treat zero as 1 per original.
- No-std note: Rust avoids std — in .NET prefer minimal runtime target (netstandard/.NET 6+) if embedding or trimming is needed.
- Factory methods/From conversions: implement static factories and implicit/explicit conversions as needed.
- Constraint solver (Kasuari) and layout logic: no direct .NET library — plan to port solver or implement equivalent distribution, caching, margins, spacing.
- Priority and resolution rules must be identical (Min highest, Fill lowest); ensure tests match rounding and caps.



