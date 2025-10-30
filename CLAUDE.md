# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**CycoTui** is a C# terminal user interface (TUI) library inspired by Ratatui (Rust). It provides a cross-platform abstraction for building rich terminal applications with widgets, layouts, and styling. The project targets .NET 8.0 and 9.0.

## Essential Commands

### Building
```bash
# Build entire solution
dotnet build CycoTui.sln

# Build in Release mode
dotnet build CycoTui.sln -c Release

# Build specific project
dotnet build src/CycoTui.Core/CycoTui.Core.csproj
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests in a specific project
dotnet test tests/CycoTui.Core.Tests/CycoTui.Core.Tests.csproj

# Run a single test by filter
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

### Running the Sample
```bash
# Run the sample application
dotnet run --project examples/CycoTui/CycoTui.Sample.csproj
```

### Cleaning
```bash
# Clean build artifacts
dotnet clean CycoTui.sln
```

## Architecture Overview

### Three-Tier Structure

1. **CycoTui.Core** - Platform-agnostic foundation
   - All cross-platform primitives and abstractions
   - No platform-specific dependencies
   - Widget system, layout engine, buffer management, styling

2. **CycoTui.Backend.{Unix,Windows}** - Platform implementations
   - Each implements `ITerminalBackend` interface
   - Unix: ANSI escape sequences via Console
   - Windows: Win32 Console API

3. **examples/CycoTui** - Sample application
   - Demonstrates complete rendering pipeline
   - Claude-style chat UI with multi-line input

### Core Subsystems (in src/CycoTui.Core/)

- **Backend/** - Terminal backend abstraction (`ITerminalBackend`)
- **Buffer/** - 2D drawing surface with diffing (`Cell`, `Buffer`, `Frame`, `BufferDiff`)
- **Widgets/** - 24 UI components (`IWidget`, `IStatefulWidget<T>`)
- **Terminal/** - Double-buffered rendering engine
- **Layout/** - Constraint-based layout system (`LayoutEngine`, `Constraint`)
- **Style/** - Color and text styling (`Style`, `Color`, `TextModifier`)
- **Text/** - Grapheme handling, width calculations (`GraphemeEnumerator`, `WidthService`)
- **Input/** - Event system and input loop abstractions
- **Logging/** - Internal logging context
- **Documentation/** - Doc generation service

## Key Architectural Patterns

### Widget System

Two primary interfaces:

1. **`IWidget`** - Stateless widgets
   ```csharp
   void Render(Frame frame, Rect area);
   ```
   Examples: `Paragraph`, `Block`, `InputLineWidget`

2. **`IStatefulWidget<TState>`** - Stateful widgets
   ```csharp
   void Render(Frame frame, Rect area, TState state);
   ```
   Examples: `ListWidget<ListState>`, `TableWidget<TableState>`

All widgets use **immutable records** with `.With*()` fluent builders. State management is delegated to the caller.

### Double-Buffered Rendering Pipeline

1. **Terminal** maintains two buffers (previous, current)
2. User provides render callback: `terminal.Draw(frame => { /* render widgets */ })`
3. Widgets render into `Frame` (wraps current buffer)
4. **Diffing**: `BufferDiff` compares previous vs. current buffers
5. **Emission**: Segments of changes sent to backend (minimizes cursor movements)
6. **Swap**: Buffers reused (no reallocation)

### Multi-Width Grapheme Handling

CycoTui is grapheme-aware and handles emoji, CJK characters:

- **Cell Structure**: `(Grapheme, Style, Width, Skip)`
- **Skip Flag**: Continuation cells of multi-width graphemes (e.g., emoji = 2 cells)
  - Cell[0]: head (width=2, skip=false) + grapheme
  - Cell[1]: continuation (width=2, skip=true) + same grapheme
- Diffing skips continuation cells to prevent redundant emissions

### Layout System (Constraint-Based)

Inspired by Ratatui. Six constraint types with precedence:

1. **Length** - Fixed size (mandatory)
2. **Min** - Minimum guarantee
3. **Percentage** - % of available space
4. **Ratio** - Proportional shares (numerator/denominator)
5. **Fill** - Consume remaining equally
6. **Max** - Ceiling (enforced last)

**Multi-pass algorithm** in `LayoutEngine.Distribute()`:
- Satisfy Length → Min → Percentage → Ratio → Fill → Max
- Handle overflow with proportional shrinking
- Support alignment: Start, Center, End, SpaceBetween, SpaceAround, SpaceEvenly

### Backend Abstraction (`ITerminalBackend`)

Core operations:
- **Drawing**: `Draw(IEnumerable<CellUpdate>)` - batch cell writes
- **Raw Output**: `WriteRaw(string sequence)` - direct ANSI/control sequences
- **Cursor Control**: Hide/show, position get/set
- **Sizing**: `GetSize()`, `GetWindowSize()` (with pixel metrics)
- **Clearing**: All, line, below cursor variants
- **Capabilities**: `BackendCapabilities` property for feature negotiation

**BackendFactory** provides:
- Registry-based backend registration
- Auto-detection (Windows → Unix → Minimal fallback)
- Explicit backend selection with platform validation

### Capability-Based Rendering

Backends report capabilities; Terminal respects them for graceful degradation:
- `ColorLevel`: None → Ansi16 → Ansi256 → TrueColor
- `SupportsUnderlineColor`: Falls back to foreground color if unsupported
- Other flags guide feature emissions (mouse, scrolling, etc.)

### Style System

**`Style` struct** (immutable):
- `Foreground`, `Background`, `UnderlineColor` (optional)
- `AddModifier`, `SubModifier` flags (Bold, Italic, Underline, etc.)

**Fluent API**:
```csharp
Style.Empty
    .WithForeground(Color.Rgb(255, 0, 0))
    .Add(TextModifier.Bold)
    .Remove(TextModifier.Dim);
