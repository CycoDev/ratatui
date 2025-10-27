using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Layout;
using System.Linq;

namespace CycoTui.Core.Tests;

public class ParagraphWrapAndScrollTests
{
    [Fact]
    public void HorizontalOffsetAppliesToWrappedLines()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var text = "ABCDEFGHIJABCDEFGHIJ"; // 20 chars
        var paragraph = Paragraph.Create().WithText(text).WithWrap(true).WithHorizontalOffset(3);
        term.Draw(f => paragraph.Render(f, new Rect(0,0,5,3)));
        var line0 = backend.Emitted.Where(c => c.Y==0).OrderBy(c=>c.X).Select(c=>c.Cell.Grapheme).Aggregate("",(a,b)=>a+b);
        Assert.Equal("DEFGH", line0); // offset skipped ABC
    }
}
