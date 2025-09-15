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
    /// Represents an item in a list widget.
    /// </summary>
    public readonly struct ListItem : IEquatable<ListItem>
    {
        /// <summary>
        /// Gets the content spans that make up this list item.
        /// </summary>
        public IReadOnlyList<Span> Content { get; }

        /// <summary>
        /// Gets the style applied to this list item.
        /// </summary>
        public TextStyle Style { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItem"/> struct.
        /// </summary>
        /// <param name="content">The content spans.</param>
        /// <param name="style">The style to apply.</param>
        public ListItem(IReadOnlyList<Span> content, TextStyle style = default)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItem"/> struct with a single span.
        /// </summary>
        /// <param name="content">The content span.</param>
        /// <param name="style">The style to apply.</param>
        public ListItem(Span content, TextStyle style = default)
        {
            Content = new[] { content };
            Style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItem"/> struct with text content.
        /// </summary>
        /// <param name="text">The text content.</param>
        /// <param name="style">The style to apply.</param>
        public ListItem(string text, TextStyle style = default)
        {
            Content = new[] { new Span(text) };
            Style = style;
        }

        /// <summary>
        /// Creates a list item from text.
        /// </summary>
        /// <param name="text">The text content.</param>
        /// <returns>A new list item.</returns>
        public static ListItem FromText(string text) => new ListItem(text);

        /// <summary>
        /// Creates a list item from a span.
        /// </summary>
        /// <param name="span">The content span.</param>
        /// <returns>A new list item.</returns>
        public static ListItem FromSpan(Span span) => new ListItem(span);

        /// <summary>
        /// Creates a list item from multiple spans.
        /// </summary>
        /// <param name="spans">The content spans.</param>
        /// <returns>A new list item.</returns>
        public static ListItem FromSpans(params Span[] spans) => new ListItem(spans);

        /// <inheritdoc />
        public bool Equals(ListItem other) =>
            Content.SequenceEqual(other.Content) && Style.Equals(other.Style);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is ListItem other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Content, Style);
#else
            HashCode.Combine(Content, Style);
#endif

        /// <summary>
        /// Determines whether two list items are equal.
        /// </summary>
        public static bool operator ==(ListItem left, ListItem right) => left.Equals(right);

        /// <summary>
        /// Determines whether two list items are not equal.
        /// </summary>
        public static bool operator !=(ListItem left, ListItem right) => !(left == right);
    }

    /// <summary>
    /// Represents the state of a list widget, including selection and scroll position.
    /// </summary>
    public class ListState
    {
        /// <summary>
        /// Gets or sets the index of the currently selected item, or null if no item is selected.
        /// </summary>
        public int? SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the scroll offset (number of items to skip from the top).
        /// </summary>
        public int ScrollOffset { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListState"/> class.
        /// </summary>
        public ListState()
        {
        }

        /// <summary>
        /// Selects the item at the specified index.
        /// </summary>
        /// <param name="index">The index to select.</param>
        public void Select(int index)
        {
            SelectedIndex = Math.Max(0, index);
        }

        /// <summary>
        /// Clears the selection.
        /// </summary>
        public void ClearSelection()
        {
            SelectedIndex = null;
        }

        /// <summary>
        /// Moves the selection to the next item.
        /// </summary>
        /// <param name="itemCount">The total number of items in the list.</param>
        /// <returns>true if the selection changed; otherwise, false.</returns>
        public bool SelectNext(int itemCount)
        {
            if (itemCount <= 0)
                return false;

            var oldSelection = SelectedIndex;
            SelectedIndex = SelectedIndex switch
            {
                null => 0,
                var current when current >= itemCount - 1 => itemCount - 1,
                var current => current + 1
            };

            return SelectedIndex != oldSelection;
        }

        /// <summary>
        /// Moves the selection to the previous item.
        /// </summary>
        /// <returns>true if the selection changed; otherwise, false.</returns>
        public bool SelectPrevious()
        {
            if (SelectedIndex == null)
                return false;

            var oldSelection = SelectedIndex;
            SelectedIndex = Math.Max(0, SelectedIndex.Value - 1);
            return SelectedIndex != oldSelection;
        }

        /// <summary>
        /// Ensures the selected item is visible by adjusting the scroll offset.
        /// </summary>
        /// <param name="visibleItemCount">The number of items visible in the list area.</param>
        public void EnsureSelectedVisible(int visibleItemCount)
        {
            if (SelectedIndex == null || visibleItemCount <= 0)
                return;

            var selected = SelectedIndex.Value;

            // If selected item is above the visible area, scroll up
            if (selected < ScrollOffset)
            {
                ScrollOffset = selected;
            }
            // If selected item is below the visible area, scroll down
            else if (selected >= ScrollOffset + visibleItemCount)
            {
                ScrollOffset = selected - visibleItemCount + 1;
            }

            ScrollOffset = Math.Max(0, ScrollOffset);
        }
    }

    /// <summary>
    /// A widget that displays a list of items with optional selection support.
    /// </summary>
    public class List : StatefulWidget<ListState>
    {
        private readonly List<ListItem> _items;

        /// <summary>
        /// Gets or sets the style applied to normal (unselected) items.
        /// </summary>
        public TextStyle ItemStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets the style applied to the selected item.
        /// </summary>
        public TextStyle SelectedStyle { get; set; } = TextStyle.Default.WithModifiers(Modifier.Reversed);

        /// <summary>
        /// Gets or sets the highlight symbol displayed next to the selected item.
        /// </summary>
        public string HighlightSymbol { get; set; } = "> ";

        /// <summary>
        /// Gets or sets a value indicating whether selection is enabled.
        /// </summary>
        public bool SelectionEnabled { get; set; } = true;

        /// <summary>
        /// Gets the items in this list.
        /// </summary>
        public IReadOnlyList<ListItem> Items => _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class.
        /// </summary>
        public List()
        {
            _items = new List<ListItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class with the specified items.
        /// </summary>
        /// <param name="items">The items to display in the list.</param>
        public List(IEnumerable<ListItem> items)
        {
            _items = new List<ListItem>(items ?? throw new ArgumentNullException(nameof(items)));
        }

        /// <summary>
        /// Creates a new list with the specified items.
        /// </summary>
        /// <param name="items">The items to display.</param>
        /// <returns>A new list instance.</returns>
        public static List WithItems(params ListItem[] items) => new List(items);

        /// <summary>
        /// Creates a new list with text items.
        /// </summary>
        /// <param name="texts">The text items to display.</param>
        /// <returns>A new list instance.</returns>
        public static List WithTexts(params string[] texts) =>
            new List(texts.Select(t => ListItem.FromText(t)));

        /// <summary>
        /// Adds an item to this list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>This list instance for method chaining.</returns>
        public List AddItem(ListItem item)
        {
            _items.Add(item);
            return this;
        }

        /// <summary>
        /// Adds a text item to this list.
        /// </summary>
        /// <param name="text">The text to add.</param>
        /// <returns>This list instance for method chaining.</returns>
        public List AddText(string text)
        {
            _items.Add(ListItem.FromText(text));
            return this;
        }

        /// <summary>
        /// Adds multiple items to this list.
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <returns>This list instance for method chaining.</returns>
        public List AddItems(params ListItem[] items)
        {
            _items.AddRange(items);
            return this;
        }

        /// <summary>
        /// Clears all items from this list.
        /// </summary>
        /// <returns>This list instance for method chaining.</returns>
        public List Clear()
        {
            _items.Clear();
            return this;
        }

        /// <summary>
        /// Sets the item style for this list.
        /// </summary>
        /// <param name="style">The style to apply to normal items.</param>
        /// <returns>This list instance for method chaining.</returns>
        public List WithItemStyle(TextStyle style)
        {
            ItemStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the selected item style for this list.
        /// </summary>
        /// <param name="style">The style to apply to the selected item.</param>
        /// <returns>This list instance for method chaining.</returns>
        public List WithSelectedStyle(TextStyle style)
        {
            SelectedStyle = style;
            return this;
        }

        /// <summary>
        /// Sets the highlight symbol for this list.
        /// </summary>
        /// <param name="symbol">The symbol to display next to the selected item.</param>
        /// <returns>This list instance for method chaining.</returns>
        public List WithHighlightSymbol(string symbol)
        {
            HighlightSymbol = symbol ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Enables or disables selection for this list.
        /// </summary>
        /// <param name="enabled">Whether selection should be enabled.</param>
        /// <returns>This list instance for method chaining.</returns>
        public List WithSelection(bool enabled)
        {
            SelectionEnabled = enabled;
            return this;
        }

        /// <inheritdoc />
        public override void Render(Rect area, IBuffer buffer, ref ListState state)
        {
            if (area.IsEmpty || _items.Count == 0)
                return;

            var visibleItems = _items.Skip(state.ScrollOffset).Take(area.Height).ToList();
            var highlightLength = HighlightSymbol.Length;

            for (int i = 0; i < visibleItems.Count; i++)
            {
                var itemIndex = state.ScrollOffset + i;
                var item = visibleItems[i];
                var y = area.Y + i;
                var isSelected = SelectionEnabled && state.SelectedIndex == itemIndex;

                var currentX = area.X;

                // Render highlight symbol for selected item
                if (isSelected && highlightLength > 0)
                {
                    for (int j = 0; j < highlightLength && currentX < area.X + area.Width; j++)
                    {
                        buffer.SetCellSafe(currentX, y, new Cell(
                            HighlightSymbol[j],
                            SelectedStyle.Foreground,
                            SelectedStyle.Background,
                            SelectedStyle.Modifiers));
                        currentX++;
                    }
                }
                else if (highlightLength > 0)
                {
                    // Render spaces where highlight symbol would be
                    for (int j = 0; j < highlightLength && currentX < area.X + area.Width; j++)
                    {
                        buffer.SetCellSafe(currentX, y, new Cell(' '));
                        currentX++;
                    }
                }

                // Determine which style to use
                var effectiveStyle = isSelected ? SelectedStyle : ItemStyle;
                var itemContentStyle = item.Style.Equals(TextStyle.Default) ? effectiveStyle : item.Style;

                // Render item content
                foreach (var span in item.Content)
                {
                    var content = span.Content ?? string.Empty;
                    var spanStyle = span.Style.Equals(TextStyle.Default) ? itemContentStyle : span.Style;

                    for (int j = 0; j < content.Length && currentX < area.X + area.Width; j++)
                    {
                        buffer.SetCellSafe(currentX, y, new Cell(
                            content[j],
                            spanStyle.Foreground,
                            spanStyle.Background,
                            spanStyle.Modifiers));
                        currentX++;
                    }
                }

                // Fill remaining space with the effective style if selected
                if (isSelected)
                {
                    while (currentX < area.X + area.Width)
                    {
                        buffer.SetCellSafe(currentX, y, new Cell(' ',
                            SelectedStyle.Foreground,
                            SelectedStyle.Background,
                            SelectedStyle.Modifiers));
                        currentX++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the maximum scroll offset for the specified area.
        /// </summary>
        /// <param name="area">The area to calculate for.</param>
        /// <returns>The maximum scroll offset.</returns>
        public int GetMaxScrollOffset(Rect area)
        {
            return Math.Max(0, _items.Count - area.Height);
        }

        /// <inheritdoc />
        public override ListState CreateDefaultState()
        {
            return new ListState();
        }
    }
}