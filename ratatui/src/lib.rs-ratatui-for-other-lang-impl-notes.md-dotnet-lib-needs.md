## ratatui\src\lib.rs-ratatui-for-other-lang-impl-notes.md

- Candidate .NET library explicitly mentioned: Terminal.Gui (https://github.com/gui-cs/Terminal.Gui).
- Need a backend abstraction in .NET: research libraries that can be wrapped behind a common terminal interface.
- Required terminal capabilities to verify in any .NET library: enter/exit alternate screen, enable/disable raw mode, set cursor position, clear screen/regions, show/hide cursor.
- Input/event support to confirm: non-blocking keyboard input, key combinations/special keys, mouse events (click/drag), and window resize events.
- Styling support: colors, bold/italic/underline and varying terminal color modes (16, 256, truecolor).
- Unicode and text handling: proper unicode width calculation, grapheme-cluster-aware cursor positioning, and truncation that respects grapheme boundaries.
- Rendering model: buffer-based (double-buffering), diffing of buffer cells to minimize terminal updates—check library support or ease of implementing this.
- Layout system requirements: constraint-based sizing (fixed, percent, remainder), vertical/horizontal splits, hierarchical composition.
- Widget architecture: immediate-mode rendering with stateless widgets (or stateful widgets where needed)—look for or design a Widget trait/interface analog.
- Performance considerations: ability to implement efficient diffing and minimize redraws for flicker-free UI.
- Init/cleanup helpers: ability to enter/exit alternate screen and enable/disable raw mode safely from .NET.
- Windows-specific: ensure library supports Windows console API and Windows Terminal behaviors; test on legacy console and Windows Terminal.
- Terminal feature detection: ability to probe terminal capabilities (ANSI/VT support, color depth, style support).
- Common challenges to confirm support or mitigation strategy: differing color/style support across terminals and varying Unicode/emoji rendering behavior.

