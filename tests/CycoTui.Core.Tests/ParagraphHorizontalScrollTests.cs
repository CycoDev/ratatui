using System.Linq;
using Xunit;
using CycoTui.Core.Widgets;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Layout;
using CycoTui.Core.Style;

namespace CycoTui.Core.Tests;

public class ParagraphHorizontalScrollTests
{
    [Fact]
    public void HorizontalOffsetSkipsLeadingGraphemes()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        var paragraph = Paragraph.Create().WithText("HELLOWORLD").WithWrap(false).WithHorizontalOffset(5);
        term.Draw(f => paragraph.Render(f, new Rect(0,0,5,1)));
        // Expect visible slice starting from column 5 => WORLD
        var emitted = backend.Emitted.Where(c => c.Y == 0).OrderBy(c => c.X).Select(c => c.Cell.Grapheme).Aggregate(string.Empty,(a,b)=>a+b);
        Assert.Equal("WORLD", emitted);
    }
}
