using System.Linq;
using CycoTui.Core.Layout;
using CycoTui.Core.Logging;
using Xunit;

namespace CycoTui.Core.Tests;

public class LayoutEngineTests
{
    private LayoutEngine Create() => new(new LoggingContext(null));

    [Fact]
    public void LengthConstraintsExactFit()
    {
        var engine = Create();
        var area = new Rect(0,0,30,5);
        var constraints = new[]{ Constraint.Length(10), Constraint.Length(10), Constraint.Length(10)};
        var rects = engine.Distribute(area, constraints, LayoutDirection.Horizontal);
        Assert.Equal(3, rects.Count);
        Assert.All(rects, r => Assert.Equal(10, r.Width));
    }

    [Fact]
    public void MixedPercentageAndFill()
    {
        var engine = Create();
        var area = new Rect(0,0,100,5);
        var constraints = new[]{ Constraint.Percentage(50), Constraint.Fill(), Constraint.Fill()};
        var rects = engine.Distribute(area, constraints, LayoutDirection.Horizontal);
        Assert.Equal(3, rects.Count);
        Assert.Equal(50, rects[0].Width);
        Assert.Equal(25, rects[1].Width);
        Assert.Equal(25, rects[2].Width);
    }

    [Fact]
    public void RatioDistribution()
    {
        var engine = Create();
        var area = new Rect(0,0,60,5);
        var constraints = new[]{ Constraint.Ratio(1,1), Constraint.Ratio(2,1), Constraint.Ratio(3,1)}; // weights 1,2,3
        var rects = engine.Distribute(area, constraints, LayoutDirection.Horizontal);
        Assert.True(rects[0].Width + rects[1].Width + rects[2].Width <= 60); // sanity
        Assert.True(rects[0].Width < rects[1].Width && rects[1].Width < rects[2].Width);
    }

    [Fact]
    public void AlignmentCenter()
    {
        var engine = Create();
        var area = new Rect(0,0,20,5);
        var constraints = new[]{ Constraint.Length(5), Constraint.Length(5)}; // use 10 of 20
        var rects = engine.Distribute(area, constraints, LayoutDirection.Horizontal, alignment: AlignmentMode.Center);
        Assert.Equal(2, rects.Count);
        Assert.Equal(5, rects[0].Width);
        Assert.True(rects[0].X > 0); // centered
    }

    [Fact]
    public void MarginAndPaddingApplied()
    {
        var engine = Create();
        var area = new Rect(0,0,50,10);
        var margin = new Margin(2,1,2,1);
        var padding = new Padding(1,1,1,1);
        var constraints = new[]{ Constraint.Fill(), Constraint.Fill()};
        var rects = engine.Distribute(area, constraints, LayoutDirection.Horizontal, margin: margin, padding: padding);
        // Effective width: 50 - (2+2) margin - (1+1) padding = 44
        Assert.Equal(22, rects[0].Width);
        Assert.Equal(22, rects[1].Width);
        Assert.Equal(1+2, rects[0].X); // margin left + padding left
    }
}
