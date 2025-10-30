using System.Linq;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Stateful table rendering extension.
/// </summary>
public static class TableWidgetStatefulExtensions
{
    public static void Render(this TableWidget table, Frame frame, Rect area, TableState state)
    {
        if (area.Width <= 0 || area.Height <= 0 || table.Columns.Count == 0) return;
        var engine = new CycoTui.Core.Layout.LayoutEngine(new CycoTui.Core.Logging.LoggingContext(null));
        var colRects = engine.Distribute(area, table.Columns.Select(c => c.Constraint).ToArray(), LayoutDirection.Horizontal);
        int headerY = area.Y;
        // Header
        tableRenderRow(frame, colRects, headerY, table.Columns.Select(c => (c.Header, c.HeaderStyle)).ToList(), table.CellStyle, table.Alignment);
        int visibleHeight = area.Height - 1;
        int start = state.Offset;
        for (int i = start; i < table.Rows.Count && i < start + visibleHeight; i++)
        {
            var y = headerY + 1 + (i - start);
            var rowCells = table.Rows[i].Cells;
            var styledCells = rowCells.Select(cell => cell).ToList();
            // Highlight selected row (invert) if match
            if (state.SelectedRow.HasValue && state.SelectedRow.Value == i)
            {
                styledCells = styledCells.Select(c => (c.Text, c.Style.Add(TextModifier.Invert))).ToList();
            }
            tableRenderRow(frame, colRects, y, styledCells, table.CellStyle, table.Alignment);
        }
    }

    private static void tableRenderRow(Frame frame, System.Collections.Generic.IReadOnlyList<Rect> colRects, int y,
        System.Collections.Generic.IReadOnlyList<(string Text, StyleType Style)> cells, StyleType fallback, ParagraphAlignment alignment)
    {
        for (int i = 0; i < colRects.Count && i < cells.Count; i++)
        {
            var (text, style) = cells[i];
            var rect = colRects[i];
            WriteAligned(frame, rect, y, text, style == StyleType.Empty ? fallback : style, alignment);
        }
    }

    private static void WriteAligned(Frame frame, Rect rect, int y, string text, StyleType style, ParagraphAlignment alignment, int horizontalOffset = 0)
    {
        if (rect.Width <= 0) return;
        var graphemes = CycoTui.Core.Text.GraphemeEnumerator.EnumerateWithZwj(text ?? string.Empty).ToList();
        int width = 0; foreach (var g in graphemes) width += CycoTui.Core.Text.WidthService.GetWidth(g);
        int offset = alignment switch
        // TODO: Integrate horizontal offset (future state) for table columns if needed.

        {
            ParagraphAlignment.Left => 0,
            ParagraphAlignment.Center => System.Math.Max(0, (rect.Width - width) / 2),
            ParagraphAlignment.Right => System.Math.Max(0, rect.Width - width),
            _ => 0
        };
        int x = rect.X + offset;
        foreach (var g in graphemes)
        {
            var w = CycoTui.Core.Text.WidthService.GetWidth(g);
            if (x + w > rect.X + rect.Width) break; // truncate
            frame.SetCell(x, y, g, style);
            x += w;
        }
    }
}
