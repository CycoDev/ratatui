using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Widgets;
using CycoAI.CycoTui.Examples.AutoComplete.Models;

namespace CycoAI.CycoTui.Examples.AutoComplete.Components;

/// <summary>
/// A dropdown widget that displays file completion suggestions.
/// </summary>
public class FileCompletionDropdown : IWidget
{
    private readonly CommandState _commandState;
    private readonly FileSystemExplorer _fileExplorer;
    private List<CompletionItem> _completionItems = new();
    private int _scrollOffset = 0;
    private bool _isLoading = false;
    private string _lastFilter = string.Empty;
    private DateTime _lastUpdateTime = DateTime.MinValue;

    /// <summary>
    /// Maximum number of items to display at once.
    /// </summary>
    public int MaxVisibleItems { get; set; } = 10;

    /// <summary>
    /// Minimum time between completion updates (debouncing).
    /// </summary>
    public TimeSpan UpdateDebounceTime { get; set; } = TimeSpan.FromMilliseconds(150);

    /// <summary>
    /// Initializes a new instance of the FileCompletionDropdown class.
    /// </summary>
    /// <param name="commandState">The command state to reference.</param>
    /// <param name="fileExplorer">The file system explorer to use.</param>
    public FileCompletionDropdown(CommandState commandState, FileSystemExplorer fileExplorer)
    {
        _commandState = commandState ?? throw new ArgumentNullException(nameof(commandState));
        _fileExplorer = fileExplorer ?? throw new ArgumentNullException(nameof(fileExplorer));
    }

    /// <summary>
    /// Renders the dropdown to the buffer.
    /// </summary>
    /// <param name="area">The area to render within.</param>
    /// <param name="buffer">The buffer to render to.</param>
    public void Render(Rect area, IBuffer buffer)
    {
        if (!_commandState.IsCompletionActive || area.Width < 3 || area.Height < 1)
            return;

        // Update completion items if needed
        UpdateCompletionItemsIfNeeded();

        // Render the dropdown border and background
        RenderDropdownBorder(area, buffer);

        // Render completion items
        RenderCompletionItems(area, buffer);

        // Render loading indicator if needed
        if (_isLoading)
        {
            RenderLoadingIndicator(area, buffer);
        }

        // Render scroll indicators if needed
        RenderScrollIndicators(area, buffer);
    }

