using System.Linq;
using CycoTui.Core.Layout;
using CycoTui.Core.Logging;
using Xunit;

namespace CycoTui.Core.Tests;

public class LayoutEngineRefinementTests
{
    private LayoutEngine Engine() => new(new LoggingContext(null));

    [Fact]
    public void RatioPreciseDistributionWithLeftover()
    {
        var area = new Rect(0,0,23,5); // odd size
        var constraints = new[]{ Constraint.Ratio(1,1), Constraint.Ratio(2,1), Constraint.Ratio(3,1)}; // weights 1,2,3 => sum weight=6
        var rects = Engine().Distribute(area, constraints, LayoutDirection.Horizontal);
        int total = rects.Sum(r => r.Width);
        Assert.Equal(23, total);
        // Expect monotonic increasing widths
        Assert.True(rects[0].Width < rects[1].Width && rects[1].Width < rects[2].Width);
    }

    [Fact]
    public void OverflowShrinkMinThenLength()
    {
        var area = new Rect(0,0,10,5);
        // Mandatory: Length(8) + Min(6) = 14 > 10, should shrink Min first then maybe Length.
        var constraints = new[]{ Constraint.Length(8), Constraint.Min(6)};
        var rects = Engine().Distribute(area, constraints, LayoutDirection.Horizontal);
        int total = rects.Sum(r => r.Width);
        Assert.Equal(10, total);
        // Ensure length did not vanish entirely
        Assert.True(rects[0].Width >= 5); // heuristic check
    }

    [Fact]
    public void VerticalCenterAlignment()
    {
        var area = new Rect(0,0,10,30);
        var constraints = new[]{ Constraint.Length(5), Constraint.Length(5)};
        var rects = Engine().Distribute(area, constraints, LayoutDirection.Vertical, alignment: AlignmentMode.Center);
        Assert.Equal(2, rects.Count);
        // Centered: top of first rect should be > 0
        Assert.True(rects[0].Y > 0);
    }

    [Fact]
    public void SpaceEvenlyHorizontalDistribution()
    {
        var area = new Rect(0,0,40,5);
        var constraints = new[]{ Constraint.Length(5), Constraint.Length(5), Constraint.Length(5)}; // use 15 of 40
        var rects = Engine().Distribute(area, constraints, LayoutDirection.Horizontal, alignment: AlignmentMode.SpaceEvenly);
        // Expect gaps before, between, after roughly even
        Assert.Equal(3, rects.Count);
        Assert.True(rects[0].X > 0);
        Assert.True(rects[1].X - (rects[0].X + rects[0].Width) > 0);
        Assert.True(rects[2].X - (rects[1].X + rects[1].Width) > 0);
    }

    [Fact]
    public void SpaceAroundVerticalDistribution()
    {
        var area = new Rect(0,0,10,50);
        var constraints = new[]{ Constraint.Length(10), Constraint.Length(10)}; // use 20 of 50
        var rects = Engine().Distribute(area, constraints, LayoutDirection.Vertical, alignment: AlignmentMode.SpaceAround);
        Assert.Equal(2, rects.Count);
        Assert.True(rects[0].Y > 0);
    }
}
