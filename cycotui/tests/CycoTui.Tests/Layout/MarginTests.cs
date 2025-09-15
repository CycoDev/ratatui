using System;
using CycoAI.CycoTui.Core.Layout;
using Xunit;

namespace CycoAI.CycoTui.Tests.Layout
{
    public class MarginTests
    {
        [Fact]
        public void Constructor_UniformMargin_SetsAllSides()
        {
            var margin = new Margin(5);

            Assert.Equal(5, margin.Left);
            Assert.Equal(5, margin.Top);
            Assert.Equal(5, margin.Right);
            Assert.Equal(5, margin.Bottom);
            Assert.Equal(10, margin.Horizontal);
            Assert.Equal(10, margin.Vertical);
        }

        [Fact]
        public void Constructor_UniformMargin_NegativeValue_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Margin(-1));
        }

        [Fact]
        public void Constructor_HorizontalVertical_SetsCorrectly()
        {
            var margin = new Margin(3, 4);

            Assert.Equal(3, margin.Left);
            Assert.Equal(4, margin.Top);
            Assert.Equal(3, margin.Right);
            Assert.Equal(4, margin.Bottom);
            Assert.Equal(6, margin.Horizontal);
            Assert.Equal(8, margin.Vertical);
        }

        [Fact]
        public void Constructor_HorizontalVertical_NegativeValues_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Margin(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Margin(0, -1));
        }

        [Fact]
        public void Constructor_IndividualSides_SetsCorrectly()
        {
            var margin = new Margin(1, 2, 3, 4);

            Assert.Equal(1, margin.Left);
            Assert.Equal(2, margin.Top);
            Assert.Equal(3, margin.Right);
            Assert.Equal(4, margin.Bottom);
            Assert.Equal(4, margin.Horizontal);
            Assert.Equal(6, margin.Vertical);
        }

        [Fact]
        public void Constructor_IndividualSides_NegativeValues_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Margin(-1, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Margin(0, -1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Margin(0, 0, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Margin(0, 0, 0, -1));
        }

        [Fact]
        public void None_ReturnsZeroMargin()
        {
            var margin = Margin.None;

            Assert.Equal(0, margin.Left);
            Assert.Equal(0, margin.Top);
            Assert.Equal(0, margin.Right);
            Assert.Equal(0, margin.Bottom);
        }

        [Fact]
        public void WithLeft_CreatesNewMarginWithLeft()
        {
            var margin = Margin.WithLeft(5);

            Assert.Equal(5, margin.Left);
            Assert.Equal(0, margin.Top);
            Assert.Equal(0, margin.Right);
            Assert.Equal(0, margin.Bottom);
        }

        [Fact]
        public void WithTop_CreatesNewMarginWithTop()
        {
            var margin = Margin.WithTop(5);

            Assert.Equal(0, margin.Left);
            Assert.Equal(5, margin.Top);
            Assert.Equal(0, margin.Right);
            Assert.Equal(0, margin.Bottom);
        }

        [Fact]
        public void WithRight_CreatesNewMarginWithRight()
        {
            var margin = Margin.WithRight(5);

            Assert.Equal(0, margin.Left);
            Assert.Equal(0, margin.Top);
            Assert.Equal(5, margin.Right);
            Assert.Equal(0, margin.Bottom);
        }

        [Fact]
        public void WithBottom_CreatesNewMarginWithBottom()
        {
            var margin = Margin.WithBottom(5);

            Assert.Equal(0, margin.Left);
            Assert.Equal(0, margin.Top);
            Assert.Equal(0, margin.Right);
            Assert.Equal(5, margin.Bottom);
        }

        [Fact]
        public void WithHorizontal_CreatesNewMarginWithHorizontal()
        {
            var margin = Margin.WithHorizontal(5);

            Assert.Equal(5, margin.Left);
            Assert.Equal(0, margin.Top);
            Assert.Equal(5, margin.Right);
            Assert.Equal(0, margin.Bottom);
        }

        [Fact]
        public void WithVertical_CreatesNewMarginWithVertical()
        {
            var margin = Margin.WithVertical(5);

            Assert.Equal(0, margin.Left);
            Assert.Equal(5, margin.Top);
            Assert.Equal(0, margin.Right);
            Assert.Equal(5, margin.Bottom);
        }

        [Fact]
        public void SetLeft_UpdatesLeft()
        {
            var margin = new Margin(1, 2, 3, 4);
            var updated = margin.SetLeft(10);

            Assert.Equal(10, updated.Left);
            Assert.Equal(2, updated.Top);
            Assert.Equal(3, updated.Right);
            Assert.Equal(4, updated.Bottom);
        }

        [Fact]
        public void SetTop_UpdatesTop()
        {
            var margin = new Margin(1, 2, 3, 4);
            var updated = margin.SetTop(10);

            Assert.Equal(1, updated.Left);
            Assert.Equal(10, updated.Top);
            Assert.Equal(3, updated.Right);
            Assert.Equal(4, updated.Bottom);
        }

        [Fact]
        public void SetRight_UpdatesRight()
        {
            var margin = new Margin(1, 2, 3, 4);
            var updated = margin.SetRight(10);

            Assert.Equal(1, updated.Left);
            Assert.Equal(2, updated.Top);
            Assert.Equal(10, updated.Right);
            Assert.Equal(4, updated.Bottom);
        }

        [Fact]
        public void SetBottom_UpdatesBottom()
        {
            var margin = new Margin(1, 2, 3, 4);
            var updated = margin.SetBottom(10);

            Assert.Equal(1, updated.Left);
            Assert.Equal(2, updated.Top);
            Assert.Equal(3, updated.Right);
            Assert.Equal(10, updated.Bottom);
        }

        [Fact]
        public void Add_AddsMargins()
        {
            var margin1 = new Margin(1, 2, 3, 4);
            var margin2 = new Margin(5, 6, 7, 8);
            var result = margin1.Add(margin2);

            Assert.Equal(6, result.Left);
            Assert.Equal(8, result.Top);
            Assert.Equal(10, result.Right);
            Assert.Equal(12, result.Bottom);
        }

        [Fact]
        public void Subtract_SubtractsMargins()
        {
            var margin1 = new Margin(10, 10, 10, 10);
            var margin2 = new Margin(3, 4, 5, 6);
            var result = margin1.Subtract(margin2);

            Assert.Equal(7, result.Left);
            Assert.Equal(6, result.Top);
            Assert.Equal(5, result.Right);
            Assert.Equal(4, result.Bottom);
        }

        [Fact]
        public void Subtract_NegativeResults_ClampsToZero()
        {
            var margin1 = new Margin(1, 1, 1, 1);
            var margin2 = new Margin(5, 5, 5, 5);
            var result = margin1.Subtract(margin2);

            Assert.Equal(0, result.Left);
            Assert.Equal(0, result.Top);
            Assert.Equal(0, result.Right);
            Assert.Equal(0, result.Bottom);
        }

        [Fact]
        public void Equals_SameMargins_ReturnsTrue()
        {
            var margin1 = new Margin(1, 2, 3, 4);
            var margin2 = new Margin(1, 2, 3, 4);

            Assert.True(margin1.Equals(margin2));
            Assert.True(margin1 == margin2);
            Assert.False(margin1 != margin2);
        }

        [Fact]
        public void Equals_DifferentMargins_ReturnsFalse()
        {
            var margin1 = new Margin(1, 2, 3, 4);
            var margin2 = new Margin(5, 6, 7, 8);

            Assert.False(margin1.Equals(margin2));
            Assert.False(margin1 == margin2);
            Assert.True(margin1 != margin2);
        }

        [Fact]
        public void GetHashCode_SameMargins_ReturnsSameHash()
        {
            var margin1 = new Margin(1, 2, 3, 4);
            var margin2 = new Margin(1, 2, 3, 4);

            Assert.Equal(margin1.GetHashCode(), margin2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            var margin = new Margin(1, 2, 3, 4);
            var result = margin.ToString();

            Assert.Equal("Margin(left=1, top=2, right=3, bottom=4)", result);
        }

        [Fact]
        public void Operators_Addition_WorksCorrectly()
        {
            var margin1 = new Margin(1, 2, 3, 4);
            var margin2 = new Margin(5, 6, 7, 8);
            var result = margin1 + margin2;

            Assert.Equal(6, result.Left);
            Assert.Equal(8, result.Top);
            Assert.Equal(10, result.Right);
            Assert.Equal(12, result.Bottom);
        }

        [Fact]
        public void Operators_Subtraction_WorksCorrectly()
        {
            var margin1 = new Margin(10, 10, 10, 10);
            var margin2 = new Margin(3, 4, 5, 6);
            var result = margin1 - margin2;

            Assert.Equal(7, result.Left);
            Assert.Equal(6, result.Top);
            Assert.Equal(5, result.Right);
            Assert.Equal(4, result.Bottom);
        }

        [Fact]
        public void Equals_WithObject_WorksCorrectly()
        {
            var margin = new Margin(1, 2, 3, 4);
            object obj = new Margin(1, 2, 3, 4);
            object differentObj = new Margin(5, 6, 7, 8);
            object? nullObj = null;
            object wrongType = "string";

            Assert.True(margin.Equals(obj));
            Assert.False(margin.Equals(differentObj));
            Assert.False(margin.Equals(nullObj));
            Assert.False(margin.Equals(wrongType));
        }
    }
}