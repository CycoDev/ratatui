using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using CycoTui.Core.Terminal;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Scrollbar widget visualizing scroll state given content size, viewport size, and current offset.
/// </summary>
public sealed class ScrollbarWidget : IWidget
{
    public ScrollbarOrientation Orientation { get; init; } = ScrollbarOrientation.Vertical;
    public int ContentLength { get; init; } = 0;
    public int ViewportLength { get; init; } = 0;
    public int Offset { get; init; } = 0;
    public StyleType BarStyle { get; init; } = StyleType.Empty.Add(TextModifier.Invert);
    public StyleType TrackStyle { get; init; } = StyleType.Empty;
    public char TrackChar { get; init; } = '░';
    public char BarChar { get; init; } = '█';

    private ScrollbarWidget() { }
    public static ScrollbarWidget Create() => new();

    public ScrollbarWidget WithData(ScrollbarOrientation orientation, int contentLength, int viewportLength, int offset) => new()
    {
        Orientation = orientation,
        ContentLength = contentLength < 0 ? 0 : contentLength,
        ViewportLength = viewportLength < 0 ? 0 : viewportLength,
        Offset = offset < 0 ? 0 : offset,
        BarStyle = BarStyle,
        TrackStyle = TrackStyle,
        TrackChar = TrackChar,
        BarChar = BarChar
    };

    public void Render(Frame frame, Rect area)
    {
        if (ContentLength <= ViewportLength || ViewportLength <= 0) return; // no scroll needed
        int barSize = ComputeBarSize(area);
        int barPos = ComputeBarPosition(area, barSize);

        if (Orientation == ScrollbarOrientation.Vertical)
        {
            for (int y = 0; y < area.Height; y++)
            {
                var ch = (y >= barPos && y < barPos + barSize) ? BarChar.ToString() : TrackChar.ToString();
                var style = (y >= barPos && y < barPos + barSize) ? BarStyle : TrackStyle;
                frame.SetCell(area.X, area.Y + y, ch, style);
            }
        }
        else
        {
            for (int x = 0; x < area.Width; x++)
            {
                var ch = (x >= barPos && x < barPos + barSize) ? BarChar.ToString() : TrackChar.ToString();
                var style = (x >= barPos && x < barPos + barSize) ? BarStyle : TrackStyle;
                frame.SetCell(area.X + x, area.Y, ch, style);
            }
        }
    }

    private int ComputeBarSize(Rect area)
    {
        int trackLength = Orientation == ScrollbarOrientation.Vertical ? area.Height : area.Width;
        double ratio = ViewportLength / (double)ContentLength;
        int size = (int)(trackLength * ratio);
        if (size < 1) size = 1;
        if (size > trackLength) size = trackLength;
        return size;
    }

    private int ComputeBarPosition(Rect area, int barSize)
    {
        int trackLength = Orientation == ScrollbarOrientation.Vertical ? area.Height : area.Width;
        int maxOffset = ContentLength - ViewportLength;
        if (maxOffset <= 0) return 0;
        double progress = Offset / (double)maxOffset;
        int pos = (int)((trackLength - barSize) * progress);
        return pos < 0 ? 0 : pos;
    }
}
