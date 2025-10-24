using CycoTui.Core.Buffer;
using CycoTui.Core.Style;
using CycoTui.Core.Backend;
using Xunit;
using System.Linq;

namespace CycoTui.Core.Tests;

public class BufferDiffTests
{
    [Fact]
    public void CellDiffDetectsSingleChange()
    {
        var prev = Buffer.Empty(new Size(4,1));
        var cur = Buffer.Empty(new Size(4,1));
        cur.SetCell(2,0, new Cell("X", Style.Empty));
        var changes = BufferDiff.EnumerateCellDiff(prev, cur).ToList();
        Assert.Single(changes);
        Assert.Equal(2, changes[0].X);
        Assert.Equal(0, changes[0].Y);
    }

    [Fact]
    public void SegmentDiffGroupsContiguousCells()
    {
        var prev = Buffer.Empty(new Size(6,1));
        var cur = Buffer.Empty(new Size(6,1));
        cur.SetString(1,0,"ABC", Style.Empty); // positions 1,2,3
        var segments = BufferDiff.EnumerateSegments(prev, cur).ToList();
        Assert.Single(segments);
        Assert.Equal(1, segments[0].StartX);
        Assert.Equal(3, segments[0].Length);
    }

    [Fact]
    public void SegmentDiffCreatesMultipleSegmentsForGaps()
    {
        var prev = Buffer.Empty(new Size(7,1));
        var cur = Buffer.Empty(new Size(7,1));
        cur.SetCell(0,0,new Cell("A", Style.Empty));
        cur.SetCell(2,0,new Cell("B", Style.Empty));
        cur.SetCell(3,0,new Cell("C", Style.Empty));
        cur.SetCell(5,0,new Cell("D", Style.Empty));
        var segments = BufferDiff.EnumerateSegments(prev, cur).ToList();
        Assert.Equal(4, segments.Count); // each isolated or contiguous run
        Assert.Equal(0, segments[0].StartX);
        Assert.Equal(2, segments[1].StartX);
        Assert.Equal(3, segments[2].StartX);
        Assert.Equal(5, segments[3].StartX);
    }
}
