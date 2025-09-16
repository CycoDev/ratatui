using CycoAI.CycoTui;
using CycoAI.CycoTui.Examples.AutoComplete.Components;

namespace CycoAI.CycoTui.Examples.AutoComplete;

/// <summary>
/// Auto-complete example application entry point.
/// Demonstrates CycoTui Application Framework with file path auto-completion.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        // Check if demo mode is requested
        bool isDemoMode = args.Contains("--demo");

        if (isDemoMode)
        {
            await RunDemoMode();
        }
        else
        {
            await RunInteractiveMode();
        }
    }

    private static async Task RunInteractiveMode()
    {
        // Ensure proper UTF-8 encoding for Unicode box characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Auto-Complete Interactive Example");
        Console.WriteLine("==================================");
        Console.WriteLine("Type commands and use '@' to trigger file completion.");
        Console.WriteLine("Use arrow keys to navigate, Enter to select, Escape to cancel.");
        Console.WriteLine("Type 'exit' or press Ctrl+C to quit.");
        Console.WriteLine();

        try
        {
            // Create the main command prompt widget
            var commandPrompt = new SimpleCommandPrompt();

            // Set up console for raw input
            Console.CancelKeyPress += (sender, e) => {
                Console.WriteLine("\nExiting...");
                Environment.Exit(0);
            };

            // Main input loop
            while (true)
            {
                // Clear the screen and render the current state
                Console.Clear();
                RenderCurrentState(commandPrompt);

                // Read a key
                var keyInfo = Console.ReadKey(true);

                // Convert to our key format and handle input
                var handled = HandleKeyInput(commandPrompt, keyInfo);

                // Check for exit conditions
                if (commandPrompt.State.InputText.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\nGoodbye!");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"\nError: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private static async Task RunDemoMode()
    {
        Console.WriteLine("Starting Auto-Complete Demo...");
        Console.WriteLine("This is a demo of CycoTui Application Framework.");
        Console.WriteLine("The example shows a simplified auto-complete interface.");
        Console.WriteLine();

        try
        {
            // Create the main command prompt widget
            var commandPrompt = new SimpleCommandPrompt();

            // Create a simple demonstration
            await RunSimpleDemo(commandPrompt);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("\nAuto-Complete Demo ended.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static async Task RunSimpleDemo(SimpleCommandPrompt prompt)
    {
        Console.WriteLine("=== Auto-Complete Demo ===");
        Console.WriteLine();
        Console.WriteLine("This demo shows how the auto-complete system would work:");
        Console.WriteLine();

        // Simulate typing sequence
        var demos = new[]
        {
            new { Action = "Type 'hello '", Keys = new[] { "h", "e", "l", "l", "o", " " } },
            new { Action = "Type '@' to trigger completion", Keys = new[] { "@" } },
            new { Action = "Type 'co' to filter", Keys = new[] { "c", "o" } },
            new { Action = "Navigate with arrows", Keys = new[] { "down" } },
            new { Action = "Select with Enter", Keys = new[] { "enter" } }
        };

        foreach (var demo in demos)
        {
            Console.WriteLine($"📝 {demo.Action}:");

            foreach (var key in demo.Keys)
            {
                prompt.SimulateKeyPress(key);
                await Task.Delay(300); // Simulate typing delay
            }

            // If this step activated completion, wait a bit for file system scan
            if (prompt.State.IsCompletionActive)
            {
                await Task.Delay(2000); // Give more time for file system scan
            }

            // Show current state
            var state = prompt.State;
            Console.WriteLine($"   Input: '{state.InputText}'");
            Console.WriteLine($"   Cursor: {state.CursorPosition}");
            Console.WriteLine($"   Completion Active: {state.IsCompletionActive}");
            if (state.IsCompletionActive)
            {
                Console.WriteLine($"   Filter: '{state.CompletionFilter}'");
                Console.WriteLine($"   Selected: {state.SelectedCompletionIndex}");

                // Show what completions are available (if any loaded)
                Console.WriteLine($"   Available completions: {prompt.CompletionItems?.Count ?? 0}");
                if (prompt.CompletionItems?.Count > 0)
                {
                    var filtered = prompt.CompletionItems
                        .Where(item => string.IsNullOrEmpty(state.CompletionFilter) ||
                                     item.SimpleDisplayText.ToLowerInvariant().Contains(state.CompletionFilter.ToLowerInvariant()))
                        .OrderBy(item => item.RelativePath.ToLowerInvariant())
                        .Take(3)
                        .ToList();
                    Console.WriteLine($"   Filtered matches: {filtered.Count}");
                    foreach (var item in filtered)
                    {
                        Console.WriteLine($"     - {item.SimpleDisplayText}");
                    }
                }
            }
            Console.WriteLine();

            await Task.Delay(800);
        }

        Console.WriteLine("🎉 Demo completed!");
        Console.WriteLine();
        Console.WriteLine("In a full implementation, this would:");
        Console.WriteLine("• Render to a terminal buffer with colors and borders");
        Console.WriteLine("• Handle real keyboard input events");
        Console.WriteLine("• Scan the actual file system for completions");
        Console.WriteLine("• Support recursive directory exploration");
        Console.WriteLine("• Include performance optimizations and caching");
        Console.WriteLine();

        Console.WriteLine("The complete implementation plan is available in PLAN.md");
    }

    private static void RenderCurrentState(SimpleCommandPrompt commandPrompt)
    {
        // Simple console-based rendering
        Console.WriteLine("Auto-Complete Interactive Example");
        Console.WriteLine("==================================");
        Console.WriteLine("Type commands and use '@' to trigger file completion.");
        Console.WriteLine("Use arrow keys to navigate, Enter to select, Escape to cancel.");
        Console.WriteLine("Type 'exit' or press Ctrl+C to quit.");
        Console.WriteLine();

        var state = commandPrompt.State;

        // Render the prompt line
        Console.Write("> ");

        // Show input with cursor
        if (string.IsNullOrEmpty(state.InputText))
        {
            Console.Write("█"); // Show cursor at beginning
        }
        else
        {
            for (int i = 0; i < state.InputText.Length; i++)
            {
                if (i == state.CursorPosition)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(state.InputText[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(state.InputText[i]);
                }
            }

            // Show cursor at end if needed
            if (state.CursorPosition >= state.InputText.Length)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("█");
                Console.ResetColor();
            }
        }

        Console.WriteLine();
        Console.WriteLine();

        // Show completion dropdown if active
        if (state.IsCompletionActive)
        {
            Console.WriteLine("Available completions:");

            // Use the actual completion items from the SimpleCommandPrompt
            var completionItems = commandPrompt.CompletionItems;

            if (completionItems.Count > 0)
            {
                var filter = state.CompletionFilter.ToLowerInvariant();
                var filteredItems = completionItems
                    .Where(item => string.IsNullOrEmpty(filter) || item.SimpleDisplayText.ToLowerInvariant().Contains(filter))
                    .OrderBy(item => item.RelativePath.ToLowerInvariant())
                    .ToList();

                // Calculate dropdown width based on all completion items (not just filtered)
                var maxPathLength = completionItems.Count > 0
                    ? completionItems.Max(item => item.SimpleDisplayText.Length)
                    : 20;
                var dropdownWidth = Math.Min(maxPathLength + 4, 120); // Allow up to 120 chars wide, +4 for borders and padding
                var contentWidth = dropdownWidth - 2; // Width for content inside borders

                // Draw top border
                Console.WriteLine("┌" + new string('─', dropdownWidth - 2) + "┐");

                const int maxVisibleItems = 5;
                var selectedIndex = state.SelectedCompletionIndex;

                // Calculate scroll offset to keep selected item visible
                var scrollOffset = 0;
                if (selectedIndex >= maxVisibleItems)
                {
                    scrollOffset = selectedIndex - maxVisibleItems + 1;
                }

                // Get the visible items window
                var visibleItems = filteredItems.Skip(scrollOffset).Take(maxVisibleItems).ToList();

                for (int i = 0; i < visibleItems.Count; i++)
                {
                    var actualIndex = scrollOffset + i;
                    var displayText = visibleItems[i].SimpleDisplayText;

                    // Calculate available space for text: total width - left border (1) - prefix (3) - right border (1) = 5
                    var availableTextWidth = dropdownWidth - 5;
                    var truncatedText = displayText.Length > availableTextWidth
                        ? displayText[..(availableTextWidth - 3)] + "..."
                        : displayText;

                    if (actualIndex == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        // Ensure exact fit within borders - account for " ► " prefix (3 chars)
                        var paddedText = truncatedText.PadRight(availableTextWidth);
                        Console.WriteLine($"│ ► {paddedText}│");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Ensure exact fit within borders - account for "   " prefix (3 chars)
                        var paddedText = truncatedText.PadRight(availableTextWidth);
                        Console.WriteLine($"│   {paddedText}│");
                    }
                }

                // Draw bottom border
                Console.WriteLine("└" + new string('─', dropdownWidth - 2) + "┘");

                // Show scroll indicators
                var scrollInfo = "";
                if (scrollOffset > 0)
                    scrollInfo += "↑ ";
                if (scrollOffset + maxVisibleItems < filteredItems.Count)
                    scrollInfo += "↓ ";

                Console.WriteLine();
                Console.WriteLine($"Filter: '{state.CompletionFilter}' (Showing {selectedIndex + 1}/{filteredItems.Count}) {scrollInfo}");
            }
            else
            {
                Console.WriteLine("│ Loading files...                  │");
                Console.WriteLine("└────────────────────────────────────┘");
                Console.WriteLine();
                Console.WriteLine($"Filter: '{state.CompletionFilter}' (Files loading...)");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Status: " + (state.IsCompletionActive
            ? "↑↓: navigate, Enter: select, Esc: cancel"
            : "Type '@' for file completion, 'exit' to quit"));
    }

    private static bool HandleKeyInput(SimpleCommandPrompt commandPrompt, ConsoleKeyInfo keyInfo)
    {
        // Convert ConsoleKeyInfo to our key format
        string key = keyInfo.Key switch
        {
            ConsoleKey.Enter => "enter",
            ConsoleKey.Escape => "escape",
            ConsoleKey.Backspace => "backspace",
            ConsoleKey.Delete => "delete",
            ConsoleKey.LeftArrow => "left",
            ConsoleKey.RightArrow => "right",
            ConsoleKey.UpArrow => "up",
            ConsoleKey.DownArrow => "down",
            ConsoleKey.Tab => "tab",
            _ => keyInfo.KeyChar.ToString()
        };

        // Handle special keys
        if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
        {
            Console.WriteLine("\nExiting...");
            Environment.Exit(0);
        }

        // Simulate key press
        commandPrompt.SimulateKeyPress(key);

        return true;
    }

    private static string GetFileIcon(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".cs" or ".vb" or ".fs" => "🔷",
            ".js" or ".ts" => "🟨",
            ".py" => "🐍",
            ".java" or ".kt" => "☕",
            ".c" or ".cpp" or ".h" => "🔧",
            ".rs" => "🦀",
            ".html" or ".xml" => "🌐",
            ".css" or ".scss" => "🎨",
            ".json" or ".yaml" or ".yml" or ".toml" => "⚙️",
            ".sql" or ".db" => "🗄️",
            ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" => "🖼️",
            ".mp3" or ".wav" or ".ogg" => "🎵",
            ".mp4" or ".avi" or ".mkv" => "🎬",
            ".zip" or ".tar" or ".gz" or ".rar" => "📦",
            ".md" or ".txt" => "📄",
            ".sh" or ".bat" => "📜",
            _ => "📄"
        };
    }
}