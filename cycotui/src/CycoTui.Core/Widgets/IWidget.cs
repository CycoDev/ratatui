using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;

namespace CycoAI.CycoTui.Core.Widgets
{
    /// <summary>
    /// Defines the interface for a stateless widget that can render itself to a buffer.
    /// Widgets are the building blocks of terminal user interfaces.
    /// </summary>
    public interface IWidget
    {
        /// <summary>
        /// Renders the widget to the specified area in the buffer.
        /// </summary>
        /// <param name="area">The rectangular area where the widget should be rendered.</param>
        /// <param name="buffer">The buffer to render to.</param>
        void Render(Rect area, IBuffer buffer);
    }

    /// <summary>
    /// Defines the interface for a stateful widget that can render itself to a buffer
    /// while maintaining internal state.
    /// </summary>
    /// <typeparam name="TState">The type of state associated with this widget.</typeparam>
    public interface IStatefulWidget<TState>
    {
        /// <summary>
        /// Renders the widget to the specified area in the buffer using the provided state.
        /// </summary>
        /// <param name="area">The rectangular area where the widget should be rendered.</param>
        /// <param name="buffer">The buffer to render to.</param>
        /// <param name="state">The state object that contains the widget's current state.</param>
        void Render(Rect area, IBuffer buffer, ref TState state);
    }
}