using System.Linq;
using Xunit;
using CycoTui.Core.Buffer;
using CycoTui.Core.Style;
using CycoTui.Core.Text;
using CycoTui.Core.Backend;

namespace CycoTui.Core.Tests;

public class MultiWidthReplacementTests
{
    [Fact]
    public void ReplacingWideGraphemeWithAnotherWideGraphemeDoesNotLeaveArtifacts()
    {
        WidthService.SetMode(WidthMode.Standard);
        var bufPrev = BufferType.Empty(new Size(6,1));
        var bufCur = BufferType.Empty(new Size(6,1));
        bufPrev.SetString(0,0,"😀", StyleType.Empty);
        bufCur.SetString(0,0,"😁", StyleType.Empty);
        var segments = BufferDiff.EnumerateSegments(bufPrev, bufCur);
        // Should only need to emit one segment (the head cell) because skip cells are placeholders
        Assert.Single(segments);
        Assert.Equal(0, segments.First().StartX);
    }
}
