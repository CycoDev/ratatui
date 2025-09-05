---
id: 005-STYLE-SYSTEM-001
title: Style System
status: draft
priority: high
date: 2023-11-28
---

# Style System

## Overview

The Style System provides comprehensive styling capabilities for terminal UI elements, enabling developers to create visually appealing applications with colors, text attributes, and other visual properties. The system offers both explicit style objects and fluent shorthand methods for maximum developer productivity.

## User Stories

### As a TUI developer, I want to style text with colors
```csharp
var redText = "Error message".Red();
var span = new Span("Warning", Style.New().Fg(Color.Yellow));
```

### As a TUI developer, I want to integrate with existing .NET color libraries
```csharp
// Convert from System.Drawing.Color
Color tuiColor = System.Drawing.Color.CornflowerBlue;

// Convert from ImageSharp (when available)
Color fromImageSharp = SixLabors.ImageSharp.Color.Red;

// Professional color space conversions
Color fromLinear = Color.FromLinearSrgb(1.0f, 0.5f, 0.25f);
```

### As a TUI developer, I want to apply text modifiers
```csharp
var boldText = "Important".Bold();
var emphasizedText = "Note".Bold().Italic().Underlined();
```

### As a TUI developer, I want to combine styles easily
```csharp
var baseStyle = Style.New().Bold();
var alertStyle = baseStyle.Red().OnYellow();
var finalStyle = alertStyle.Patch(Style.New().Blink());
```

### As a TUI developer, I want background colors
```csharp
var highlighted = "Selected Item".White().OnBlue();
var errorBox = Style.New().Fg(Color.White).Bg(Color.Red);
```

### As a TUI developer, I want style variables for consistency
```csharp
var headerStyle = Style.New().Bold().Underlined().Blue();
var errorStyle = Style.New().Red().Bold();
// Reuse throughout application
```

### As a TUI developer, I want 24-bit color support
```csharp
var customColor = Color.Rgb(72, 133, 237); // Google Blue
var styledText = "Brand Text".Fg(customColor);
```

### As a TUI developer, I want graceful degradation on limited terminals
```csharp
// Automatically falls back to supported color depth
var richStyle = Style.New().Fg(Color.Rgb(255, 100, 50));
// Works on 8-color, 256-color, and 24-bit terminals
```

### As a TUI developer, I want predefined color palettes for consistent design
```csharp
// Material Design colors
var errorStyle = Style.New().Fg(MaterialColors.Red.C500).Bg(MaterialColors.Red.C50);
var primaryButton = Style.New().Fg(Color.White).Bg(MaterialColors.Blue.C600);

// Tailwind CSS colors  
var warningAlert = Style.New().Fg(TailwindColors.Yellow.C800).Bg(TailwindColors.Yellow.C100);
var darkTheme = Style.New().Fg(TailwindColors.Gray.C200).Bg(TailwindColors.Gray.C900);
```

## Core Requirements

### Color Models
Based on analysis of `ratatui-core/src/style/color.rs`, the color system must provide:

#### ANSI Color Support
- **Standard ANSI Colors**: Black, Red, Green, Yellow, Blue, Magenta, Cyan, Gray (codes 30-37, 40-47)
- **Bright ANSI Colors**: DarkGray, LightRed, LightGreen, LightYellow, LightBlue, LightMagenta, LightCyan, White (codes 90-97, 100-107)
- **Reset Color**: Special value to reset colors to terminal defaults

#### Extended Color Support
- **RGB Colors**: 24-bit true color (16.7 million colors) for modern terminals
- **Indexed Colors**: 8-bit color palette (256 colors) for extended terminal support
- **Color Parsing**: Comprehensive string parsing with extensive alias support

#### Color String Parsing Requirements
The system must support all these color string formats:
- **ANSI Names**: "red", "blue", "lightgreen", "darkgray", case-insensitive
- **Aliases and Variants**: 
  - "grey"/"gray", "silver" → Gray
  - "bright red"/"light red" → LightRed  
  - "lightblack" → DarkGray, "lightwhite"/"lightgray" → White
- **Separators**: "light red", "light-red", "light_red" all equivalent
- **Hex Colors**: "#FF0000", "#00ff00" (case-insensitive, exactly 6 hex digits)
- **Indexed**: "0", "42", "255" (numeric strings for 0-255 range)

