using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;
using CycoTui.Core.Layout;
using Xunit;
using System.Linq;

namespace CycoTui.Core.Tests;

public class ParagraphWidgetTests
{
    [Fact]
    public void ParagraphWrapsLongText()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var para = Paragraph.Create().WithText("HelloWorld", Style.Empty); // width 10
        term.Draw(f => para.Render(f, new Rect(0,0,5,3))); // should wrap
        // Expect multiple frames? single frame; verify at least one cell beyond width 5 triggers second line
        Assert.True(backend.Emitted.Any(c => c.Y == 1));
    }

    [Fact]
    public void ParagraphCenterAlignment()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var para = Paragraph.Create().WithText("Hi", Style.Empty).WithAlignment(ParagraphAlignment.Center);
        term.Draw(f => para.Render(f, new Rect(0,0,6,2)));
        // Centered in width 6: display width 2 => offset 2
        Assert.Contains(backend.Emitted, c => c.X == 2 && c.Y == 0);
    }

    [Fact]
    public void ParagraphRightAlignment()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var para = Paragraph.Create().WithText("Hi", Style.Empty).WithAlignment(ParagraphAlignment.Right);
        term.Draw(f => para.Render(f, new Rect(0,0,6,2)));
        // Right offset: width 6 - text 2 = 4
        Assert.Contains(backend.Emitted, c => c.X == 4 && c.Y == 0);
    }

    [Fact]
    public void ParagraphMaxLinesLimitsRendering()
    {
        var backend = new TestBackend();
        var term = new Terminal(backend, new LoggingContext(null));
        var para = Paragraph.Create().WithText("LineOneLineTwoLineThree", Style.Empty).WithMaxLines(1);
        term.Draw(f => para.Render(f, new Rect(0,0,5,5)));
        Assert.False(backend.Emitted.Any(c => c.Y == 1));
    }
}
