using System;
using CycoAI.CycoTui.Core.Buffer;
using Buffer = CycoAI.CycoTui.Core.Buffer.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Text;
using CycoAI.CycoTui.Core.Widgets;
using Xunit;

namespace CycoAI.CycoTui.Tests.Widgets
{
    public class GaugeTests
    {
        [Fact]
        public void Gauge_Constructor_InitializesWithDefaults()
        {
            var gauge = new Gauge();

            Assert.Equal(0.0, gauge.Ratio);
            Assert.Null(gauge.Label);
            Assert.Equal(TextStyle.Default, gauge.GaugeStyle);
            Assert.Equal(TextStyle.Default, gauge.LabelStyle);
            Assert.Equal('█', gauge.FillChar);
            Assert.Equal(' ', gauge.EmptyChar);
            Assert.True(gauge.UseUnicodeBlocks);
        }

        [Fact]
        public void Gauge_Constructor_WithRatio_SetsRatio()
        {
            var gauge = new Gauge(0.75);

            Assert.Equal(0.75, gauge.Ratio);
        }

        [Fact]
        public void Gauge_Constructor_WithRatio_ClampsToValidRange()
        {
            var gaugeHigh = new Gauge(1.5);
            var gaugeLow = new Gauge(-0.5);

            Assert.Equal(1.0, gaugeHigh.Ratio);
            Assert.Equal(0.0, gaugeLow.Ratio);
        }

        [Fact]
        public void Gauge_WithRatio_CreatesInstance()
        {
            var gauge = Gauge.WithRatio(0.6);

            Assert.Equal(0.6, gauge.Ratio);
        }

        [Fact]
        public void Gauge_WithPercentage_ConvertsCorrectly()
        {
            var gauge = Gauge.WithPercentage(75);

            Assert.Equal(0.75, gauge.Ratio);
        }

        [Fact]
        public void Gauge_SetRatio_UpdatesRatio()
        {
            var gauge = new Gauge();

            gauge.SetRatio(0.8);

            Assert.Equal(0.8, gauge.Ratio);
        }

        [Fact]
        public void Gauge_SetRatio_ClampsToValidRange()
        {
            var gauge = new Gauge();

            gauge.SetRatio(1.5);
            Assert.Equal(1.0, gauge.Ratio);

            gauge.SetRatio(-0.3);
            Assert.Equal(0.0, gauge.Ratio);
        }

        [Fact]
        public void Gauge_SetPercentage_ConvertsCorrectly()
        {
            var gauge = new Gauge();

            gauge.SetPercentage(50);

            Assert.Equal(0.5, gauge.Ratio);
        }

        [Fact]
        public void Gauge_WithLabel_SetsLabel()
        {
            var gauge = new Gauge().WithLabel("Progress");

            Assert.Equal("Progress", gauge.Label);
        }

        [Fact]
        public void Gauge_WithGaugeStyle_SetsGaugeStyle()
        {
            var style = TextStyle.Default.WithForeground(Color.Green);
            var gauge = new Gauge().WithGaugeStyle(style);

            Assert.Equal(style, gauge.GaugeStyle);
        }

        [Fact]
        public void Gauge_WithLabelStyle_SetsLabelStyle()
        {
            var style = TextStyle.Default.WithForeground(Color.Blue);
            var gauge = new Gauge().WithLabelStyle(style);

            Assert.Equal(style, gauge.LabelStyle);
        }

        [Fact]
        public void Gauge_WithFillChar_SetsFillChar()
        {
            var gauge = new Gauge().WithFillChar('▓');

            Assert.Equal('▓', gauge.FillChar);
        }

        [Fact]
        public void Gauge_WithEmptyChar_SetsEmptyChar()
        {
            var gauge = new Gauge().WithEmptyChar('░');

            Assert.Equal('░', gauge.EmptyChar);
        }

        [Fact]
        public void Gauge_WithUnicodeBlocks_SetsUnicodeBlocks()
        {
            var gauge = new Gauge().WithUnicodeBlocks(false);

            Assert.False(gauge.UseUnicodeBlocks);
        }

        [Fact]
        public void Gauge_GetMinimumSize_ReturnsCorrectSize()
        {
            var gauge = new Gauge();

            var size = gauge.GetMinimumSize();

            Assert.Equal((1, 1), size);
        }

        [Fact]
        public void Gauge_Render_EmptyArea_DoesNotThrow()
        {
            var gauge = new Gauge(0.5);
            var buffer = new Buffer(10, 5);
            var emptyArea = new Rect(0, 0, 0, 0);

            // Should not throw
            gauge.Render(emptyArea, buffer);
        }

        [Fact]
        public void Gauge_Render_ZeroRatio_RendersEmpty()
        {
            var gauge = new Gauge(0.0);
            var buffer = new Buffer(10, 1);
            var area = new Rect(0, 0, 10, 1);

            gauge.Render(area, buffer);

            // All should be empty characters
            for (int x = 0; x < 10; x++)
            {
                Assert.Equal(' ', buffer.GetCellSafe(x, 0).Character);
            }
        }

        [Fact]
        public void Gauge_Render_FullRatio_RendersFull()
        {
            var gauge = new Gauge(1.0);
            var buffer = new Buffer(5, 1);
            var area = new Rect(0, 0, 5, 1);

            gauge.Render(area, buffer);

            // All should be fill characters
            for (int x = 0; x < 5; x++)
            {
                Assert.Equal('█', buffer.GetCellSafe(x, 0).Character);
            }
        }