#### Optional Color Space Support
- **HSL Support**: Hue, Saturation, Lightness color space conversion
- **HSLuv Support**: Perceptually uniform HSL variant
- **Color Library Integration**: Support for external color space libraries

#### Color Capability Detection
```csharp
public enum ColorSupport
{
    None,           // No color support
    Basic,          // 16 ANSI colors only  
    Extended,       // 256 indexed colors
    TrueColor       // 24-bit RGB colors
}
```

### Predefined Color Palettes

Based on analysis of `ratatui-core/src/style/palette/material.rs` and `ratatui-core/src/style/palette/tailwind.rs`:

#### Material Design Palette System
- **Complete Material Design 2014 Compliance**: All 16 accented palettes (Red, Pink, Purple, DeepPurple, Indigo, Blue, LightBlue, Cyan, Teal, Green, LightGreen, Lime, Yellow, Amber, Orange, DeepOrange)
- **Non-Accented Palettes**: Brown, Gray, BlueGray (no accent variants)
- **Color Variants**: C50 (lightest) through C900 (darkest) for all palettes
- **Accent Colors**: A100, A200, A400, A700 variants for accented palettes only
- **Static Access Pattern**: `MaterialColors.Red.C500`, `MaterialColors.Blue.A400`
- **Compile-Time Initialization**: Zero runtime overhead through static readonly fields
- **Hex Value Accuracy**: All colors exactly match official Material Design specification

#### Tailwind CSS Palette System  
- **Complete Tailwind CSS Color System**: All 22 color families including Slate, Gray, Zinc, Neutral, Stone, Red, Orange, Amber, Yellow, Lime, Green, Emerald, Teal, Cyan, Sky, Blue, Indigo, Violet, Purple, Fuchsia, Pink, Rose
- **Extended Range**: C50 through C950 (11 variants) vs Material Design's 10 variants
- **Unified Structure**: Single Palette struct for all Tailwind colors (no accent variants)
- **Static Access Pattern**: `TailwindColors.Blue.C500`, `TailwindColors.Gray.C950`

#### Palette Implementation Architecture
```csharp
// Material Design structure
public readonly struct AccentedPalette
{
    public Color C50 { get; }    // Lightest
    public Color C500 { get; }   // Base color
    public Color C900 { get; }   // Darkest
    public Color A100 { get; }   // Light accent
    public Color A700 { get; }   // Dark accent
    
    public static AccentedPalette FromVariants(uint[] variants); // 14 elements
}

public readonly struct NonAccentedPalette  
{
    public Color C50 { get; }    // Lightest
    public Color C500 { get; }   // Base color
    public Color C900 { get; }   // Darkest
    
    public static NonAccentedPalette FromVariants(uint[] variants); // 10 elements
}

// Tailwind CSS structure
public readonly struct Palette
{
    public Color C50 { get; }    // Lightest
    public Color C500 { get; }   // Base color  
    public Color C900 { get; }   // Dark
    public Color C950 { get; }   // Darkest (Tailwind-specific)
    
    public static Palette FromVariants(uint[] variants); // 11 elements
}
```

