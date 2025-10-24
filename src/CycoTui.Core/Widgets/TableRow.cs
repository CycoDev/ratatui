using System.Collections.Generic;
using CycoTui.Core.Style;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Represents a row in the Table widget.
/// </summary>
public sealed class TableRow
{
    public IReadOnlyList<(string Text, Style Style)> Cells { get; }

    public TableRow(IReadOnlyList<(string Text, Style Style)> cells) => Cells = cells;
}
