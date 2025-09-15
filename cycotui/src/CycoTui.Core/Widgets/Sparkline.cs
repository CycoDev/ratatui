using System;
using System.Collections.Generic;
using System.Linq;
using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Text;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Widgets
{
    /// <summary>
    /// Specifies the direction of a sparkline chart.
    /// </summary>
    public enum SparklineDirection
    {
        /// <summary>
        /// Left to right direction.
        /// </summary>
        LeftToRight,

        /// <summary>
        /// Right to left direction.
        /// </summary>
        RightToLeft
    }

    /// <summary>
    /// A compact widget that displays small line charts using Unicode block characters.
    /// Sparklines are typically used to show trends in a minimal space.
    /// </summary>
    public class Sparkline : Widget
    {
        private readonly List<double> _data;

        /// <summary>
        /// Gets or sets the maximum value for scaling. If null, uses the maximum value from the data.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the style applied to the sparkline.
        /// </summary>
        public TextStyle Style { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets the direction of the sparkline.
        /// </summary>
        public SparklineDirection Direction { get; set; } = SparklineDirection.LeftToRight;

        /// <summary>
        /// Gets the data displayed in this sparkline.
        /// </summary>
        public IReadOnlyList<double> Data => _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sparkline"/> class.
        /// </summary>
        public Sparkline()
        {
            _data = new List<double>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sparkline"/> class with the specified data.
        /// </summary>
        /// <param name="data">The data to display.</param>
        public Sparkline(IEnumerable<double> data)
        {
            _data = new List<double>(data ?? throw new ArgumentNullException(nameof(data)));
        }

        /// <summary>
        /// Creates a new sparkline with the specified data.
        /// </summary>
        /// <param name="data">The data to display.</param>
        /// <returns>A new sparkline instance.</returns>
        public static Sparkline WithData(params double[] data) => new Sparkline(data);

        /// <summary>
        /// Adds a data point to this sparkline.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>This sparkline instance for method chaining.</returns>
        public Sparkline AddValue(double value)
        {
            _data.Add(value);
            return this;
        }

        /// <summary>
        /// Adds multiple data points to this sparkline.
        /// </summary>
        /// <param name="values">The values to add.</param>
        /// <returns>This sparkline instance for method chaining.</returns>
        public Sparkline AddValues(params double[] values)
        {
            _data.AddRange(values);
            return this;
        }

        /// <summary>
        /// Clears all data from this sparkline.
        /// </summary>
        /// <returns>This sparkline instance for method chaining.</returns>
        public Sparkline Clear()
        {
            _data.Clear();
            return this;
        }

        /// <summary>
        /// Sets the maximum value for scaling.
        /// </summary>
        /// <param name="maxValue">The maximum value to use for scaling.</param>
        /// <returns>This sparkline instance for method chaining.</returns>
        public Sparkline WithMaxValue(double maxValue)
        {
            MaxValue = maxValue;
            return this;
        }

        /// <summary>
        /// Sets the style for this sparkline.
        /// </summary>
        /// <param name="style">The style to apply.</param>
        /// <returns>This sparkline instance for method chaining.</returns>
        public Sparkline WithStyle(TextStyle style)
        {
            Style = style;
            return this;
        }

        /// <summary>
        /// Sets the direction for this sparkline.
        /// </summary>
        /// <param name="direction">The direction to set.</param>
        /// <returns>This sparkline instance for method chaining.</returns>
        public Sparkline WithDirection(SparklineDirection direction)
        {
            Direction = direction;
            return this;
        }

        /// <inheritdoc />
        public override (int Width, int Height) GetMinimumSize() => (1, 1);

        /// <inheritdoc />
        public override void Render(Rect area, IBuffer buffer)
        {
            if (area.IsEmpty || _data.Count == 0)
                return;

            var maxValue = GetEffectiveMaxValue();
            if (maxValue <= 0)
                return;

            var availableWidth = area.Width;
            var availableHeight = area.Height;

            // Group data points by available width
            var groupedData = GroupDataForWidth(availableWidth);

            for (int x = 0; x < groupedData.Count && x < availableWidth; x++)
            {
                var actualX = Direction == SparklineDirection.LeftToRight ? x : availableWidth - 1 - x;
                var value = groupedData[x];

                RenderColumn(area.X + actualX, area.Y, availableHeight, value, maxValue, buffer);
            }
        }

        private double GetEffectiveMaxValue()
        {
            if (MaxValue.HasValue && MaxValue.Value > 0)
                return MaxValue.Value;

            if (_data.Count == 0)
                return 1.0;

            var max = _data.Max();
            return max > 0 ? max : 1.0;
        }

        private List<double> GroupDataForWidth(int width)
        {
            if (_data.Count <= width)
            {
                // If we have fewer data points than width, spread them out
                var result = new List<double>(new double[width]);
                for (int i = 0; i < _data.Count; i++)
                {
                    var targetIndex = (int)Math.Round((double)i / (_data.Count - 1) * (width - 1));
                    result[targetIndex] = Math.Max(result[targetIndex], _data[i]);
                }
                return result;
            }
            else
            {
                // If we have more data points than width, group them
                var result = new List<double>();
                var pointsPerGroup = (double)_data.Count / width;

                for (int i = 0; i < width; i++)
                {
                    var startIndex = (int)(i * pointsPerGroup);
                    var endIndex = (int)((i + 1) * pointsPerGroup);
                    endIndex = Math.Min(endIndex, _data.Count);

                    var groupValue = 0.0;
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        groupValue = Math.Max(groupValue, _data[j]);
                    }
                    result.Add(groupValue);
                }

                return result;
            }
        }

        private void RenderColumn(int x, int startY, int height, double value, double maxValue, IBuffer buffer)
        {
            if (height <= 0)
                return;

            // Calculate how much of the column should be filled
            var normalizedValue = value / maxValue;
            var totalBlocks = height * 8; // 8 levels per character
            var filledBlocks = (int)Math.Round(normalizedValue * totalBlocks);

            // Render from bottom to top
            for (int row = 0; row < height; row++)
            {
                var y = startY + height - 1 - row;
                var rowStartBlock = row * 8;
                var rowEndBlock = (row + 1) * 8;

                char character;
                if (filledBlocks <= rowStartBlock)
                {
                    // Empty row
                    character = ' ';
                }
                else if (filledBlocks >= rowEndBlock)
                {
                    // Full row
                    character = '█';
                }
                else
                {
                    // Partial row
                    var blocksInRow = filledBlocks - rowStartBlock;
                    character = GetBlockCharacter(blocksInRow);
                }

                buffer.SetCellSafe(x, y, new Cell(character, Style.Foreground, Style.Background, Style.Modifiers));
            }
        }

        private char GetBlockCharacter(int level)
        {
            // Unicode block characters for different fill levels
            return level switch
            {
                1 => '▁',  // 1/8 block
                2 => '▂',  // 2/8 block
                3 => '▃',  // 3/8 block
                4 => '▄',  // 4/8 block
                5 => '▅',  // 5/8 block
                6 => '▆',  // 6/8 block
                7 => '▇',  // 7/8 block
                8 => '█',  // full block
                _ => ' '   // empty
            };
        }

        /// <summary>
        /// Gets the current maximum value used for scaling.
        /// </summary>
        /// <returns>The maximum value used for scaling.</returns>
        public double GetCurrentMaxValue() => GetEffectiveMaxValue();

        /// <summary>
        /// Gets the current minimum value in the data.
        /// </summary>
        /// <returns>The minimum value in the data.</returns>
        public double GetCurrentMinValue()
        {
            if (_data.Count == 0)
                return 0.0;

            return _data.Min();
        }

        /// <summary>
        /// Gets the average value in the data.
        /// </summary>
        /// <returns>The average value in the data.</returns>
        public double GetAverageValue()
        {
            if (_data.Count == 0)
                return 0.0;

            return _data.Average();
        }

        /// <summary>
        /// Gets statistics about the current data.
        /// </summary>
        /// <returns>A tuple containing (min, max, average, count).</returns>
        public (double Min, double Max, double Average, int Count) GetStatistics()
        {
            if (_data.Count == 0)
                return (0, 0, 0, 0);

            return (_data.Min(), _data.Max(), _data.Average(), _data.Count);
        }
    }
}