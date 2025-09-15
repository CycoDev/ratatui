using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;

namespace CycoAI.CycoTui.Core.Widgets
{
    /// <summary>
    /// Abstract base class for stateless widgets.
    /// Provides common functionality and structure for implementing custom widgets.
    /// </summary>
    public abstract class Widget : IWidget
    {
        /// <summary>
        /// Renders the widget to the specified area in the buffer.
        /// </summary>
        /// <param name="area">The rectangular area where the widget should be rendered.</param>
        /// <param name="buffer">The buffer to render to.</param>
        public abstract void Render(Rect area, IBuffer buffer);

        /// <summary>
        /// Gets the minimum size required to render this widget.
        /// Returns (0, 0) by default, indicating the widget can be rendered at any size.
        /// </summary>
        /// <returns>The minimum width and height required.</returns>
        public virtual (int Width, int Height) GetMinimumSize() => (0, 0);

        /// <summary>
        /// Gets the preferred size for rendering this widget.
        /// Returns null by default, indicating the widget has no size preference.
        /// </summary>
        /// <returns>The preferred width and height, or null if no preference.</returns>
        public virtual (int Width, int Height)? GetPreferredSize() => null;

        /// <summary>
        /// Determines whether this widget can be rendered in the specified area.
        /// By default, checks if the area is not empty and meets the minimum size requirements.
        /// </summary>
        /// <param name="area">The area to check.</param>
        /// <returns>true if the widget can be rendered in the area; otherwise, false.</returns>
        public virtual bool CanRender(Rect area)
        {
            if (area.IsEmpty)
                return false;

            var (minWidth, minHeight) = GetMinimumSize();
            return area.Width >= minWidth && area.Height >= minHeight;
        }
    }

    /// <summary>
    /// Abstract base class for stateful widgets.
    /// Provides common functionality for widgets that maintain internal state.
    /// </summary>
    /// <typeparam name="TState">The type of state associated with this widget.</typeparam>
    public abstract class StatefulWidget<TState> : IStatefulWidget<TState>
    {
        /// <summary>
        /// Renders the widget to the specified area in the buffer using the provided state.
        /// </summary>
        /// <param name="area">The rectangular area where the widget should be rendered.</param>
        /// <param name="buffer">The buffer to render to.</param>
        /// <param name="state">The state object that contains the widget's current state.</param>
        public abstract void Render(Rect area, IBuffer buffer, ref TState state);

        /// <summary>
        /// Gets the minimum size required to render this widget.
        /// Returns (0, 0) by default, indicating the widget can be rendered at any size.
        /// </summary>
        /// <returns>The minimum width and height required.</returns>
        public virtual (int Width, int Height) GetMinimumSize() => (0, 0);

        /// <summary>
        /// Gets the preferred size for rendering this widget.
        /// Returns null by default, indicating the widget has no size preference.
        /// </summary>
        /// <returns>The preferred width and height, or null if no preference.</returns>
        public virtual (int Width, int Height)? GetPreferredSize() => null;

        /// <summary>
        /// Determines whether this widget can be rendered in the specified area.
        /// By default, checks if the area is not empty and meets the minimum size requirements.
        /// </summary>
        /// <param name="area">The area to check.</param>
        /// <returns>true if the widget can be rendered in the area; otherwise, false.</returns>
        public virtual bool CanRender(Rect area)
        {
            if (area.IsEmpty)
                return false;

            var (minWidth, minHeight) = GetMinimumSize();
            return area.Width >= minWidth && area.Height >= minHeight;
        }

        /// <summary>
        /// Creates a default state instance for this widget.
        /// Override this method to provide custom default state initialization.
        /// </summary>
        /// <returns>A default state instance.</returns>
        public virtual TState CreateDefaultState()
        {
            if (typeof(TState).IsValueType)
            {
                return default(TState)!;
            }

            // Try to create using parameterless constructor
            var constructor = typeof(TState).GetConstructor(System.Type.EmptyTypes);
            if (constructor != null)
            {
                return (TState)constructor.Invoke(null);
            }

            throw new System.InvalidOperationException(
                $"Cannot create default state for type {typeof(TState).Name}. " +
                "Override CreateDefaultState() or ensure the state type has a parameterless constructor.");
        }
    }
}