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
    /// Represents a single bar in a bar chart.
    /// </summary>
    public readonly struct BarData : IEquatable<BarData>
    {
        /// <summary>
        /// Gets the label for this bar.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Gets the value for this bar.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Gets the style applied to this bar.
        /// </summary>
        public TextStyle Style { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarData"/> struct.
        /// </summary>
        /// <param name="label">The label for the bar.</param>
        /// <param name="value">The value for the bar.</param>
        /// <param name="style">The style to apply to the bar.</param>
        public BarData(string label, double value, TextStyle style = default)
        {
            Label = label ?? string.Empty;
            Value = value;
            Style = style;
        }

        /// <inheritdoc />
        public bool Equals(BarData other) =>
            Label == other.Label && Value.Equals(other.Value) && Style.Equals(other.Style);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is BarData other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Label, Value, Style);
#else
            HashCode.Combine(Label, Value, Style);
#endif

        /// <summary>
        /// Determines whether two bar data instances are equal.
        /// </summary>
        public static bool operator ==(BarData left, BarData right) => left.Equals(right);

        /// <summary>
        /// Determines whether two bar data instances are not equal.
        /// </summary>
        public static bool operator !=(BarData left, BarData right) => !(left == right);
    }

    /// <summary>
    /// Specifies the direction of bars in a bar chart.
    /// </summary>
    public enum BarDirection
    {
        /// <summary>
        /// Vertical bars extending upward.
        /// </summary>
        Vertical,

        /// <summary>
        /// Horizontal bars extending rightward.
        /// </summary>
        Horizontal
    }

    /// <summary>
    /// A widget that displays data as a bar chart with customizable styling and layout.
    /// </summary>
    public class BarChart : Widget
    {
        private readonly List<BarData> _data;

        /// <summary>
        /// Gets or sets the direction of the bars.
        /// </summary>
        public BarDirection Direction { get; set; } = BarDirection.Vertical;

        /// <summary>
        /// Gets or sets the character used to render the bars.
        /// </summary>
        public char BarChar { get; set; } = '█';

        /// <summary>
        /// Gets or sets the maximum value for scaling. If null, uses the maximum value from the data.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the default style applied to bars.
        /// </summary>
        public TextStyle BarStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets the style applied to bar labels.
        /// </summary>
        public TextStyle LabelStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets a value indicating whether to show bar values.
        /// </summary>
        public bool ShowValues { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show bar labels.
        /// </summary>
        public bool ShowLabels { get; set; } = true;

        /// <summary>
        /// Gets or sets the width of each bar (for vertical charts) or height (for horizontal charts).
        /// </summary>
        public int BarSize { get; set; } = 1;

        /// <summary>
        /// Gets or sets the gap between bars.
        /// </summary>
        public int BarGap { get; set; } = 1;

        /// <summary>
        /// Gets the data displayed in this bar chart.
        /// </summary>
        public IReadOnlyList<BarData> Data => _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarChart"/> class.
        /// </summary>
        public BarChart()
        {
            _data = new List<BarData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarChart"/> class with the specified data.
        /// </summary>
        /// <param name="data">The bar data to display.</param>
        public BarChart(IEnumerable<BarData> data)
        {
            _data = new List<BarData>(data ?? throw new ArgumentNullException(nameof(data)));
        }

        /// <summary>
        /// Creates a new bar chart with the specified data.
        /// </summary>
        /// <param name="data">The bar data to display.</param>
        /// <returns>A new bar chart instance.</returns>
        public static BarChart WithData(params BarData[] data) => new BarChart(data);

        /// <summary>
        /// Creates a new bar chart with simple value data.
        /// </summary>
        /// <param name="values">The values to display as bars.</param>
        /// <returns>A new bar chart instance.</returns>
        public static BarChart WithValues(params double[] values) =>
            new BarChart(values.Select((v, i) => new BarData($"Bar {i + 1}", v)));

        /// <summary>
        /// Adds a bar to this chart.
        /// </summary>
        /// <param name="bar">The bar data to add.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart AddBar(BarData bar)
        {
            _data.Add(bar);
            return this;
        }

        /// <summary>
        /// Adds a bar to this chart.
        /// </summary>
        /// <param name="label">The label for the bar.</param>
        /// <param name="value">The value for the bar.</param>
        /// <param name="style">The style for the bar.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart AddBar(string label, double value, TextStyle style = default)
        {
            _data.Add(new BarData(label, value, style));
            return this;
        }

        /// <summary>
        /// Clears all bars from this chart.
        /// </summary>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart Clear()
        {
            _data.Clear();
            return this;
        }

        /// <summary>
        /// Sets the direction for this chart.
        /// </summary>
        /// <param name="direction">The direction to set.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithDirection(BarDirection direction)
        {
            Direction = direction;
            return this;
        }

        /// <summary>
        /// Sets the bar character for this chart.
        /// </summary>
        /// <param name="barChar">The character to use for bars.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithBarChar(char barChar)
        {
            BarChar = barChar;
            return this;
        }

        /// <summary>
        /// Sets the maximum value for scaling.
        /// </summary>
        /// <param name="maxValue">The maximum value to use for scaling.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithMaxValue(double maxValue)
        {
            MaxValue = maxValue;
            return this;
        }

        /// <summary>
        /// Sets the bar style for this chart.
        /// </summary>
        /// <param name="style">The style to apply to bars.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithBarStyle(TextStyle style)
        {
            BarStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the label style for this chart.
        /// </summary>
        /// <param name="style">The style to apply to labels.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithLabelStyle(TextStyle style)
        {
            LabelStyle = style;
            return this;
        }

        /// <summary>
        /// Configures whether to show values on bars.
        /// </summary>
        /// <param name="show">Whether to show values.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithShowValues(bool show)
        {
            ShowValues = show;
            return this;
        }

        /// <summary>
        /// Configures whether to show labels on bars.
        /// </summary>
        /// <param name="show">Whether to show labels.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithShowLabels(bool show)
        {
            ShowLabels = show;
            return this;
        }

        /// <summary>
        /// Sets the bar size and gap.
        /// </summary>
        /// <param name="size">The size of each bar.</param>
        /// <param name="gap">The gap between bars.</param>
        /// <returns>This bar chart instance for method chaining.</returns>
        public BarChart WithBarDimensions(int size, int gap)
        {
            BarSize = Math.Max(1, size);
            BarGap = Math.Max(0, gap);
            return this;
        }

        /// <inheritdoc />
        public override (int Width, int Height) GetMinimumSize()
        {
            if (_data.Count == 0)
                return (1, 1);

            if (Direction == BarDirection.Vertical)
            {
                var width = _data.Count * (BarSize + BarGap) - BarGap;
                var height = ShowLabels ? 2 : 1;
                return (Math.Max(1, width), height);
            }
            else
            {
                var width = ShowLabels ? 10 : 1; // Minimum space for labels
                var height = _data.Count * (BarSize + BarGap) - BarGap;
                return (width, Math.Max(1, height));
            }
        }

        /// <inheritdoc />
        public override void Render(Rect area, IBuffer buffer)
        {
            if (area.IsEmpty || _data.Count == 0)
                return;

            var maxValue = GetEffectiveMaxValue();
            if (maxValue <= 0)
                return;

            if (Direction == BarDirection.Vertical)
            {
                RenderVerticalBars(area, buffer, maxValue);
            }
            else
            {
                RenderHorizontalBars(area, buffer, maxValue);
            }
        }

        private double GetEffectiveMaxValue()
        {
            if (MaxValue.HasValue && MaxValue.Value > 0)
                return MaxValue.Value;

            if (_data.Count == 0)
                return 1.0;

            var max = _data.Max(d => d.Value);
            return max > 0 ? max : 1.0;
        }

        private void RenderVerticalBars(Rect area, IBuffer buffer, double maxValue)
        {
            var availableHeight = ShowLabels ? area.Height - 1 : area.Height;
            var availableWidth = area.Width;
            var totalBarWidth = _data.Count * BarSize + (_data.Count - 1) * BarGap;

            if (totalBarWidth > availableWidth || availableHeight <= 0)
                return;

            var startX = area.X + (availableWidth - totalBarWidth) / 2;

            for (int i = 0; i < _data.Count; i++)
            {
                var bar = _data[i];
                var barX = startX + i * (BarSize + BarGap);
                var barHeight = (int)Math.Round((bar.Value / maxValue) * availableHeight);
                var effectiveStyle = bar.Style.Equals(TextStyle.Default) ? BarStyle : bar.Style;

                // Render bar
                for (int y = 0; y < barHeight; y++)
                {
                    var renderY = area.Y + availableHeight - 1 - y;
                    for (int x = 0; x < BarSize && barX + x < area.X + area.Width; x++)
                    {
                        buffer.SetCellSafe(barX + x, renderY, new Cell(
                            BarChar,
                            effectiveStyle.Foreground,
                            effectiveStyle.Background,
                            effectiveStyle.Modifiers));
                    }
                }

                // Render value if enabled
                if (ShowValues && barHeight > 0)
                {
                    var valueStr = bar.Value.ToString("0.#");
                    var valueY = area.Y + availableHeight - barHeight - 1;
                    if (valueY >= area.Y)
                    {
                        RenderText(valueStr, barX, valueY, BarSize, buffer, effectiveStyle);
                    }
                }

                // Render label if enabled
                if (ShowLabels && !string.IsNullOrEmpty(bar.Label))
                {
                    var labelY = area.Y + area.Height - 1;
                    RenderText(bar.Label, barX, labelY, BarSize, buffer, LabelStyle);
                }
            }
        }

        private void RenderHorizontalBars(Rect area, IBuffer buffer, double maxValue)
        {
            var labelWidth = ShowLabels ? GetMaxLabelWidth() : 0;
            var availableWidth = area.Width - labelWidth;
            var availableHeight = area.Height;
            var totalBarHeight = _data.Count * BarSize + (_data.Count - 1) * BarGap;

            if (totalBarHeight > availableHeight || availableWidth <= 0)
                return;

            var startY = area.Y + (availableHeight - totalBarHeight) / 2;

            for (int i = 0; i < _data.Count; i++)
            {
                var bar = _data[i];
                var barY = startY + i * (BarSize + BarGap);
                var barWidth = (int)Math.Round((bar.Value / maxValue) * availableWidth);
                var effectiveStyle = bar.Style.Equals(TextStyle.Default) ? BarStyle : bar.Style;

                // Render label if enabled
                if (ShowLabels && !string.IsNullOrEmpty(bar.Label))
                {
                    RenderText(bar.Label, area.X, barY, labelWidth, buffer, LabelStyle);
                }

                // Render bar
                var barStartX = area.X + labelWidth;
                for (int x = 0; x < barWidth; x++)
                {
                    for (int y = 0; y < BarSize && barY + y < area.Y + area.Height; y++)
                    {
                        buffer.SetCellSafe(barStartX + x, barY + y, new Cell(
                            BarChar,
                            effectiveStyle.Foreground,
                            effectiveStyle.Background,
                            effectiveStyle.Modifiers));
                    }
                }

                // Render value if enabled
                if (ShowValues)
                {
                    var valueStr = bar.Value.ToString("0.#");
                    var valueX = barStartX + barWidth + 1;
                    if (valueX < area.X + area.Width)
                    {
                        RenderText(valueStr, valueX, barY, area.X + area.Width - valueX, buffer, effectiveStyle);
                    }
                }
            }
        }

        private int GetMaxLabelWidth()
        {
            if (!ShowLabels || _data.Count == 0)
                return 0;

            return _data.Max(d => d.Label?.Length ?? 0) + 1; // Add 1 for spacing
        }

        private void RenderText(string text, int x, int y, int maxWidth, IBuffer buffer, TextStyle style)
        {
            if (string.IsNullOrEmpty(text) || maxWidth <= 0)
                return;

            var displayText = text.Length > maxWidth ? text.Substring(0, maxWidth) : text;
            for (int i = 0; i < displayText.Length; i++)
            {
                if (x + i >= 0 && x + i < buffer.Width && y >= 0 && y < buffer.Height)
                {
                    buffer.SetCellSafe(x + i, y, new Cell(
                        displayText[i],
                        style.Foreground,
                        style.Background,
                        style.Modifiers));
                }
            }
        }
    }
}