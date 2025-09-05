# Source File Inventory

This document provides an inventory of the core Ratatui source files organized by component. This will guide our documentation development process.

## Core/Common Component

| File | Description |
|------|-------------|
| `/ratatui-core/src/lib.rs` | Core library entry point |
| `/ratatui-core/src/symbols.rs` | Symbol definitions (main module) |
| `/ratatui-core/src/symbols/bar.rs` | Bar symbols |
| `/ratatui-core/src/symbols/block.rs` | Block symbols |
| `/ratatui-core/src/symbols/border.rs` | Border symbols |
| `/ratatui-core/src/symbols/braille.rs` | Braille symbols |
| `/ratatui-core/src/symbols/half_block.rs` | Half block symbols |
| `/ratatui-core/src/symbols/line.rs` | Line symbols |
| `/ratatui-core/src/symbols/marker.rs` | Marker symbols |
| `/ratatui-core/src/symbols/merge.rs` | Symbol merging |
| `/ratatui-core/src/symbols/scrollbar.rs` | Scrollbar symbols |
| `/ratatui-core/src/symbols/shade.rs` | Shade symbols |
| `/ratatui/src/lib.rs` | Main library entry point |
| `/ratatui/src/prelude.rs` | Prelude module for common imports |
| `/ratatui/src/init.rs` | Initialization utilities |

## Backend Component

| File | Description |
|------|-------------|
| `/ratatui-core/src/backend.rs` | Backend abstraction (main module) |
| `/ratatui-core/src/backend/test.rs` | Test backend implementation |
| `/ratatui-core/src/terminal.rs` | Terminal abstraction (main module) |
| `/ratatui-core/src/terminal/frame.rs` | Frame for rendering |
| `/ratatui-core/src/terminal/terminal.rs` | Terminal implementation |
| `/ratatui-core/src/terminal/viewport.rs` | Viewport handling |
| `/ratatui-crossterm/src/lib.rs` | Crossterm backend implementation |
| `/ratatui-termion/src/lib.rs` | Termion backend implementation |
| `/ratatui-termwiz/src/lib.rs` | Termwiz backend implementation |

## Buffer Component

| File | Description |
|------|-------------|
| `/ratatui-core/src/buffer.rs` | Buffer module (main module) |
| `/ratatui-core/src/buffer/assert.rs` | Buffer assertions |
| `/ratatui-core/src/buffer/buffer.rs` | Buffer implementation |
| `/ratatui-core/src/buffer/cell.rs` | Cell implementation |

## Text and Style Component

| File | Description |
|------|-------------|
| `/ratatui-core/src/style.rs` | Style module (main module) |
| `/ratatui-core/src/style/anstyle.rs` | ANStyle integration |
| `/ratatui-core/src/style/color.rs` | Color implementation |
| `/ratatui-core/src/style/palette.rs` | Color palettes (main module) |
| `/ratatui-core/src/style/palette/material.rs` | Material palette |
| `/ratatui-core/src/style/palette/tailwind.rs` | Tailwind palette |
| `/ratatui-core/src/style/palette_conversion.rs` | Palette conversions |
| `/ratatui-core/src/style/stylize.rs` | Stylize trait |
| `/ratatui-core/src/text.rs` | Text module (main module) |
| `/ratatui-core/src/text/grapheme.rs` | Grapheme handling |
| `/ratatui-core/src/text/line.rs` | Line implementation |
| `/ratatui-core/src/text/masked.rs` | Masked text |
| `/ratatui-core/src/text/span.rs` | Span implementation |
| `/ratatui-core/src/text/text.rs` | Text implementation |

## Layout Component

| File | Description |
|------|-------------|
| `/ratatui-core/src/layout.rs` | Layout module (main module) |
| `/ratatui-core/src/layout/alignment.rs` | Alignment handling |
| `/ratatui-core/src/layout/constraint.rs` | Layout constraints |
| `/ratatui-core/src/layout/direction.rs` | Layout direction |
| `/ratatui-core/src/layout/flex.rs` | Flex layout |
| `/ratatui-core/src/layout/layout.rs` | Layout implementation |
| `/ratatui-core/src/layout/margin.rs` | Margin handling |
| `/ratatui-core/src/layout/position.rs` | Position handling |
| `/ratatui-core/src/layout/rect.rs` | Rectangle implementation |
| `/ratatui-core/src/layout/rect/iter.rs` | Rectangle iteration |
| `/ratatui-core/src/layout/size.rs` | Size handling |

## Widget Component

