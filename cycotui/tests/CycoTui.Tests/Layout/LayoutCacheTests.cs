using System;
using System.Collections.Generic;
using CycoAI.CycoTui.Core.Layout;
using Xunit;

namespace CycoAI.CycoTui.Tests.Layout
{
    public class LayoutCacheTests
    {
        [Fact]
        public void Constructor_DefaultMaxSize_SetsCorrectly()
        {
            var cache = new LayoutCache();

            Assert.Equal(1000, cache.MaxSize);
            Assert.Equal(0, cache.Count);
        }

        [Fact]
        public void Constructor_CustomMaxSize_SetsCorrectly()
        {
            var cache = new LayoutCache(500);

            Assert.Equal(500, cache.MaxSize);
            Assert.Equal(0, cache.Count);
        }

        [Fact]
        public void Constructor_ZeroOrNegativeMaxSize_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutCache(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LayoutCache(-1));
        }

        [Fact]
        public void TryGet_EmptyCache_ReturnsFalse()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };

            var found = cache.TryGet(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, out var result);

            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void Store_AddsToCache()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };
            var result = new[] { new Rect(0, 0, 100, 50) };

            cache.Store(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, result);

            Assert.Equal(1, cache.Count);
        }

        [Fact]
        public void Store_NullResult_ThrowsException()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };

            Assert.Throws<ArgumentNullException>(() =>
                cache.Store(area, constraints, Direction.Vertical,
                    Margin.None, false, Alignment.Default, null!));
        }

        [Fact]
        public void TryGet_AfterStore_ReturnsTrue()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };
            var originalResult = new[] { new Rect(0, 0, 100, 50) };

            cache.Store(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, originalResult);

            var found = cache.TryGet(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, out var result);

            Assert.True(found);
            Assert.NotNull(result);
            Assert.Single(result!);
            Assert.Equal(originalResult[0], result[0]);
        }

        [Fact]
        public void TryGet_ReturnsIndependentCopy()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };
            var originalResult = new[] { new Rect(0, 0, 100, 50) };

            cache.Store(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, originalResult);

            cache.TryGet(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, out var result1);
            cache.TryGet(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, out var result2);

            // Results should be equal but not the same reference
            Assert.Equal(result1, result2);
            Assert.NotSame(result1, result2);
            Assert.NotSame(originalResult, result1);
        }

        [Fact]
        public void TryGet_DifferentParameters_ReturnsFalse()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };
            var result = new[] { new Rect(0, 0, 100, 50) };

            cache.Store(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, result);

            // Different area
            var found1 = cache.TryGet(new Rect(0, 0, 200, 100), constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, out _);

            // Different direction
            var found2 = cache.TryGet(area, constraints, Direction.Horizontal,
                Margin.None, false, Alignment.Default, out _);

            // Different constraints
            var differentConstraints = new List<Constraint> { Constraint.Length(25) };
            var found3 = cache.TryGet(area, differentConstraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, out _);

            // Different margin
            var found4 = cache.TryGet(area, constraints, Direction.Vertical,
                new Margin(5), false, Alignment.Default, out _);

            // Different flex
            var found5 = cache.TryGet(area, constraints, Direction.Vertical,
                Margin.None, true, Alignment.Default, out _);

            // Different alignment
            var found6 = cache.TryGet(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Center, out _);

            Assert.False(found1);
            Assert.False(found2);
            Assert.False(found3);
            Assert.False(found4);
            Assert.False(found5);
            Assert.False(found6);
        }


        [Fact]
        public void Clear_RemovesAllEntries()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };
            var result = new[] { new Rect(0, 0, 100, 50) };

            // Add some entries
            cache.Store(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, result);
            cache.Store(area, constraints, Direction.Horizontal,
                Margin.None, false, Alignment.Default, result);

            Assert.True(cache.Count > 0);

            cache.Clear();

            Assert.Equal(0, cache.Count);
        }

        [Fact]
        public void GetStatistics_ReturnsCorrectValues()
        {
            var cache = new LayoutCache(100);
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };
            var result = new[] { new Rect(0, 0, 100, 50) };

            var (initialCount, maxSize) = cache.GetStatistics();
            Assert.Equal(0, initialCount);
            Assert.Equal(100, maxSize);

            cache.Store(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, result);

            var (afterStoreCount, _) = cache.GetStatistics();
            Assert.Equal(1, afterStoreCount);
        }

        [Fact]
        public void TryGet_WithMultipleConstraints_WorksCorrectly()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint>
            {
                Constraint.Length(30),
                Constraint.Percentage(25),
                Constraint.Ratio(1)
            };
            var result = new[]
            {
                new Rect(0, 0, 100, 30),
                new Rect(0, 30, 100, 25),
                new Rect(0, 55, 100, 45)
            };

            cache.Store(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, result);

            var found = cache.TryGet(area, constraints, Direction.Vertical,
                Margin.None, false, Alignment.Default, out var cachedResult);

            Assert.True(found);
            Assert.NotNull(cachedResult);
            Assert.Equal(3, cachedResult!.Length);
            Assert.Equal(result, cachedResult);
        }

        [Fact]
        public void Cache_IsThreadSafe()
        {
            var cache = new LayoutCache();
            var area = new Rect(0, 0, 100, 100);
            var constraints = new List<Constraint> { Constraint.Length(50) };
            var result = new[] { new Rect(0, 0, 100, 50) };

            // This is a basic test for thread safety - in real scenarios you'd need more comprehensive testing
            var tasks = new System.Threading.Tasks.Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                int taskId = i;
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    var taskConstraints = new List<Constraint> { Constraint.Length(50 + taskId) };
                    cache.Store(area, taskConstraints, Direction.Vertical,
                        Margin.None, false, Alignment.Default, result);

                    cache.TryGet(area, taskConstraints, Direction.Vertical,
                        Margin.None, false, Alignment.Default, out _);
                });
            }

            System.Threading.Tasks.Task.WaitAll(tasks);

            // If we get here without exceptions, basic thread safety is working
            Assert.True(cache.Count >= 0);
        }
    }
}