        [Fact]
        public void Gauge_Render_HalfRatio_RendersHalf()
        {
            var gauge = new Gauge(0.5);
            var buffer = new Buffer(10, 1);
            var area = new Rect(0, 0, 10, 1);

            gauge.Render(area, buffer);

            // First half should be filled
            for (int x = 0; x < 5; x++)
            {
                Assert.Equal('█', buffer.GetCellSafe(x, 0).Character);
            }

            // Second half should be empty
            for (int x = 5; x < 10; x++)
            {
                Assert.Equal(' ', buffer.GetCellSafe(x, 0).Character);
            }
        }

        [Fact]
        public void Gauge_Render_WithLabel_RendersLabel()
        {
            var gauge = new Gauge(0.5).WithLabel("50%");
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);

            gauge.Render(area, buffer);

            // Label should be centered in the middle row
            var middleY = area.Y + area.Height / 2;
            Assert.Equal('5', buffer.GetCellSafe(3, middleY).Character); // Centered "50%"
            Assert.Equal('0', buffer.GetCellSafe(4, middleY).Character);
            Assert.Equal('%', buffer.GetCellSafe(5, middleY).Character);
        }

        [Fact]
        public void Gauge_Render_CustomFillChar_UsesCustomChar()
        {
            var gauge = new Gauge(1.0).WithFillChar('*');
            var buffer = new Buffer(3, 1);
            var area = new Rect(0, 0, 3, 1);

            gauge.Render(area, buffer);

            for (int x = 0; x < 3; x++)
            {
                Assert.Equal('*', buffer.GetCellSafe(x, 0).Character);
            }
        }

        [Fact]
        public void Gauge_Render_CustomEmptyChar_UsesCustomChar()
        {
            var gauge = new Gauge(0.0).WithEmptyChar('.');
            var buffer = new Buffer(3, 1);
            var area = new Rect(0, 0, 3, 1);

            gauge.Render(area, buffer);

            for (int x = 0; x < 3; x++)
            {
                Assert.Equal('.', buffer.GetCellSafe(x, 0).Character);
            }
        }

        [Fact]
        public void Gauge_Render_MultipleRows_FillsAllRows()
        {
            var gauge = new Gauge(0.5);
            var buffer = new Buffer(4, 3);
            var area = new Rect(0, 0, 4, 3);

            gauge.Render(area, buffer);

            // All rows should have the same pattern
            for (int y = 0; y < 3; y++)
            {
                Assert.Equal('█', buffer.GetCellSafe(0, y).Character);
                Assert.Equal('█', buffer.GetCellSafe(1, y).Character);
                Assert.Equal(' ', buffer.GetCellSafe(2, y).Character);
                Assert.Equal(' ', buffer.GetCellSafe(3, y).Character);
            }
        }

        [Fact]
        public void Gauge_GetPercentage_ReturnsCorrectValue()
        {
            var gauge = new Gauge(0.75);

            var percentage = gauge.GetPercentage();

            Assert.Equal(75.0, percentage);
        }

        [Fact]
        public void Gauge_GetFormattedPercentage_ReturnsFormattedString()
        {
            var gauge = new Gauge(0.755);

            var formatted = gauge.GetFormattedPercentage();
            var customFormatted = gauge.GetFormattedPercentage("F0");

            Assert.Equal("75.5%", formatted);
            Assert.Equal("76%", customFormatted);
        }

        [Theory]
        [InlineData(0.0, 0)]
        [InlineData(0.25, 2)]
        [InlineData(0.5, 4)]
        [InlineData(0.75, 6)]
        [InlineData(1.0, 8)]
        public void Gauge_Render_VariousRatios_RendersCorrectWidth(double ratio, int expectedFillWidth)
        {
            var gauge = new Gauge(ratio);
            var buffer = new Buffer(8, 1);
            var area = new Rect(0, 0, 8, 1);

            gauge.Render(area, buffer);

            // Count filled characters
            int fillCount = 0;
            for (int x = 0; x < 8; x++)
            {
                if (buffer.GetCellSafe(x, 0).Character == '█')
                    fillCount++;
            }

            Assert.Equal(expectedFillWidth, fillCount);
        }

        [Fact]
        public void Gauge_Render_GaugeStyle_AppliesStyle()
        {
            var style = TextStyle.Default.WithForeground(Color.Green).WithBackground(Color.Red);
            var gauge = new Gauge(1.0).WithGaugeStyle(style);
            var buffer = new Buffer(3, 1);
            var area = new Rect(0, 0, 3, 1);

            gauge.Render(area, buffer);

            var cell = buffer.GetCellSafe(0, 0);
            Assert.Equal('█', cell.Character);
            Assert.Equal(Color.Green, cell.Foreground);
            Assert.Equal(Color.Red, cell.Background);
        }

        [Fact]
        public void Gauge_Render_PartialArea_RendersCorrectly()
        {
            var gauge = new Gauge(1.0);
            var buffer = new Buffer(10, 10);
            var area = new Rect(2, 3, 4, 2); // Offset area

            gauge.Render(area, buffer);

            // Check that only the specified area is filled
            for (int y = 3; y < 5; y++)
            {
                for (int x = 2; x < 6; x++)
                {
                    Assert.Equal('█', buffer.GetCellSafe(x, y).Character);
                }
            }

            // Areas outside should remain empty
            Assert.Equal(' ', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 3).Character);
            Assert.Equal(' ', buffer.GetCellSafe(6, 3).Character);
        }
    }
}