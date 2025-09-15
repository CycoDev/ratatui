using System;
using System.Collections.Generic;
using System.Linq;
using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Text;

namespace CycoAI.CycoTui.Core.Widgets
{
    /// <summary>
    /// Specifies how text should be wrapped within a paragraph.
    /// </summary>
    public enum WrapMode
    {
        /// <summary>
        /// No text wrapping. Text that exceeds the width will be clipped.
        /// </summary>
        None,

        /// <summary>
        /// Wrap text at word boundaries.
        /// </summary>
        Word,

        /// <summary>
        /// Wrap text at any character boundary.
        /// </summary>
        Character
    }

    /// <summary>
    /// A widget that renders styled text with support for line wrapping and alignment.
    /// </summary>
    public class Paragraph : Widget
    {
        private readonly List<Span> _spans;

        /// <summary>
        /// Gets or sets the horizontal alignment of text within the paragraph.
        /// </summary>
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;

        /// <summary>
        /// Gets or sets the text wrapping mode.
        /// </summary>
        public WrapMode Wrap { get; set; } = WrapMode.Word;

        /// <summary>
        /// Gets or sets the scroll offset (number of lines to skip from the top).
        /// </summary>
        public int ScrollOffset { get; set; } = 0;

        /// <summary>
        /// Gets the spans that make up this paragraph.
        /// </summary>
        public IReadOnlyList<Span> Spans => _spans;

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        public Paragraph()
        {
            _spans = new List<Span>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class with the specified text.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public Paragraph(string text)
        {
            _spans = new List<Span> { new Span(text) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class with the specified spans.
        /// </summary>
        /// <param name="spans">The spans to display.</param>
        public Paragraph(params Span[] spans)
        {
            _spans = new List<Span>(spans);
        }

        /// <summary>
        /// Creates a new paragraph with the specified text.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <returns>A new paragraph instance.</returns>
        public static Paragraph WithText(string text) => new Paragraph(text);

        /// <summary>
        /// Creates a new paragraph with the specified spans.
        /// </summary>
        /// <param name="spans">The spans to display.</param>
        /// <returns>A new paragraph instance.</returns>
        public static Paragraph WithSpans(params Span[] spans) => new Paragraph(spans);

        /// <summary>
        /// Adds a span to this paragraph.
        /// </summary>
        /// <param name="span">The span to add.</param>
        /// <returns>This paragraph instance for method chaining.</returns>
        public Paragraph AddSpan(Span span)
        {
            _spans.Add(span);
            return this;
        }

        /// <summary>
        /// Adds text to this paragraph.
        /// </summary>
        /// <param name="text">The text to add.</param>
        /// <returns>This paragraph instance for method chaining.</returns>
        public Paragraph AddText(string text)
        {
            _spans.Add(new Span(text));
            return this;
        }

        /// <summary>
        /// Adds multiple spans to this paragraph.
        /// </summary>
        /// <param name="spans">The spans to add.</param>
        /// <returns>This paragraph instance for method chaining.</returns>
        public Paragraph AddSpans(params Span[] spans)
        {
            _spans.AddRange(spans);
            return this;
        }

        /// <summary>
        /// Clears all spans from this paragraph.
        /// </summary>
        /// <returns>This paragraph instance for method chaining.</returns>
        public Paragraph Clear()
        {
            _spans.Clear();
            return this;
        }

        /// <summary>
        /// Sets the horizontal alignment for this paragraph.
        /// </summary>
        /// <param name="alignment">The alignment to set.</param>
        /// <returns>This paragraph instance for method chaining.</returns>
        public Paragraph WithAlignment(HorizontalAlignment alignment)
        {
            Alignment = alignment;
            return this;
        }

        /// <summary>
        /// Sets the wrap mode for this paragraph.
        /// </summary>
        /// <param name="wrap">The wrap mode to set.</param>
        /// <returns>This paragraph instance for method chaining.</returns>
        public Paragraph WithWrap(WrapMode wrap)
        {
            Wrap = wrap;
            return this;
        }

        /// <summary>
        /// Sets the scroll offset for this paragraph.
        /// </summary>
        /// <param name="offset">The scroll offset to set.</param>
        /// <returns>This paragraph instance for method chaining.</returns>
        public Paragraph WithScrollOffset(int offset)
        {
            ScrollOffset = Math.Max(0, offset);
            return this;
        }

        /// <inheritdoc />
        public override void Render(Rect area, IBuffer buffer)
        {
            if (area.IsEmpty || _spans.Count == 0)
                return;

            var lines = WrapText(area.Width);
            var visibleLines = lines.Skip(ScrollOffset).Take(area.Height).ToList();

            for (int i = 0; i < visibleLines.Count; i++)
            {
                var line = visibleLines[i];
                var y = area.Y + i;

                RenderLine(line, area.X, y, area.Width, buffer);
            }
        }

        private List<List<(char Character, TextStyle Style)>> WrapText(int width)
        {
            var lines = new List<List<(char, TextStyle)>>();
            var currentLine = new List<(char, TextStyle)>();

            foreach (var span in _spans)
            {
                var content = span.Content ?? string.Empty;
                var style = span.Style;

                for (int i = 0; i < content.Length; i++)
                {
                    var ch = content[i];

                    // Handle newlines
                    if (ch == '\n')
                    {
                        lines.Add(currentLine);
                        currentLine = new List<(char, TextStyle)>();
                        continue;
                    }

                    // Handle carriage returns (ignore them)
                    if (ch == '\r')
                        continue;

                    // Check if we need to wrap
                    if (Wrap != WrapMode.None && currentLine.Count >= width)
                    {
                        if (Wrap == WrapMode.Word)
                        {
                            // Try to find a good break point (space or punctuation)
                            var breakPoint = FindWordBreakPoint(currentLine);
                            if (breakPoint > 0 && breakPoint < currentLine.Count)
                            {
                                // Move characters after break point to next line
                                var overflow = currentLine.Skip(breakPoint).ToList();
                                currentLine = currentLine.Take(breakPoint).ToList();
                                lines.Add(currentLine);
                                currentLine = overflow;
                            }
                            else
                            {
                                // No good break point, force wrap
                                lines.Add(currentLine);
                                currentLine = new List<(char, TextStyle)>();
                            }
                        }
                        else // Character wrap
                        {
                            lines.Add(currentLine);
                            currentLine = new List<(char, TextStyle)>();
                        }
                    }

                    currentLine.Add((ch, style));
                }
            }

            // Add the last line if it has content
            if (currentLine.Count > 0)
            {
                lines.Add(currentLine);
            }

            // Ensure we have at least one line
            if (lines.Count == 0)
            {
                lines.Add(new List<(char, TextStyle)>());
            }

            return lines;
        }

        private int FindWordBreakPoint(List<(char Character, TextStyle Style)> line)
        {
            // Look for the last space or punctuation character
            for (int i = line.Count - 1; i >= 0; i--)
            {
                var ch = line[i].Character;
                if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch))
                {
                    return i + 1; // Break after the delimiter
                }
            }

            return -1; // No break point found
        }

        private void RenderLine(List<(char Character, TextStyle Style)> line, int x, int y, int width, IBuffer buffer)
        {
            if (line.Count == 0)
                return;

            var content = line.ToList();
            var lineWidth = content.Count;

            // Calculate starting position based on alignment
            var startX = Alignment switch
            {
                HorizontalAlignment.Left => x,
                HorizontalAlignment.Center => x + Math.Max(0, (width - lineWidth) / 2),
                HorizontalAlignment.Right => x + Math.Max(0, width - lineWidth),
                _ => x
            };

            // Render characters
            for (int i = 0; i < content.Count; i++)
            {
                var charX = startX + i;
                if (charX >= x && charX < x + width)
                {
                    var (character, style) = content[i];
                    buffer.SetCellSafe(charX, y, new Cell(character, style.Foreground, style.Background, style.Modifiers));
                }
            }
        }

        /// <summary>
        /// Gets the number of lines that would be needed to render this paragraph in the specified width.
        /// </summary>
        /// <param name="width">The width to calculate for.</param>
        /// <returns>The number of lines needed.</returns>
        public int GetLineCount(int width)
        {
            if (width <= 0)
                return 0;

            return WrapText(width).Count;
        }

        /// <summary>
        /// Gets the maximum scroll offset for the specified area.
        /// </summary>
        /// <param name="area">The area to calculate for.</param>
        /// <returns>The maximum scroll offset.</returns>
        public int GetMaxScrollOffset(Rect area)
        {
            var lineCount = GetLineCount(area.Width);
            return Math.Max(0, lineCount - area.Height);
        }
    }
}