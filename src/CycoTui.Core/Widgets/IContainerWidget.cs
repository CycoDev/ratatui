using CycoTui.Core.Layout;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Contract for a widget that defines an inner content region within a provided outer area.
/// </summary>
public interface IContainerWidget : IWidget
{
    /// <summary>Compute the inner content area given the outer area allocated to the container.</summary>
    Rect GetContentArea(Rect outerArea);
}
