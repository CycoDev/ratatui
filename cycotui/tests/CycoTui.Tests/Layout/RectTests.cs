using CycoAI.CycoTui.Core.Layout;
using Xunit;

namespace CycoAI.CycoTui.Tests.Layout
{
    public class RectTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var rect = new Rect(10, 20, 30, 40);

            Assert.Equal(10, rect.X);
            Assert.Equal(20, rect.Y);
            Assert.Equal(30, rect.Width);
            Assert.Equal(40, rect.Height);
        }

        [Fact]
        public void Constructor_NegativeDimensions_ClampsToZero()
        {
            var rect = new Rect(10, 20, -5, -10);

            Assert.Equal(10, rect.X);
            Assert.Equal(20, rect.Y);
            Assert.Equal(0, rect.Width);
            Assert.Equal(0, rect.Height);
        }

        [Fact]
        public void Right_CalculatesCorrectly()
        {
            var rect = new Rect(10, 20, 30, 40);
            Assert.Equal(40, rect.Right); // 10 + 30
        }

        [Fact]
        public void Bottom_CalculatesCorrectly()
        {
            var rect = new Rect(10, 20, 30, 40);
            Assert.Equal(60, rect.Bottom); // 20 + 40
        }

        [Fact]
        public void Area_CalculatesCorrectly()
        {
            var rect = new Rect(10, 20, 30, 40);
            Assert.Equal(1200, rect.Area); // 30 * 40
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(10, 0, true)]
        [InlineData(0, 10, true)]
        [InlineData(10, 10, false)]
        public void IsEmpty_ReturnsCorrectValue(int width, int height, bool expected)
        {
            var rect = new Rect(0, 0, width, height);
            Assert.Equal(expected, rect.IsEmpty);
        }

        [Fact]
        public void FromCoords_CreatesRect()
        {
            var rect = Rect.FromCoords(5, 10, 15, 20);

            Assert.Equal(5, rect.X);
            Assert.Equal(10, rect.Y);
            Assert.Equal(15, rect.Width);
            Assert.Equal(20, rect.Height);
        }

        [Fact]
        public void FromSize_CreatesRectAtOrigin()
        {
            var rect = Rect.FromSize(25, 35);

            Assert.Equal(0, rect.X);
            Assert.Equal(0, rect.Y);
            Assert.Equal(25, rect.Width);
            Assert.Equal(35, rect.Height);
        }

        [Theory]
        [InlineData(15, 25, true)]  // Inside
        [InlineData(10, 20, true)]  // On edge (inclusive)
        [InlineData(39, 59, true)]  // On opposite edge (exclusive)
        [InlineData(40, 60, false)] // Outside (exclusive)
        [InlineData(5, 15, false)]  // Outside left/top
        public void Contains_Point_ReturnsCorrectValue(int x, int y, bool expected)
        {
            var rect = new Rect(10, 20, 30, 40); // Right=40, Bottom=60
            Assert.Equal(expected, rect.Contains(x, y));
        }

        [Fact]
        public void Contains_Rect_CompletelyInside_ReturnsTrue()
        {
            var container = new Rect(10, 20, 50, 60);
            var inner = new Rect(15, 25, 20, 30);

            Assert.True(container.Contains(inner));
        }

        [Fact]
        public void Contains_Rect_PartiallyOutside_ReturnsFalse()
        {
            var container = new Rect(10, 20, 50, 60);
            var partial = new Rect(5, 25, 20, 30);

            Assert.False(container.Contains(partial));
        }

        [Fact]
        public void Contains_Rect_CompletelyOutside_ReturnsFalse()
        {
            var container = new Rect(10, 20, 50, 60);
            var outside = new Rect(70, 90, 20, 30);

            Assert.False(container.Contains(outside));
        }

        [Theory]
        [InlineData(15, 25, 10, 10, true)]   // Overlapping
        [InlineData(20, 30, 10, 10, false)] // Adjacent (no overlap)
        [InlineData(25, 35, 10, 10, false)] // Completely separate
        public void Intersects_ReturnsCorrectValue(int x, int y, int width, int height, bool expected)
        {
            var rect1 = new Rect(10, 20, 10, 10); // 10-20, 20-30
            var rect2 = new Rect(x, y, width, height);

            Assert.Equal(expected, rect1.Intersects(rect2));
        }

        [Fact]
        public void Intersect_OverlappingRects_ReturnsIntersection()
        {
            var rect1 = new Rect(10, 20, 30, 40); // 10-40, 20-60
            var rect2 = new Rect(25, 35, 30, 40); // 25-55, 35-75

            var intersection = rect1.Intersect(rect2);

            // Intersection should be 25-40, 35-60
            Assert.Equal(new Rect(25, 35, 15, 25), intersection);
        }

        [Fact]
        public void Intersect_NonOverlappingRects_ReturnsEmptyRect()
        {
            var rect1 = new Rect(10, 20, 10, 10);
            var rect2 = new Rect(30, 40, 10, 10);

            var intersection = rect1.Intersect(rect2);

            Assert.True(intersection.IsEmpty);
        }

        [Fact]
        public void WithMargin_UniformMargin_AppliesCorrectly()
        {
            var rect = new Rect(10, 20, 50, 60);
            var result = rect.WithMargin(5);

            // Should shrink by 5 on each side and offset position
            Assert.Equal(new Rect(15, 25, 40, 50), result);
        }

        [Fact]
        public void WithMargin_IndividualSides_AppliesCorrectly()
        {
            var rect = new Rect(10, 20, 50, 60);
            var result = rect.WithMargin(1, 2, 3, 4); // left, top, right, bottom

            // X: 10 + 1 = 11, Y: 20 + 2 = 22
            // Width: 50 - 1 - 3 = 46, Height: 60 - 2 - 4 = 54
            Assert.Equal(new Rect(11, 22, 46, 54), result);
        }

        [Fact]
        public void WithMargin_MarginStruct_AppliesCorrectly()
        {
            var rect = new Rect(10, 20, 50, 60);
            var margin = new Margin(2, 3, 4, 5);
            var result = rect.WithMargin(margin);

            // X: 10 + 2 = 12, Y: 20 + 3 = 23
            // Width: 50 - 2 - 4 = 44, Height: 60 - 3 - 5 = 52
            Assert.Equal(new Rect(12, 23, 44, 52), result);
        }

        [Fact]
        public void WithMargin_ExcessiveMargin_ClampsToZero()
        {
            var rect = new Rect(10, 20, 20, 30);
            var result = rect.WithMargin(15); // Margin larger than half the dimensions

            // Width: 20 - 15 - 15 = -10 -> 0
            // Height: 30 - 15 - 15 = 0
            Assert.Equal(new Rect(25, 35, 0, 0), result);
        }

        [Fact]
        public void Equals_SameRects_ReturnsTrue()
        {
            var rect1 = new Rect(10, 20, 30, 40);
            var rect2 = new Rect(10, 20, 30, 40);

            Assert.True(rect1.Equals(rect2));
            Assert.True(rect1 == rect2);
            Assert.False(rect1 != rect2);
        }

        [Fact]
        public void Equals_DifferentRects_ReturnsFalse()
        {
            var rect1 = new Rect(10, 20, 30, 40);
            var rect2 = new Rect(15, 25, 35, 45);

            Assert.False(rect1.Equals(rect2));
            Assert.False(rect1 == rect2);
            Assert.True(rect1 != rect2);
        }

        [Fact]
        public void GetHashCode_SameRects_ReturnsSameHash()
        {
            var rect1 = new Rect(10, 20, 30, 40);
            var rect2 = new Rect(10, 20, 30, 40);

            Assert.Equal(rect1.GetHashCode(), rect2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            var rect = new Rect(10, 20, 30, 40);
            var result = rect.ToString();

            Assert.Equal("Rect(x=10, y=20, width=30, height=40)", result);
        }

        [Fact]
        public void Equals_WithObject_WorksCorrectly()
        {
            var rect = new Rect(10, 20, 30, 40);
            object obj = new Rect(10, 20, 30, 40);
            object differentObj = new Rect(15, 25, 35, 45);
            object? nullObj = null;
            object wrongType = "string";

            Assert.True(rect.Equals(obj));
            Assert.False(rect.Equals(differentObj));
            Assert.False(rect.Equals(nullObj));
            Assert.False(rect.Equals(wrongType));
        }
    }
}