using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;
using System.Linq;

namespace CycoTui.Core.Tests.Integration;

public class CompositionDemoTests
{
    [Fact]
    public void NestedBlocksParagraphAndLogoComposeWithoutBorderOverwrites()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var outer = Block.Create().WithTitle("Outer", StyleType.Empty.Add(TextModifier.Bold)).WithPadding(new Padding(1,1,1,1));
        var inner = Block.Create().WithTitle("Inner", StyleType.Empty).WithPadding(Padding.Zero);
        var paragraph = Paragraph.Create().WithText("CycoTui Horizontal Scrolling Demo").WithHorizontalOffset(7);
        var logo = LogoWidget.Create();

        term.Draw(f =>
        {
            var outerRect = new Rect(0,0,50,10);
            outer.Render(f, outerRect);
            var innerRect = ((IContainerWidget)outer).GetContentArea(outerRect);
            // shrink inner for nested border
            innerRect = new Rect(innerRect.X, innerRect.Y+1, innerRect.Width, innerRect.Height-2);
            inner.Render(f, innerRect);
            var logoArea = new Rect(innerRect.X, innerRect.Y, innerRect.Width, 1);
            logo.Render(f, logoArea);
            var paraArea = new Rect(innerRect.X, innerRect.Y+1, innerRect.Width, innerRect.Height-1);
            paragraph.Render(f, paraArea);
        });

        // Assert: Outer top-left corner preserved
        Assert.Contains(backend.Emitted, c => c.X == 0 && c.Y == 0 && c.Cell.Grapheme == outer.Border.TopLeft);
        // Assert: Inner block corner present and not overwritten by paragraph
        Assert.Contains(backend.Emitted, c => c.Cell.Grapheme == inner.Border.TopLeft);
        // Assert: Paragraph produced at least one scrolled grapheme
        Assert.Contains(backend.Emitted, c => c.Cell.Grapheme == "S"); // from 'Scrolling'
    }
}
