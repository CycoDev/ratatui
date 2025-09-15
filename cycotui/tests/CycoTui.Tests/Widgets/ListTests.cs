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
    public class ListTests
    {
        [Fact]
        public void ListItem_Constructor_SetsProperties()
        {
            var spans = new[] { new Span("Test"), new Span("Item") };
            var style = TextStyle.Default.WithForeground(Color.Red);
            var item = new ListItem(spans, style);

            Assert.Equal(spans, item.Content);
            Assert.Equal(style, item.Style);
        }

        [Fact]
        public void ListItem_Constructor_SingleSpan_SetsContent()
        {
            var span = new Span("Single");
            var item = new ListItem(span);

            Assert.Single(item.Content);
            Assert.Equal(span, item.Content[0]);
        }

        [Fact]
        public void ListItem_Constructor_String_CreatesSpan()
        {
            var item = new ListItem("Text");

            Assert.Single(item.Content);
            Assert.Equal("Text", item.Content[0].Content);
        }

        [Fact]
        public void ListItem_FromText_CreatesItem()
        {
            var item = ListItem.FromText("Hello");

            Assert.Single(item.Content);
            Assert.Equal("Hello", item.Content[0].Content);
        }

        [Fact]
        public void ListItem_FromSpan_CreatesItem()
        {
            var span = new Span("Span");
            var item = ListItem.FromSpan(span);

            Assert.Single(item.Content);
            Assert.Equal(span, item.Content[0]);
        }

        [Fact]
        public void ListItem_FromSpans_CreatesItem()
        {
            var spans = new[] { new Span("One"), new Span("Two") };
            var item = ListItem.FromSpans(spans);

            Assert.Equal(2, item.Content.Count);
            Assert.Equal(spans, item.Content);
        }

        [Fact]
        public void ListItem_Equality_WorksCorrectly()
        {
            var item1 = new ListItem("Same");
            var item2 = new ListItem("Same");
            var item3 = new ListItem("Different");

            Assert.Equal(item1, item2);
            Assert.NotEqual(item1, item3);
            Assert.True(item1 == item2);
            Assert.True(item1 != item3);
        }

        [Fact]
        public void ListState_Constructor_InitializesDefaults()
        {
            var state = new ListState();

            Assert.Null(state.SelectedIndex);
            Assert.Equal(0, state.ScrollOffset);
        }

        [Fact]
        public void ListState_Select_SetsSelectedIndex()
        {
            var state = new ListState();

            state.Select(5);

            Assert.Equal(5, state.SelectedIndex);
        }

        [Fact]
        public void ListState_Select_NegativeValue_ClampsToZero()
        {
            var state = new ListState();

            state.Select(-3);

            Assert.Equal(0, state.SelectedIndex);
        }

        [Fact]
        public void ListState_ClearSelection_SetsIndexToNull()
        {
            var state = new ListState { SelectedIndex = 5 };

            state.ClearSelection();

            Assert.Null(state.SelectedIndex);
        }

        [Fact]
        public void ListState_SelectNext_NoSelection_SelectsFirst()
        {
            var state = new ListState();

            var changed = state.SelectNext(5);

            Assert.True(changed);
            Assert.Equal(0, state.SelectedIndex);
        }

        [Fact]
        public void ListState_SelectNext_AdvancesSelection()
        {
            var state = new ListState { SelectedIndex = 2 };

            var changed = state.SelectNext(5);

            Assert.True(changed);
            Assert.Equal(3, state.SelectedIndex);
        }

        [Fact]
        public void ListState_SelectNext_AtEnd_StaysAtEnd()
        {
            var state = new ListState { SelectedIndex = 4 };

            var changed = state.SelectNext(5);

            Assert.False(changed);
            Assert.Equal(4, state.SelectedIndex);
        }

        [Fact]
        public void ListState_SelectNext_EmptyList_ReturnsFalse()
        {
            var state = new ListState();

            var changed = state.SelectNext(0);

            Assert.False(changed);
            Assert.Null(state.SelectedIndex);
        }

        [Fact]
        public void ListState_SelectPrevious_NoSelection_ReturnsFalse()
        {
            var state = new ListState();

            var changed = state.SelectPrevious();

            Assert.False(changed);
            Assert.Null(state.SelectedIndex);
        }

        [Fact]
        public void ListState_SelectPrevious_MovesToPrevious()
        {
            var state = new ListState { SelectedIndex = 3 };

            var changed = state.SelectPrevious();

            Assert.True(changed);
            Assert.Equal(2, state.SelectedIndex);
        }

        [Fact]
        public void ListState_SelectPrevious_AtStart_StaysAtStart()
        {
            var state = new ListState { SelectedIndex = 0 };

            var changed = state.SelectPrevious();

            Assert.False(changed);
            Assert.Equal(0, state.SelectedIndex);
        }

        [Fact]
        public void ListState_EnsureSelectedVisible_ScrollsUp()
        {
            var state = new ListState { SelectedIndex = 2, ScrollOffset = 5 };

            state.EnsureSelectedVisible(3);

            Assert.Equal(2, state.ScrollOffset);
        }

        [Fact]
        public void ListState_EnsureSelectedVisible_ScrollsDown()
        {
            var state = new ListState { SelectedIndex = 5, ScrollOffset = 0 };

            state.EnsureSelectedVisible(3);

            Assert.Equal(3, state.ScrollOffset); // 5 - 3 + 1 = 3
        }

        [Fact]
        public void ListState_EnsureSelectedVisible_NoSelection_DoesNothing()
        {
            var state = new ListState { ScrollOffset = 5 };

            state.EnsureSelectedVisible(3);

            Assert.Equal(5, state.ScrollOffset); // Unchanged
        }

        [Fact]
        public void List_Constructor_InitializesEmpty()
        {
            var list = new List();

            Assert.Empty(list.Items);
            Assert.Equal(TextStyle.Default, list.ItemStyle);
            Assert.Equal("> ", list.HighlightSymbol);
            Assert.True(list.SelectionEnabled);
        }

        [Fact]
        public void List_Constructor_WithItems_SetsItems()
        {
            var items = new[] { ListItem.FromText("One"), ListItem.FromText("Two") };
            var list = new List(items);

            Assert.Equal(2, list.Items.Count);
            Assert.Equal(items, list.Items);
        }

        [Fact]
        public void List_WithItems_CreatesListWithItems()
        {
            var items = new[] { ListItem.FromText("A"), ListItem.FromText("B") };
            var list = List.WithItems(items);

            Assert.Equal(2, list.Items.Count);
        }

        [Fact]
        public void List_WithTexts_CreatesListWithTextItems()
        {
            var list = List.WithTexts("Alpha", "Beta", "Gamma");

            Assert.Equal(3, list.Items.Count);
            Assert.Equal("Alpha", list.Items[0].Content[0].Content);
            Assert.Equal("Beta", list.Items[1].Content[0].Content);
            Assert.Equal("Gamma", list.Items[2].Content[0].Content);
        }

        [Fact]
        public void List_AddItem_AddsItem()
        {
            var list = new List();
            var item = ListItem.FromText("Added");

            list.AddItem(item);

            Assert.Single(list.Items);
            Assert.Equal(item, list.Items[0]);
        }

        [Fact]
        public void List_AddText_AddsTextItem()
        {
            var list = new List();

            list.AddText("Text Item");

            Assert.Single(list.Items);
            Assert.Equal("Text Item", list.Items[0].Content[0].Content);
        }

        [Fact]
        public void List_AddItems_AddsMultipleItems()
        {
            var list = new List();
            var items = new[] { ListItem.FromText("First"), ListItem.FromText("Second") };

            list.AddItems(items);

            Assert.Equal(2, list.Items.Count);
        }

        [Fact]
        public void List_Clear_RemovesAllItems()
        {
            var list = List.WithTexts("One", "Two");

            list.Clear();

            Assert.Empty(list.Items);
        }

        [Fact]
        public void List_WithItemStyle_SetsItemStyle()
        {
            var style = TextStyle.Default.WithForeground(Color.Blue);
            var list = new List().WithItemStyle(style);

            Assert.Equal(style, list.ItemStyle);
        }

        [Fact]
        public void List_WithSelectedStyle_SetsSelectedStyle()
        {
            var style = TextStyle.Default.WithBackground(Color.Red);
            var list = new List().WithSelectedStyle(style);

            Assert.Equal(style, list.SelectedStyle);
        }

        [Fact]
        public void List_WithHighlightSymbol_SetsHighlightSymbol()
        {
            var list = new List().WithHighlightSymbol("* ");

            Assert.Equal("* ", list.HighlightSymbol);
        }

        [Fact]
        public void List_WithHighlightSymbol_Null_SetsEmpty()
        {
            var list = new List().WithHighlightSymbol(null!);

            Assert.Equal(string.Empty, list.HighlightSymbol);
        }

        [Fact]
        public void List_WithSelection_SetsSelectionEnabled()
        {
            var list = new List().WithSelection(false);

            Assert.False(list.SelectionEnabled);
        }

        [Fact]
        public void List_Render_EmptyList_DoesNotThrow()
        {
            var list = new List();
            var buffer = new Buffer(10, 5);
            var area = new Rect(0, 0, 10, 5);
            var state = new ListState();

            // Should not throw
            list.Render(area, buffer, ref state);
        }

        [Fact]
        public void List_Render_EmptyArea_DoesNotThrow()
        {
            var list = List.WithTexts("Item");
            var buffer = new Buffer(10, 5);
            var emptyArea = new Rect(0, 0, 0, 0);
            var state = new ListState();

            // Should not throw
            list.Render(area: emptyArea, buffer, ref state);
        }

        [Fact]
        public void List_Render_SimpleItems_RendersCorrectly()
        {
            var list = List.WithTexts("First", "Second", "Third");
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);
            var state = new ListState();

            list.Render(area, buffer, ref state);

            // Check first item
            Assert.Equal('F', buffer.GetCellSafe(2, 0).Character); // After "> " symbol
            Assert.Equal('i', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('r', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal('s', buffer.GetCellSafe(5, 0).Character);
            Assert.Equal('t', buffer.GetCellSafe(6, 0).Character);

            // Check second item
            Assert.Equal('S', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal('e', buffer.GetCellSafe(3, 1).Character);
            Assert.Equal('c', buffer.GetCellSafe(4, 1).Character);
            Assert.Equal('o', buffer.GetCellSafe(5, 1).Character);
            Assert.Equal('n', buffer.GetCellSafe(6, 1).Character);
            Assert.Equal('d', buffer.GetCellSafe(7, 1).Character);

            // Check third item
            Assert.Equal('T', buffer.GetCellSafe(2, 2).Character);
            Assert.Equal('h', buffer.GetCellSafe(3, 2).Character);
            Assert.Equal('i', buffer.GetCellSafe(4, 2).Character);
            Assert.Equal('r', buffer.GetCellSafe(5, 2).Character);
            Assert.Equal('d', buffer.GetCellSafe(6, 2).Character);
        }

        [Fact]
        public void List_Render_WithSelection_HighlightsSelectedItem()
        {
            var list = List.WithTexts("First", "Second", "Third");
            var buffer = new Buffer(10, 3);
            var area = new Rect(0, 0, 10, 3);
            var state = new ListState { SelectedIndex = 1 };

            list.Render(area, buffer, ref state);

            // Check highlight symbol on selected item (line 1)
            Assert.Equal('>', buffer.GetCellSafe(0, 1).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 1).Character);

            // Check no highlight on other items
            Assert.Equal(' ', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(0, 2).Character);
        }

        [Fact]
        public void List_Render_SelectionDisabled_NoHighlight()
        {
            var list = List.WithTexts("First", "Second").WithSelection(false);
            var buffer = new Buffer(10, 2);
            var area = new Rect(0, 0, 10, 2);
            var state = new ListState { SelectedIndex = 0 };

            list.Render(area, buffer, ref state);

            // Should have spaces instead of highlight symbol
            Assert.Equal(' ', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('F', buffer.GetCellSafe(2, 0).Character);
        }

        [Fact]
        public void List_Render_CustomHighlightSymbol_UsesCustomSymbol()
        {
            var list = List.WithTexts("Item").WithHighlightSymbol("* ");
            var buffer = new Buffer(10, 1);
            var area = new Rect(0, 0, 10, 1);
            var state = new ListState { SelectedIndex = 0 };

            list.Render(area, buffer, ref state);

            Assert.Equal('*', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(1, 0).Character);
            Assert.Equal('I', buffer.GetCellSafe(2, 0).Character);
        }

        [Fact]
        public void List_Render_WithScrolling_ShowsCorrectItems()
        {
            var list = List.WithTexts("Item0", "Item1", "Item2", "Item3", "Item4");
            var buffer = new Buffer(10, 2);
            var area = new Rect(0, 0, 10, 2);
            var state = new ListState { ScrollOffset = 2 };

            list.Render(area, buffer, ref state);

            // Should show Item2 and Item3
            Assert.Equal('I', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('t', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('e', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal('m', buffer.GetCellSafe(5, 0).Character);
            Assert.Equal('2', buffer.GetCellSafe(6, 0).Character);

            Assert.Equal('I', buffer.GetCellSafe(2, 1).Character);
            Assert.Equal('t', buffer.GetCellSafe(3, 1).Character);
            Assert.Equal('e', buffer.GetCellSafe(4, 1).Character);
            Assert.Equal('m', buffer.GetCellSafe(5, 1).Character);
            Assert.Equal('3', buffer.GetCellSafe(6, 1).Character);
        }

        [Fact]
        public void List_Render_SelectedItemStyle_AppliesStyle()
        {
            var selectedStyle = TextStyle.Default.WithBackground(Color.Blue);
            var list = List.WithTexts("Item").WithSelectedStyle(selectedStyle);
            var buffer = new Buffer(10, 1);
            var area = new Rect(0, 0, 10, 1);
            var state = new ListState { SelectedIndex = 0 };

            list.Render(area, buffer, ref state);

            // Check that selected item has the correct background color
            var cell = buffer.GetCellSafe(2, 0); // 'I' from "Item"
            Assert.Equal('I', cell.Character);
            Assert.Equal(Color.Blue, cell.Background);
        }

        [Fact]
        public void List_Render_MultiSpanItem_RendersAllSpans()
        {
            var item = new ListItem(new[]
            {
                new Span("Red", TextStyle.Default.WithForeground(Color.Red)),
                new Span("Blue", TextStyle.Default.WithForeground(Color.Blue))
            });
            var list = new List(new[] { item });
            var buffer = new Buffer(10, 1);
            var area = new Rect(0, 0, 10, 1);
            var state = new ListState();

            list.Render(area, buffer, ref state);

            // Check colors are preserved
            var redCell = buffer.GetCellSafe(2, 0); // 'R' from "Red"
            Assert.Equal('R', redCell.Character);
            Assert.Equal(Color.Red, redCell.Foreground);

            var blueCell = buffer.GetCellSafe(5, 0); // 'B' from "Blue"
            Assert.Equal('B', blueCell.Character);
            Assert.Equal(Color.Blue, blueCell.Foreground);
        }

        [Fact]
        public void List_Render_LongText_ClipsAtAreaBoundary()
        {
            var list = List.WithTexts("This is a very long text that should be clipped");
            var buffer = new Buffer(10, 1);
            var area = new Rect(0, 0, 10, 1);
            var state = new ListState();

            list.Render(area, buffer, ref state);

            // Should only show up to the area width
            Assert.Equal('T', buffer.GetCellSafe(2, 0).Character);
            Assert.Equal('h', buffer.GetCellSafe(3, 0).Character);
            Assert.Equal('i', buffer.GetCellSafe(4, 0).Character);
            Assert.Equal('s', buffer.GetCellSafe(5, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(6, 0).Character);
            Assert.Equal('i', buffer.GetCellSafe(7, 0).Character);
            Assert.Equal('s', buffer.GetCellSafe(8, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(9, 0).Character);
        }

        [Fact]
        public void List_GetMaxScrollOffset_ReturnsCorrectValue()
        {
            var list = List.WithTexts("1", "2", "3", "4", "5");
            var area = new Rect(0, 0, 10, 3); // Can show 3 items

            var maxOffset = list.GetMaxScrollOffset(area);

            Assert.Equal(2, maxOffset); // 5 items - 3 visible = 2
        }

        [Fact]
        public void List_GetMaxScrollOffset_FewItems_ReturnsZero()
        {
            var list = List.WithTexts("1", "2");
            var area = new Rect(0, 0, 10, 5); // Can show 5 items

            var maxOffset = list.GetMaxScrollOffset(area);

            Assert.Equal(0, maxOffset); // No scrolling needed
        }

        [Fact]
        public void List_CreateDefaultState_ReturnsNewListState()
        {
            var list = new List();

            var state = list.CreateDefaultState();

            Assert.NotNull(state);
            Assert.IsType<ListState>(state);
            Assert.Null(state.SelectedIndex);
            Assert.Equal(0, state.ScrollOffset);
        }

        [Fact]
        public void List_Render_PartialArea_RendersCorrectly()
        {
            var list = List.WithTexts("Test");
            var buffer = new Buffer(10, 10);
            var area = new Rect(3, 2, 5, 1); // Offset area
            var state = new ListState();

            list.Render(area, buffer, ref state);

            // Check text appears at correct offset
            // Area is Rect(3, 2, 5, 1) so x goes from 3 to 7 (width 5)
            // Highlight symbol "> " takes 2 positions, so text starts at x=5
            Assert.Equal('T', buffer.GetCellSafe(5, 2).Character); // 3 + 2 (highlight symbol)
            Assert.Equal('e', buffer.GetCellSafe(6, 2).Character);
            Assert.Equal('s', buffer.GetCellSafe(7, 2).Character);
            // Position 8 is outside the area (3+5=8), so no 't' there

            // Areas outside should remain empty
            Assert.Equal(' ', buffer.GetCellSafe(0, 0).Character);
            Assert.Equal(' ', buffer.GetCellSafe(2, 2).Character);
        }
    }
}