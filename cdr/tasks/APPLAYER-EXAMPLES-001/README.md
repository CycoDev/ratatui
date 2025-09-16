# Comprehensive Example Applications Implementation

## Overview

This task creates comprehensive example applications that demonstrate CycoTui usage patterns from simple Hello World scenarios to complex interactive dashboards. These examples serve as both learning materials for developers and validation tests for the application framework functionality.

## Implementation Approach

1. **Create Progressive Learning Examples**:
   - Implement Hello World and basic text display examples
   - Build simple interactive applications with input handling
   - Create layout demonstration applications showing different arrangements
   - Develop complex multi-widget applications with state management

2. **Build Real-World Application Examples**:
   - Create system monitoring dashboard with live metrics
   - Implement file browser with navigation and operations
   - Build interactive forms with validation and submission
   - Create terminal-based games and productivity tools

3. **Demonstrate Integration Patterns**:
   - Show integration with existing .NET CLI applications
   - Create examples using dependency injection and configuration
   - Implement hosted service integration for long-running applications
   - Demonstrate plugin system usage and extension patterns

4. **Create Performance and Feature Showcases**:
   - Build stress test applications showing performance capabilities
   - Create feature demonstration apps highlighting widget capabilities
   - Implement cross-platform compatibility examples
   - Develop accessibility and internationalization examples

## Key Challenges

1. **Learning Progression**: Designing examples that build complexity gradually
2. **Real-World Relevance**: Creating examples that solve actual developer problems
3. **Code Quality**: Maintaining high-quality, well-documented example code
4. **Platform Coverage**: Ensuring examples work across all supported platforms
5. **Maintenance**: Keeping examples current with API changes and best practices

## Implementation Notes

### Example Application Categories

```csharp
// Category 1: Getting Started Examples
public static class GettingStarted
{
    // examples/01-hello-world/Program.cs
    public static async Task HelloWorld()
    {
        await CycoTui.RunAsync("Hello, World!");
    }

    // examples/02-basic-widget/Program.cs
    public static async Task BasicWidget()
    {
        var widget = Widgets.Block("Welcome")
            .WithContent(Widgets.Text("Welcome to CycoTui!").Bold())
            .WithBorder(BorderType.Rounded);

        await CycoTui.RunAsync(widget);
    }

    // examples/03-simple-layout/Program.cs
    public static async Task SimpleLayout()
    {
        await CycoTui.CreateApp()
            .WithLayout(layout =>
            {
                layout.Vertical(
                    Widgets.Text("Header").WithBackground(Colors.Blue),
                    Widgets.Text("Content Area").Fill(),
                    Widgets.Text("Footer").WithBackground(Colors.Gray)
                );
            })
            .RunAsync();
    }

    // examples/04-input-handling/Program.cs
    public static async Task InputHandling()
    {
        var counter = 0;
        var text = Widgets.Text($"Count: {counter}");

        await CycoTui.CreateApp(text)
            .WithKeyHandler<KeyEvent>(async key =>
            {
                if (key.Key == Key.Space)
                {
                    counter++;
                    text.Content = $"Count: {counter}";
                }
                else if (key.Key == Key.Q)
                {
                    Application.Current.Stop();
                }
            })
            .RunAsync();
    }
}
```

### Intermediate Examples

```csharp
// Category 2: Interactive Applications
public static class InteractiveExamples
{
    // examples/05-todo-app/Program.cs
    public class TodoApp
    {
        private readonly List<TodoItem> items = new();
        private readonly List<TodoItem> widget;

        public async Task RunAsync()
        {
            widget = Widgets.List(items, item => $"[{(item.Done ? "x" : " ")}] {item.Text}");

            await CycoTui.CreateApp()
                .WithLayout(BuildLayout)
                .WithKeyHandler<KeyEvent>(HandleKey)
                .RunAsync();
        }

        private void BuildLayout(ILayoutBuilder layout)
        {
            layout.Vertical(
                Widgets.Block("Todo List")
                    .WithContent(widget),
                Widgets.Text("Space: Toggle | A: Add | D: Delete | Q: Quit")
                    .WithBackground(Colors.Theme.Secondary)
            );
        }

        private async Task HandleKey(KeyEvent key)
        {
            switch (key.Key)
            {
                case Key.Space:
                    ToggleSelected();
                    break;
                case Key.A:
                    await AddItem();
                    break;
                case Key.D:
                    DeleteSelected();
                    break;
                case Key.Q:
                    Application.Current.Stop();
                    break;
            }
        }
    }

    // examples/06-file-browser/Program.cs
    public class FileBrowser
    {
        private string currentPath = Environment.CurrentDirectory;
        private readonly List<FileInfo> widget;

        public async Task RunAsync()
        {
            RefreshFileList();

            await CycoTui.CreateApp()
                .WithLayout(BuildLayout)
                .EnableMouse()
                .WithKeyHandler<KeyEvent>(HandleKey)
                .RunAsync();
        }

        private void BuildLayout(ILayoutBuilder layout)
        {
            layout.Vertical(
                Widgets.Text($"Path: {currentPath}").WithBackground(Colors.Blue),
                Widgets.Block("Files")
                    .WithContent(widget)
                    .Fill(),
                Widgets.Text("Enter: Open | Backspace: Up | Q: Quit")
                    .WithBackground(Colors.Gray)
            );
        }
    }
}
```

