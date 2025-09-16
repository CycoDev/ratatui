# High-Level Developer API Implementation

## Overview

This task implements the high-level developer API that provides an intuitive, idiomatic .NET interface for building terminal applications with CycoTui. The API focuses on developer productivity, discoverability, and ease of use while maintaining the full power and flexibility of the underlying systems.

## Implementation Approach

1. **Create Fluent API Design**:
   - Implement fluent builder patterns for complex application configuration
   - Create extension methods for common operations and widget creation
   - Build method chaining APIs for layout and styling operations
   - Support declarative application construction patterns

2. **Implement Developer Convenience Features**:
   - Create factory methods and shortcuts for common scenarios
   - Build implicit conversion operators for seamless type integration
   - Implement helpful defaults and intelligent fallbacks
   - Support both explicit and convention-based configuration

3. **Build Integration Helpers**:
   - Create seamless integration patterns for existing .NET CLI applications
   - Implement hosting extensions for dependency injection frameworks
   - Build configuration system integration (IConfiguration, IOptions)
   - Support structured logging and diagnostics integration

4. **Create Documentation and Examples**:
   - Build comprehensive API documentation with examples
   - Create tutorial progression from simple to advanced scenarios
   - Implement IntelliSense-friendly XML documentation
   - Support interactive examples and code snippets

## Key Challenges

1. **API Discoverability**: Making functionality easy to find and understand
2. **Type Safety**: Balancing flexibility with compile-time safety
3. **Performance**: Ensuring convenience doesn't compromise performance
4. **Compatibility**: Maintaining compatibility with .NET ecosystem patterns
5. **Learning Curve**: Minimizing complexity for common use cases

## Implementation Notes

### Fluent Application Builder API

```csharp
// Primary fluent API for application construction
public static class CycoTui
{
    public static ApplicationBuilder CreateApp()
        => new ApplicationBuilder();

    public static ApplicationBuilder CreateApp<TWidget>(TWidget rootWidget) where TWidget : IWidget
        => new ApplicationBuilder().WithRootWidget(rootWidget);

    // Quick start methods
    public static Task RunAsync<TWidget>(TWidget widget) where TWidget : IWidget
        => CreateApp(widget).RunAsync();

    public static Task RunAsync(string message)
        => RunAsync(new Text(message));

    public static Task RunAsync(Action<ILayoutBuilder> buildLayout)
    {
        var builder = new LayoutBuilder();
        buildLayout(builder);
        return RunAsync(builder.Build());
    }
}

public class ApplicationBuilder
{
    private readonly ApplicationOptions options = new();
    private IWidget rootWidget;
    private readonly List<Action<IServiceCollection>> serviceConfigurations = new();
    private readonly List<IApplicationPlugin> plugins = new();

    // Widget and layout configuration
    public ApplicationBuilder WithRootWidget<TWidget>(TWidget widget) where TWidget : IWidget
    {
        rootWidget = widget;
        return this;
    }

    public ApplicationBuilder WithLayout(Action<ILayoutBuilder> buildLayout)
    {
        var builder = new LayoutBuilder();
        buildLayout(builder);
        return WithRootWidget(builder.Build());
    }

    // Terminal configuration
    public ApplicationBuilder UseFullscreen()
    {
        options.Viewport = Viewport.Fullscreen;
        return this;
    }

    public ApplicationBuilder UseInline(int height)
    {
        options.Viewport = Viewport.Inline(height);
        return this;
    }

    public ApplicationBuilder UseFixedArea(Rect area)
    {
        options.Viewport = Viewport.Fixed(area);
        return this;
    }

    // Input configuration
    public ApplicationBuilder EnableMouse(bool enable = true)
    {
        options.EnableMouse = enable;
        return this;
    }

    public ApplicationBuilder WithKeyHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : KeyEvent
    {
        // Register key event handler
        return this;
    }

    public ApplicationBuilder WithShortcut(KeyCombination keys, Func<Task> action)
    {
        // Register keyboard shortcut
        return this;
    }

    // Service integration
    public ApplicationBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        serviceConfigurations.Add(configure);
        return this;
    }

    public ApplicationBuilder AddPlugin<TPlugin>() where TPlugin : class, IApplicationPlugin
    {
        return ConfigureServices(services => services.AddSingleton<TPlugin>());
    }

    public ApplicationBuilder AddPlugin<TPlugin>(TPlugin plugin) where TPlugin : IApplicationPlugin
    {
        plugins.Add(plugin);
        return this;
    }

    // Execution methods
    public Application Build() => new Application(rootWidget, options, /* plugins, services */);
    public Task RunAsync() => Build().RunAsync();
    public void Run() => Build().Run();
}
```

