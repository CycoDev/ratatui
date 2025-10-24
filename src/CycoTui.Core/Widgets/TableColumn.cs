using CycoTui.Core.Layout;
using CycoTui.Core.Style;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Column specification for Table widget.
/// </summary>
public sealed class TableColumn
{
    public string Header { get; }
    public Style HeaderStyle { get; }
    public Constraint Constraint { get; }

    public TableColumn(string header, Constraint constraint, Style headerStyle)
    {
        Header = header;
        Constraint = constraint;
        HeaderStyle = headerStyle;
    }
}
