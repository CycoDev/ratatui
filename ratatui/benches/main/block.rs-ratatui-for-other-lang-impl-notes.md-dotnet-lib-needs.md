## ratatui\benches\main\block.rs-ratatui-for-other-lang-impl-notes.md

- Need a cross‑platform terminal backend: research .NET libraries that expose raw ANSI/VT sequences and Windows Console APIs.
- Check Spectre.Console (high‑level rendering, styling, Unicode) for reuse or inspiration.
- Check Terminal.Gui for widget patterns (higher‑level, not raw terminal control).
- Ensure ability to enable VT processing on Windows (System.Console + Win32 calls) or use a VT‑capable wrapper.
- Look for libraries or patterns that provide an off‑screen Buffer/double‑buffering API to minimize writes.
- Verify full Unicode and box‑drawing character support (including combining characters).
- Library must allow emitting ANSI SGR color/attribute sequences or use Windows attribute mappings.
- Prefer backends that support batched writes or provide diffing/virtual DOM to reduce I/O.
- Need precise cursor control, clipping/area calculations, and region rendering APIs.
- Require async/key+mouse input handling across platforms.
- Performance features: reuse style objects, minimize allocations, efficient inner‑area math.
- Must support drawing primitives for borders (plain/rounded/double/thick), titles, padding, and border‑merging logic.
- Avoid GUI frameworks; focus on terminal‑only libraries or low‑level console APIs.