### Advanced Examples

```csharp
// Category 3: Complex Applications
public static class AdvancedExamples
{
    // examples/07-dashboard/Program.cs
    public class SystemDashboard
    {
        private readonly PerformanceCounter cpuCounter;
        private readonly PerformanceCounter memoryCounter;
        private readonly Chart cpuChart;
        private readonly Chart memoryChart;
        private readonly Timer updateTimer;

        public async Task RunAsync()
        {
            await CycoTui.CreateApp()
                .WithLayout(BuildDashboard)
                .AddPlugin<MetricsCollectionPlugin>()
                .RunAsync();
        }

        private void BuildDashboard(ILayoutBuilder layout)
        {
            layout.Grid(2,
                Widgets.Block("CPU Usage")
                    .WithContent(cpuChart),
                Widgets.Block("Memory Usage")
                    .WithContent(memoryChart),
                Widgets.Block("System Info")
                    .WithContent(new SystemInfoWidget()),
                Widgets.Block("Network")
                    .WithContent(new NetworkStatsWidget())
            );
        }
    }

    // examples/08-data-visualization/Program.cs
    public class DataVisualization
    {
        public async Task RunAsync()
        {
            var data = await LoadSampleDataAsync();

            await CycoTui.CreateApp()
                .WithLayout(layout =>
                {
                    layout.Horizontal(
                        Widgets.Block("Line Chart")
                            .WithContent(Widgets.Chart(data.TimeSeries)),
                        Widgets.Vertical(
                            Widgets.Block("Bar Chart")
                                .WithContent(Widgets.BarChart(data.Categories)),
                            Widgets.Block("Statistics")
                                .WithContent(new StatisticsWidget(data))
                        )
                    );
                })
                .RunAsync();
        }
    }

    // examples/09-forms/Program.cs
    public class FormExample
    {
        public async Task RunAsync()
        {
            var form = new Form()
                .AddField("Name", new TextInput { Required = true })
                .AddField("Email", new EmailInput { Required = true })
                .AddField("Age", new NumberInput { Min = 0, Max = 120 })
                .AddField("Comments", new TextArea { Rows = 4 });

            await CycoTui.CreateApp()
                .WithLayout(layout =>
                {
                    layout.Vertical(
                        Widgets.Block("User Registration")
                            .WithContent(form),
                        Widgets.Button("Submit", () => HandleSubmit(form))
                    );
                })
                .RunAsync();
        }
    }
}
```

### Integration Examples

```csharp
// Category 4: .NET Integration Examples
public static class IntegrationExamples
{
    // examples/10-cli-integration/Program.cs
    public class CliIntegration
    {
        public static async Task Main(string[] args)
        {
            if (args.Contains("--interactive"))
            {
                await RunInteractiveMode();
            }
            else
            {
                await RunCommandLineMode(args);
            }
        }

        private static async Task RunInteractiveMode()
        {
            await CycoTui.CreateApp()
                .WithRootWidget(new InteractiveMenu())
                .RunAsync();
        }
    }

    // examples/11-hosted-service/Program.cs
    public class HostedServiceExample
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseCycoTui<MonitoringApp>()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IMetricsService, MetricsService>();
                })
                .Build();

            await host.RunAsync();
        }
    }

    // examples/12-dependency-injection/Program.cs
    public class DependencyInjectionExample
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddCycoTui()
                .AddSingleton<IDataService, DataService>()
                .AddSingleton<INotificationService, NotificationService>()
                .BuildServiceProvider();

            var app = services.GetRequiredService<IApplicationFactory>()
                .CreateApplication<DataDashboard>();

            await app.RunAsync();
        }
    }

    // examples/13-configuration/Program.cs
    public class ConfigurationExample
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            await CycoTui.CreateApp()
                .ConfigureServices(services =>
                {
                    services.Configure<AppSettings>(configuration.GetSection("App"));
                })
                .WithRootWidget<ConfiguredApp>()
                .RunAsync();
        }
    }
}
```

### Showcase Examples

