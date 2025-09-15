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
    public class BlockTests
    {
        [Fact]
        public void Block_Constructor_InitializesWithDefaults()
        {
            var block = new Block();

            Assert.Equal(Borders.None, block.Borders);
            Assert.Equal(BorderType.Plain, block.BorderType);
            Assert.Equal(TextStyle.Default, block.BorderStyle);
            Assert.Null(block.Title);
            Assert.Equal(HorizontalAlignment.Left, block.TitleAlignment);
            Assert.Null(block.Inner);
        }

        [Fact]
        public void Block_WithBorders_SetsBorders()
        {
            var block = Block.WithBorders(Borders.All);

            Assert.Equal(Borders.All, block.Borders);
        }

        [Fact]
        public void Block_WithAllBorders_SetsAllBorders()
        {
            var block = Block.WithAllBorders();

            Assert.Equal(Borders.All, block.Borders);
        }

        [Fact]
        public void Block_WithBorderType_SetsBorderType()
        {
            var block = new Block().WithBorderType(BorderType.Rounded);

            Assert.Equal(BorderType.Rounded, block.BorderType);
        }

        [Fact]
        public void Block_WithBorderStyle_SetsBorderStyle()
        {
            var style = TextStyle.Default.WithForeground(Color.Red);
            var block = new Block().WithBorderStyle(style);

            Assert.Equal(style, block.BorderStyle);
        }

        [Fact]
        public void Block_WithTitle_String_SetsTitle()
        {
            var block = new Block().WithTitle("Test Title");

            Assert.NotNull(block.Title);
            Assert.Equal("Test Title", block.Title.Value.Content);
        }

        [Fact]
        public void Block_WithTitle_Span_SetsTitle()
        {
            var span = new Span("Styled Title", TextStyle.Default.WithForeground(Color.Blue));
            var block = new Block().WithTitle(span);

            Assert.NotNull(block.Title);
            Assert.Equal(span, block.Title.Value);
        }

        [Fact]
        public void Block_WithTitleAlignment_SetsTitleAlignment()
        {
            var block = new Block().WithTitleAlignment(HorizontalAlignment.Center);

            Assert.Equal(HorizontalAlignment.Center, block.TitleAlignment);
        }

        [Fact]
        public void Block_WithInner_SetsInnerWidget()
        {
            var innerWidget = new Block();
            var block = new Block().WithInner(innerWidget);

            Assert.Same(innerWidget, block.Inner);
        }

        [Fact]
        public void Block_GetInnerArea_NoBorders_ReturnsOriginalArea()
        {
            var block = new Block();
            var area = new Rect(5, 10, 20, 15);

            var innerArea = block.GetInnerArea(area);

            Assert.Equal(area, innerArea);
        }

        [Fact]
        public void Block_GetInnerArea_AllBorders_ReducesArea()
        {
            var block = new Block { Borders = Borders.All };
            var area = new Rect(5, 10, 20, 15);

            var innerArea = block.GetInnerArea(area);

            Assert.Equal(new Rect(6, 11, 18, 13), innerArea);
        }

        [Fact]
        public void Block_GetInnerArea_PartialBorders_ReducesCorrectly()
        {
            var block = new Block { Borders = Borders.Left | Borders.Top };
            var area = new Rect(0, 0, 10, 8);

            var innerArea = block.GetInnerArea(area);

            Assert.Equal(new Rect(1, 1, 9, 7), innerArea);
        }

        [Fact]
        public void Block_GetInnerArea_SmallArea_ClampsToZero()
        {
            var block = new Block { Borders = Borders.All };
            var tinyArea = new Rect(0, 0, 1, 1);

            var innerArea = block.GetInnerArea(tinyArea);

            Assert.Equal(new Rect(1, 1, 0, 0), innerArea);
        }

        [Fact]
        public void Block_GetMinimumSize_NoBorders_ReturnsZero()
        {
            var block = new Block();

            var size = block.GetMinimumSize();

            Assert.Equal((0, 0), size);
        }

        [Fact]
        public void Block_GetMinimumSize_AllBorders_ReturnsBorderSize()
        {
            var block = new Block { Borders = Borders.All };

            var size = block.GetMinimumSize();

            Assert.Equal((2, 2), size);
        }

        [Fact]
        public void Block_GetMinimumSize_PartialBorders_ReturnsCorrectSize()
        {
            var block = new Block { Borders = Borders.Left | Borders.Right };

            var size = block.GetMinimumSize();

            Assert.Equal((2, 0), size);
        }

        [Fact]
        public void Block_Render_EmptyArea_DoesNotThrow()
        {
            var block = new Block();
            var buffer = new Buffer(10, 10);
            var emptyArea = new Rect(0, 0, 0, 0);

            // Should not throw
            block.Render(emptyArea, buffer);
        }

        [Fact]
        public void Block_Render_NoBorders_DoesNotDrawBorders()
        {
            var block = new Block();
            var buffer = new Buffer(5, 5);
            var area = new Rect(0, 0, 5, 5);

            block.Render(area, buffer);

            // All cells should remain empty (default ' ')
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
        public void Block_Render_AllBorders_DrawsCompleteFrame()
        {
            var block = new Block { Borders = Borders.All, BorderType = BorderType.Plain };
            var buffer = new Buffer(5, 3);
            var area = new Rect(0, 0, 5, 3);

            block.Render(area, buffer);

            // Check corners
            Assert.Equal('+', buffer.GetCellSafe(0, 0).Character); // Top-left
            Assert.Equal('+', buffer.GetCellSafe(4, 0).Character); // Top-right
            Assert.Equal('+', buffer.GetCellSafe(0, 2).Character); // Bottom-left
            Assert.Equal('+', buffer.GetCellSafe(4, 2).Character); // Bottom-right

            // Check horizontal borders
            Assert.Equal('-', buffer.GetCellSafe(1, 0).Character); // Top
            Assert.Equal('-', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('-', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('-', buffer.GetCellSafe(1, 2).Character); // Bottom
            Assert.Equal('-', buffer.GetCellSafe(2, 2).Character);
            Assert.Equal('-', buffer.GetCellSafe(3, 2).Character);

            // Check vertical borders
            Assert.Equal('|', buffer.GetCellSafe(0, 1).Character); // Left
            Assert.Equal('|', buffer.GetCellSafe(4, 1).Character); // Right
        }

        [Fact]
        public void Block_Render_RoundedBorders_UsesUnicodeCharacters()
        {
            var block = new Block { Borders = Borders.All, BorderType = BorderType.Rounded };
            var buffer = new Buffer(3, 3);
            var area = new Rect(0, 0, 3, 3);

            block.Render(area, buffer);

            // Check rounded corners
            Assert.Equal('╭', buffer.GetCellSafe(0, 0).Character); // Top-left
            Assert.Equal('╮', buffer.GetCellSafe(2, 0).Character); // Top-right
            Assert.Equal('╰', buffer.GetCellSafe(0, 2).Character); // Bottom-left
            Assert.Equal('╯', buffer.GetCellSafe(2, 2).Character); // Bottom-right

            // Check Unicode borders
            Assert.Equal('─', buffer.GetCellSafe(1, 0).Character); // Top
            Assert.Equal('─', buffer.GetCellSafe(1, 2).Character); // Bottom
            Assert.Equal('│', buffer.GetCellSafe(0, 1).Character); // Left
            Assert.Equal('│', buffer.GetCellSafe(2, 1).Character); // Right
        }

        [Fact]
        public void Block_Render_PartialBorders_DrawsOnlySpecifiedBorders()
        {
            var block = new Block { Borders = Borders.Top | Borders.Left, BorderType = BorderType.Plain };
            var buffer = new Buffer(3, 3);
            var area = new Rect(0, 0, 3, 3);

            block.Render(area, buffer);

            // Should only have top and left borders
            Assert.Equal('+', buffer.GetCellSafe(0, 0).Character); // Top-left corner
            Assert.Equal('-', buffer.GetCellSafe(1, 0).Character); // Top border
            Assert.Equal('-', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('|', buffer.GetCellSafe(0, 1).Character); // Left border
            Assert.Equal('|', buffer.GetCellSafe(0, 2).Character);

            // Other positions should be empty
            Assert.Equal(' ', buffer.GetCellSafe(1, 1).Character);
            Assert.Equal(' ', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 2).Character);
            Assert.Equal(' ', buffer.GetCellSafe(2, 2).Character);
        }

        [Fact]
        public void Block_Render_WithTitle_RendersTitle()
        {
            var block = new Block
            {
                Borders = Borders.All,
                BorderType = BorderType.Plain,
                Title = new Span("Test")
            };
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);

            block.Render(area, buffer);

            // Title should be rendered on the top border
            Assert.Equal('T', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('s', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('t', buffer.GetCellSafe(4, 0).Character);
        }

        [Fact]
        public void Block_Render_TitleAlignment_Center_CentersTitle()
        {
            var block = new Block
            {
                Borders = Borders.All,
                BorderType = BorderType.Plain,
                Title = new Span("Hi"),
                TitleAlignment = HorizontalAlignment.Center
            };
            var buffer = new Buffer(8, 3);
            var area = new Rect(0, 0, 8, 3);

            block.Render(area, buffer);

            // Available width for title is 6 (8 - 2 borders), title is 2 chars
            // Should be centered at position (6-2)/2 = 2, plus 1 for left border = 3
            Assert.Equal('H', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('i', buffer.GetCellSafe(4, 0).Character);
        }

        [Fact]
        public void Block_Render_TitleAlignment_Right_RightAlignsTitle()
        {
            var block = new Block
            {
                Borders = Borders.All,
                BorderType = BorderType.Plain,
                Title = new Span("End"),
                TitleAlignment = HorizontalAlignment.Right
            };
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);

            block.Render(area, buffer);

            // Title should be right-aligned within available space
            // Available width = 10 - 2 (borders) = 8
            // Title length = 3, so position = 1 (start after left border) + 8 - 3 = 6
            Assert.Equal('E', buffer.GetCellSafe(6, 0).Character);
            Assert.Equal('n', buffer.GetCellSafe(7, 0).Character);
            Assert.Equal('d', buffer.GetCellSafe(8, 0).Character);
        }

        [Fact]
        public void Block_Render_LongTitle_TruncatesTitle()
        {
            var block = new Block
            {
                Borders = Borders.All,
                BorderType = BorderType.Plain,
                Title = new Span("Very Long Title That Should Be Truncated")
            };
            var buffer = new Buffer(6, 3);
            var area = new Rect(0, 0, 6, 3);

            block.Render(area, buffer);

            // Only 4 characters available (6 - 2 borders)
            Assert.Equal('V', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('r', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('y', buffer.GetCellSafe(4, 0).Character);
        }

        [Fact]
        public void Block_Render_WithInnerWidget_RendersInnerInCorrectArea()
        {
            var innerWidget = new TestInnerWidget();
            var block = new Block
            {
                Borders = Borders.All,
                Inner = innerWidget
            };
            var buffer = new Buffer(5, 5);
            var area = new Rect(0, 0, 5, 5);

            block.Render(area, buffer);

            Assert.True(innerWidget.RenderCalled);
            Assert.Equal(new Rect(1, 1, 3, 3), innerWidget.LastRenderArea);
        }

        [Fact]
        public void Block_Render_BorderStyle_AppliesStyleToBorders()
        {
            var borderStyle = TextStyle.Default.WithForeground(Color.Red).WithBackground(Color.Blue);
            var block = new Block
            {
                Borders = Borders.All,
                BorderType = BorderType.Plain,
                BorderStyle = borderStyle
            };
            var buffer = new Buffer(3, 3);
            var area = new Rect(0, 0, 3, 3);

            block.Render(area, buffer);

            // Check that border cells have the correct style
            var topLeftCell = buffer.GetCellSafe(0, 0);
            Assert.Equal('+', topLeftCell.Character);
            Assert.Equal(Color.Red, topLeftCell.Foreground);
            Assert.Equal(Color.Blue, topLeftCell.Background);
        }

        /// <summary>
        /// Test widget for testing inner widget rendering.
        /// </summary>
        private class TestInnerWidget : Widget
        {
            public bool RenderCalled { get; private set; }
            public Rect LastRenderArea { get; private set; }

            public override void Render(Rect area, IBuffer buffer)
            {
                RenderCalled = true;
                LastRenderArea = area;

                // Fill with 'I' to mark inner widget area
                for (int y = area.Y; y < area.Y + area.Height; y++)
                {
                    for (int x = area.X; x < area.X + area.Width; x++)
                    {
                        buffer.SetCellSafe(x, y, new Cell('I'));
                    }
                }
            }
        }
    }
}