#### Black and White Constants
- **Universal Colors**: `MaterialColors.Black`, `MaterialColors.White`, `TailwindColors.Black`, `TailwindColors.White`  
- **Completeness**: Included to avoid terminal theme interference
- **Consistency**: Same hex values (#000000, #FFFFFF) across both palette systems

### Text Modifiers
- **Bold**: Heavy/thick text rendering
- **Dim**: Faint/light text rendering  
- **Italic**: Slanted text rendering
- **Underlined**: Underlined text
- **Slow Blink**: Slow blinking text (if supported)
- **Rapid Blink**: Fast blinking text (if supported)
- **Reversed**: Swap foreground and background colors
- **Hidden**: Hidden/invisible text
- **Crossed Out**: Strikethrough text
- **Modifier Combination**: Support for multiple simultaneous modifiers
- **Modifier Removal**: Ability to explicitly remove modifiers

### Style Composition
- **Immutable Style Objects**: Styles are immutable value types
- **Style Patching**: Merge multiple styles with later styles taking precedence
- **Additive Modifiers**: Text modifiers accumulate when styles are combined
- **Color Override**: Later colors override earlier colors in composition
- **Builder Pattern**: Fluent method chaining for style creation

### Platform Adaptations
- **Terminal Capability Detection**: Automatic detection of color depth support
- **Graceful Fallbacks**: Automatic downgrade to supported features
- **Windows Console Support**: Support for both legacy console and Windows Terminal
- **ANSI Escape Sequences**: Proper ANSI/VT sequence generation
- **Cross-Platform Consistency**: Consistent behavior across platforms

### Fluent API Support
- **Extension Methods**: Style shorthand methods on strings and styled types
- **Method Chaining**: Fluent builder pattern support
- **Type Conversions**: Implicit conversions from colors and modifiers to styles
- **Ergonomic API**: Natural, readable syntax for common styling operations

### External Library Integration
- **Bidirectional Conversions**: Convert to/from external color and style libraries
- **Type-Safe Conversions**: Use explicit conversion operators for potentially failing conversions
- **Error Handling**: Proper exception handling for incompatible color/style types
- **Feature Flag Support**: Conditional compilation for optional features (e.g., underline colors)
- **Conversion Patterns**: Consistent patterns for adding support for new external libraries

## Technical Strategy

### Implementation Approach
- Use `readonly struct` for immutable, value-semantic Style type
- Implement text modifiers as `[Flags] enum` for efficient bitwise operations  
- Provide extension methods for fluent styling API
- Use nullable value types for optional color properties
- Implement comprehensive implicit conversion operators

### Performance Strategy
- Minimize allocations through value types and struct usage
- Efficient bitwise operations for modifier flags
- Optimized style patching algorithm
- Avoid boxing in common usage patterns

### API Design Strategy
- Maintain high compatibility with Ratatui's API patterns
- Use C# idioms (PascalCase, extension methods, nullable types)
- Provide both explicit and shorthand syntax options
- Support implicit conversions for ergonomic usage

## Dependencies
- Buffer system (for applying styles to cells)
- Color system implementation
- Terminal backend (for capability detection)

## Implementation Tasks
- **CORE-STYLE-SYSTEM-001**: Implement core Style struct and TextModifier enum
- **CORE-COLOR-ENUM-001**: Implement comprehensive Color enum with ANSI, RGB, and indexed support
- **CORE-COLOR-PARSING-001**: Implement string parsing with comprehensive format and alias support
- **STYLE-PALETTE-001**: Implement Material Design palette system with AccentedPalette and NonAccentedPalette structures
- **STYLE-EXTENSIONS-001**: Create fluent API extension methods
- **STYLE-COMPOSITION-001**: Implement style patching algorithm
- **STYLE-CONVERSIONS-001**: Add implicit conversion operators
- **STYLE-SHORTHANDS-001**: Generate color and modifier shorthand methods
- **STYLE-EXTERNAL-INTEGRATION-001**: Implement external library integration patterns
- **STYLE-SERIALIZATION-001**: Add optional JSON serialization support with backward compatibility

## Acceptance Criteria

### Functional Requirements
- ✅ Style struct supports foreground, background, and optional underline colors
- ✅ Text modifiers can be combined using bitwise operations
- ✅ Style patching correctly merges multiple styles
- ✅ Fluent API enables readable style definitions
- ✅ Implicit conversions work from colors and modifiers to styles
- ✅ Both explicit (`Style.New().Red()`) and shorthand (`"text".Red()`) syntax work

### Performance Requirements
- ✅ Style creation and modification operations are allocation-free
- ✅ Style patching completes in constant time
- ✅ Extension methods have minimal overhead

### Platform Requirements
- ✅ Works consistently on Windows, macOS, and Linux
- ✅ Gracefully handles terminals with limited color support
- ✅ Properly generates ANSI escape sequences for supported features

### API Requirements
- ✅ API feels natural to C# developers
- ✅ Common styling operations require minimal code
- ✅ Type safety prevents invalid style combinations
- ✅ IntelliSense and documentation provide clear guidance

## See Also
- **SPEC-STYLE-005.md**: Technical specification for style system
- **SPEC-BUFFER-002.md**: Buffer system that applies styles to cells
- **CORE-STYLE-SYSTEM-001**: Primary implementation task