### Layout Builder API

```csharp
public interface ILayoutBuilder
{
    ILayoutBuilder Horizontal(params IWidget[] widgets);
    ILayoutBuilder Vertical(params IWidget[] widgets);
    ILayoutBuilder Grid(int columns, params IWidget[] widgets);
    ILayoutBuilder Stack(params IWidget[] widgets);

    ILayoutBuilder WithConstraints(params Constraint[] constraints);
    ILayoutBuilder WithMargin(int margin);
    ILayoutBuilder WithPadding(int padding);

    IWidget Build();
}

public class LayoutBuilder : ILayoutBuilder
{
    public ILayoutBuilder Horizontal(params IWidget[] widgets)
        => Horizontal(builder => widgets.ToList().ForEach(w => builder.Add(w)));

    public ILayoutBuilder Horizontal(Action<IContainerBuilder> configure)
    {
        var container = new FlexContainer(Direction.Horizontal);
        var builder = new ContainerBuilder(container);
        configure(builder);
        return new LayoutBuilder(container);
    }

    // Additional layout methods...
}

// Extension methods for common patterns
public static class LayoutExtensions
{
    public static ILayoutBuilder WithTitle(this ILayoutBuilder builder, string title)
        => builder.WithWidget(new Block { Title = title });

    public static ILayoutBuilder WithBorder(this ILayoutBuilder builder, BorderType borderType = BorderType.Rounded)
        => builder.WithWidget(new Block { BorderType = borderType });

    public static ILayoutBuilder Centered(this ILayoutBuilder builder)
        => builder.WithConstraints(Constraint.Percentage(100)).WithAlignment(Alignment.Center);
}
```

### Widget Factory and Extensions

```csharp
// Factory methods for common widgets
public static class Widgets
{
    public static Text Text(string content, Style style = default)
        => new Text(content) { Style = style };

    public static Paragraph Paragraph(string content)
        => new Paragraph(content);

    public static Block Block(string title = null, BorderType border = BorderType.Rounded)
        => new Block { Title = title, BorderType = border };

    public static Button Button(string text, Action onClick = null)
        => new Button(text) { OnClick = onClick };

    public static ProgressBar Progress(double value, double max = 100)
        => new ProgressBar { Value = value, Maximum = max };

    public static Chart Chart(IEnumerable<double> data, string title = null)
        => new Chart(data) { Title = title };

    // Collection widgets
    public static List<T> List<T>(IEnumerable<T> items, Func<T, string> display = null)
        => new List<T>(items) { DisplayFunc = display ?? (x => x?.ToString() ?? string.Empty) };

    public static Table Table(IEnumerable<object> data)
        => Table.FromData(data);
}

// Extension methods for widget configuration
public static class WidgetExtensions
{
    public static T WithStyle<T>(this T widget, Style style) where T : IWidget
    {
        widget.Style = style;
        return widget;
    }

    public static T WithBackground<T>(this T widget, Color color) where T : IWidget
        => widget.WithStyle(widget.Style.WithBackground(color));

    public static T WithForeground<T>(this T widget, Color color) where T : IWidget
        => widget.WithStyle(widget.Style.WithForeground(color));

    public static T Bold<T>(this T widget) where T : IWidget
        => widget.WithStyle(widget.Style.AddModifier(Modifier.Bold));

    public static T Italic<T>(this T widget) where T : IWidget
        => widget.WithStyle(widget.Style.AddModifier(Modifier.Italic));

    // Layout extensions
    public static T WithWidth<T>(this T widget, Constraint width) where T : IWidget
    {
        // Apply width constraint to widget
        return widget;
    }

    public static T WithHeight<T>(this T widget, Constraint height) where T : IWidget
    {
        // Apply height constraint to widget
        return widget;
    }

    public static T Fill<T>(this T widget) where T : IWidget
        => widget.WithWidth(Constraint.Percentage(100)).WithHeight(Constraint.Percentage(100));
}
```

