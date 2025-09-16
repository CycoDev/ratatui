using CycoAI.CycoTui.Core.Buffer;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
using CycoAI.CycoTui.Core.Widgets;
using CycoAI.CycoTui.Examples.AutoComplete.Models;

namespace CycoAI.CycoTui.Examples.AutoComplete.Components;

/// <summary>
/// A simplified command prompt widget for the auto-complete example.
/// This version works with the current CycoTui infrastructure.
/// </summary>
public class SimpleCommandPrompt : IWidget, IDisposable
{
    private readonly CommandState _state = new();
    private readonly FileSystemExplorer _fileExplorer = new()
    {
        MaxItems = 500,        // Show more items
        MaxScanDepth = 5,      // Scan deeper
        RecursiveScan = true,  // Ensure recursive scanning
        IncludeHidden = false  // Don't include hidden files
    };
    private List<CompletionItem> _completionItems = new();
    private List<CompletionItem> _filteredSortedItems = new();
    private DateTime _lastUpdateTime = DateTime.MinValue;
    private string _lastFilter = string.Empty;
    private int _dropdownWidth = 20; // Pre-calculated dropdown width based on all items

    /// <summary>
    /// Renders the command prompt to the buffer.
    /// </summary>
    /// <param name="area">The area to render within.</param>
    /// <param name="buffer">The buffer to render to.</param>
    public void Render(Rect area, IBuffer buffer)
    {
        // Clear the area with black background
        var defaultCell = new Cell(' ', Color.White, Color.Black, Modifier.None);
        for (int y = area.Y; y < area.Y + area.Height; y++)
        {
            for (int x = area.X; x < area.X + area.Width; x++)
            {
                buffer[x, y] = defaultCell;
            }
        }

        if (area.Height < 1 || area.Width < 3)
            return;

        // Render the prompt line
        RenderPromptLine(area, buffer);

        // Render the dropdown if active
        if (_state.IsCompletionActive)
        {
            RenderSimpleDropdown(area, buffer);
        }

        // Render status line
        RenderStatusLine(area, buffer);

        // Render instructions at the top
        RenderInstructions(area, buffer);
    }

    private void RenderPromptLine(Rect area, IBuffer buffer)
    {
        var promptY = area.Y + 3; // Leave room for instructions
        if (promptY >= area.Y + area.Height)
            return;

        var promptCell = new Cell('>', Color.Green, Color.Black, Modifier.None);
        var inputCell = new Cell(' ', Color.White, Color.Black, Modifier.None);
        var cursorCell = new Cell('█', Color.Black, Color.Yellow, Modifier.None);

        // Render the prompt symbol
        buffer[area.X, promptY] = promptCell;
        buffer[area.X + 1, promptY] = inputCell;

        // Render the input text
        var inputStartX = area.X + 2;
        var availableWidth = Math.Max(0, area.Width - 2);
        var inputText = _state.InputText;

        // Simple rendering - no scrolling for now
        for (int i = 0; i < Math.Min(inputText.Length, availableWidth); i++)
        {
            var x = inputStartX + i;
            var cell = (i == _state.CursorPosition) ? cursorCell.WithCharacter(inputText[i]) : inputCell.WithCharacter(inputText[i]);
            buffer[x, promptY] = cell;
        }

        // Render cursor if at end
        if (_state.CursorPosition == inputText.Length && _state.CursorPosition < availableWidth)
        {
            var x = inputStartX + _state.CursorPosition;
            buffer[x, promptY] = cursorCell;
        }
    }