```

**Patching**: Styles merge (additive) rather than replace, enabling nested widget composition.

## Development Guidelines

### Code Style
- **Nullable reference types**: Enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings**: Enabled in most projects
- **Latest C# language version**: `<LangVersion>latest</LangVersion>`
- **Documentation**: XML docs required (CS1591 suppressed in Debug builds)
- **Warnings as errors**: Release builds only

### Testing Strategy
- **Framework**: xUnit (2.6.6)
- **Coverage**: coverlet.collector
- **Internal visibility**: `InternalsVisibleTo` attribute exposes internals to test project
- **Test backends**: Use `TestBackend` for unit testing widgets/terminal logic
- **Scripted input**: Use `ScriptedBlockingSource` for input testing

### Immutability Pattern
All public types should be:
- Immutable records or readonly structs
- Fluent `.With*()` methods for modifications
- No mutable state in widgets (use `IStatefulWidget<T>` for stateful behavior)

### Rendering Performance
- Widgets should minimize allocations in `Render()` methods
- Reuse buffers where possible
- Avoid string concatenation in hot paths
- Let Terminal handle diffing and optimization

### Cross-Platform Considerations
- Core logic should be in `CycoTui.Core` (platform-agnostic)
- Platform-specific code goes in respective backend projects
- Use `BackendCapabilities` for feature detection, not platform checks
- Test on both Windows and Unix when modifying backend code

### Widget Development
When creating new widgets:
1. Implement `IWidget` (stateless) or `IStatefulWidget<T>` (stateful)
2. Use immutable record with fluent builders
3. Optionally implement `IContainerWidget` if widget has inner content area
4. Add `IFocusableWidget` if widget receives input
5. Handle grapheme width correctly (use `WidthService`)
6. Write unit tests with `TestBackend`

### Style Emission
- Terminal tracks active style state per segment
- Only emits style transitions (not full style on every cell)
- Deduplicates consecutive identical style sequences
- Compresses ANSI codes: `\u001b[1m\u001b[3m` → `\u001b[1;3m`

## Common Patterns

### Creating a Widget
```csharp
public record MyWidget : IWidget
{
    public string Content { get; init; } = "";
    public Style Style { get; init; } = Style.Empty;

    public void Render(Frame frame, Rect area)
    {
        frame.WriteString(area.X, area.Y, Content, Style);
    }

    public MyWidget WithContent(string content) => this with { Content = content };
    public MyWidget WithStyle(Style style) => this with { Style = style };
}
```

### Using Layout Engine
```csharp
var constraints = new List<Constraint>
{
    Constraint.Length(3),        // Fixed 3 rows
    Constraint.Fill(1),          // Consume remaining
    Constraint.Percentage(20)    // 20% of available
};

var rects = LayoutEngine.Distribute(
    area,
    constraints,
    LayoutDirection.Vertical,
    alignment: AlignmentMode.Start
);
```

### Backend Selection
```csharp
// Auto-detect platform
var backend = BackendFactory.Create();

// Explicit selection
var backend = BackendFactory.Create(BackendPreference.Unix);

// With logging
var logging = new LoggingContext(loggerFactory);
var terminal = new Terminal(backend, logging);
```

## Project-Specific Notes

### README Context
The README references Ratatui (Rust TUI library) because this project started as documentation for Ratatui but is actually implementing CycoTui. The README content is Ratatui-specific and should be ignored when working on CycoTui code.

### Current Phase
Backend implementations are Phase-1:
- Basic ANSI/Console API support
- No scrolling regions yet
- No mouse parsing yet
- Limited capability probing

### TODOs in Codebase
Notable in-flight work:
- Full ZWJ emoji sequence handling (Text/)
- Backend capability probing enhancements (underline color, mouse)
- Focus navigation system (`FocusManager` design)
- Style batching optimizations (Terminal/EmitSegment)
- Documentation generation service completion

### Copilot/PR Guidelines
From `.github/copilot-instructions.md`:
- Keep PRs small and focused (< 500 lines)
- Verify code follows project conventions
- Require deprecation warnings rather than immediate removal of public APIs
- Question fundamental changes to configuration/build without justification
- Ensure new functionality includes tests and documentation

## File Organization

```
CycoTui.sln
├── src/
│   ├── CycoTui.Core/              # Platform-agnostic core
│   ├── CycoTui.Backend.Unix/      # Unix terminal backend
│   ├── CycoTui.Backend.Windows/   # Windows terminal backend
│   └── DocGenProbe/               # Documentation generation tool
├── examples/
│   └── CycoTui/                   # Sample chat application
├── tests/
│   └── CycoTui.Core.Tests/        # Core library tests (xUnit)
├── tools/
│   └── DocGen/                    # Documentation generation
└── Directory.Build.props           # Shared MSBuild properties
```

## Testing Notes

- Test framework: xUnit 2.6.6
- Target framework: net9.0 (tests)
- Coverage: coverlet.collector included
- Warnings suppressed: CS1591 (missing XML docs), CS8633, xUnit2012
- Internal visibility: Tests can access internal members via `InternalsVisibleTo`

## Additional Resources

- **ARCHITECTURE.md** - Detailed Ratatui crate organization (reference, not directly applicable to CycoTui)
- **Directory.Build.props** - Shared MSBuild configuration (logging, nullable, warnings)
- **Sample app** - `examples/CycoTui/Program.cs` for usage patterns
