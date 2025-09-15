using System;
using System.Collections.Generic;
using System.Linq;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Layout
{
    /// <summary>
    /// Represents a cache key for layout calculations.
    /// </summary>
    internal readonly struct LayoutCacheKey : IEquatable<LayoutCacheKey>
    {
        public Rect Area { get; }
        public IReadOnlyList<Constraint> Constraints { get; }
        public Direction Direction { get; }
        public Margin Margin { get; }
        public bool Flex { get; }
        public Alignment Alignment { get; }

        public LayoutCacheKey(Rect area, IReadOnlyList<Constraint> constraints, Direction direction,
            Margin margin, bool flex, Alignment alignment)
        {
            Area = area;
            Constraints = constraints;
            Direction = direction;
            Margin = margin;
            Flex = flex;
            Alignment = alignment;
        }

        public bool Equals(LayoutCacheKey other)
        {
            return Area.Equals(other.Area) &&
                   Direction == other.Direction &&
                   Margin.Equals(other.Margin) &&
                   Flex == other.Flex &&
                   Alignment.Equals(other.Alignment) &&
                   ConstraintsEqual(Constraints, other.Constraints);
        }

        private static bool ConstraintsEqual(IReadOnlyList<Constraint> left, IReadOnlyList<Constraint> right)
        {
            if (left.Count != right.Count)
                return false;

            for (int i = 0; i < left.Count; i++)
            {
                if (!left[i].Equals(right[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object? obj) => obj is LayoutCacheKey other && Equals(other);

        public override int GetHashCode()
        {
            var hash = Area.GetHashCode();
#if NETSTANDARD2_0 || NETSTANDARD2_1
            hash = Compat.CombineHashCodes(hash, Direction);
            hash = Compat.CombineHashCodes(hash, Margin);
            hash = Compat.CombineHashCodes(hash, Flex);
            hash = Compat.CombineHashCodes(hash, Alignment);
#else
            hash = HashCode.Combine(hash, Direction, Margin, Flex, Alignment);
#endif

            foreach (var constraint in Constraints)
            {
#if NETSTANDARD2_0 || NETSTANDARD2_1
                hash = Compat.CombineHashCodes(hash, constraint);
#else
                hash = HashCode.Combine(hash, constraint);
#endif
            }

            return hash;
        }
    }

    /// <summary>
    /// Provides caching functionality for layout calculations to improve performance.
    /// </summary>
    public class LayoutCache
    {
        private readonly Dictionary<LayoutCacheKey, Rect[]> _cache;
        private readonly int _maxSize;
        private readonly object _lock = new object();

        /// <summary>
        /// Gets the number of cached entries.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _cache.Count;
                }
            }
        }

        /// <summary>
        /// Gets the maximum number of entries that can be cached.
        /// </summary>
        public int MaxSize => _maxSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutCache"/> class.
        /// </summary>
        /// <param name="maxSize">The maximum number of entries to cache (default: 1000).</param>
        public LayoutCache(int maxSize = 1000)
        {
            if (maxSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxSize), "Max size must be positive.");

            _maxSize = maxSize;
            _cache = new Dictionary<LayoutCacheKey, Rect[]>();
        }

        /// <summary>
        /// Attempts to get a cached layout result.
        /// </summary>
        /// <param name="area">The area to split.</param>
        /// <param name="constraints">The constraints for the layout.</param>
        /// <param name="direction">The split direction.</param>
        /// <param name="margin">The margin to apply.</param>
        /// <param name="flex">Whether flex layout is enabled.</param>
        /// <param name="alignment">The alignment to apply.</param>
        /// <param name="result">The cached result if found.</param>
        /// <returns>true if a cached result was found; otherwise, false.</returns>
        public bool TryGet(Rect area, IReadOnlyList<Constraint> constraints, Direction direction,
            Margin margin, bool flex, Alignment alignment, out Rect[]? result)
        {
            var key = new LayoutCacheKey(area, constraints, direction, margin, flex, alignment);

            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var cachedResult))
                {
                    // Create a copy of the cached result to prevent external modification
                    result = new Rect[cachedResult.Length];
                    Array.Copy(cachedResult, result, cachedResult.Length);
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Stores a layout result in the cache.
        /// </summary>
        /// <param name="area">The area that was split.</param>
        /// <param name="constraints">The constraints that were used.</param>
        /// <param name="direction">The split direction that was used.</param>
        /// <param name="margin">The margin that was applied.</param>
        /// <param name="flex">Whether flex layout was enabled.</param>
        /// <param name="alignment">The alignment that was applied.</param>
        /// <param name="result">The layout result to cache.</param>
        public void Store(Rect area, IReadOnlyList<Constraint> constraints, Direction direction,
            Margin margin, bool flex, Alignment alignment, Rect[] result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var key = new LayoutCacheKey(area, constraints, direction, margin, flex, alignment);

            lock (_lock)
            {
                // If cache is full, remove some entries using a simple LRU-like strategy
                if (_cache.Count >= _maxSize)
                {
                    var entriesToRemove = _maxSize / 4; // Remove 25% of entries
                    var keysToRemove = _cache.Keys.Take(entriesToRemove).ToArray();

                    foreach (var keyToRemove in keysToRemove)
                    {
                        _cache.Remove(keyToRemove);
                    }
                }

                // Create a copy of the result to prevent external modification
                var cachedResult = new Rect[result.Length];
                Array.Copy(result, cachedResult, result.Length);

                _cache[key] = cachedResult;
            }
        }

        /// <summary>
        /// Clears all cached entries.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// Gets cache statistics.
        /// </summary>
        /// <returns>A tuple containing the current count and maximum size.</returns>
        public (int Count, int MaxSize) GetStatistics()
        {
            lock (_lock)
            {
                return (_cache.Count, _maxSize);
            }
        }
    }
}