    private void RenderSimpleDropdown(Rect area, IBuffer buffer)
    {
        var dropdownY = area.Y + 5; // Below prompt line
        if (dropdownY >= area.Y + area.Height - 2)
            return;

        var borderCell = new Cell(' ', Color.Cyan, Color.Black, Modifier.None);
        var itemCell = new Cell(' ', Color.White, Color.Black, Modifier.None);
        var selectedCell = new Cell(' ', Color.Black, Color.Yellow, Modifier.None);

        // Update completion items if needed
        UpdateCompletionItemsIfNeeded();

        // Get filtered and sorted files
        UpdateFilteredSortedItems();

        if (_filteredSortedItems.Count == 0)
            return;

        const int maxVisibleItems = 5;
        var selectedIndex = _state.SelectedCompletionIndex;

        // Calculate scroll offset to keep selected item visible
        var scrollOffset = 0;
        if (selectedIndex >= maxVisibleItems)
        {
            scrollOffset = selectedIndex - maxVisibleItems + 1;
        }

        // Get the visible items window
        var visibleItems = _filteredSortedItems.Skip(scrollOffset).Take(maxVisibleItems).ToList();

        // Use pre-calculated dropdown width
        var dropdownWidth = _dropdownWidth;
        var dropdownHeight = visibleItems.Count + 2; // +2 for borders

        // Draw border
        buffer[area.X, dropdownY] = borderCell.WithCharacter('┌');
        for (int x = 1; x < dropdownWidth - 1; x++)
        {
            buffer[area.X + x, dropdownY] = borderCell.WithCharacter('─');
        }
        buffer[area.X + dropdownWidth - 1, dropdownY] = borderCell.WithCharacter('┐');

        // Draw items
        for (int i = 0; i < visibleItems.Count; i++)
        {
            var y = dropdownY + 1 + i;
            var actualIndex = scrollOffset + i;
            var isSelected = actualIndex == selectedIndex;
            var cellStyle = isSelected ? selectedCell : itemCell;

            buffer[area.X, y] = borderCell.WithCharacter('│');

            var item = visibleItems[i].SimpleDisplayText;
            var displayText = item.Length > dropdownWidth - 3 ? item[..(dropdownWidth - 6)] + "..." : item;

            for (int x = 0; x < Math.Min(displayText.Length, dropdownWidth - 2); x++)
            {
                buffer[area.X + 1 + x, y] = cellStyle.WithCharacter(displayText[x]);
            }

            // Fill remaining space
            for (int x = displayText.Length; x < dropdownWidth - 2; x++)
            {
                buffer[area.X + 1 + x, y] = cellStyle;
            }

            buffer[area.X + dropdownWidth - 1, y] = borderCell.WithCharacter('│');
        }

        // Bottom border
        var bottomY = dropdownY + visibleItems.Count + 1;
        buffer[area.X, bottomY] = borderCell.WithCharacter('└');
        for (int x = 1; x < dropdownWidth - 1; x++)
        {
            buffer[area.X + x, bottomY] = borderCell.WithCharacter('─');
        }
        buffer[area.X + dropdownWidth - 1, bottomY] = borderCell.WithCharacter('┘');

        // Show scroll indicators
        if (bottomY + 1 < area.Y + area.Height)
        {
            var scrollInfoY = bottomY + 1;
            var statusCell = new Cell(' ', Color.Gray, Color.Black, Modifier.None);

            var scrollInfo = $"({selectedIndex + 1}/{_filteredSortedItems.Count})";
            if (scrollOffset > 0)
                scrollInfo += " ↑";
            if (scrollOffset + maxVisibleItems < _filteredSortedItems.Count)
                scrollInfo += " ↓";

            for (int i = 0; i < Math.Min(scrollInfo.Length, dropdownWidth); i++)
            {
                buffer[area.X + i, scrollInfoY] = statusCell.WithCharacter(scrollInfo[i]);
            }
        }
    }

    private void RenderStatusLine(Rect area, IBuffer buffer)
    {
        if (area.Height < 2)
            return;

        var statusY = area.Y + area.Height - 1;
        var statusCell = new Cell(' ', Color.Gray, Color.Black, Modifier.None);

        var statusText = _state.IsCompletionActive
            ? "↑↓: navigate, Enter: select, Esc: cancel"
            : "Type '@' for file completion, 'exit' to quit";

        for (int i = 0; i < Math.Min(statusText.Length, area.Width); i++)
        {
            buffer[area.X + i, statusY] = statusCell.WithCharacter(statusText[i]);
        }
    }

    private void RenderInstructions(Rect area, IBuffer buffer)
    {
        var instructionCell = new Cell(' ', Color.Cyan, Color.Black, Modifier.None);
        var instructions = new[]
        {
            "Auto-Complete Example - Type text and use '@' to trigger completion",
            "Commands: 'exit' to quit, arrow keys to navigate",
        };

        for (int i = 0; i < Math.Min(instructions.Length, area.Height - 4); i++)
        {
            var text = instructions[i];
            for (int x = 0; x < Math.Min(text.Length, area.Width); x++)
            {
                buffer[area.X + x, area.Y + i] = instructionCell.WithCharacter(text[x]);
            }
        }
    }

    /// <summary>
    /// Simulates key input for testing (since we don't have full event system integration yet).
    /// </summary>
    /// <param name="key">The key pressed.</param>
    public void SimulateKeyPress(string key)
    {
        switch (key.ToLowerInvariant())
        {
            case "enter":
                HandleEnter();
                break;
            case "escape":
                _state.DeactivateCompletion();
                break;
            case "up":
                if (_state.IsCompletionActive)
                {
                    UpdateFilteredSortedItems();
                    if (_filteredSortedItems.Count > 0)
                    {
                        _state.SelectedCompletionIndex = (_state.SelectedCompletionIndex - 1 + _filteredSortedItems.Count) % _filteredSortedItems.Count;
                    }
                }
                break;
            case "down":
                if (_state.IsCompletionActive)
                {
                    UpdateFilteredSortedItems();
                    if (_filteredSortedItems.Count > 0)
                    {
                        _state.SelectedCompletionIndex = (_state.SelectedCompletionIndex + 1) % _filteredSortedItems.Count;
                    }
                }
                break;
            case "backspace":
                if (_state.CursorPosition > 0)
                {
                    _state.InputText = _state.InputText.Remove(_state.CursorPosition - 1, 1);
                    _state.CursorPosition--;
                    CheckCompletionTrigger();
                }
                break;
            case "@":
                _state.InputText = _state.InputText.Insert(_state.CursorPosition, "@");
                _state.CursorPosition++;
                CheckCompletionTrigger();
                break;
            default:
                if (key.Length == 1)
                {
                    _state.InputText = _state.InputText.Insert(_state.CursorPosition, key);
                    _state.CursorPosition++;
                    CheckCompletionTrigger();
                }
                break;
        }
    }

