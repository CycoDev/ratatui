# API Convenience and Prelude Pattern Implementation

## Overview

Implement C# equivalents of Ratatui's prelude pattern to provide convenient access to commonly used types and functionality. This task addresses how to create a developer-friendly API surface that reduces the need for multiple using statements while maintaining clarity and avoiding namespace pollution.

## Implementation Approach

Based on analysis of `ratatui/src/prelude.rs`, we need to implement several C# patterns:

### 1. Global Using Directives (C# 10+)
Create a `GlobalUsings.cs` file with common using statements:
```csharp
global using CycoTui.Core;
global using CycoTui.Widgets;
global using CycoTui.Layout;
global using CycoTui.Style;
global using CycoTui.Text;
global using static CycoTui.Symbols;
```

### 2. Prelude Static Class
Create a static class with type aliases and commonly used functionality:
```csharp
public static class Prelude
{
    // Type aliases for convenience
    public static readonly Type Widget = typeof(IWidget);
    public static readonly Type StatefulWidget = typeof(IStatefulWidget);
    
    // Extension method access points
    public static LayoutBuilder Layout => new();
    public static StyleBuilder Style => new();
}
```

### 3. Extension Methods
Provide fluent interfaces through extension methods:
```csharp
public static class WidgetExtensions
{
    public static T WithStyle<T>(this T widget, Style style) where T : IWidget
    public static T WithBlock<T>(this T widget, Block block) where T : IWidget
}
```

### 4. Namespace Organization
Organize namespaces to minimize conflicts:
- `CycoTui` - Core types (Terminal, Frame, Buffer)
- `CycoTui.Widgets` - All widget implementations
- `CycoTui.Layout` - Layout system
- `CycoTui.Style` - Styling system
- `CycoTui.Text` - Text handling
- `CycoTui.Backends` - Backend implementations

## Key Challenges

### Platform-Specific Backends
Handle conditional backend availability similar to Rust's feature flags:
```csharp
#if WINDOWS
using CycoTui.Backends.Windows;
#endif
#if CROSSTERM
using CycoTui.Backends.Crossterm;
#endif
```

### Type Conflicts
Handle potential naming conflicts between library and user types:
- Use namespace aliases where needed
- Provide explicit type access through static members
- Consider prefixing conflicting types

### API Discoverability
Ensure developers can easily discover available functionality:
- Use XML documentation extensively
- Provide IntelliSense-friendly patterns
- Group related functionality logically

## Related Components

- **Namespace Design**: All namespace organization
- **Extension Methods**: Widget and utility extensions
- **Static Classes**: Utility and convenience classes
- **Backend Selection**: Platform-specific backend loading

## Integration Points

- **Project Templates**: Include prelude setup in project templates
- **Documentation**: Document recommended using patterns
- **IDE Integration**: Consider providing analyzers for optimal usage patterns

## Platform-Specific Details

### Windows
- Include Windows-specific backend by default
- Conditional compilation for Windows Console API features

### Unix (Linux/macOS)
- Include Unix-specific backends where available
- Handle termios-based backends appropriately

### Cross-Platform
- Provide runtime backend detection and selection
- Fallback mechanisms for unsupported platforms

## Testing Approach

### API Usage Tests
Test common usage patterns:
```csharp
[Test]
public void PreludeUsage_ShouldProvideCommonTypes()
{
    using static CycoTui.Prelude;
    
    var widget = new Paragraph("Hello")
        .WithStyle(Style.Default.Foreground(Color.Blue));
    
    Assert.IsNotNull(widget);
}
```

### Namespace Conflict Tests
Verify that common naming patterns don't conflict:
```csharp
[Test]
public void CommonTypeNames_ShouldNotConflict()
{
    // Ensure we can use both library and user types
    var userLine = new Line(); // User type
    var libLine = new CycoTui.Text.Line("content"); // Library type
}
```

## Acceptance Criteria

1. **Single Using Statement**: Developers can access most functionality with minimal using statements
2. **No Namespace Pollution**: Common patterns don't pollute the global namespace unexpectedly
3. **Platform Support**: Backend selection works appropriately on all target platforms
4. **Conflict Resolution**: Library types don't conflict with common user type names
5. **Documentation**: Clear guidance on recommended usage patterns
6. **IDE Support**: Good IntelliSense and discoverability
7. **Backward Compatibility**: Pattern works across supported .NET versions

## See Also

- [VISION-API-003.md](../../vision/VISION-API-003.md): API design vision
- [SPEC-BACKEND-001.md](../../specs/SPEC-BACKEND-001.md): Backend abstraction specification
- [FILE-ANALYSIS: ratatui/src/prelude.rs](../../file-analyses/ratatui-src-prelude.md): Source analysis