using CycoTui.Core.Terminal;
using CycoTui.Core.Layout;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Stateless widget interface. Implementations render into a frame within the provided area.
/// </summary>
public interface IWidget
{
    /// <summary>Render widget content inside <paramref name="area"/>.</summary>
    void Render(Frame frame, Rect area);
}
