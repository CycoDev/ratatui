using System;
using CycoAI.CycoTui.Core.Buffer;
using Buffer = CycoAI.CycoTui.Core.Buffer.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Widgets;
using Xunit;

namespace CycoAI.CycoTui.Tests.Widgets
{
    public class WidgetTests
    {
        /// <summary>
        /// Test widget implementation for testing base widget functionality.
        /// </summary>
        private class TestWidget : Widget
        {
            public bool RenderCalled { get; private set; }
            public Rect LastRenderArea { get; private set; }
            public IBuffer? LastRenderBuffer { get; private set; }
            public (int Width, int Height) MinSize { get; set; } = (0, 0);

            public override void Render(Rect area, IBuffer buffer)
            {
                RenderCalled = true;
                LastRenderArea = area;
                LastRenderBuffer = buffer;

                // Simple test rendering - fill with 'T'
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    for (int x = area.X; x < area.X + area.Width; x++)
                    {
                        buffer.SetCellSafe(x, y, new Cell('T'));
                    }
                }
            }

            public override (int Width, int Height) GetMinimumSize() => MinSize;
        }

        /// <summary>
        /// Test stateful widget implementation.
        /// </summary>
        private class TestStatefulWidget : StatefulWidget<int>
        {
            public bool RenderCalled { get; private set; }
            public Rect LastRenderArea { get; private set; }
            public IBuffer? LastRenderBuffer { get; private set; }
            public int LastState { get; private set; }

            public override void Render(Rect area, IBuffer buffer, ref int state)
            {
                RenderCalled = true;
                LastRenderArea = area;
                LastRenderBuffer = buffer;
                LastState = state;

                // Simple test rendering - fill with state value as char
                var character = (char)('0' + (state % 10));
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    for (int x = area.X; x < area.X + area.Width; x++)
                    {
                        buffer.SetCellSafe(x, y, new Cell(character));
                    }
                }

                state++; // Modify state
            }

