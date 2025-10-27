using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;

namespace CycoTui.Core.Tests;

public class BlockCompositionTests
{
    [Fact]
    public void InnerContentRectAccountsForBorderAndPadding()
    {
        var area = new Rect(0,0,20,6);
        var padding = new Padding(1,1,2,0); // left, top, right, bottom
        var inner = Block.GetInnerContentRect(area, padding);
        // Border reduces width/height by 2; padding further adjusts.
        Assert.Equal(0 + 1 + 1, inner.X); // area.X + border + left padding
        Assert.Equal(0 + 1 + 1, inner.Y); // area.Y + border + top padding
        Assert.Equal(20 - 2 - (1+2), inner.Width); // width minus border (2) minus left+right
        Assert.Equal(6 - 2 - (1+0), inner.Height); // height minus border (2) minus top+bottom
    }

    [Fact]
    public void ComposeParagraphInsideBlockDoesNotOverwriteBorderWhenPreserve()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var block = Block.Create().WithMergeStrategy(MergeStrategy.Preserve).WithTitle("Title", StyleType.Empty);
        term.Draw(f => block.Render(f, new Rect(0,0,10,4)));
        var inner = Block.GetInnerContentRect(new Rect(0,0,10,4), Padding.Zero);
        term.Draw(f => f.WriteString(inner.X, inner.Y, "Nested", StyleType.Empty));
        // Ensure corner still present (top-left corner char retained)
        Assert.Contains(backend.Emitted, c => c.X == 0 && c.Y == 0 && c.Cell.Grapheme == block.Border.TopLeft);
    }
}
