using CycoAI.CycoTui.Core.Layout;
using Xunit;

namespace CycoAI.CycoTui.Tests.Layout
{
    public class AlignmentTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var alignment = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);

            Assert.Equal(HorizontalAlignment.Center, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Bottom, alignment.Vertical);
        }

        [Fact]
        public void Default_ReturnsTopLeft()
        {
            var alignment = Alignment.Default;

            Assert.Equal(HorizontalAlignment.Left, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Top, alignment.Vertical);
        }

        [Fact]
        public void Center_ReturnsCenterCenter()
        {
            var alignment = Alignment.Center;

            Assert.Equal(HorizontalAlignment.Center, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Center, alignment.Vertical);
        }

        [Fact]
        public void TopLeft_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.TopLeft;

            Assert.Equal(HorizontalAlignment.Left, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Top, alignment.Vertical);
        }

        [Fact]
        public void TopCenter_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.TopCenter;

            Assert.Equal(HorizontalAlignment.Center, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Top, alignment.Vertical);
        }

        [Fact]
        public void TopRight_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.TopRight;

            Assert.Equal(HorizontalAlignment.Right, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Top, alignment.Vertical);
        }

        [Fact]
        public void CenterLeft_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.CenterLeft;

            Assert.Equal(HorizontalAlignment.Left, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Center, alignment.Vertical);
        }

        [Fact]
        public void CenterRight_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.CenterRight;

            Assert.Equal(HorizontalAlignment.Right, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Center, alignment.Vertical);
        }

        [Fact]
        public void BottomLeft_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.BottomLeft;

            Assert.Equal(HorizontalAlignment.Left, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Bottom, alignment.Vertical);
        }

        [Fact]
        public void BottomCenter_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.BottomCenter;

            Assert.Equal(HorizontalAlignment.Center, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Bottom, alignment.Vertical);
        }

        [Fact]
        public void BottomRight_ReturnsCorrectAlignment()
        {
            var alignment = Alignment.BottomRight;

            Assert.Equal(HorizontalAlignment.Right, alignment.Horizontal);
            Assert.Equal(VerticalAlignment.Bottom, alignment.Vertical);
        }

        [Fact]
        public void WithHorizontal_UpdatesHorizontal()
        {
            var alignment = Alignment.Default;
            var updated = alignment.WithHorizontal(HorizontalAlignment.Right);

            Assert.Equal(HorizontalAlignment.Right, updated.Horizontal);
            Assert.Equal(VerticalAlignment.Top, updated.Vertical);
        }

        [Fact]
        public void WithVertical_UpdatesVertical()
        {
            var alignment = Alignment.Default;
            var updated = alignment.WithVertical(VerticalAlignment.Bottom);

            Assert.Equal(HorizontalAlignment.Left, updated.Horizontal);
            Assert.Equal(VerticalAlignment.Bottom, updated.Vertical);
        }

        [Theory]
        [InlineData(HorizontalAlignment.Left, 0, 0, 20, 10)]
        [InlineData(HorizontalAlignment.Center, 40, 0, 20, 10)]
        [InlineData(HorizontalAlignment.Right, 80, 0, 20, 10)]
        public void ApplyAlignment_HorizontalAlignment_PositionsCorrectly(
            HorizontalAlignment horizontal, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
        {
            var alignment = new Alignment(horizontal, VerticalAlignment.Top);
            var container = new Rect(0, 0, 100, 50);
            var contentSize = (20, 10);

            var result = alignment.ApplyAlignment(container, contentSize);

            Assert.Equal(new Rect(expectedX, expectedY, expectedWidth, expectedHeight), result);
        }

        [Theory]
        [InlineData(VerticalAlignment.Top, 0, 0, 20, 10)]
        [InlineData(VerticalAlignment.Center, 0, 20, 20, 10)]
        [InlineData(VerticalAlignment.Bottom, 0, 40, 20, 10)]
        public void ApplyAlignment_VerticalAlignment_PositionsCorrectly(
            VerticalAlignment vertical, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
        {
            var alignment = new Alignment(HorizontalAlignment.Left, vertical);
            var container = new Rect(0, 0, 100, 50);
            var contentSize = (20, 10);

            var result = alignment.ApplyAlignment(container, contentSize);

            Assert.Equal(new Rect(expectedX, expectedY, expectedWidth, expectedHeight), result);
        }

        [Fact]
        public void ApplyAlignment_CenterCenter_CentersCorrectly()
        {
            var alignment = Alignment.Center;
            var container = new Rect(10, 20, 100, 60);
            var contentSize = (20, 10);

            var result = alignment.ApplyAlignment(container, contentSize);

            // Center X: 10 + (100 - 20) / 2 = 10 + 40 = 50
            // Center Y: 20 + (60 - 10) / 2 = 20 + 25 = 45
            Assert.Equal(new Rect(50, 45, 20, 10), result);
        }

        [Fact]
        public void ApplyAlignment_ContentLargerThanContainer_ClampsToContainer()
        {
            var alignment = Alignment.Center;
            var container = new Rect(0, 0, 50, 30);
            var contentSize = (100, 60); // Larger than container

            var result = alignment.ApplyAlignment(container, contentSize);

            // Content should be clamped to container size
            Assert.Equal(new Rect(0, 0, 50, 30), result);
        }

        [Fact]
        public void ApplyAlignment_WithRect_UsesRectSize()
        {
            var alignment = Alignment.Center;
            var container = new Rect(0, 0, 100, 50);
            var content = new Rect(10, 20, 20, 10); // Position should be ignored, only size used

            var result = alignment.ApplyAlignment(container, content);

            Assert.Equal(new Rect(40, 20, 20, 10), result);
        }

        [Fact]
        public void Equals_SameAlignments_ReturnsTrue()
        {
            var alignment1 = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            var alignment2 = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);

            Assert.True(alignment1.Equals(alignment2));
            Assert.True(alignment1 == alignment2);
            Assert.False(alignment1 != alignment2);
        }

        [Fact]
        public void Equals_DifferentAlignments_ReturnsFalse()
        {
            var alignment1 = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            var alignment2 = new Alignment(HorizontalAlignment.Left, VerticalAlignment.Top);

            Assert.False(alignment1.Equals(alignment2));
            Assert.False(alignment1 == alignment2);
            Assert.True(alignment1 != alignment2);
        }

        [Fact]
        public void GetHashCode_SameAlignments_ReturnsSameHash()
        {
            var alignment1 = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            var alignment2 = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);

            Assert.Equal(alignment1.GetHashCode(), alignment2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            var alignment = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            var result = alignment.ToString();

            Assert.Equal("Alignment(Center, Bottom)", result);
        }

        [Fact]
        public void Equals_WithObject_WorksCorrectly()
        {
            var alignment = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            object obj = new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            object differentObj = new Alignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            object? nullObj = null;
            object wrongType = "string";

            Assert.True(alignment.Equals(obj));
            Assert.False(alignment.Equals(differentObj));
            Assert.False(alignment.Equals(nullObj));
            Assert.False(alignment.Equals(wrongType));
        }

        [Fact]
        public void ApplyAlignment_WithNonZeroContainerPosition_OffsetsCorrectly()
        {
            var alignment = Alignment.BottomRight;
            var container = new Rect(50, 100, 80, 60);
            var contentSize = (20, 15);

            var result = alignment.ApplyAlignment(container, contentSize);

            // Bottom-right position:
            // X: 50 + 80 - 20 = 110
            // Y: 100 + 60 - 15 = 145
            Assert.Equal(new Rect(110, 145, 20, 15), result);
        }
    }
}