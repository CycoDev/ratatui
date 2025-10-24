using System.Linq;
using CycoTui.Core.Buffer;
using CycoTui.Core.Style;
using CycoTui.Core.Backend;
using CycoTui.Core.Text;
using Xunit;

namespace CycoTui.Core.Tests;

public class MultiWidthDiffTests
{
    [Fact]
    public void ChangingEmojiUpdatesPrimaryAndContinuation()
    {
        WidthService.SetMode(WidthMode.Standard);
        var prev = Buffer.Empty(new Size(6,1));
        var cur = Buffer.Empty(new Size(6,1));
        prev.SetString(0,0,"🙂A", Style.Empty);
        cur.SetString(0,0,"😀A", Style.Empty); // different emoji
        var cells = BufferDiff.EnumerateCellDiff(prev, cur).ToList();
        // Expect at least one change for primary cell; continuation also differs because grapheme differs.
        Assert.True(cells.Any(c => c.X == 0));
        Assert.True(cells.Any(c => c.X == 1)); // continuation cell changed
    }
}
