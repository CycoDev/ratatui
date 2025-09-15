using System;
using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Text;

namespace CycoAI.CycoTui.Core.Widgets
{
    /// <summary>
    /// A widget that renders a block with optional borders, title, and inner content area.
    /// This is one of the most fundamental widgets, often used as a container for other widgets.
    /// </summary>
    public class Block : Widget
    {
        /// <summary>
        /// Gets or sets the borders to render.
        /// </summary>
        public Borders Borders { get; set; } = Widgets.Borders.None;

        /// <summary>
        /// Gets or sets the border type (characters used for borders).
        /// </summary>
        public BorderType BorderType { get; set; } = Widgets.BorderType.Plain;

        /// <summary>
        /// Gets or sets the style applied to the borders.
        /// </summary>
        public TextStyle BorderStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets the title displayed on the top border.
        /// </summary>
        public Span? Title { get; set; }

        /// <summary>
        /// Gets or sets the alignment of the title on the top border.
        /// </summary>
        public HorizontalAlignment TitleAlignment { get; set; } = HorizontalAlignment.Left;

        /// <summary>
        /// Gets or sets the inner widget to render within the block.
        /// </summary>
        public IWidget? Inner { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        public Block()
        {
        }

        /// <summary>
        /// Creates a new block with the specified borders.
        /// </summary>
        /// <param name="borders">The borders to render.</param>
        /// <returns>A new block instance.</returns>
        public static Block WithBorders(Borders borders) => new Block { Borders = borders };

        /// <summary>
        /// Creates a new block with all borders.
        /// </summary>
        /// <returns>A new block instance with all borders.</returns>
        public static Block WithAllBorders() => new Block { Borders = Widgets.Borders.All };

        /// <summary>
        /// Sets the border type for this block.
        /// </summary>
        /// <param name="borderType">The border type to use.</param>
        /// <returns>This block instance for method chaining.</returns>
        public Block WithBorderType(BorderType borderType)
        {
            BorderType = borderType;
            return this;
        }

        /// <summary>
        /// Sets the border style for this block.
        /// </summary>
        /// <param name="style">The style to apply to borders.</param>
        /// <returns>This block instance for method chaining.</returns>
        public Block WithBorderStyle(TextStyle style)
        {
            BorderStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the title for this block.
        /// </summary>
        /// <param name="title">The title to display.</param>
        /// <returns>This block instance for method chaining.</returns>
        public Block WithTitle(string title)
        {
            Title = new Span(title);
            return this;
        }

        /// <summary>
        /// Sets the title for this block.
        /// </summary>
        /// <param name="title">The styled title to display.</param>
        /// <returns>This block instance for method chaining.</returns>
        public Block WithTitle(Span title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        /// Sets the title alignment for this block.
        /// </summary>
        /// <param name="alignment">The alignment for the title.</param>
        /// <returns>This block instance for method chaining.</returns>
        public Block WithTitleAlignment(HorizontalAlignment alignment)
        {
            TitleAlignment = alignment;
            return this;
        }

        /// <summary>
        /// Sets the inner widget for this block.
        /// </summary>
        /// <param name="widget">The widget to render inside the block.</param>
        /// <returns>This block instance for method chaining.</returns>
        public Block WithInner(IWidget widget)
        {
            Inner = widget;
            return this;
        }

        /// <summary>
        /// Gets the inner area available for content after accounting for borders.
        /// </summary>
        /// <param name="area">The total area of the block.</param>
        /// <returns>The inner area available for content.</returns>
        public Rect GetInnerArea(Rect area)
        {
            var x = area.X;
            var y = area.Y;
            var width = area.Width;
            var height = area.Height;

            if (Borders.HasFlag(Widgets.Borders.Left))
            {
                x += 1;
                width = Math.Max(0, width - 1);
            }

            if (Borders.HasFlag(Widgets.Borders.Right))
            {
                width = Math.Max(0, width - 1);
            }

            if (Borders.HasFlag(Widgets.Borders.Top))
            {
                y += 1;
                height = Math.Max(0, height - 1);
            }

            if (Borders.HasFlag(Widgets.Borders.Bottom))
            {
                height = Math.Max(0, height - 1);
            }

            return new Rect(x, y, width, height);
        }

        /// <inheritdoc />
        public override (int Width, int Height) GetMinimumSize()
        {
            var borderWidth = 0;
            var borderHeight = 0;

            if (Borders.HasFlag(Widgets.Borders.Left)) borderWidth++;
            if (Borders.HasFlag(Widgets.Borders.Right)) borderWidth++;
            if (Borders.HasFlag(Widgets.Borders.Top)) borderHeight++;
            if (Borders.HasFlag(Widgets.Borders.Bottom)) borderHeight++;

            return (borderWidth, borderHeight);
        }

        /// <inheritdoc />
        public override void Render(Rect area, IBuffer buffer)
        {
            if (area.IsEmpty)
                return;

            // Render borders
            RenderBorders(area, buffer);

            // Render title if present
            if (Title != null && Borders.HasFlag(Widgets.Borders.Top))
            {
                RenderTitle(area, buffer);
            }

            // Render inner widget if present
            if (Inner != null)
            {
                var innerArea = GetInnerArea(area);
                if (!innerArea.IsEmpty)
                {
                    Inner.Render(innerArea, buffer);
                }
            }
        }

        private void RenderBorders(Rect area, IBuffer buffer)
        {
            if (Borders == Widgets.Borders.None)
                return;

            // Render corners
            if (Borders.HasFlag(Widgets.Borders.Top) && Borders.HasFlag(Widgets.Borders.Left))
            {
                buffer.SetCellSafe(area.X, area.Y, new Cell(BorderType.TopLeft, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
            }

            if (Borders.HasFlag(Widgets.Borders.Top) && Borders.HasFlag(Widgets.Borders.Right))
            {
                buffer.SetCellSafe(area.X + area.Width - 1, area.Y, new Cell(BorderType.TopRight, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
            }

            if (Borders.HasFlag(Widgets.Borders.Bottom) && Borders.HasFlag(Widgets.Borders.Left))
            {
                buffer.SetCellSafe(area.X, area.Y + area.Height - 1, new Cell(BorderType.BottomLeft, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
            }

            if (Borders.HasFlag(Widgets.Borders.Bottom) && Borders.HasFlag(Widgets.Borders.Right))
            {
                buffer.SetCellSafe(area.X + area.Width - 1, area.Y + area.Height - 1, new Cell(BorderType.BottomRight, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
            }

            // Render horizontal borders
            if (Borders.HasFlag(Widgets.Borders.Top))
            {
                var startX = area.X + (Borders.HasFlag(Widgets.Borders.Left) ? 1 : 0);
                var endX = area.X + area.Width - (Borders.HasFlag(Widgets.Borders.Right) ? 1 : 0);
                for (int x = startX; x < endX; x++)
                {
                    buffer.SetCellSafe(x, area.Y, new Cell(BorderType.Horizontal, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
                }
            }

            if (Borders.HasFlag(Widgets.Borders.Bottom))
            {
                var startX = area.X + (Borders.HasFlag(Widgets.Borders.Left) ? 1 : 0);
                var endX = area.X + area.Width - (Borders.HasFlag(Widgets.Borders.Right) ? 1 : 0);
                for (int x = startX; x < endX; x++)
                {
                    buffer.SetCellSafe(x, area.Y + area.Height - 1, new Cell(BorderType.Horizontal, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
                }
            }

            // Render vertical borders
            if (Borders.HasFlag(Widgets.Borders.Left))
            {
                var startY = area.Y + (Borders.HasFlag(Widgets.Borders.Top) ? 1 : 0);
                var endY = area.Y + area.Height - (Borders.HasFlag(Widgets.Borders.Bottom) ? 1 : 0);
                for (int y = startY; y < endY; y++)
                {
                    buffer.SetCellSafe(area.X, y, new Cell(BorderType.Vertical, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
                }
            }

            if (Borders.HasFlag(Widgets.Borders.Right))
            {
                var startY = area.Y + (Borders.HasFlag(Widgets.Borders.Top) ? 1 : 0);
                var endY = area.Y + area.Height - (Borders.HasFlag(Widgets.Borders.Bottom) ? 1 : 0);
                for (int y = startY; y < endY; y++)
                {
                    buffer.SetCellSafe(area.X + area.Width - 1, y, new Cell(BorderType.Vertical, BorderStyle.Foreground, BorderStyle.Background, BorderStyle.Modifiers));
                }
            }
        }

        private void RenderTitle(Rect area, IBuffer buffer)
        {
            if (Title == null || area.Width < 3) // Need at least 3 characters for title with borders
                return;

            var availableWidth = area.Width - (Borders.HasFlag(Widgets.Borders.Left) ? 1 : 0) - (Borders.HasFlag(Widgets.Borders.Right) ? 1 : 0);
            if (availableWidth <= 0)
                return;

            var titleContent = Title.Value.Content;
            if (string.IsNullOrEmpty(titleContent))
                return;

            // Truncate title if too long
            var displayTitle = titleContent.Length > availableWidth ? titleContent.Substring(0, availableWidth) : titleContent;

            // Calculate title position based on alignment
            var startX = area.X + (Borders.HasFlag(Widgets.Borders.Left) ? 1 : 0);
            var titleX = TitleAlignment switch
            {
                HorizontalAlignment.Left => startX,
                HorizontalAlignment.Center => startX + (availableWidth - displayTitle.Length) / 2,
                HorizontalAlignment.Right => startX + availableWidth - displayTitle.Length,
                _ => startX
            };

            // Ensure title stays within bounds
            titleX = Math.Max(titleX, startX);
            titleX = Math.Min(titleX, startX + availableWidth - displayTitle.Length);

            // Render title characters
            for (int i = 0; i < displayTitle.Length && titleX + i < area.X + area.Width; i++)
            {
                if (titleX + i >= area.X && titleX + i < area.X + area.Width)
                {
                    buffer.SetCellSafe(titleX + i, area.Y, new Cell(
                        displayTitle[i],
                        Title.Value.Style.Foreground,
                        Title.Value.Style.Background,
                        Title.Value.Style.Modifiers));
                }
            }
        }
    }
}