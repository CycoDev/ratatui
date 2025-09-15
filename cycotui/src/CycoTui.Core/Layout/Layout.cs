using System;
using System.Collections.Generic;
using System.Linq;

namespace CycoAI.CycoTui.Core.Layout
{
    /// <summary>
    /// Provides layout calculation functionality for dividing rectangular areas
    /// according to constraints and direction.
    /// </summary>
    public class Layout
    {
        private static readonly LayoutCache _globalCache = new LayoutCache();

        private readonly List<Constraint> _constraints;
        private Direction _direction;
        private Margin _margin;
        private bool _flex;
        private Alignment _alignment;
        private bool _enableCaching;

        /// <summary>
        /// Gets or sets the direction for splitting the area.
        /// </summary>
        public Direction Direction
        {
            get => _direction;
            set => _direction = value;
        }

        /// <summary>
        /// Gets or sets the margin applied to the area before splitting.
        /// </summary>
        public Margin Margin
        {
            get => _margin;
            set => _margin = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether flex layout is enabled.
        /// When enabled, constraints can grow to fill available space.
        /// </summary>
        public bool Flex
        {
            get => _flex;
            set => _flex = value;
        }

        /// <summary>
        /// Gets or sets the alignment for positioning content within layout areas.
        /// </summary>
        public Alignment Alignment
        {
            get => _alignment;
            set => _alignment = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether layout caching is enabled.
        /// When enabled, layout calculations are cached for improved performance.
        /// </summary>
        public bool EnableCaching
        {
            get => _enableCaching;
            set => _enableCaching = value;
        }

        /// <summary>
        /// Gets the list of constraints for this layout.
        /// </summary>
        public IReadOnlyList<Constraint> Constraints => _constraints.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="Layout"/> class.
        /// </summary>
        /// <param name="direction">The direction for splitting.</param>
        public Layout(Direction direction = Direction.Vertical)
        {
            _constraints = new List<Constraint>();
            _direction = direction;
            _margin = Margin.None;
            _flex = false;
            _alignment = Alignment.Default;
            _enableCaching = true; // Enable caching by default for performance
        }

        /// <summary>
        /// Adds a constraint to this layout.
        /// </summary>
        /// <param name="constraint">The constraint to add.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout AddConstraint(Constraint constraint)
        {
            _constraints.Add(constraint);
            return this;
        }

        /// <summary>
        /// Adds multiple constraints to this layout.
        /// </summary>
        /// <param name="constraints">The constraints to add.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout AddConstraints(params Constraint[] constraints)
        {
            _constraints.AddRange(constraints);
            return this;
        }

        /// <summary>
        /// Adds multiple constraints to this layout.
        /// </summary>
        /// <param name="constraints">The constraints to add.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout AddConstraints(IEnumerable<Constraint> constraints)
        {
            _constraints.AddRange(constraints);
            return this;
        }

        /// <summary>
        /// Clears all constraints from this layout.
        /// </summary>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout ClearConstraints()
        {
            _constraints.Clear();
            return this;
        }

        /// <summary>
        /// Sets the direction for this layout.
        /// </summary>
        /// <param name="direction">The direction to set.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithDirection(Direction direction)
        {
            _direction = direction;
            return this;
        }

        /// <summary>
        /// Sets the margin for this layout.
        /// </summary>
        /// <param name="margin">The margin to set.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithMargin(Margin margin)
        {
            _margin = margin;
            return this;
        }

        /// <summary>
        /// Sets the margin for this layout using uniform spacing.
        /// </summary>
        /// <param name="margin">The uniform margin to apply to all sides.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithMargin(int margin)
        {
            _margin = new Margin(margin);
            return this;
        }

        /// <summary>
        /// Enables or disables flex layout.
        /// </summary>
        /// <param name="flex">Whether to enable flex layout.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithFlex(bool flex)
        {
            _flex = flex;
            return this;
        }

        /// <summary>
        /// Sets the alignment for this layout.
        /// </summary>
        /// <param name="alignment">The alignment to set.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithAlignment(Alignment alignment)
        {
            _alignment = alignment;
            return this;
        }

        /// <summary>
        /// Sets the horizontal alignment for this layout.
        /// </summary>
        /// <param name="horizontal">The horizontal alignment to set.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithHorizontalAlignment(HorizontalAlignment horizontal)
        {
            _alignment = _alignment.WithHorizontal(horizontal);
            return this;
        }

        /// <summary>
        /// Sets the vertical alignment for this layout.
        /// </summary>
        /// <param name="vertical">The vertical alignment to set.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithVerticalAlignment(VerticalAlignment vertical)
        {
            _alignment = _alignment.WithVertical(vertical);
            return this;
        }

        /// <summary>
        /// Enables or disables layout caching.
        /// </summary>
        /// <param name="enable">Whether to enable caching.</param>
        /// <returns>This layout instance for method chaining.</returns>
        public Layout WithCaching(bool enable)
        {
            _enableCaching = enable;
            return this;
        }

        /// <summary>
        /// Splits the given area according to the constraints and returns the resulting rectangles.
        /// </summary>
        /// <param name="area">The area to split.</param>
        /// <returns>An array of rectangles, one for each constraint.</returns>
        public Rect[] Split(Rect area)
        {
            if (_constraints.Count == 0)
                return new[] { area };

            // Check cache first if caching is enabled
            if (_enableCaching && _globalCache.TryGet(area, _constraints.AsReadOnly(), _direction, _margin, _flex, _alignment, out var cachedResult))
            {
                return cachedResult!;
            }

            // Apply margin to the area
            var workingArea = area.WithMargin(_margin.Left, _margin.Top, _margin.Right, _margin.Bottom);
            if (workingArea.IsEmpty)
            {
                var emptyResult = new Rect[_constraints.Count]; // Return empty rectangles

                // Cache the empty result if caching is enabled
                if (_enableCaching)
                {
                    _globalCache.Store(area, _constraints.AsReadOnly(), _direction, _margin, _flex, _alignment, emptyResult);
                }

                return emptyResult;
            }

            var result = _direction == Direction.Horizontal
                ? SplitHorizontally(workingArea)
                : SplitVertically(workingArea);

            // Cache the result if caching is enabled
            if (_enableCaching)
            {
                _globalCache.Store(area, _constraints.AsReadOnly(), _direction, _margin, _flex, _alignment, result);
            }

            return result;
        }

        private Rect[] SplitHorizontally(Rect area)
        {
            var results = new Rect[_constraints.Count];
            var availableWidth = area.Width;

            // First pass: calculate fixed sizes and total ratio
            var fixedWidth = 0;
            var totalRatio = 0.0f;
            var sizes = new int[_constraints.Count];

            for (int i = 0; i < _constraints.Count; i++)
            {
                var constraint = _constraints[i];
                if (constraint.Type == ConstraintType.Ratio)
                {
                    totalRatio += constraint.Value;
                }
                else
                {
                    var size = constraint.Apply(availableWidth);
                    sizes[i] = size;
                    fixedWidth += size;
                }
            }

            // Second pass: calculate flexible sizes
            var remainingWidth = Math.Max(0, availableWidth - fixedWidth);
            for (int i = 0; i < _constraints.Count; i++)
            {
                var constraint = _constraints[i];
                if (constraint.Type == ConstraintType.Ratio)
                {
                    sizes[i] = constraint.Apply(remainingWidth, totalRatio);
                }
            }

            // Third pass: apply flex if enabled and adjust sizes
            if (_flex && remainingWidth > 0)
            {
                var actualUsed = sizes.Sum();
                var extraSpace = Math.Max(0, availableWidth - actualUsed);
                if (extraSpace > 0)
                {
                    DistributeExtraSpace(sizes, extraSpace);
                }
            }

            // Apply min/max constraints
            for (int i = 0; i < _constraints.Count; i++)
            {
                var constraint = _constraints[i];
                if (constraint.Type == ConstraintType.Min)
                {
                    sizes[i] = Math.Max(sizes[i], (int)constraint.Value);
                }
                else if (constraint.Type == ConstraintType.Max)
                {
                    sizes[i] = Math.Min(sizes[i], (int)constraint.Value);
                }
            }

            // Generate rectangles
            var currentX = area.X;
            for (int i = 0; i < _constraints.Count; i++)
            {
                var width = Math.Min(sizes[i], area.X + area.Width - currentX);
                results[i] = new Rect(currentX, area.Y, width, area.Height);
                currentX += width;

                if (currentX >= area.X + area.Width)
                    break;
            }

            return results;
        }

        private Rect[] SplitVertically(Rect area)
        {
            var results = new Rect[_constraints.Count];
            var availableHeight = area.Height;

            // First pass: calculate fixed sizes and total ratio
            var fixedHeight = 0;
            var totalRatio = 0.0f;
            var sizes = new int[_constraints.Count];

            for (int i = 0; i < _constraints.Count; i++)
            {
                var constraint = _constraints[i];
                if (constraint.Type == ConstraintType.Ratio)
                {
                    totalRatio += constraint.Value;
                }
                else
                {
                    var size = constraint.Apply(availableHeight);
                    sizes[i] = size;
                    fixedHeight += size;
                }
            }

            // Second pass: calculate flexible sizes
            var remainingHeight = Math.Max(0, availableHeight - fixedHeight);
            for (int i = 0; i < _constraints.Count; i++)
            {
                var constraint = _constraints[i];
                if (constraint.Type == ConstraintType.Ratio)
                {
                    sizes[i] = constraint.Apply(remainingHeight, totalRatio);
                }
            }

            // Third pass: apply flex if enabled and adjust sizes
            if (_flex && remainingHeight > 0)
            {
                var actualUsed = sizes.Sum();
                var extraSpace = Math.Max(0, availableHeight - actualUsed);
                if (extraSpace > 0)
                {
                    DistributeExtraSpace(sizes, extraSpace);
                }
            }

            // Apply min/max constraints
            for (int i = 0; i < _constraints.Count; i++)
            {
                var constraint = _constraints[i];
                if (constraint.Type == ConstraintType.Min)
                {
                    sizes[i] = Math.Max(sizes[i], (int)constraint.Value);
                }
                else if (constraint.Type == ConstraintType.Max)
                {
                    sizes[i] = Math.Min(sizes[i], (int)constraint.Value);
                }
            }

            // Generate rectangles
            var currentY = area.Y;
            for (int i = 0; i < _constraints.Count; i++)
            {
                var height = Math.Min(sizes[i], area.Y + area.Height - currentY);
                results[i] = new Rect(area.X, currentY, area.Width, height);
                currentY += height;

                if (currentY >= area.Y + area.Height)
                    break;
            }

            return results;
        }

        private static void DistributeExtraSpace(int[] sizes, int extraSpace)
        {
            if (sizes.Length == 0)
                return;

            var perItem = extraSpace / sizes.Length;
            var remainder = extraSpace % sizes.Length;

            for (int i = 0; i < sizes.Length; i++)
            {
                sizes[i] += perItem;
                if (i < remainder)
                {
                    sizes[i] += 1;
                }
            }
        }

        /// <summary>
        /// Creates a simple horizontal layout with the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints for the layout.</param>
        /// <returns>A new horizontal layout.</returns>
        public static Layout Horizontal(params Constraint[] constraints)
        {
            return new Layout(Direction.Horizontal).AddConstraints(constraints);
        }

        /// <summary>
        /// Creates a simple vertical layout with the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints for the layout.</param>
        /// <returns>A new vertical layout.</returns>
        public static Layout Vertical(params Constraint[] constraints)
        {
            return new Layout(Direction.Vertical).AddConstraints(constraints);
        }

        /// <summary>
        /// Gets statistics about the global layout cache.
        /// </summary>
        /// <returns>A tuple containing the current count and maximum size of the cache.</returns>
        public static (int Count, int MaxSize) GetCacheStatistics()
        {
            return _globalCache.GetStatistics();
        }

        /// <summary>
        /// Clears the global layout cache.
        /// </summary>
        public static void ClearCache()
        {
            _globalCache.Clear();
        }
    }

    /// <summary>
    /// Specifies the direction for layout splitting.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Split horizontally (left to right).
        /// </summary>
        Horizontal,

        /// <summary>
        /// Split vertically (top to bottom).
        /// </summary>
        Vertical
    }
}