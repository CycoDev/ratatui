## ratatui\src\widgets\widget_ref.rs-ratatui-for-other-lang-impl-notes.md

- Key goal: render widgets by reference (non-consuming) — model as IWidgetRef.Render(Rect, Buffer).
- Look for .NET libraries that offer a full-screen, cell-buffer model: Terminal.Gui (gui.cs) and SadConsole.
- For higher-level console features (colors, live rendering, progress): evaluate Spectre.Console.
- For low-level terminal control and capabilities (ANSI/VT, cursor, modes): consider ncurses bindings or direct ANSI + Windows VT APIs.
- Buffer abstraction: need a 2D cell struct (char + style/colors); prefer libraries that expose/allow custom buffers or provide backbuffer access.
- Efficient updates: research libraries or examples that support diffing/double-buffer blit to minimize redraws.
- Heterogeneous widget collections: use interfaces (IWidgetRef) and object references; .NET GC removes manual refcount concerns.
- Stateful vs stateless: define separate interfaces (IStatefulWidgetRef) like in ratatui.
- Memory & performance: use Span<T>/Memory<T> for buffers, minimize allocations, consider pooling.
- Rendering loop: look for libraries that let you control the loop or offer a swap-buffer render API.
- Cross-platform terminal backends: ensure chosen lib supports Windows (VT), Linux, macOS consistently.
- Interop: if needed, P/Invoke native terminals for features missing in managed libs.
- Concurrency: use async/Task for input and rendering separation if library allows.
- Pick libraries that allow custom widget composition (embedding one widget inside another) and dynamic dispatch for boxed widgets.

