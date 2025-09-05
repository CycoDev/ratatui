# Color Palette Implementation

## Overview

Implement predefined color palette support for CycoTui, providing Material Design and Tailwind CSS color systems with organized access to color variants. This task creates the palette data structures, static color constants, and utility methods needed for consistent design system support.

## Implementation Approach

### Core Palette Structures

1. **Create Palette Value Types**:
   - `AccentedPalette` struct for Material Design colors with accent variants
   - `NonAccentedPalette` struct for Material Design neutral colors  
   - `Palette` struct for Tailwind CSS colors with C50-C950 variants

2. **Static Color Constants**:
   - Create static readonly instances for all Material and Tailwind palettes
   - Organize in separate namespaces: `CycoTui.Style.Palette.Material` and `CycoTui.Style.Palette.Tailwind`
   - Include all color variants as defined in respective design systems

3. **Color Data Arrays**:
   - Store actual color values in private static arrays
   - Use uint hex values for compile-time constant initialization  
   - Organize by palette family (Red, Blue, etc.)

### Key Challenges

1. **Static Initialization**:
   - Challenge: Rust uses `const fn` for compile-time palette creation
   - Solution: Use static readonly fields with constructor calls or static initialization

2. **Memory Efficiency**:
   - Challenge: Large number of color constants could impact memory usage
   - Solution: Use lazy initialization or pack color data efficiently

3. **API Design**:
   - Challenge: Balancing ease of use with type safety
   - Solution: Provide both strongly-typed access and convenience methods

## Related Components

- **Color.cs**: Core color type that palettes return
- **Style.cs**: Style system that uses palette colors
- **MaterialColors.cs**: Material Design palette implementation
- **TailwindColors.cs**: Tailwind CSS palette implementation

## Integration Points

- **Style System**: Palettes integrate with existing Style.Fg() and Style.Bg() methods
- **Color Parsing**: Palette colors work with existing Color conversion system
- **Backend Rendering**: Palette colors use same rendering pipeline as other colors

## Performance Considerations

- **Static Initialization**: Minimize startup cost through efficient initialization
- **Memory Usage**: Consider lazy loading for rarely-used palettes
- **Access Patterns**: Optimize for common palette color access scenarios

## Testing Approach

1. **Palette Completeness**: Verify all expected colors are present and correct
2. **Color Accuracy**: Test that palette colors match design system specifications
3. **Integration Tests**: Verify palette colors work with style system and rendering
4. **Performance Tests**: Measure initialization and access performance

## Acceptance Criteria

- [ ] Material Design AccentedPalette struct implemented with 14 color variants (C50-C900, A100-A700)
- [ ] Material Design NonAccentedPalette struct implemented with 10 color variants (C50-C900)
- [ ] Tailwind CSS Palette struct implemented with 11 color variants (C50-C950)
- [ ] All Material Design palettes available as static constants (Red, Pink, Purple, etc.)
- [ ] All Tailwind CSS palettes available as static constants (Slate, Gray, Zinc, etc.)
- [ ] Black and White constants included in both palette systems
- [ ] Palette colors integrate seamlessly with existing Style system
- [ ] Unit tests verify color accuracy against design system specifications
- [ ] Documentation examples demonstrate palette usage patterns
- [ ] Performance benchmarks show acceptable initialization and access times

## Implementation Notes

### Material Design Color Variants

Reference the exact color values from the Material Design color system:
- **Accented palettes**: Red, Pink, Purple, Deep Purple, Indigo, Blue, Light Blue, Cyan, Teal, Green, Light Green, Lime, Yellow, Amber, Orange, Deep Orange
- **Non-accented palettes**: Brown, Gray, Blue Gray
- **Color variants**: C50 (lightest) through C900 (darkest), plus A100-A700 accent variants

### Tailwind CSS Color Variants

Based on analysis of `ratatui-core/src/style/palette/tailwind.rs`, reference the exact Tailwind CSS default color palette values:

- **All 22 palettes**: Slate, Gray, Zinc, Neutral, Stone, Red, Orange, Amber, Yellow, Lime, Green, Emerald, Teal, Cyan, Sky, Blue, Indigo, Violet, Purple, Fuchsia, Pink, Rose
- **Color variants**: C50 (lightest) through C950 (darkest) - 11 total variants per palette
- **Additional constants**: Black (#000000) and White (#ffffff) for completeness
- **Documentation**: Include HTML/CSS color swatches in XML documentation comments for visual reference
- **Serialization**: Support optional JSON serialization via System.Text.Json when needed

#### Specific Color Values (from Tailwind CSS specification)

The implementation must use the exact hex values from Tailwind CSS:

```csharp
// Example: Tailwind Blue palette
private static readonly uint[] BlueVariants = { 
    0xeff6ff, // C50
    0xdbeafe, // C100  
    0xbfdbfe, // C200
    0x93c5fd, // C300
    0x60a5fa, // C400
    0x3b82f6, // C500 (base)
    0x2563eb, // C600
    0x1d4ed8, // C700
    0x1e40af, // C800
    0x1e3a8a, // C900
    0x172554  // C950
};
```

#### Documentation Pattern

Follow Ratatui's approach of including visual color swatches in documentation:

```csharp
/// <summary>
/// Tailwind CSS Blue color palette.
/// <para>Visual preview: [Color swatches would be rendered here in documentation]</para>
/// </summary>
/// <example>
/// <code>
/// var primaryBlue = TailwindColors.Blue.C500;  // Base blue color
/// var lightBlue = TailwindColors.Blue.C100;    // Light variant
/// var darkBlue = TailwindColors.Blue.C900;     // Dark variant
/// </code>
/// </example>
public static readonly Palette Blue = Palette.FromU32Array(BlueVariants);
```

### Example Implementation Pattern

```csharp
namespace CycoTui.Style.Palette.Material
{
    public readonly struct AccentedPalette
    {
        public Color C50 { get; }
        public Color C100 { get; }
        // ... other variants
        public Color A100 { get; }
        // ... accent variants
        
        public static AccentedPalette FromVariants(uint[] variants) => 
            new AccentedPalette(/* variant assignments */);
    }
    
    public static class MaterialColors
    {
        public static readonly AccentedPalette Red = AccentedPalette.FromVariants(RedVariants);
        // ... other palette constants
        
        private static readonly uint[] RedVariants = { 0xFFEBEE, 0xFFCDD2, /* ... */ };
        // ... other variant arrays
    }
}
```

## See Also

- **SPEC-STYLE-005.md**: Style system specification with palette requirements
- **005-STYLE-SYSTEM-001.md**: Style system feature document
- **ratatui-core/src/style/palette.rs**: Original Rust implementation
- **ratatui-core/src/style/palette/material.rs**: Material Design reference
- **ratatui-core/src/style/palette/tailwind.rs**: Tailwind CSS reference