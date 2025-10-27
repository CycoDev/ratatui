using System.Collections.Generic;
using CycoTui.Core.Style;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Represents a row in the Table widget.
/// </summary>
public sealed class TableRow
{
    public IReadOnlyList<(string Text, StyleType Style)> Cells { get; }

    public TableRow(IReadOnlyList<(string Text, StyleType Style)> cells) => Cells = cells;
}
