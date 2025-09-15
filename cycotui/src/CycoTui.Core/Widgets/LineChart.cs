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
    /// Represents a dataset for a line chart.
    /// </summary>
    public readonly struct Dataset : IEquatable<Dataset>
    {
        /// <summary>
        /// Gets the name of this dataset.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the data points for this dataset.
        /// </summary>
        public IReadOnlyList<(double X, double Y)> Data { get; }

        /// <summary>
        /// Gets the style applied to this dataset.
        /// </summary>
        public TextStyle Style { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dataset"/> struct.
        /// </summary>
        /// <param name="name">The name of the dataset.</param>
        /// <param name="data">The data points.</param>
        /// <param name="style">The style to apply.</param>
        public Dataset(string name, IEnumerable<(double X, double Y)> data, TextStyle style = default)
        {
            Name = name ?? string.Empty;
            Data = data?.ToList() ?? new List<(double X, double Y)>();
            Style = style;
        }

        /// <summary>
        /// Creates a dataset with Y values only (X values will be indices).
        /// </summary>
        /// <param name="name">The name of the dataset.</param>
        /// <param name="yValues">The Y values.</param>
        /// <param name="style">The style to apply.</param>
        /// <returns>A new dataset.</returns>
        public static Dataset FromYValues(string name, IEnumerable<double> yValues, TextStyle style = default)
        {
            var data = yValues.Select((y, i) => ((double)i, y));
            return new Dataset(name, data, style);
        }

        /// <inheritdoc />
        public bool Equals(Dataset other) =>
            Name == other.Name && Data.SequenceEqual(other.Data) && Style.Equals(other.Style);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Dataset other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Name, Data, Style);
#else
            HashCode.Combine(Name, Data, Style);
#endif

        /// <summary>
        /// Determines whether two datasets are equal.
        /// </summary>
        public static bool operator ==(Dataset left, Dataset right) => left.Equals(right);

        /// <summary>
        /// Determines whether two datasets are not equal.
        /// </summary>
        public static bool operator !=(Dataset left, Dataset right) => !(left == right);
    }

    /// <summary>
    /// A widget that displays data as line charts with multiple datasets and customizable styling.
    /// </summary>
    public class LineChart : Widget
    {
        private readonly List<Dataset> _datasets;

        /// <summary>
        /// Gets or sets the X-axis bounds. If null, bounds are calculated from data.
        /// </summary>
        public (double Min, double Max)? XBounds { get; set; }

        /// <summary>
        /// Gets or sets the Y-axis bounds. If null, bounds are calculated from data.
        /// </summary>
        public (double Min, double Max)? YBounds { get; set; }

        /// <summary>
        /// Gets or sets the style applied to axes.
        /// </summary>
        public TextStyle AxisStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets the style applied to labels.
        /// </summary>
        public TextStyle LabelStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets a value indicating whether to show axes.
        /// </summary>
        public bool ShowAxes { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show labels.
        /// </summary>
        public bool ShowLabels { get; set; } = true;

        /// <summary>
        /// Gets or sets the character used for plotting points.
        /// </summary>
        public char PointChar { get; set; } = '•';

        /// <summary>
        /// Gets or sets the character used for drawing lines.
        /// </summary>
        public char LineChar { get; set; } = '─';

        /// <summary>
        /// Gets or sets the X-axis label.
        /// </summary>
        public string? XLabel { get; set; }

        /// <summary>
        /// Gets or sets the Y-axis label.
        /// </summary>
        public string? YLabel { get; set; }

        /// <summary>
        /// Gets the datasets displayed in this line chart.
        /// </summary>
        public IReadOnlyList<Dataset> Datasets => _datasets;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineChart"/> class.
        /// </summary>
        public LineChart()
        {
            _datasets = new List<Dataset>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineChart"/> class with the specified datasets.
        /// </summary>
        /// <param name="datasets">The datasets to display.</param>
        public LineChart(IEnumerable<Dataset> datasets)
        {
            _datasets = new List<Dataset>(datasets ?? throw new ArgumentNullException(nameof(datasets)));
        }

        /// <summary>
        /// Creates a new line chart with the specified datasets.
        /// </summary>
        /// <param name="datasets">The datasets to display.</param>
        /// <returns>A new line chart instance.</returns>
        public static LineChart WithDatasets(params Dataset[] datasets) => new LineChart(datasets);

        /// <summary>
        /// Creates a simple line chart with Y values only.
        /// </summary>
        /// <param name="name">The dataset name.</param>
        /// <param name="yValues">The Y values.</param>
        /// <returns>A new line chart instance.</returns>
        public static LineChart WithYValues(string name, params double[] yValues) =>
            new LineChart(new[] { Dataset.FromYValues(name, yValues) });

        /// <summary>
        /// Adds a dataset to this chart.
        /// </summary>
        /// <param name="dataset">The dataset to add.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart AddDataset(Dataset dataset)
        {
            _datasets.Add(dataset);
            return this;
        }

        /// <summary>
        /// Adds a dataset to this chart.
        /// </summary>
        /// <param name="name">The dataset name.</param>
        /// <param name="data">The data points.</param>
        /// <param name="style">The style to apply.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart AddDataset(string name, IEnumerable<(double X, double Y)> data, TextStyle style = default)
        {
            _datasets.Add(new Dataset(name, data, style));
            return this;
        }

        /// <summary>
        /// Clears all datasets from this chart.
        /// </summary>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart Clear()
        {
            _datasets.Clear();
            return this;
        }

        /// <summary>
        /// Sets the X-axis bounds for this chart.
        /// </summary>
        /// <param name="min">The minimum X value.</param>
        /// <param name="max">The maximum X value.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithXBounds(double min, double max)
        {
            XBounds = (min, max);
            return this;
        }

        /// <summary>
        /// Sets the Y-axis bounds for this chart.
        /// </summary>
        /// <param name="min">The minimum Y value.</param>
        /// <param name="max">The maximum Y value.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithYBounds(double min, double max)
        {
            YBounds = (min, max);
            return this;
        }

        /// <summary>
        /// Sets the axis style for this chart.
        /// </summary>
        /// <param name="style">The style to apply to axes.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithAxisStyle(TextStyle style)
        {
            AxisStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the label style for this chart.
        /// </summary>
        /// <param name="style">The style to apply to labels.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithLabelStyle(TextStyle style)
        {
            LabelStyle = style;
            return this;
        }

        /// <summary>
        /// Configures whether to show axes.
        /// </summary>
        /// <param name="show">Whether to show axes.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithShowAxes(bool show)
        {
            ShowAxes = show;
            return this;
        }

        /// <summary>
        /// Configures whether to show labels.
        /// </summary>
        /// <param name="show">Whether to show labels.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithShowLabels(bool show)
        {
            ShowLabels = show;
            return this;
        }

        /// <summary>
        /// Sets the characters used for plotting.
        /// </summary>
        /// <param name="pointChar">The character for points.</param>
        /// <param name="lineChar">The character for lines.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithChars(char pointChar, char lineChar)
        {
            PointChar = pointChar;
            LineChar = lineChar;
            return this;
        }

        /// <summary>
        /// Sets the axis labels.
        /// </summary>
        /// <param name="xLabel">The X-axis label.</param>
        /// <param name="yLabel">The Y-axis label.</param>
        /// <returns>This line chart instance for method chaining.</returns>
        public LineChart WithLabels(string? xLabel, string? yLabel)
        {
            XLabel = xLabel;
            YLabel = yLabel;
            return this;
        }

        /// <inheritdoc />
        public override (int Width, int Height) GetMinimumSize()
        {
            var width = ShowAxes ? 3 : 1; // Y-axis + content
            var height = ShowAxes ? 3 : 1; // X-axis + content
            return (width, height);
        }

        /// <inheritdoc />
        public override void Render(Rect area, IBuffer buffer)
        {
            if (area.IsEmpty || _datasets.Count == 0)
                return;

            var (xBounds, yBounds) = GetEffectiveBounds();
            if (xBounds.Max <= xBounds.Min || yBounds.Max <= yBounds.Min)
                return;

            var plotArea = GetPlotArea(area);
            if (plotArea.IsEmpty)
                return;

            // Render axes
            if (ShowAxes)
            {
                RenderAxes(area, buffer);
            }

            // Render datasets
            foreach (var dataset in _datasets)
            {
                RenderDataset(dataset, plotArea, buffer, xBounds, yBounds);
            }

            // Render labels
            if (ShowLabels)
            {
                RenderLabels(area, buffer);
            }
        }

        private ((double Min, double Max) X, (double Min, double Max) Y) GetEffectiveBounds()
        {
            var xBounds = XBounds ?? CalculateXBounds();
            var yBounds = YBounds ?? CalculateYBounds();
            return (xBounds, yBounds);
        }

        private (double Min, double Max) CalculateXBounds()
        {
            if (_datasets.Count == 0 || !_datasets.Any(d => d.Data.Count > 0))
                return (0, 1);

            var allX = _datasets.SelectMany(d => d.Data.Select(p => p.X));
            var min = allX.Min();
            var max = allX.Max();

            // Add some padding
            var range = max - min;
            if (range == 0) range = 1;
            return (min - range * 0.05, max + range * 0.05);
        }

        private (double Min, double Max) CalculateYBounds()
        {
            if (_datasets.Count == 0 || !_datasets.Any(d => d.Data.Count > 0))
                return (0, 1);

            var allY = _datasets.SelectMany(d => d.Data.Select(p => p.Y));
            var min = allY.Min();
            var max = allY.Max();

            // Add some padding
            var range = max - min;
            if (range == 0) range = 1;
            return (min - range * 0.1, max + range * 0.1);
        }

        private Rect GetPlotArea(Rect area)
        {
            var x = area.X;
            var y = area.Y;
            var width = area.Width;
            var height = area.Height;

            if (ShowAxes)
            {
                x += 1; // Y-axis
                width = Math.Max(0, width - 1);
                height = Math.Max(0, height - 1); // X-axis
            }

            return new Rect(x, y, width, height);
        }

        private void RenderAxes(Rect area, IBuffer buffer)
        {
            // Y-axis (vertical line on the left)
            for (int y = area.Y; y < area.Y + area.Height - 1; y++)
            {
                buffer.SetCellSafe(area.X, y, new Cell('│', AxisStyle.Foreground, AxisStyle.Background, AxisStyle.Modifiers));
            }

            // X-axis (horizontal line on the bottom)
            for (int x = area.X; x < area.X + area.Width; x++)
            {
                buffer.SetCellSafe(x, area.Y + area.Height - 1, new Cell('─', AxisStyle.Foreground, AxisStyle.Background, AxisStyle.Modifiers));
            }

            // Origin
            buffer.SetCellSafe(area.X, area.Y + area.Height - 1, new Cell('└', AxisStyle.Foreground, AxisStyle.Background, AxisStyle.Modifiers));
        }

        private void RenderDataset(Dataset dataset, Rect plotArea, IBuffer buffer,
            (double Min, double Max) xBounds, (double Min, double Max) yBounds)
        {
            if (dataset.Data.Count == 0 || plotArea.IsEmpty)
                return;

            var effectiveStyle = dataset.Style.Equals(TextStyle.Default) ? AxisStyle : dataset.Style;
            var points = new List<(int X, int Y)>();

            // Convert data points to screen coordinates
            foreach (var (dataX, dataY) in dataset.Data)
            {
                var screenX = (int)Math.Round((dataX - xBounds.Min) / (xBounds.Max - xBounds.Min) * (plotArea.Width - 1));
                var screenY = (int)Math.Round((1.0 - (dataY - yBounds.Min) / (yBounds.Max - yBounds.Min)) * (plotArea.Height - 1));

                screenX = Math.Max(0, Math.Min(plotArea.Width - 1, screenX));
                screenY = Math.Max(0, Math.Min(plotArea.Height - 1, screenY));

                points.Add((plotArea.X + screenX, plotArea.Y + screenY));
            }

            // Draw lines between points
            for (int i = 0; i < points.Count - 1; i++)
            {
                DrawLine(points[i], points[i + 1], buffer, effectiveStyle);
            }

            // Draw points
            foreach (var point in points)
            {
                buffer.SetCellSafe(point.X, point.Y, new Cell(PointChar, effectiveStyle.Foreground, effectiveStyle.Background, effectiveStyle.Modifiers));
            }
        }

        private void DrawLine((int X, int Y) from, (int X, int Y) to, IBuffer buffer, TextStyle style)
        {
            var dx = Math.Abs(to.X - from.X);
            var dy = Math.Abs(to.Y - from.Y);
            var sx = from.X < to.X ? 1 : -1;
            var sy = from.Y < to.Y ? 1 : -1;
            var err = dx - dy;

            var x = from.X;
            var y = from.Y;

            while (true)
            {
                // Use appropriate line character based on direction
                var character = dx > dy ? '─' : '│';
                if (dx == dy) character = dx > 0 ? '\\' : '/';

                if (x >= 0 && x < buffer.Width && y >= 0 && y < buffer.Height)
                {
                    var existing = buffer.GetCellSafe(x, y);
                    if (existing.Character == ' ')
                    {
                        buffer.SetCellSafe(x, y, new Cell(character, style.Foreground, style.Background, style.Modifiers));
                    }
                }

                if (x == to.X && y == to.Y) break;

                var e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }

        private void RenderLabels(Rect area, IBuffer buffer)
        {
            // X-axis label
            if (!string.IsNullOrEmpty(XLabel))
            {
                var xLabelX = area.X + (area.Width - XLabel.Length) / 2;
                var xLabelY = area.Y + area.Height - 1;
                RenderText(XLabel, xLabelX, xLabelY, area.Width, buffer, LabelStyle);
            }

            // Y-axis label (vertical)
            if (!string.IsNullOrEmpty(YLabel))
            {
                var yLabelX = area.X;
                var yLabelStartY = area.Y + (area.Height - YLabel.Length) / 2;
                for (int i = 0; i < YLabel.Length && yLabelStartY + i < area.Y + area.Height; i++)
                {
                    if (yLabelStartY + i >= area.Y)
                    {
                        buffer.SetCellSafe(yLabelX, yLabelStartY + i, new Cell(
                            YLabel[i],
                            LabelStyle.Foreground,
                            LabelStyle.Background,
                            LabelStyle.Modifiers));
                    }
                }
            }
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