    /// <summary>
    /// Handles keyboard input for the dropdown.
    /// </summary>
    /// <param name="keyEvent">The keyboard event.</param>
    /// <returns>True if the event was handled.</returns>
    public bool HandleKeyInput(KeyboardEvent keyEvent)
    {
        if (!_commandState.IsCompletionActive)
            return false;

        switch (keyEvent.Key)
        {
            case KeyCode.Up:
                MoveToPreviousItem();
                return true;

            case KeyCode.Down:
                MoveToNextItem();
                return true;

            case KeyCode.Enter:
                SelectCurrentItem();
                return true;

            case KeyCode.Escape:
                _commandState.DeactivateCompletion();
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Forces an immediate update of completion items.
    /// </summary>
    public async Task UpdateCompletionItemsAsync()
    {
        if (!_commandState.IsCompletionActive)
        {
            _completionItems.Clear();
            return;
        }

        var filter = _commandState.CompletionFilter;

        // Avoid unnecessary updates
        if (filter == _lastFilter && DateTime.UtcNow - _lastUpdateTime < UpdateDebounceTime)
            return;

        _isLoading = true;
        _lastFilter = filter;
        _lastUpdateTime = DateTime.UtcNow;

        try
        {
            var items = await _fileExplorer.GetCompletionItemsAsync(filter);
            _completionItems = items;

            // Reset selection to first item
            _commandState.SelectedCompletionIndex = 0;
            _scrollOffset = 0;
        }
        catch (Exception)
        {
            // In a real implementation, we'd log this error
            _completionItems.Clear();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void UpdateCompletionItemsIfNeeded()
    {
        var filter = _commandState.CompletionFilter;
        if (filter != _lastFilter || DateTime.UtcNow - _lastUpdateTime > UpdateDebounceTime)
        {
            // Fire and forget - we'll update async
            _ = Task.Run(UpdateCompletionItemsAsync);
        }
    }

    private void RenderDropdownBorder(Rect area, IBuffer buffer)
    {
        var borderCell = new Cell(' ', Color.Gray, Color.Black, Modifier.None);
        var backgroundCell = new Cell(' ', Color.White, Color.Black, Modifier.None);

        // Top border
        if (area.Height > 0)
        {
            buffer[area.X, area.Y] = borderCell.WithCharacter('┌');
            for (int x = area.X + 1; x < area.X + area.Width - 1; x++)
            {
                buffer[x, area.Y] = borderCell.WithCharacter('─');
            }
            if (area.Width > 1)
            {
                buffer[area.X + area.Width - 1, area.Y] = borderCell.WithCharacter('┐');
            }
        }

        // Sides and background
        for (int y = area.Y + 1; y < area.Y + area.Height - 1; y++)
        {
            buffer[area.X, y] = borderCell.WithCharacter('│');
            for (int x = area.X + 1; x < area.X + area.Width - 1; x++)
            {
                buffer[x, y] = backgroundCell;
            }
            if (area.Width > 1)
            {
                buffer[area.X + area.Width - 1, y] = borderCell.WithCharacter('│');
            }
        }

        // Bottom border
        if (area.Height > 1)
        {
            var bottomY = area.Y + area.Height - 1;
            buffer[area.X, bottomY] = borderCell.WithCharacter('└');
            for (int x = area.X + 1; x < area.X + area.Width - 1; x++)
            {
                buffer[x, bottomY] = borderCell.WithCharacter('─');
            }
            if (area.Width > 1)
            {
                buffer[area.X + area.Width - 1, bottomY] = borderCell.WithCharacter('┘');
            }
        }
    }

    private void RenderCompletionItems(Rect area, IBuffer buffer)
    {
        var contentArea = new Rect(area.X + 1, area.Y + 1, area.Width - 2, area.Height - 2);

        if (contentArea.Width <= 0 || contentArea.Height <= 0)
            return;

        var normalCell = new Cell(' ', Color.White, Color.Black, Modifier.None);
        var selectedCell = new Cell(' ', Color.Black, Color.Yellow, Modifier.None);

        // Calculate visible items
        var startIndex = _scrollOffset;
        var endIndex = Math.Min(startIndex + contentArea.Height, _completionItems.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            var item = _completionItems[i];
            var y = contentArea.Y + (i - startIndex);
            var isSelected = i == _commandState.SelectedCompletionIndex;
            var cellStyle = isSelected ? selectedCell : normalCell;

            // Render the item
            var displayText = item.DisplayText;
            var maxLength = contentArea.Width;

            // Truncate if necessary
            if (displayText.Length > maxLength)
            {
                displayText = displayText[..(maxLength - 3)] + "...";
            }

            // Render the text
            for (int x = 0; x < Math.Min(displayText.Length, maxLength); x++)
            {
                buffer[contentArea.X + x, y] = cellStyle.WithCharacter(displayText[x]);
            }

            // Fill remaining space with background
            for (int x = displayText.Length; x < maxLength; x++)
            {
                buffer[contentArea.X + x, y] = cellStyle;
            }
        }

        // Show "no items" message if empty
        if (_completionItems.Count == 0 && !_isLoading)
        {
            var message = "No matches found";
            var messageCell = new Cell(' ', Color.Gray, Color.Black, Modifier.None);

            for (int x = 0; x < Math.Min(message.Length, contentArea.Width); x++)
            {
                buffer[contentArea.X + x, contentArea.Y] = messageCell.WithCharacter(message[x]);
            }
        }
    }

    private void RenderLoadingIndicator(Rect area, IBuffer buffer)
    {
        var contentArea = new Rect(area.X + 1, area.Y + 1, area.Width - 2, area.Height - 2);

        if (contentArea.Width <= 0 || contentArea.Height <= 0)
            return;

        var loadingCell = new Cell(' ', Color.Blue, Color.Black, Modifier.None);
        var message = "Loading...";

        for (int x = 0; x < Math.Min(message.Length, contentArea.Width); x++)
        {
            buffer[contentArea.X + x, contentArea.Y] = loadingCell.WithCharacter(message[x]);
        }
    }

    private void RenderScrollIndicators(Rect area, IBuffer buffer)
    {
        if (_completionItems.Count <= MaxVisibleItems)
            return;

        var contentArea = new Rect(area.X + 1, area.Y + 1, area.Width - 2, area.Height - 2);
        var indicatorCell = new Cell(' ', Color.Blue, Color.Black, Modifier.None);

        // Up arrow if we can scroll up
        if (_scrollOffset > 0)
        {
            buffer[area.X + area.Width - 1, area.Y + 1] = indicatorCell.WithCharacter('▲');
        }

        // Down arrow if we can scroll down
        if (_scrollOffset + contentArea.Height < _completionItems.Count)
        {
            buffer[area.X + area.Width - 1, area.Y + area.Height - 2] = indicatorCell.WithCharacter('▼');
        }
    }

    private void MoveToPreviousItem()
    {
        if (_completionItems.Count == 0)
            return;

        if (_commandState.SelectedCompletionIndex > 0)
        {
            _commandState.SelectedCompletionIndex--;
        }
        else
        {
            // Wrap to last item
            _commandState.SelectedCompletionIndex = _completionItems.Count - 1;
        }

        UpdateScrollOffset();
    }

    private void MoveToNextItem()
    {
        if (_completionItems.Count == 0)
            return;

        if (_commandState.SelectedCompletionIndex < _completionItems.Count - 1)
        {
            _commandState.SelectedCompletionIndex++;
        }
        else
        {
            // Wrap to first item
            _commandState.SelectedCompletionIndex = 0;
        }

        UpdateScrollOffset();
    }

    private void SelectCurrentItem()
    {
        if (_completionItems.Count == 0 ||
            _commandState.SelectedCompletionIndex < 0 ||
            _commandState.SelectedCompletionIndex >= _completionItems.Count)
            return;

        var selectedItem = _completionItems[_commandState.SelectedCompletionIndex];
        _commandState.InsertCompletion(selectedItem.CompletionText);
    }

    private void UpdateScrollOffset()
    {
        var visibleHeight = MaxVisibleItems;

        // Ensure selected item is visible
        if (_commandState.SelectedCompletionIndex < _scrollOffset)
        {
            _scrollOffset = _commandState.SelectedCompletionIndex;
        }
        else if (_commandState.SelectedCompletionIndex >= _scrollOffset + visibleHeight)
        {
            _scrollOffset = _commandState.SelectedCompletionIndex - visibleHeight + 1;
        }

        // Ensure scroll offset is valid
        _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, _completionItems.Count - visibleHeight));
    }
}