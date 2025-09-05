## ratatui\src\widgets\stateful_widget_ref.rs-ratatui-for-other-lang-impl-notes.md

- Purpose: trait for non-consuming, reference-based rendering of widgets that keep mutable state between renders.  
- Core API: render_ref(area: Rect, buf: Buffer, state: ref State). Research how to express ref/out params in .NET (ref, in/out).  
- State: associated, possibly unsized in Rust — in .NET use interface/base State class or object with generics.  
- Non-consuming widgets: widget instances must be reusable; design interfaces that don't transfer ownership.  
- Heterogeneous collections: use interface types (IStatefulWidget) or System.Object with dynamic dispatch.  
- Dynamic dispatch: research interface dispatch, delegates, and virtual methods for performance.  
- Buffer and Rect: need framebuffer and layout types; investigate Spectre.Console and Terminal.Gui for equivalents.  
- Memory model: .NET GC removes manual lifetimes; consider pooling for performance.  
- Boxing strategy: use interfaces, abstract base classes, or generics for type erasure.  
- Cross-platform: trait is platform-agnostic — pick cross-platform .NET terminal lib (Spectre.Console, Terminal.Gui).

