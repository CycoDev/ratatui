using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Layout;
using System.Linq;

namespace CycoTui.Core.Tests;

public class LogoWidgetTests
{
    [Fact]
    public void CenterAlignmentPlacesLogoRoughlyCentered()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var logo = LogoWidget.Create();
        term.Draw(f => logo.Render(f, new Rect(0,0,20,3)));
        var minX = backend.Emitted.Where(c => c.Y == 0).Min(c => c.X);
        Assert.True(minX >= 5); // centered in width 20
    }

    [Fact]
    public void RightAlignmentPlacesLogoNearRightEdge()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var logo = LogoWidget.Create().WithAlignment(ParagraphAlignment.Right);
        term.Draw(f => logo.Render(f, new Rect(0,0,20,3)));
        var maxX = backend.Emitted.Where(c => c.Y == 0).Max(c => c.X);
        Assert.True(maxX >= 13); // near right edge
    }
}
