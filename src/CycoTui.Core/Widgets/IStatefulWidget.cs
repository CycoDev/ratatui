using CycoTui.Core.Layout;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Stateful widget interface allowing a render with external state object.
/// </summary>
public interface IStatefulWidget<TState>
{
    /// <summary>Render widget with state inside <paramref name="area"/>.</summary>
    void Render(Frame frame, Rect area, TState state);
}
