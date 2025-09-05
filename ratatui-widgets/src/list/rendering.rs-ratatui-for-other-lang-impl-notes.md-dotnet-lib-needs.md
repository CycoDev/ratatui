## ratatui-widgets\src\list\rendering.rs-ratatui-for-other-lang-impl-notes.md

- Need a cross-platform terminal backend: raw mode, alternate screen, mouse capture (Windows + Unix).
- Unicode width handling (wcwidth) for correct layout of wide/combining chars.
- Double-buffer or diffed rendering to minimize terminal updates.
- Layout/Rect system to compute areas and clipping.
- Text measurement and wrapping for multi-line items.
- Style/color/attribute support with fallbacks for limited terminals.
- Scroll/viewport management with scroll-padding and virtualized rendering.
- Selection state management (ListState) and highlighted-item styling.
- Support for directional rendering (top-to-bottom and bottom-to-top).
- Border/block primitives for framed widgets.
- Efficient rendering of variable-height items and boundary handling.
- Tests/mocks for terminal buffer and no-tty CI testing.
- .NET libraries to research: Spectre.Console, Terminal.Gui (gui.cs), Konscious/ConsoleControl, SharpTerm — verify which provide raw mode, unicode width, buffered rendering, and layout primitives.

