using System;
using System.Linq;
using CycoAI.CycoTui.Core.Buffer;
using Buffer = CycoAI.CycoTui.Core.Buffer.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Text;
using CycoAI.CycoTui.Core.Widgets;
using Xunit;

namespace CycoAI.CycoTui.Tests.Widgets
{
    public class ParagraphTests
    {
        [Fact]
        public void Paragraph_Constructor_InitializesWithDefaults()
        {
            var paragraph = new Paragraph();

            Assert.Equal(HorizontalAlignment.Left, paragraph.Alignment);
            Assert.Equal(WrapMode.Word, paragraph.Wrap);
            Assert.Equal(0, paragraph.ScrollOffset);
            Assert.Empty(paragraph.Spans);
        }

        [Fact]
        public void Paragraph_ConstructorWithText_SetsSpan()
        {
            var paragraph = new Paragraph("Hello World");

            Assert.Single(paragraph.Spans);
            Assert.Equal("Hello World", paragraph.Spans[0].Content);
        }

        [Fact]
        public void Paragraph_ConstructorWithSpans_SetsSpans()
        {
            var spans = new[]
            {
                new Span("Hello "),
                new Span("World", TextStyle.Default.WithForeground(Color.Red))
            };
            var paragraph = new Paragraph(spans);

            Assert.Equal(2, paragraph.Spans.Count);
            Assert.Equal("Hello ", paragraph.Spans[0].Content);
            Assert.Equal("World", paragraph.Spans[1].Content);
        }

        [Fact]
        public void Paragraph_WithText_CreatesInstance()
        {
            var paragraph = Paragraph.WithText("Test Text");

            Assert.Single(paragraph.Spans);
            Assert.Equal("Test Text", paragraph.Spans[0].Content);
        }

        [Fact]
        public void Paragraph_WithSpans_CreatesInstance()
        {
            var spans = new[] { new Span("One"), new Span("Two") };
            var paragraph = Paragraph.WithSpans(spans);

            Assert.Equal(2, paragraph.Spans.Count);
        }

        [Fact]
        public void Paragraph_AddSpan_AddsSpan()
        {
            var paragraph = new Paragraph();
            var span = new Span("Added");

            paragraph.AddSpan(span);

            Assert.Single(paragraph.Spans);
            Assert.Equal("Added", paragraph.Spans[0].Content);
        }

        [Fact]
        public void Paragraph_AddText_AddsTextSpan()
        {
            var paragraph = new Paragraph();

            paragraph.AddText("Added Text");

            Assert.Single(paragraph.Spans);
            Assert.Equal("Added Text", paragraph.Spans[0].Content);
        }

        [Fact]
        public void Paragraph_AddSpans_AddsMultipleSpans()
        {
            var paragraph = new Paragraph();
            var spans = new[] { new Span("One"), new Span("Two") };

            paragraph.AddSpans(spans);

            Assert.Equal(2, paragraph.Spans.Count);
        }

        [Fact]
        public void Paragraph_Clear_RemovesAllSpans()
        {
            var paragraph = new Paragraph("Initial");

            paragraph.Clear();

            Assert.Empty(paragraph.Spans);
        }

        [Fact]
        public void Paragraph_WithAlignment_SetsAlignment()
        {
            var paragraph = new Paragraph().WithAlignment(HorizontalAlignment.Center);

            Assert.Equal(HorizontalAlignment.Center, paragraph.Alignment);
        }

        [Fact]
        public void Paragraph_WithWrap_SetsWrapMode()
        {
            var paragraph = new Paragraph().WithWrap(WrapMode.Character);

            Assert.Equal(WrapMode.Character, paragraph.Wrap);
        }

        [Fact]
        public void Paragraph_WithScrollOffset_SetsScrollOffset()
        {
            var paragraph = new Paragraph().WithScrollOffset(5);

            Assert.Equal(5, paragraph.ScrollOffset);
        }

        [Fact]
        public void Paragraph_WithScrollOffset_NegativeValue_ClampsToZero()
        {
            var paragraph = new Paragraph().WithScrollOffset(-3);

            Assert.Equal(0, paragraph.ScrollOffset);
        }

        [Fact]
        public void Paragraph_Render_EmptyArea_DoesNotThrow()
        {
            var paragraph = new Paragraph("Text");
            var buffer = new Buffer(10, 10);
            var emptyArea = new Rect(0, 0, 0, 0);

            // Should not throw
            paragraph.Render(emptyArea, buffer);
        }

        [Fact]
        public void Paragraph_Render_EmptySpans_DoesNotThrow()
        {
            var paragraph = new Paragraph();
            var buffer = new Buffer(10, 10);
            var area = new Rect(0, 0, 10, 10);

            // Should not throw
            paragraph.Render(area, buffer);
        }

        [Fact]
        public void Paragraph_Render_SimpleText_RendersCorrectly()
        {
            var paragraph = new Paragraph("Hello");
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);