```csharp
// Category 5: Feature Showcases
public static class ShowcaseExamples
{
    // examples/14-stress-test/Program.cs
    public class StressTest
    {
        public async Task RunAsync()
        {
            const int widgetCount = 1000;
            var widgets = Enumerable.Range(0, widgetCount)
                .Select(i => Widgets.Text($"Widget {i}"))
                .ToArray();

            await CycoTui.CreateApp()
                .WithLayout(layout =>
                {
                    layout.Grid(20, widgets); // 20 columns
                })
                .RunAsync();
        }
    }

    // examples/15-animation/Program.cs
    public class AnimationExample
    {
        public async Task RunAsync()
        {
            var spinner = new AnimatedSpinner();
            var progressBar = new AnimatedProgressBar();

            await CycoTui.CreateApp()
                .WithLayout(layout =>
                {
                    layout.Vertical(
                        Widgets.Block("Animations")
                            .WithContent(layout =>
                            {
                                layout.Vertical(
                                    spinner,
                                    progressBar,
                                    Widgets.Text("Press Q to quit")
                                );
                            })
                    );
                })
                .AddPlugin<AnimationPlugin>()
                .RunAsync();
        }
    }

    // examples/16-games/snake/Program.cs
    public class SnakeGame
    {
        private readonly SnakeGameState gameState = new();
        private readonly Canvas gameCanvas;

        public async Task RunAsync()
        {
            await CycoTui.CreateApp()
                .WithLayout(BuildGameLayout)
                .WithKeyHandler<KeyEvent>(HandleGameInput)
                .AddPlugin<GameLoopPlugin>()
                .RunAsync();
        }
    }

    // examples/17-themes/Program.cs
    public class ThemeExample
    {
        public async Task RunAsync()
        {
            var themes = new[] { "Dark", "Light", "Blue", "Green" };
            var currentTheme = 0;

            await CycoTui.CreateApp()
                .WithLayout(layout => BuildThemedLayout(layout, themes[currentTheme]))
                .WithKeyHandler<KeyEvent>(key =>
                {
                    if (key.Key == Key.T)
                    {
                        currentTheme = (currentTheme + 1) % themes.Length;
                        ApplyTheme(themes[currentTheme]);
                    }
                })
                .RunAsync();
        }
    }
}
```

### Example Project Structure

```
examples/
├── README.md                          # Examples overview and index
├── getting-started/
│   ├── 01-hello-world/
│   ├── 02-basic-widget/
│   ├── 03-simple-layout/
│   └── 04-input-handling/
├── interactive/
│   ├── 05-todo-app/
│   ├── 06-file-browser/
│   └── 07-calculator/
├── advanced/
│   ├── 08-dashboard/
│   ├── 09-data-visualization/
│   └── 10-forms/
├── integration/
│   ├── 11-cli-integration/
│   ├── 12-hosted-service/
│   ├── 13-dependency-injection/
│   └── 14-configuration/
├── showcase/
│   ├── 15-stress-test/
│   ├── 16-animation/
│   ├── 17-games/
│   └── 18-themes/
└── shared/
    ├── Common/                         # Shared utilities
    ├── Widgets/                        # Custom widget examples
    └── Services/                       # Example services
```

## Testing Approach

1. **Compilation Tests**: Verify all examples compile without errors
2. **Runtime Tests**: Automated testing of example application behavior
3. **Cross-Platform Tests**: Verify examples work on Windows, macOS, and Linux
4. **Performance Tests**: Ensure examples meet performance expectations
5. **User Experience Tests**: Manual testing of example usability
6. **Documentation Tests**: Verify example documentation is accurate and helpful
7. **Integration Tests**: Test examples with various .NET project types

## Related Components

- `examples/` - Root directory for all example applications
- `examples/shared/` - Shared utilities and components
- `examples/README.md` - Comprehensive examples documentation
- Integration with main CycoTui library and all widget implementations

## Integration Points

- **Application Framework**: Examples demonstrate all major application features
- **Widget System**: Examples showcase all available widgets and their capabilities
- **Layout System**: Examples demonstrate various layout patterns and configurations
- **Event System**: Examples show input handling and event management patterns
- **Plugin System**: Examples demonstrate plugin development and usage

## Performance Considerations

- **Example Complexity**: Balance feature demonstration with performance
- **Resource Usage**: Ensure examples don't consume excessive system resources
- **Startup Time**: Examples should start quickly for good developer experience
- **Memory Usage**: Monitor memory usage in long-running examples
- **Cross-Platform Performance**: Consistent performance across all platforms

## Documentation Integration

Each example should include:
- **README.md**: Purpose, features demonstrated, and running instructions
- **Inline Comments**: Detailed code explanation and best practices
- **Architecture Notes**: Design decisions and patterns used
- **Extension Ideas**: Suggestions for further development
- **Troubleshooting**: Common issues and solutions

## Acceptance Criteria

- [ ] Complete set of progressive learning examples from basic to advanced
- [ ] Real-world application examples solving common developer problems
- [ ] Integration examples demonstrating .NET ecosystem compatibility
- [ ] Feature showcase examples highlighting CycoTui capabilities
- [ ] All examples compile and run successfully on target platforms
- [ ] Comprehensive documentation for each example application
- [ ] Consistent code quality and style across all examples
- [ ] Performance benchmarks meeting target thresholds
- [ ] Cross-platform compatibility verified for all examples
- [ ] Examples demonstrate best practices and recommended patterns
- [ ] Learning progression tested with actual developers
- [ ] Integration with CI/CD pipeline for automated testing
- [ ] Examples repository structure organized and navigable
- [ ] Shared utilities and components properly documented
- [ ] Example applications useful as starting points for real projects

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [APPLAYER-API-001](../APPLAYER-API-001/README.md): High-level developer API implementation
- [APPLAYER-DOCS-001](../APPLAYER-DOCS-001/README.md): Developer documentation
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap