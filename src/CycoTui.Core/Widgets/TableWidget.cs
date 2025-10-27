using System;
using System.Collections.Generic;
using System.Linq;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;
using CycoTui.Core.Text;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Table widget with column constraint distribution, header row, and vertically scrollable body via TableState.
/// </summary>
public sealed class TableWidget : IWidget
{
    public IReadOnlyList<TableColumn> Columns { get; init; } = Array.Empty<TableColumn>();
    public IReadOnlyList<TableRow> Rows { get; init; } = Array.Empty<TableRow>();
    public StyleType CellStyle { get; init; } = StyleType.Empty;
    public ParagraphAlignment Alignment { get; init; } = ParagraphAlignment.Left;
    public int HorizontalOffset { get; init; } = 0;

    private TableWidget() { }
    public static TableWidget Create() => new();

    public TableWidget WithColumns(IReadOnlyList<TableColumn> columns) => new()
    {
        Columns = columns,
        Rows = Rows,
        CellStyle = CellStyle,
        Alignment = Alignment
    };
    public TableWidget WithRows(IReadOnlyList<TableRow> rows) => new()
    {
        Columns = Columns,
        Rows = rows,
        CellStyle = CellStyle,
        Alignment = Alignment
    };
    public TableWidget WithHorizontalOffset(int offset) => new()
    {
        Columns = Columns,
        Rows = Rows,
        CellStyle = CellStyle,
        Alignment = Alignment,
        HorizontalOffset = offset
    };

    public TableWidget WithCellStyle(StyleType style) => new()
    {
        Columns = Columns,
        Rows = Rows,
        CellStyle = style,
        Alignment = Alignment
    };
    public TableWidget WithAlignment(ParagraphAlignment align) => new()
    {
        Columns = Columns,
        Rows = Rows,
        CellStyle = CellStyle,
        Alignment = align
    };

    public void Render(Frame frame, Rect area)
    {
        if (area.Width <= 0 || area.Height <= 0 || Columns.Count == 0) return;
        var constraints = Columns.Select(c => c.Constraint).ToArray();
        var engine = new LayoutEngine(new CycoTui.Core.Logging.LoggingContext(null));
        var colRects = engine.Distribute(area, constraints, LayoutDirection.Horizontal);
        int headerY = area.Y;
        RenderRow(frame, colRects, headerY, Columns.Select(c => (c.Header, c.HeaderStyle)).ToList());
        int rowStartY = headerY + 1;
        int maxRows = area.Height - 1;
        for (int i = 0; i < Rows.Count && i < maxRows; i++)
        {
            RenderRow(frame, colRects, rowStartY + i, Rows[i].Cells);
        }
    }

    private void RenderRow(Frame frame, IReadOnlyList<Rect> colRects, int y, IReadOnlyList<(string Text, StyleType Style)> cells)
    {
        for (int i = 0; i < colRects.Count && i < cells.Count; i++)
        {
            var (text, style) = cells[i];
            var rect = colRects[i];
            WriteAligned(frame, rect, y, text, style);
        }
    }

    private void WriteAligned(Frame frame, Rect rect, int y, string text, StyleType style)
    {
        if (rect.Width <= 0) return;
        var baseText = text ?? string.Empty;
        var graphemes = GraphemeEnumerator.EnumerateWithZwj(baseText).ToList();
        int width = 0; foreach (var g in graphemes) width += WidthService.GetWidth(g);
        int offset = Alignment switch
        {
            ParagraphAlignment.Left => 0,
            ParagraphAlignment.Center => Math.Max(0, (rect.Width - width) / 2),
            ParagraphAlignment.Right => Math.Max(0, rect.Width - width),
            _ => 0
        };
        int x = rect.X + offset;
        // Apply horizontal offset windowing if needed
        IEnumerable<string> window = graphemes;
        if (HorizontalOffset > 0)
        {
            window = CycoTui.Core.Text.HorizontalTextScroller.EnumerateVisibleGraphemes(baseText, HorizontalOffset, rect.Width - offset);
        }
        foreach (var g in window)
        {
            var w = WidthService.GetWidth(g);
            if (x + w > rect.X + rect.Width) break; // truncate
            frame.SetCell(x, y, g, style == StyleType.Empty ? CellStyle : style);
            x += w;
        }
    }
}