### Colors and Styling API

```csharp
// Fluent color API
public static class Colors
{
    public static Color Rgb(byte r, byte g, byte b) => Color.Rgb(r, g, b);
    public static Color Hex(string hex) => Color.FromHex(hex);

    // Named colors
    public static Color Red => Color.Red;
    public static Color Green => Color.Green;
    public static Color Blue => Color.Blue;
    public static Color Yellow => Color.Yellow;
    public static Color Cyan => Color.Cyan;
    public static Color Magenta => Color.Magenta;
    public static Color White => Color.White;
    public static Color Black => Color.Black;
    public static Color Gray => Color.Gray;

    // Material Design colors
    public static class Material
    {
        public static Color Red500 => Color.Rgb(244, 67, 54);
        public static Color Blue500 => Color.Rgb(33, 150, 243);
        public static Color Green500 => Color.Rgb(76, 175, 80);
        // More material colors...
    }

    // Theme-based colors
    public static class Theme
    {
        public static Color Primary => Color.Blue;
        public static Color Secondary => Color.Gray;
        public static Color Success => Color.Green;
        public static Color Warning => Color.Yellow;
        public static Color Error => Color.Red;
        public static Color Info => Color.Cyan;
    }
}

// Style builder API
public static class Styles
{
    public static Style Default => Style.Default;

    public static StyleBuilder New() => new StyleBuilder();

    public static StyleBuilder Foreground(Color color)
        => new StyleBuilder().WithForeground(color);

    public static StyleBuilder Background(Color color)
        => new StyleBuilder().WithBackground(color);
}

public class StyleBuilder
{
    private Style style = Style.Default;

    public StyleBuilder WithForeground(Color color)
    {
        style = style.WithForeground(color);
        return this;
    }

    public StyleBuilder WithBackground(Color color)
    {
        style = style.WithBackground(color);
        return this;
    }

    public StyleBuilder Bold()
    {
        style = style.AddModifier(Modifier.Bold);
        return this;
    }

    public StyleBuilder Italic()
    {
        style = style.AddModifier(Modifier.Italic);
        return this;
    }

    public StyleBuilder Underline()
    {
        style = style.AddModifier(Modifier.Underlined);
        return this;
    }

    public static implicit operator Style(StyleBuilder builder) => builder.style;
}
```

### Integration Helpers

```csharp
// ASP.NET Core integration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCycoTui(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationFactory, ApplicationFactory>();
        services.AddSingleton<ITerminalController, TerminalController>();
        services.AddSingleton<IRenderEngine, RenderEngine>();
        return services;
    }

    public static IServiceCollection AddCycoTui(this IServiceCollection services, Action<CycoTuiOptions> configure)
    {
        services.Configure(configure);
        return services.AddCycoTui();
    }
}

// Configuration system integration
public class CycoTuiOptions
{
    public TerminalBackendType Backend { get; set; } = TerminalBackendType.Crossterm;
    public bool EnableMouse { get; set; } = true;
    public bool EnableResize { get; set; } = true;
    public TimeSpan RenderInterval { get; set; } = TimeSpan.FromMilliseconds(16);
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

// Hosting extensions
public static class HostBuilderExtensions
{
    public static IHostBuilder UseCycoTui(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddCycoTui();
            services.Configure<CycoTuiOptions>(context.Configuration.GetSection("CycoTui"));
        });
    }

    public static IHostBuilder UseCycoTui<TApp>(this IHostBuilder hostBuilder)
        where TApp : class, ICycoTuiApplication
    {
        return hostBuilder.UseCycoTui()
            .ConfigureServices(services => services.AddSingleton<ICycoTuiApplication, TApp>());
    }
}
```

### Quick Start Examples

