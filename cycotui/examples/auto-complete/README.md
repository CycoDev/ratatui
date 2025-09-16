# Auto-Complete Example Application

A terminal-based console application that demonstrates CycoTui's Application Framework with an interactive command prompt featuring file path auto-completion.

## Features

- **Interactive Command Prompt**: Type commands with a '>' prompt
- **File Auto-Completion**: Type '@' to trigger file and directory completion
- **Recursive File Scanning**: Shows files from current directory and all subdirectories (up to 5 levels deep)
- **Flat File List**: All files and folders shown in a single list with relative paths
- **Scrollable Navigation**: Use arrow keys to scroll through all completion items
- **Smart Filtering**: Completion items are filtered and sorted alphabetically as you type
- **Clean Text Display**: No emojis or icons - just clean file paths with trailing slashes for directories
- **Responsive UI**: Dropdown box automatically sizes to fit the longest file path
- **Cross-Platform**: Works on Windows, Linux, and macOS
- **Performance Optimized**: Async file scanning with caching (up to 500 items)

## Usage

### Running the Application

**Interactive Mode (Default):**
```bash
cd cycotui/examples/auto-complete
dotnet run
```

**Demo Mode:**
```bash
cd cycotui/examples/auto-complete
dotnet run -- --demo
```

### Interactive Mode Commands

1. **Start typing**: Enter any text at the prompt
2. **Trigger completion**: Type '@' to show file completion dropdown
3. **Navigate**: Use Up/Down arrow keys to select items
4. **Select**: Press Enter to insert the selected file path
5. **Cancel**: Press Escape to close the dropdown
6. **Exit**: Type 'exit' or press Ctrl+C to quit

### Demo Mode

Demo mode shows a pre-scripted demonstration of the auto-completion functionality:
- Simulates typing "hello @do"
- Shows completion activation and filtering
- Demonstrates navigation and selection
- Perfect for understanding the feature flow without interaction

### Example Usage Flow

```
> Hello @
┌─────────────────────────────────────────────────────────────────────────────┐
│ ► auto-complete.csproj                                                      │
│   bin/                                                                      │
│   bin/Debug/                                                                │
│   bin/Debug/net9.0/                                                         │
│   bin/Debug/net9.0/auto-complete                                            │
└─────────────────────────────────────────────────────────────────────────────┘
Filter: '' (Showing 1/85) ↓

Navigate with arrows, press Enter to select:
> Hello auto-complete.csproj
```

**Recursive Scanning**: The completion shows files from:
- Current directory: `README.md`, `Program.cs`
- Subdirectories: `Components/`, `Models/`
- Nested subdirectories: `bin/Debug/net9.0/auto-complete`

All files are shown in a flat list with their relative paths.

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `@` | Trigger file completion dropdown |
| `↑` / `↓` | Navigate completion items (when dropdown is open) |
| `Enter` | Select completion item / Execute command |
| `Escape` | Cancel completion / Clear input |
| `←` / `→` | Move cursor in input text |
| `Backspace` | Delete character before cursor |
| `Delete` | Delete character after cursor |
| `Ctrl+C` | Exit application |
| `exit` | Exit application |

## Architecture

### Components

- **CommandPrompt**: Main widget handling user input and display
- **FileCompletionDropdown**: Dropdown widget showing file suggestions
- **FileSystemExplorer**: Background service for scanning directories
- **CommandState**: State management for input and completion
- **CompletionItem**: Model representing files and directories

### Key Features

- **Async File Scanning**: Non-blocking directory traversal
- **Smart Caching**: Caches directory contents to improve performance
- **Debounced Updates**: Prevents excessive file system calls
- **Error Handling**: Gracefully handles permissions and file system errors
- **Memory Efficient**: Limits results and uses streaming approaches

## Configuration

The application includes several configurable options:

### FileSystemExplorer Settings

```csharp
var explorer = new FileSystemExplorer
{
    MaxScanDepth = 3,        // Maximum recursion depth
    MaxItems = 100,          // Maximum completion items
    IncludeHidden = false,   // Show hidden files/directories
    RecursiveScan = true,    // Scan subdirectories
    CacheExpiration = TimeSpan.FromMinutes(5)
};
```

### Dropdown Settings

```csharp
var dropdown = new FileCompletionDropdown(state, explorer)
{
    MaxVisibleItems = 10,    // Items shown at once
    UpdateDebounceTime = TimeSpan.FromMilliseconds(150)
};
```

## Display Format

The application displays files and directories in a clean text format:

| Type | Display Format |
|------|----------------|
| Directories | `folder/` (with trailing slash) |
| Files | `filename.ext` (with extension) |
| Nested items | `path/to/file.ext` (with relative path) |

All items are sorted alphabetically and shown with their full relative path from the current directory.

## Performance

The application is optimized for performance:

- **Lazy Loading**: Only scans directories when needed
- **Background Processing**: File system operations run asynchronously
- **Result Limiting**: Limits results to prevent UI overflow
- **Smart Caching**: Caches directory contents with expiration
- **Debouncing**: Prevents excessive updates during rapid typing

## Troubleshooting

### Common Issues

1. **No completion items shown**: Check directory permissions
2. **Slow performance**: Reduce `MaxScanDepth` or `MaxItems`
3. **Hidden files not shown**: Set `IncludeHidden = true`
4. **Application crashes**: Check file system permissions and available memory

### Debug Information

The application provides cache statistics for debugging:

```csharp
var stats = fileExplorer.GetCacheStatistics();
Console.WriteLine($"Cache entries: {stats.TotalEntries}");
Console.WriteLine($"Valid entries: {stats.ValidEntries}");
Console.WriteLine($"Total items: {stats.TotalItems}");
Console.WriteLine($"Hit ratio: {stats.CacheHitRatio:P}");
```

## Extension Points

The application is designed to be extensible:

### Custom Completion Sources

Implement additional completion sources beyond file system:

```csharp
public interface ICompletionSource
{
    Task<List<CompletionItem>> GetCompletionsAsync(string filter);
}
```

### Custom Display Formatting

Extend the `CompletionItem.SimpleDisplayText` property to customize how files and directories are displayed.

### Theme Customization

Modify the `Style` objects in the render methods to customize colors and appearance.

## Dependencies

- **CycoAI.CycoTui**: Main application framework
- **CycoAI.CycoTui.Core**: Core widgets and layout system
- **.NET 9.0**: Target framework

## Building from Source

```bash
# Clone the repository
git clone [repository-url]
cd ratatui/cycotui/examples/auto-complete

# Build the project
dotnet build

# Run the application
dotnet run
```

## License

This example is part of the CycoTui project and follows the same license terms.