    private void HandleEnter()
    {
        if (_state.IsCompletionActive)
        {
            // Select completion item
            UpdateFilteredSortedItems();
            if (_state.SelectedCompletionIndex < _filteredSortedItems.Count)
            {
                var selectedItem = _filteredSortedItems[_state.SelectedCompletionIndex];
                _state.InsertCompletion(selectedItem.CompletionText);
            }
        }
        else
        {
            // Execute command
            var command = _state.InputText.Trim();
            if (command.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Environment.Exit(0);
            }
            else
            {
                _state.AddToHistory(command);
                _state.Reset();
            }
        }
    }

    private void CheckCompletionTrigger()
    {
        var atPosition = _state.InputText.LastIndexOf('@', Math.Max(0, _state.CursorPosition - 1));
        if (atPosition >= 0)
        {
            _state.ActivateCompletion();

            // Force immediate load of completion items if empty
            if (_completionItems.Count == 0)
            {
                // For demo/testing purposes, do a synchronous load
                try
                {
                    var task = _fileExplorer.GetCompletionItemsAsync();
                    task.Wait(1000); // Wait up to 1 second
                    if (task.IsCompletedSuccessfully)
                    {
                        _completionItems = task.Result;
                        CalculateDropdownWidth();
                        UpdateFilteredSortedItems();
                        _state.SelectedCompletionIndex = 0;
                    }
                }
                catch (Exception)
                {
                    _completionItems.Clear();
                    CalculateDropdownWidth();

                    // Fallback to async load
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var items = await _fileExplorer.GetCompletionItemsAsync();
                            _completionItems = items;
                            CalculateDropdownWidth();
                            UpdateFilteredSortedItems();
                            _state.SelectedCompletionIndex = 0;
                        }
                        catch (Exception)
                        {
                            _completionItems.Clear();
                            CalculateDropdownWidth();
                        }
                    });
                }
            }
        }
        else
        {
            _state.DeactivateCompletion();
        }
    }

    /// <summary>
    /// Gets the current state for external access.
    /// </summary>
    public CommandState State => _state;

    /// <summary>
    /// Gets the current completion items for testing/debugging.
    /// </summary>
    public IReadOnlyList<CompletionItem> CompletionItems => _completionItems.AsReadOnly();

    /// <summary>
    /// Updates completion items from the file system if needed.
    /// </summary>
    private void UpdateCompletionItemsIfNeeded()
    {
        if (!_state.IsCompletionActive)
        {
            return;
        }

        var filter = _state.CompletionFilter;
        var now = DateTime.UtcNow;

        // Update if filter changed or it's been a while since last update
        if (filter != _lastFilter || (now - _lastUpdateTime).TotalMilliseconds > 500)
        {
            _lastFilter = filter;
            _lastUpdateTime = now;

            // Fire and forget async update
            _ = Task.Run(async () =>
            {
                try
                {
                    var items = await _fileExplorer.GetCompletionItemsAsync(filter);
                    _completionItems = items;
                    CalculateDropdownWidth();
                    UpdateFilteredSortedItems();

                    // Reset selection to first item
                    _state.SelectedCompletionIndex = 0;
                }
                catch (Exception)
                {
                    // In case of error, clear the list
                    _completionItems.Clear();
                    CalculateDropdownWidth();
                }
            });
        }
    }

    /// <summary>
    /// Updates the filtered and sorted items list based on current filter.
    /// </summary>
    private void UpdateFilteredSortedItems()
    {
        var filter = _state.CompletionFilter.ToLowerInvariant();
        _filteredSortedItems = _completionItems
            .Where(item => string.IsNullOrEmpty(filter) || item.SimpleDisplayText.ToLowerInvariant().Contains(filter))
            .OrderBy(item => item.RelativePath.ToLowerInvariant())
            .ToList();

        // Ensure selected index is within bounds
        if (_state.SelectedCompletionIndex >= _filteredSortedItems.Count)
        {
            _state.SelectedCompletionIndex = Math.Max(0, _filteredSortedItems.Count - 1);
        }
    }

    /// <summary>
    /// Calculates and caches the dropdown width based on all completion items.
    /// </summary>
    private void CalculateDropdownWidth()
    {
        var maxPathLength = _completionItems.Count > 0
            ? _completionItems.Max(item => item.SimpleDisplayText.Length)
            : 20;
        _dropdownWidth = Math.Min(maxPathLength + 4, 120); // Allow up to 120 chars wide, +4 for borders and padding
    }

    /// <summary>
    /// Disposes of resources used by the command prompt.
    /// </summary>
    public void Dispose()
    {
        _fileExplorer?.Dispose();
    }
}