```csharp
// Example 1: Hello World
await CycoTui.RunAsync("Hello, World!");

// Example 2: Simple layout
await CycoTui.RunAsync(layout =>
{
    layout.Vertical(
        Widgets.Text("Welcome to CycoTui!").Bold(),
        Widgets.Paragraph("This is a simple terminal application."),
        Widgets.Button("Click me!", () => Console.WriteLine("Button clicked!"))
    );
});

// Example 3: Dashboard
await CycoTui.CreateApp()
    .WithLayout(layout =>
    {
        layout.Horizontal(
            Widgets.Block("System Info")
                .WithContent(new SystemInfoWidget()),
            Widgets.Block("Metrics")
                .WithContent(Widgets.Chart(GetMetricsData()))
        );
    })
    .EnableMouse()
    .WithShortcut(Keys.Q, async () => Environment.Exit(0))
    .RunAsync();

// Example 4: Data-driven application
var data = await LoadDataAsync();
await CycoTui.CreateApp()
    .WithRootWidget(Widgets.Table(data))
    .UseInline(20)
    .RunAsync();

// Example 5: Integration with existing CLI
public static async Task Main(string[] args)
{
    if (args.Contains("--interactive"))
    {
        await ShowInteractiveDashboard();
    }
    else
    {
        await RunCommandLineMode(args);
    }
}

private static async Task ShowInteractiveDashboard()
{
    await CycoTui.CreateApp()
        .WithLayout(CreateDashboardLayout)
        .EnableMouse()
        .AddPlugin<StatusPlugin>()
        .RunAsync();
}
```

## Testing Approach

1. **API Usability Tests**: Test developer experience with common scenarios
2. **Type Safety Tests**: Verify compile-time type checking and IntelliSense
3. **Performance Tests**: Ensure API convenience doesn't impact performance
4. **Integration Tests**: Test with real applications and various .NET projects
5. **Documentation Tests**: Verify all examples compile and run correctly
6. **Regression Tests**: Ensure API changes maintain backward compatibility
7. **Discoverability Tests**: Test that developers can find relevant functionality

## Related Components

- `src/CycoTui/CycoTui.cs` - Main entry point and factory methods
- `src/CycoTui/ApplicationBuilder.cs` - Fluent application builder
- `src/CycoTui/Widgets.cs` - Widget factory methods
- `src/CycoTui/LayoutBuilder.cs` - Fluent layout construction
- `src/CycoTui/Extensions/` - Extension methods for various types
- `src/CycoTui/Integration/` - Framework integration helpers

## Integration Points

- **Application Framework**: High-level API builds on top of core Application class
- **Widget System**: Provides convenient access to all widget functionality
- **Layout System**: Simplifies layout construction and configuration
- **Styling System**: Makes colors and styling easily accessible
- **Configuration**: Integrates with .NET configuration and dependency injection

## Performance Considerations

- **Compile-Time Optimization**: Use static methods and cached delegates where possible
- **Memory Efficiency**: Avoid unnecessary object allocations in hot paths
- **Builder Pattern Optimization**: Minimize intermediate object creation
- **Extension Method Performance**: Ensure extension methods don't add significant overhead
- **Lazy Initialization**: Defer expensive operations until actually needed

## Platform-Specific Details

### .NET Version Targeting
- **.NET 8.0**: Primary target with latest language features
- **.NET Standard 2.0**: Compatibility target for broader ecosystem support
- **Language Features**: Use appropriate C# language features for each target

## Acceptance Criteria

- [ ] Fluent API implemented with intuitive method chaining
- [ ] Widget factory methods providing easy widget creation
- [ ] Layout builder API simplifying complex layout construction
- [ ] Extension methods enhancing discoverability and usability
- [ ] Integration helpers for dependency injection and configuration
- [ ] Color and styling API with convenient access patterns
- [ ] Quick start examples working for common scenarios
- [ ] Type safety maintained with good IntelliSense support
- [ ] Performance overhead minimal compared to direct API usage
- [ ] Documentation and examples comprehensive and accurate
- [ ] Integration testing with real .NET applications successful
- [ ] API discoverability verified through user testing
- [ ] Backward compatibility maintained across updates
- [ ] Error messages helpful and actionable for developers
- [ ] Cross-platform compatibility verified

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [APPLAYER-APP-001](../APPLAYER-APP-001/README.md): Main application class implementation
- [SPEC-WIDGET-003.md](../../specs/SPEC-WIDGET-003.md): Widget system specification
- [API-CONVENIENCE-001](../API-CONVENIENCE-001/README.md): API convenience implementation
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap