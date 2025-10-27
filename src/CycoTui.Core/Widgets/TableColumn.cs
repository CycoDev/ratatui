using CycoTui.Core.Layout;
using CycoTui.Core.Style;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Column specification for Table widget.
/// </summary>
public sealed class TableColumn
{
    public string Header { get; }
    public StyleType HeaderStyle { get; }
    public Constraint Constraint { get; }

    public TableColumn(string header, Constraint constraint, StyleType headerStyle)
    {
        Header = header;
        Constraint = constraint;
        HeaderStyle = headerStyle;
    }
}