            paragraph.Render(area, buffer);

            // Check first line
            Assert.Equal('H', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('l', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('l', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('o', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(5, 0).Character); // Rest should be empty
        }

        [Fact]
        public void Paragraph_Render_MultilineText_RendersOnSeparateLines()
        {
            var paragraph = new Paragraph("Line 1\nLine 2");
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);

            paragraph.Render(area, buffer);

            // Check first line
            Assert.Equal('L', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal('i', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('n', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal('1', buffer.GetCellSafe(5, 0).Character);

            // Check second line
            Assert.Equal('L', buffer.GetCellSafe(0, 1).Character);
            Assert.Equal('i', buffer.GetCellSafe(1, 1).Character);
            Assert.Equal('n', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal('e', buffer.GetCellSafe(3, 1).Character);
            Assert.Equal(' ', buffer.GetCellSafe(4, 1).Character);
            Assert.Equal('2', buffer.GetCellSafe(5, 1).Character);
        }

        [Fact]
        public void Paragraph_Render_WordWrap_WrapsAtWordBoundaries()
        {
            var paragraph = new Paragraph("Hello world this is a test").WithWrap(WrapMode.Word);
            var buffer = new Buffer(8, 5);
            var area = new Rect(0, 0, 8, 5);

            paragraph.Render(area, buffer);

            // First line should be "Hello" (wraps after "Hello")
            Assert.Equal('H', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('l', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('l', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('o', buffer.GetCellSafe(4, 0).Character);

            // Second line should start with "world"
            Assert.Equal('w', buffer.GetCellSafe(0, 1).Character);
            Assert.Equal('o', buffer.GetCellSafe(1, 1).Character);
            Assert.Equal('r', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal('l', buffer.GetCellSafe(3, 1).Character);
            Assert.Equal('d', buffer.GetCellSafe(4, 1).Character);
        }

        [Fact]
        public void Paragraph_Render_CharacterWrap_WrapsAtCharacterBoundaries()
        {
            var paragraph = new Paragraph("12345678901234").WithWrap(WrapMode.Character);
            var buffer = new Buffer(5, 3);
            var area = new Rect(0, 0, 5, 3);

            paragraph.Render(area, buffer);

            // First line: "12345"
            Assert.Equal('1', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal('2', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('3', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('4', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('5', buffer.GetCellSafe(4, 0).Character);

            // Second line: "67890"
            Assert.Equal('6', buffer.GetCellSafe(0, 1).Character);
            Assert.Equal('7', buffer.GetCellSafe(1, 1).Character);
            Assert.Equal('8', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal('9', buffer.GetCellSafe(3, 1).Character);
            Assert.Equal('0', buffer.GetCellSafe(4, 1).Character);

            // Third line: "1234"
            Assert.Equal('1', buffer.GetCellSafe(0, 2).Character);
            Assert.Equal('2', buffer.GetCellSafe(1, 2).Character);
            Assert.Equal('3', buffer.GetCellSafe(2, 2).Character);
            Assert.Equal('4', buffer.GetCellSafe(3, 2).Character);
        }

        [Fact]
        public void Paragraph_Render_NoWrap_ClipsText()
        {
            var paragraph = new Paragraph("This is a very long line that should be clipped").WithWrap(WrapMode.None);
            var buffer = new Buffer(5, 2);
            var area = new Rect(0, 0, 5, 2);

            paragraph.Render(area, buffer);

            // Should only show first 5 characters
            Assert.Equal('T', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal('h', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('i', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('s', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(4, 0).Character);

            // Second line should be empty
            Assert.Equal(' ', buffer.GetCellSafe(0, 1).Character);
        }

        [Fact]
        public void Paragraph_Render_CenterAlignment_CentersText()
        {
            var paragraph = new Paragraph("Hi").WithAlignment(HorizontalAlignment.Center);
            var buffer = new Buffer(6, 2);
            var area = new Rect(0, 0, 6, 2);

            paragraph.Render(area, buffer);

            // Text should be centered: (6-2)/2 = 2 offset
            Assert.Equal(' ', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('H', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('i', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(5, 0).Character);
        }

        [Fact]
        public void Paragraph_Render_RightAlignment_RightAlignsText()
        {
            var paragraph = new Paragraph("End").WithAlignment(HorizontalAlignment.Right);
            var buffer = new Buffer(6, 2);
            var area = new Rect(0, 0, 6, 2);

            paragraph.Render(area, buffer);

            // Text should be right-aligned
            Assert.Equal(' ', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('E', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('n', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal('d', buffer.GetCellSafe(5, 0).Character);
        }

        [Fact]
        public void Paragraph_Render_WithScrollOffset_SkipsLines()
        {
            var paragraph = new Paragraph("Line 1\nLine 2\nLine 3\nLine 4").WithScrollOffset(2);
            var buffer = new Buffer(10, 2);
            var area = new Rect(0, 0, 10, 2);

            paragraph.Render(area, buffer);

            // Should start from line 3 (index 2)
            Assert.Equal('L', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal('i', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('n', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal('3', buffer.GetCellSafe(5, 0).Character);

            // Second visible line should be line 4
            Assert.Equal('L', buffer.GetCellSafe(0, 1).Character);
            Assert.Equal('i', buffer.GetCellSafe(1, 1).Character);
            Assert.Equal('n', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal('e', buffer.GetCellSafe(3, 1).Character);
            Assert.Equal(' ', buffer.GetCellSafe(4, 1).Character);
            Assert.Equal('4', buffer.GetCellSafe(5, 1).Character);
        }

        [Fact]
        public void Paragraph_Render_MultipleSpans_PreservesStyles()
        {
            var spans = new[]
            {
                new Span("Red", TextStyle.Default.WithForeground(Color.Red)),
                new Span("Blue", TextStyle.Default.WithForeground(Color.Blue))
            };
            var paragraph = new Paragraph(spans);
            var buffer = new Buffer(10, 2);
            var area = new Rect(0, 0, 10, 2);

            paragraph.Render(area, buffer);

            // Check that styles are preserved
            var redCell = buffer.GetCellSafe(0, 0); // 'R' from "Red"
            Assert.Equal('R', redCell.Character);
            Assert.Equal(Color.Red, redCell.Foreground);

            var blueCell = buffer.GetCellSafe(3, 0); // 'B' from "Blue"
            Assert.Equal('B', blueCell.Character);
            Assert.Equal(Color.Blue, blueCell.Foreground);
        }

        [Fact]
        public void Paragraph_GetLineCount_ReturnsCorrectCount()
        {
            var paragraph = new Paragraph("Line 1\nLine 2\nLine 3");

            var lineCount = paragraph.GetLineCount(10);

            Assert.Equal(3, lineCount);
        }

        [Fact]
        public void Paragraph_GetLineCount_WithWrapping_ReturnsWrappedCount()
        {
            var paragraph = new Paragraph("This is a long line that will wrap").WithWrap(WrapMode.Word);

            var lineCount = paragraph.GetLineCount(8);

            Assert.True(lineCount > 1); // Should wrap to multiple lines
        }

        [Fact]
        public void Paragraph_GetLineCount_ZeroWidth_ReturnsZero()
        {
            var paragraph = new Paragraph("Text");

            var lineCount = paragraph.GetLineCount(0);

            Assert.Equal(0, lineCount);
        }

        [Fact]
        public void Paragraph_GetMaxScrollOffset_ReturnsCorrectValue()
        {
            var paragraph = new Paragraph("Line 1\nLine 2\nLine 3\nLine 4\nLine 5");
            var area = new Rect(0, 0, 10, 3); // Can show 3 lines

            var maxOffset = paragraph.GetMaxScrollOffset(area);

            Assert.Equal(2, maxOffset); // 5 lines - 3 visible = 2
        }

        [Fact]
        public void Paragraph_GetMaxScrollOffset_FewLines_ReturnsZero()
        {
            var paragraph = new Paragraph("Line 1\nLine 2");
            var area = new Rect(0, 0, 10, 5); // Can show 5 lines

            var maxOffset = paragraph.GetMaxScrollOffset(area);

            Assert.Equal(0, maxOffset); // No scrolling needed
        }

        [Fact]
        public void Paragraph_Render_HandlesCarriageReturns()
        {
            var paragraph = new Paragraph("Hello\r\nWorld");
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);

            paragraph.Render(area, buffer);

            // Should treat \r\n as a single line break
            Assert.Equal('H', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('l', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('l', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('o', buffer.GetCellSafe(4, 0).Character);

            Assert.Equal('W', buffer.GetCellSafe(0, 1).Character);
            Assert.Equal('o', buffer.GetCellSafe(1, 1).Character);
            Assert.Equal('r', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal('l', buffer.GetCellSafe(3, 1).Character);
            Assert.Equal('d', buffer.GetCellSafe(4, 1).Character);
        }

        [Fact]
        public void Paragraph_Render_PartialArea_RendersCorrectly()
        {
            var paragraph = new Paragraph("Hello World");
            var buffer = new Buffer(10, 10);
            var area = new Rect(2, 3, 5, 2); // Offset area

            paragraph.Render(area, buffer);

            // Check text appears at correct offset
            Assert.Equal('H', buffer.GetCellSafe(2, 3).Character);
            Assert.Equal('e', buffer.GetCellSafe(3, 3).Character);
            Assert.Equal('l', buffer.GetCellSafe(4, 3).Character);
            Assert.Equal('l', buffer.GetCellSafe(5, 3).Character);
            Assert.Equal('o', buffer.GetCellSafe(6, 3).Character);

            // Areas outside should remain empty
            Assert.Equal(' ', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 3).Character);
            Assert.Equal(' ', buffer.GetCellSafe(7, 3).Character);
        }
    }
}