            public override int CreateDefaultState() => 42;
        }

        [Fact]
        public void Widget_Render_CallsRenderMethod()
        {
            var widget = new TestWidget();
            var buffer = new Buffer(10, 5);
            var area = new Rect(0, 0, 10, 5);

            widget.Render(area, buffer);

            Assert.True(widget.RenderCalled);
            Assert.Equal(area, widget.LastRenderArea);
            Assert.Same(buffer, widget.LastRenderBuffer);
        }

        [Fact]
        public void Widget_GetMinimumSize_ReturnsConfiguredSize()
        {
            var widget = new TestWidget { MinSize = (5, 3) };

            var size = widget.GetMinimumSize();

            Assert.Equal((5, 3), size);
        }

        [Fact]
        public void Widget_GetMinimumSize_DefaultReturnsZero()
        {
            var widget = new TestWidget();

            var size = widget.GetMinimumSize();

            Assert.Equal((0, 0), size);
        }

        [Fact]
        public void Widget_CanRender_EmptyArea_ReturnsFalse()
        {
            var widget = new TestWidget();
            var emptyArea = new Rect(0, 0, 0, 0);

            var canRender = widget.CanRender(emptyArea);

            Assert.False(canRender);
        }

        [Fact]
        public void Widget_CanRender_ValidArea_ReturnsTrue()
        {
            var widget = new TestWidget();
            var validArea = new Rect(0, 0, 5, 3);

            var canRender = widget.CanRender(validArea);

            Assert.True(canRender);
        }

        [Fact]
        public void Widget_CanRender_MinimumSizeRequirement_ReturnsCorrectly()
        {
            var widget = new TestWidget { MinSize = (5, 3) };

            // Area smaller than minimum
            Assert.False(widget.CanRender(new Rect(0, 0, 4, 3)));
            Assert.False(widget.CanRender(new Rect(0, 0, 5, 2)));

            // Area equal to minimum
            Assert.True(widget.CanRender(new Rect(0, 0, 5, 3)));

            // Area larger than minimum
            Assert.True(widget.CanRender(new Rect(0, 0, 10, 5)));
        }

        [Fact]
        public void Widget_Render_FillsAreaCorrectly()
        {
            var widget = new TestWidget();
            var buffer = new Buffer(10, 5);
            var area = new Rect(2, 1, 3, 2);

            widget.Render(area, buffer);

            // Check that only the specified area is filled
            for (int y = 0; y < buffer.Height; y++)
            {
                for (int x = 0; x < buffer.Width; x++)
                {
                    var cell = buffer.GetCellSafe(x, y);
                    if (x >= area.X && x < area.X + area.Width &&
                        y >= area.Y && y < area.Y + area.Height)
                    {
                        Assert.Equal('T', cell.Character);
                    }
                    else
                    {
                        Assert.Equal(' ', cell.Character); // Default empty cell
                    }
                }
            }
        }

        [Fact]
        public void StatefulWidget_Render_PassesStateCorrectly()
        {
            var widget = new TestStatefulWidget();
            var buffer = new Buffer(5, 3);
            var area = new Rect(0, 0, 5, 3);
            var state = 7;

            widget.Render(area, buffer, ref state);

            Assert.True(widget.RenderCalled);
            Assert.Equal(7, widget.LastState);
            Assert.Equal(8, state); // State should be incremented
        }

        [Fact]
        public void StatefulWidget_CreateDefaultState_ReturnsExpectedValue()
        {
            var widget = new TestStatefulWidget();

            var defaultState = widget.CreateDefaultState();

            Assert.Equal(42, defaultState);
        }

        [Fact]
        public void StatefulWidget_Render_FillsWithStateCharacter()
        {
            var widget = new TestStatefulWidget();
            var buffer = new Buffer(3, 2);
            var area = new Rect(0, 0, 3, 2);
            var state = 5;

            widget.Render(area, buffer, ref state);

            // All cells should contain '5' (state % 10)
            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    var cell = buffer.GetCellSafe(x, y);
                    Assert.Equal('5', cell.Character);
                }
            }
        }

        [Fact]
        public void Widget_IWidget_Interface_IsImplemented()
        {
            var widget = new TestWidget();

            Assert.IsAssignableFrom<IWidget>(widget);
        }

        [Fact]
        public void StatefulWidget_IStatefulWidget_Interface_IsImplemented()
        {
            var widget = new TestStatefulWidget();

            Assert.IsAssignableFrom<IStatefulWidget<int>>(widget);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(10, 5, 10, 5)]
        [InlineData(-1, -1, 0, 0)] // Negative areas should be handled gracefully
        public void Widget_CanRender_VariousAreaSizes(int width, int height, int expectedWidth, int expectedHeight)
        {
            var widget = new TestWidget();
            var area = new Rect(0, 0, width, height);

            var canRender = widget.CanRender(area);
            var expectedCanRender = expectedWidth > 0 && expectedHeight > 0;

            Assert.Equal(expectedCanRender, canRender);
        }

        [Fact]
        public void Widget_Render_OutOfBoundsArea_HandledGracefully()
        {
            var widget = new TestWidget();
            var buffer = new Buffer(5, 5);
            var outOfBoundsArea = new Rect(10, 10, 5, 5); // Completely outside buffer

            // Should not throw exception
            widget.Render(outOfBoundsArea, buffer);

            Assert.True(widget.RenderCalled);

            // Buffer should remain unchanged (all empty cells)
            for (int y = 0; y < buffer.Height; y++)
            {
                for (int x = 0; x < buffer.Width; x++)
                {
                    var cell = buffer.GetCellSafe(x, y);
                    Assert.Equal(' ', cell.Character);
                }
            }
        }

        [Fact]
        public void Widget_Render_PartiallyOutOfBoundsArea_RendersVisiblePortion()
        {
            var widget = new TestWidget();
            var buffer = new Buffer(5, 5);
            var partialArea = new Rect(3, 3, 5, 5); // Partially outside buffer

            widget.Render(partialArea, buffer);

            // Check that only the visible portion is rendered
            for (int y = 0; y < buffer.Height; y++)
            {
                for (int x = 0; x < buffer.Width; x++)
                {
                    var cell = buffer.GetCellSafe(x, y);
                    if (x >= 3 && y >= 3)
                    {
                        Assert.Equal('T', cell.Character);
                    }
                    else
                    {
                        Assert.Equal(' ', cell.Character);
                    }
                }
            }
        }
    }
}