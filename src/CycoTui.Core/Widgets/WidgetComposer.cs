using System;
using CycoTui.Core.Layout;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Helper utilities for composing container widgets with nested render actions.
/// </summary>
public static class WidgetComposer
{
    public static void Compose(Frame frame, Rect outer, IContainerWidget container, Action<Frame, Rect> innerRender)
    {
        container.Render(frame, outer);
        var inner = container.GetContentArea(outer);
        innerRender(frame, inner);
    }
}
