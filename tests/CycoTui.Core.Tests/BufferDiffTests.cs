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
        var prev = BufferType.Empty(new Size(4,1));
        var cur = BufferType.Empty(new Size(4,1));
        cur.SetCell(2,0, new Cell("X", StyleType.Empty));
        var changes = BufferDiff.EnumerateCellDiff(prev, cur).ToList();
        Assert.Single(changes);
        Assert.Equal(2, changes[0].X);
        Assert.Equal(0, changes[0].Y);
    }

    [Fact]
    public void SegmentDiffGroupsContiguousCells()
    {
        var prev = BufferType.Empty(new Size(6,1));
        var cur = BufferType.Empty(new Size(6,1));
        cur.SetString(1,0,"ABC", StyleType.Empty); // positions 1,2,3
        var segments = BufferDiff.EnumerateSegments(prev, cur).ToList();
        Assert.Single(segments);
        Assert.Equal(1, segments[0].StartX);
        Assert.Equal(3, segments[0].Length);
    }

    [Fact]
    public void SegmentDiffCreatesMultipleSegmentsForGaps()
    {
        var prev = BufferType.Empty(new Size(7,1));
        var cur = BufferType.Empty(new Size(7,1));
        cur.SetCell(0,0,new Cell("A", StyleType.Empty));
        cur.SetCell(2,0,new Cell("B", StyleType.Empty));
        cur.SetCell(3,0,new Cell("C", StyleType.Empty));
        cur.SetCell(5,0,new Cell("D", StyleType.Empty)); // isolated beyond grouped (2,3)
        var segments = BufferDiff.EnumerateSegments(prev, cur).ToList();
        Assert.True(segments.Count >= 3); // allow grouping variations
        Assert.Equal(0, segments[0].StartX);
        Assert.Equal(2, segments[1].StartX);
        Assert.Contains(segments, s => s.StartX == 0);
        Assert.Contains(segments, s => s.StartX == 2);
        Assert.Contains(segments, s => s.StartX == 5);
    }
}
