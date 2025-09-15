using System;
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
    /// A widget that displays a horizontal progress gauge with customizable styling.
    /// </summary>
    public class Gauge : Widget
    {
        /// <summary>
        /// Gets or sets the progress ratio (0.0 to 1.0).
        /// </summary>
        public double Ratio { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the label displayed on the gauge.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the style applied to the gauge.
        /// </summary>
        public TextStyle GaugeStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets the style applied to the label.
        /// </summary>
        public TextStyle LabelStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets the character used to fill the progress area.
        /// </summary>
        public char FillChar { get; set; } = '█';

        /// <summary>
        /// Gets or sets the character used for the empty area.
        /// </summary>
        public char EmptyChar { get; set; } = ' ';

        /// <summary>
        /// Gets or sets a value indicating whether to use Unicode block characters for smoother progress display.
        /// </summary>
        public bool UseUnicodeBlocks { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gauge"/> class.
        /// </summary>
        public Gauge()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gauge"/> class with the specified ratio.
        /// </summary>
        /// <param name="ratio">The progress ratio (0.0 to 1.0).</param>
        public Gauge(double ratio)
        {
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Ratio = Compat.Clamp(ratio, 0.0, 1.0);
#else
            Ratio = Math.Clamp(ratio, 0.0, 1.0);
#endif
        }

        /// <summary>
        /// Creates a new gauge with the specified ratio.
        /// </summary>
        /// <param name="ratio">The progress ratio (0.0 to 1.0).</param>
        /// <returns>A new gauge instance.</returns>
        public static Gauge WithRatio(double ratio) => new Gauge(ratio);

        /// <summary>
        /// Creates a new gauge with the specified percentage.
        /// </summary>
        /// <param name="percentage">The progress percentage (0 to 100).</param>
        /// <returns>A new gauge instance.</returns>
        public static Gauge WithPercentage(double percentage) => new Gauge(percentage / 100.0);

        /// <summary>
        /// Sets the progress ratio for this gauge.
        /// </summary>
        /// <param name="ratio">The progress ratio (0.0 to 1.0).</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge SetRatio(double ratio)
        {
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Ratio = Compat.Clamp(ratio, 0.0, 1.0);
#else
            Ratio = Math.Clamp(ratio, 0.0, 1.0);
#endif
            return this;
        }

        /// <summary>
        /// Sets the progress percentage for this gauge.
        /// </summary>
        /// <param name="percentage">The progress percentage (0 to 100).</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge SetPercentage(double percentage)
        {
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Ratio = Compat.Clamp(percentage / 100.0, 0.0, 1.0);
#else
            Ratio = Math.Clamp(percentage / 100.0, 0.0, 1.0);
#endif
            return this;
        }

        /// <summary>
        /// Sets the label for this gauge.
        /// </summary>
        /// <param name="label">The label to display.</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge WithLabel(string label)
        {
            Label = label;
            return this;
        }

        /// <summary>
        /// Sets the gauge style for this gauge.
        /// </summary>
        /// <param name="style">The style to apply to the gauge.</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge WithGaugeStyle(TextStyle style)
        {
            GaugeStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the label style for this gauge.
        /// </summary>
        /// <param name="style">The style to apply to the label.</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge WithLabelStyle(TextStyle style)
        {
            LabelStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the fill character for this gauge.
        /// </summary>
        /// <param name="fillChar">The character used to fill the progress area.</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge WithFillChar(char fillChar)
        {
            FillChar = fillChar;
            return this;
        }

        /// <summary>
        /// Sets the empty character for this gauge.
        /// </summary>
        /// <param name="emptyChar">The character used for the empty area.</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge WithEmptyChar(char emptyChar)
        {
            EmptyChar = emptyChar;
            return this;
        }

        /// <summary>
        /// Enables or disables Unicode block characters for smoother progress display.
        /// </summary>
        /// <param name="useUnicode">Whether to use Unicode block characters.</param>
        /// <returns>This gauge instance for method chaining.</returns>
        public Gauge WithUnicodeBlocks(bool useUnicode)
        {
            UseUnicodeBlocks = useUnicode;
            return this;
        }

        /// <inheritdoc />
        public override (int Width, int Height) GetMinimumSize() => (1, 1);

        /// <inheritdoc />
        public override void Render(Rect area, IBuffer buffer)
        {
            if (area.IsEmpty)
                return;

#if NETSTANDARD2_0 || NETSTANDARD2_1
            var clampedRatio = Compat.Clamp(Ratio, 0.0, 1.0);
#else
            var clampedRatio = Math.Clamp(Ratio, 0.0, 1.0);
#endif

            // Calculate the filled width
            double exactFilledWidth = area.Width * clampedRatio;
            int filledWidth = (int)exactFilledWidth;
            double fractionalPart = exactFilledWidth - filledWidth;

            // Render each row of the gauge
            for (int y = area.Y; y < area.Y + area.Height; y++)
            {
                for (int x = area.X; x < area.X + area.Width; x++)
                {
                    var relativeX = x - area.X;
                    char character;
                    var style = GaugeStyle;

                    if (relativeX < filledWidth)
                    {
                        // Fully filled area
                        character = FillChar;
                    }
                    else if (relativeX == filledWidth && UseUnicodeBlocks && fractionalPart > 0)
                    {
                        // Partial fill using Unicode block characters
                        character = GetPartialBlockCharacter(fractionalPart);
                    }
                    else
                    {
                        // Empty area
                        character = EmptyChar;
                    }

                    buffer.SetCellSafe(x, y, new Cell(character, style.Foreground, style.Background, style.Modifiers));
                }
            }

            // Render label if present and area is large enough
            if (!string.IsNullOrEmpty(Label) && area.Height > 0)
            {
                RenderLabel(area, buffer);
            }
        }

        private char GetPartialBlockCharacter(double fraction)
        {
            // Unicode block characters for partial fills
            if (fraction >= 0.875) return '▉';  // 7/8 block
            if (fraction >= 0.75) return '▊';   // 3/4 block
            if (fraction >= 0.625) return '▋';  // 5/8 block
            if (fraction >= 0.5) return '▌';    // 1/2 block
            if (fraction >= 0.375) return '▍';  // 3/8 block
            if (fraction >= 0.25) return '▎';   // 1/4 block
            if (fraction >= 0.125) return '▏';  // 1/8 block
            return EmptyChar;
        }

        private void RenderLabel(Rect area, IBuffer buffer)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            // Use the center row for the label
            var labelY = area.Y + area.Height / 2;
            var availableWidth = area.Width;

            // Truncate label if too long
            var displayLabel = Label.Length > availableWidth ? Label.Substring(0, availableWidth) : Label;

            // Center the label horizontally
            var startX = area.X + (availableWidth - displayLabel.Length) / 2;

            // Render label characters
            for (int i = 0; i < displayLabel.Length; i++)
            {
                var x = startX + i;
                if (x >= area.X && x < area.X + area.Width)
                {
                    buffer.SetCellSafe(x, labelY, new Cell(
                        displayLabel[i],
                        LabelStyle.Foreground,
                        LabelStyle.Background,
                        LabelStyle.Modifiers));
                }
            }
        }

        /// <summary>
        /// Gets the current progress as a percentage.
        /// </summary>
        /// <returns>The progress percentage (0 to 100).</returns>
        public double GetPercentage() => Ratio * 100.0;

        /// <summary>
        /// Gets a formatted percentage string.
        /// </summary>
        /// <param name="format">The format string (default: "F1").</param>
        /// <returns>A formatted percentage string.</returns>
        public string GetFormattedPercentage(string format = "F1")
        {
            return $"{GetPercentage().ToString(format)}%";
        }
    }
}