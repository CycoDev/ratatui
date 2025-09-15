using System;
using CycoAI.CycoTui.Core.Layout;
using Xunit;

namespace CycoAI.CycoTui.Tests.Layout
{
    public class ConstraintTests
    {
        [Fact]
        public void Length_CreatesLengthConstraint()
        {
            var constraint = Constraint.Length(10);

            Assert.Equal(ConstraintType.Length, constraint.Type);
            Assert.Equal(10, constraint.Value);
            Assert.True(constraint.IsFixed);
            Assert.False(constraint.IsFlexible);
        }

        [Fact]
        public void Length_NegativeValue_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Constraint.Length(-1));
        }

        [Fact]
        public void Percentage_CreatesPercentageConstraint()
        {
            var constraint = Constraint.Percentage(25.5f);

            Assert.Equal(ConstraintType.Percentage, constraint.Type);
            Assert.Equal(25.5f, constraint.Value);
            Assert.False(constraint.IsFixed);
            Assert.True(constraint.IsFlexible);
        }

        [Fact]
        public void Percentage_InvalidRange_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Constraint.Percentage(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Constraint.Percentage(101));
        }

        [Fact]
        public void Ratio_CreatesRatioConstraint()
        {
            var constraint = Constraint.Ratio(2.0f);

            Assert.Equal(ConstraintType.Ratio, constraint.Type);
            Assert.Equal(2.0f, constraint.Value);
            Assert.False(constraint.IsFixed);
            Assert.True(constraint.IsFlexible);
        }

        [Fact]
        public void Ratio_NonPositiveValue_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Constraint.Ratio(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Constraint.Ratio(-1));
        }

        [Fact]
        public void Min_CreatesMinConstraint()
        {
            var constraint = Constraint.Min(5);

            Assert.Equal(ConstraintType.Min, constraint.Type);
            Assert.Equal(5, constraint.Value);
            Assert.True(constraint.IsFixed);
            Assert.False(constraint.IsFlexible);
        }

        [Fact]
        public void Min_NegativeValue_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Constraint.Min(-1));
        }

        [Fact]
        public void Max_CreatesMaxConstraint()
        {
            var constraint = Constraint.Max(15);

            Assert.Equal(ConstraintType.Max, constraint.Type);
            Assert.Equal(15, constraint.Value);
            Assert.True(constraint.IsFixed);
            Assert.False(constraint.IsFlexible);
        }

        [Fact]
        public void Max_NegativeValue_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Constraint.Max(-1));
        }

        [Fact]
        public void Fill_CreatesRatioConstraint()
        {
            var constraint = Constraint.Fill();

            Assert.Equal(ConstraintType.Ratio, constraint.Type);
            Assert.Equal(1.0f, constraint.Value);
            Assert.False(constraint.IsFixed);
            Assert.True(constraint.IsFlexible);
        }


        [Fact]
        public void Equals_SameConstraints_ReturnsTrue()
        {
            var constraint1 = Constraint.Length(10);
            var constraint2 = Constraint.Length(10);

            Assert.True(constraint1.Equals(constraint2));
            Assert.True(constraint1 == constraint2);
            Assert.False(constraint1 != constraint2);
        }

        [Fact]
        public void Equals_DifferentConstraints_ReturnsFalse()
        {
            var constraint1 = Constraint.Length(10);
            var constraint2 = Constraint.Length(20);

            Assert.False(constraint1.Equals(constraint2));
            Assert.False(constraint1 == constraint2);
            Assert.True(constraint1 != constraint2);
        }

        [Fact]
        public void Equals_DifferentTypes_ReturnsFalse()
        {
            var constraint1 = Constraint.Length(10);
            var constraint2 = Constraint.Percentage(10);

            Assert.False(constraint1.Equals(constraint2));
        }

        [Fact]
        public void GetHashCode_SameConstraints_ReturnsSameHash()
        {
            var constraint1 = Constraint.Length(10);
            var constraint2 = Constraint.Length(10);

            Assert.Equal(constraint1.GetHashCode(), constraint2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            var lengthConstraint = Constraint.Length(10);
            var percentageConstraint = Constraint.Percentage(25.5f);
            var ratioConstraint = Constraint.Ratio(2);
            var minConstraint = Constraint.Min(5);
            var maxConstraint = Constraint.Max(15);

            Assert.Equal("Length(10)", lengthConstraint.ToString());
            Assert.Equal("Percentage(25.5%)", percentageConstraint.ToString());
            Assert.Equal("Ratio(2)", ratioConstraint.ToString());
            Assert.Equal("Min(5)", minConstraint.ToString());
            Assert.Equal("Max(15)", maxConstraint.ToString());
        }

        [Fact]
        public void Equals_WithObject_WorksCorrectly()
        {
            var constraint = Constraint.Length(10);
            object obj = Constraint.Length(10);
            object differentObj = Constraint.Length(20);
            object? nullObj = null;
            object wrongType = "string";

            Assert.True(constraint.Equals(obj));
            Assert.False(constraint.Equals(differentObj));
            Assert.False(constraint.Equals(nullObj));
            Assert.False(constraint.Equals(wrongType));
        }

    }
}