| File | Description |
|------|-------------|
| `/ratatui-core/src/widgets.rs` | Widgets module (main module) |
| `/ratatui-core/src/widgets/stateful_widget.rs` | Stateful widget trait |
| `/ratatui-core/src/widgets/widget.rs` | Widget trait |
| `/ratatui/src/widgets.rs` | Widgets module in main crate |
| `/ratatui/src/widgets/stateful_widget_ref.rs` | Stateful widget ref trait |
| `/ratatui/src/widgets/widget_ref.rs` | Widget ref trait |
| `/ratatui-widgets/src/lib.rs` | Widgets library entry point |
| `/ratatui-widgets/src/barchart.rs` | Bar chart widget (main module) |
| `/ratatui-widgets/src/barchart/bar.rs` | Bar implementation |
| `/ratatui-widgets/src/barchart/bar_group.rs` | Bar group implementation |
| `/ratatui-widgets/src/block.rs` | Block widget (main module) |
| `/ratatui-widgets/src/block/padding.rs` | Block padding |
| `/ratatui-widgets/src/borders.rs` | Border styles |
| `/ratatui-widgets/src/calendar.rs` | Calendar widget |
| `/ratatui-widgets/src/canvas.rs` | Canvas widget (main module) |
| `/ratatui-widgets/src/canvas/circle.rs` | Circle shape |
| `/ratatui-widgets/src/canvas/line.rs` | Line shape |
| `/ratatui-widgets/src/canvas/map.rs` | Map rendering |
| `/ratatui-widgets/src/canvas/points.rs` | Points rendering |
| `/ratatui-widgets/src/canvas/rectangle.rs` | Rectangle shape |
| `/ratatui-widgets/src/canvas/world.rs` | World coordinates |
| `/ratatui-widgets/src/chart.rs` | Chart widget |
| `/ratatui-widgets/src/clear.rs` | Clear widget |
| `/ratatui-widgets/src/gauge.rs` | Gauge widget |
| `/ratatui-widgets/src/list.rs` | List widget (main module) |
| `/ratatui-widgets/src/list/item.rs` | List item |
| `/ratatui-widgets/src/list/rendering.rs` | List rendering |
| `/ratatui-widgets/src/list/state.rs` | List state |
| `/ratatui-widgets/src/logo.rs` | Logo widget |
| `/ratatui-widgets/src/mascot.rs` | Mascot widget |
| `/ratatui-widgets/src/paragraph.rs` | Paragraph widget |
| `/ratatui-widgets/src/polyfills.rs` | Polyfill utilities |
| `/ratatui-widgets/src/reflow.rs` | Text reflowing |
| `/ratatui-widgets/src/scrollbar.rs` | Scrollbar widget |
| `/ratatui-widgets/src/sparkline.rs` | Sparkline widget |
| `/ratatui-widgets/src/table.rs` | Table widget (main module) |
| `/ratatui-widgets/src/table/cell.rs` | Table cell |
| `/ratatui-widgets/src/table/highlight_spacing.rs` | Table highlight spacing |
| `/ratatui-widgets/src/table/row.rs` | Table row |
| `/ratatui-widgets/src/table/state.rs` | Table state |
| `/ratatui-widgets/src/tabs.rs` | Tabs widget |

## Macros Component

| File | Description |
|------|-------------|
| `/ratatui-macros/src/lib.rs` | Macros library entry point |
| `/ratatui-macros/src/layout.rs` | Layout macros |
| `/ratatui-macros/src/line.rs` | Line macros |
| `/ratatui-macros/src/row.rs` | Row macros |
| `/ratatui-macros/src/span.rs` | Span macros |
| `/ratatui-macros/src/text.rs` | Text macros |

## Build Utilities Component

| File | Description |
|------|-------------|
| `/xtask/src/main.rs` | Build utilities entry point |
| `/xtask/src/commands.rs` | Commands module |
| `/xtask/src/commands/backend.rs` | Backend-related utilities |
| `/xtask/src/commands/check.rs` | Code checking utilities |
| `/xtask/src/commands/clippy.rs` | Clippy integration |
| `/xtask/src/commands/coverage.rs` | Test coverage utilities |
| `/xtask/src/commands/docs.rs` | Documentation utilities |
| `/xtask/src/commands/format.rs` | Code formatting utilities |
| `/xtask/src/commands/rdme.rs` | README generation utilities |
| `/xtask/src/commands/test_docs.rs` | Doc testing utilities |
| `/xtask/src/commands/typos.rs` | Typo checking utilities |