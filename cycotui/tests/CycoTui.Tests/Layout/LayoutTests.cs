using System;
using System.Linq;
using CycoAI.CycoTui.Core.Layout;
using Xunit;

namespace CycoAI.CycoTui.Tests.Layout
{
    public class LayoutTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            var layout = new Core.Layout.Layout();

            Assert.Equal(Direction.Vertical, layout.Direction);
            Assert.Equal(Margin.None, layout.Margin);
            Assert.False(layout.Flex);
            Assert.Equal(Alignment.Default, layout.Alignment);
            Assert.True(layout.EnableCaching);
            Assert.Empty(layout.Constraints);
        }

        [Fact]
        public void Constructor_InitializesWithDirection()
        {
            var layout = new Core.Layout.Layout(Direction.Horizontal);

            Assert.Equal(Direction.Horizontal, layout.Direction);
        }

        [Fact]
        public void AddConstraint_AddsToList()
        {
            var layout = new Core.Layout.Layout();
            var constraint = Constraint.Length(10);

            var result = layout.AddConstraint(constraint);

            Assert.Same(layout, result);
            Assert.Single(layout.Constraints);
            Assert.Equal(constraint, layout.Constraints[0]);
        }

        [Fact]
        public void AddConstraints_AddsMultipleToList()
        {
            var layout = new Core.Layout.Layout();
            var constraints = new[] { Constraint.Length(10), Constraint.Percentage(50) };

            var result = layout.AddConstraints(constraints);

            Assert.Same(layout, result);
            Assert.Equal(2, layout.Constraints.Count);
            Assert.Equal(constraints[0], layout.Constraints[0]);
            Assert.Equal(constraints[1], layout.Constraints[1]);
        }

        [Fact]
        public void ClearConstraints_RemovesAll()
        {
            var layout = new Core.Layout.Layout()
                .AddConstraint(Constraint.Length(10))
                .AddConstraint(Constraint.Percentage(50));

            var result = layout.ClearConstraints();

            Assert.Same(layout, result);
            Assert.Empty(layout.Constraints);
        }

        [Fact]
        public void WithDirection_SetsDirection()
        {
            var layout = new Core.Layout.Layout();

            var result = layout.WithDirection(Direction.Horizontal);

            Assert.Same(layout, result);
            Assert.Equal(Direction.Horizontal, layout.Direction);
        }

        [Fact]
        public void WithMargin_SetsMargin()
        {
            var layout = new Core.Layout.Layout();
            var margin = new Margin(1, 2, 3, 4);

            var result = layout.WithMargin(margin);

            Assert.Same(layout, result);
            Assert.Equal(margin, layout.Margin);
        }

        [Fact]
        public void WithMargin_SetsUniformMargin()
        {
            var layout = new Core.Layout.Layout();

            var result = layout.WithMargin(5);

            Assert.Same(layout, result);
            Assert.Equal(new Margin(5), layout.Margin);
        }

        [Fact]
        public void WithFlex_SetsFlex()
        {
            var layout = new Core.Layout.Layout();

            var result = layout.WithFlex(true);

            Assert.Same(layout, result);
            Assert.True(layout.Flex);
        }

        [Fact]
        public void WithAlignment_SetsAlignment()
        {
            var layout = new Core.Layout.Layout();
            var alignment = Alignment.Center;

            var result = layout.WithAlignment(alignment);

            Assert.Same(layout, result);
            Assert.Equal(alignment, layout.Alignment);
        }

        [Fact]
        public void WithCaching_SetsCaching()
        {
            var layout = new Core.Layout.Layout();

            var result = layout.WithCaching(false);

            Assert.Same(layout, result);
            Assert.False(layout.EnableCaching);
        }

        [Fact]
        public void Split_NoConstraints_ReturnsOriginalArea()
        {
            var layout = new Core.Layout.Layout();
            var area = new Rect(0, 0, 100, 50);

            var result = layout.Split(area);

            Assert.Single(result);
            Assert.Equal(area, result[0]);
        }

        [Fact]
        public void Split_VerticalDirection_SplitsCorrectly()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .AddConstraint(Constraint.Length(20))
                .AddConstraint(Constraint.Length(30));

            var area = new Rect(0, 0, 100, 50);
            var result = layout.Split(area);

            Assert.Equal(2, result.Length);
            Assert.Equal(new Rect(0, 0, 100, 20), result[0]);
            Assert.Equal(new Rect(0, 20, 100, 30), result[1]);
        }

        [Fact]
        public void Split_HorizontalDirection_SplitsCorrectly()
        {
            var layout = new Core.Layout.Layout(Direction.Horizontal)
                .AddConstraint(Constraint.Length(30))
                .AddConstraint(Constraint.Length(40));

            var area = new Rect(0, 0, 100, 50);
            var result = layout.Split(area);

            Assert.Equal(2, result.Length);
            Assert.Equal(new Rect(0, 0, 30, 50), result[0]);
            Assert.Equal(new Rect(30, 0, 40, 50), result[1]);
        }

        [Fact]
        public void Split_WithMargin_AppliesMargin()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .WithMargin(new Margin(5, 10, 5, 10))
                .AddConstraint(Constraint.Length(20));

            var area = new Rect(0, 0, 100, 50);
            var result = layout.Split(area);

            // Area should be reduced by margin: 100-10=90 width, 50-20=30 height
            // Position should be offset by margin: x+5, y+10
            Assert.Single(result);
            Assert.Equal(new Rect(5, 10, 90, 20), result[0]);
        }

        [Fact]
        public void Split_WithPercentageConstraints_CalculatesCorrectly()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .AddConstraint(Constraint.Percentage(25))
                .AddConstraint(Constraint.Percentage(75));

            var area = new Rect(0, 0, 100, 100);
            var result = layout.Split(area);

            Assert.Equal(2, result.Length);
            Assert.Equal(new Rect(0, 0, 100, 25), result[0]);
            Assert.Equal(new Rect(0, 25, 100, 75), result[1]);
        }

        [Fact]
        public void Split_WithRatioConstraints_CalculatesCorrectly()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .AddConstraint(Constraint.Ratio(1))
                .AddConstraint(Constraint.Ratio(2))
                .AddConstraint(Constraint.Ratio(1));

            var area = new Rect(0, 0, 100, 100);
            var result = layout.Split(area);

            Assert.Equal(3, result.Length);
            Assert.Equal(new Rect(0, 0, 100, 25), result[0]);  // 1/4 of 100
            Assert.Equal(new Rect(0, 25, 100, 50), result[1]); // 2/4 of 100
            Assert.Equal(new Rect(0, 75, 100, 25), result[2]); // 1/4 of 100
        }

        [Fact]
        public void Split_WithMixedConstraints_CalculatesCorrectly()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .AddConstraint(Constraint.Length(20))
                .AddConstraint(Constraint.Percentage(50))
                .AddConstraint(Constraint.Ratio(1));

            var area = new Rect(0, 0, 100, 100);
            var result = layout.Split(area);

            Assert.Equal(3, result.Length);
            Assert.Equal(new Rect(0, 0, 100, 20), result[0]);  // Fixed 20
            Assert.Equal(new Rect(0, 20, 100, 50), result[1]); // 50% of 100
            Assert.Equal(new Rect(0, 70, 100, 30), result[2]); // Remaining space (100-20-50=30)
        }

        [Fact]
        public void Split_WithFlex_DistributesExtraSpace()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .WithFlex(true)
                .AddConstraint(Constraint.Length(20))
                .AddConstraint(Constraint.Length(30));

            var area = new Rect(0, 0, 100, 100);
            var result = layout.Split(area);

            // Total fixed space: 50, remaining: 50, distributed equally: 25 each
            Assert.Equal(2, result.Length);
            Assert.Equal(new Rect(0, 0, 100, 45), result[0]);  // 20 + 25
            Assert.Equal(new Rect(0, 45, 100, 55), result[1]); // 30 + 25
        }

        [Fact]
        public void Split_EmptyArea_ReturnsEmptyRectangles()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .AddConstraint(Constraint.Length(20))
                .AddConstraint(Constraint.Length(30));

            var area = new Rect(0, 0, 0, 0);
            var result = layout.Split(area);

            Assert.Equal(2, result.Length);
            Assert.True(result.All(r => r.IsEmpty));
        }

        [Fact]
        public void Split_WithCaching_UsesCache()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .WithCaching(true)
                .AddConstraint(Constraint.Length(50));

            var area = new Rect(0, 0, 100, 100);

            // Clear cache first
            Core.Layout.Layout.ClearCache();

            // First call should cache the result
            var result1 = layout.Split(area);
            var (count1, _) = Core.Layout.Layout.GetCacheStatistics();

            // Second call should use cached result
            var result2 = layout.Split(area);
            var (count2, _) = Core.Layout.Layout.GetCacheStatistics();

            Assert.Equal(result1, result2);
            Assert.True(count1 > 0);
            Assert.Equal(count1, count2); // Cache size should not increase
        }

        [Fact]
        public void Split_WithoutCaching_DoesNotUseCache()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .WithCaching(false)
                .AddConstraint(Constraint.Length(50));

            var area = new Rect(0, 0, 100, 100);

            // Clear cache first
            Core.Layout.Layout.ClearCache();

            // Calls should not affect cache
            layout.Split(area);
            layout.Split(area);

            var (count, _) = Core.Layout.Layout.GetCacheStatistics();
            Assert.Equal(0, count);
        }

        [Fact]
        public void Horizontal_StaticMethod_CreatesHorizontalLayout()
        {
            var constraints = new[] { Constraint.Length(10), Constraint.Percentage(50) };
            var layout = Core.Layout.Layout.Horizontal(constraints);

            Assert.Equal(Direction.Horizontal, layout.Direction);
            Assert.Equal(2, layout.Constraints.Count);
            Assert.Equal(constraints[0], layout.Constraints[0]);
            Assert.Equal(constraints[1], layout.Constraints[1]);
        }

        [Fact]
        public void Vertical_StaticMethod_CreatesVerticalLayout()
        {
            var constraints = new[] { Constraint.Length(10), Constraint.Percentage(50) };
            var layout = Core.Layout.Layout.Vertical(constraints);

            Assert.Equal(Direction.Vertical, layout.Direction);
            Assert.Equal(2, layout.Constraints.Count);
            Assert.Equal(constraints[0], layout.Constraints[0]);
            Assert.Equal(constraints[1], layout.Constraints[1]);
        }

        [Fact]
        public void ClearCache_ClearsGlobalCache()
        {
            var layout = new Core.Layout.Layout(Direction.Vertical)
                .AddConstraint(Constraint.Length(50));

            var area = new Rect(0, 0, 100, 100);

            // Generate some cache entries
            layout.Split(area);
            var (count1, _) = Core.Layout.Layout.GetCacheStatistics();

            // Clear cache
            Core.Layout.Layout.ClearCache();
            var (count2, _) = Core.Layout.Layout.GetCacheStatistics();

            Assert.True(count1 > 0);
            Assert.Equal(0, count2);
        }
    }
}