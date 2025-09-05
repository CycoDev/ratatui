---
id: SPEC-STYLE-005
title: Style System Specification
status: draft
date: 2023-11-28
---

# Style System Specification

## Overview

The Style System in CycoTui provides a comprehensive approach for styling terminal UI elements with colors, text attributes, and other visual properties. Based on analysis of Ratatui's style implementation, this specification defines how CycoTui will handle styling across different terminal capabilities.

The style system focuses on providing two main approaches:
1. **Explicit Style Struct**: Creating and using Style objects (e.g., `Style.New().Fg(Color.Red)`)
2. **Style Shorthands**: Fluent extension methods (e.g., `"hello".Red().Bold()`)

## Scope

This specification covers:

1. Style struct definition and behavior
2. Text modifier flags and combinations
3. Color models and color handling
4. Style composition and merging (patching)
5. Fluent API patterns and extension methods
6. Platform-specific adaptations
7. Optional feature handling

## Requirements

### Text System Integration

The style system must integrate seamlessly with the text hierarchy system:

#### Text Type Styling Support
- Support styling for Span, Line, and Text types
- Enable style inheritance from parent to child text elements
- Provide style composition for complex text scenarios
- Support style override mechanisms

#### Conversion and Application
- Enable fluent styling API for text types (e.g., `"text".Red().Bold().ToSpan()`)
- Support style application during text type conversion
- Provide efficient style merging for text composition
- Handle default style propagation throughout text hierarchy

#### Performance Optimization
- Minimize style object allocations during text operations
- Support style pooling and reuse for common scenarios
- Optimize style comparison and merging operations
- Provide efficient style inheritance calculations

### External Color Library Integration

Based on analysis of `ratatui-core/src/style/palette_conversion.rs`, the style system must provide integration with external color libraries for professional color handling:

#### Color Library Conversion Support

The system must support conversion operators for integration with .NET color libraries:

```csharp
public readonly struct Color : IEquatable<Color>
{
    // ... existing implementation ...
    
    // Conversion operators for external color libraries
    public static implicit operator Color(System.Drawing.Color drawingColor) =>
        Rgb(drawingColor.R, drawingColor.G, drawingColor.B);
        
    public static explicit operator System.Drawing.Color(Color color) => color.Type switch
    {
        ColorType.Rgb => System.Drawing.Color.FromArgb(color.R, color.G, color.B),
        ColorType.Reset => System.Drawing.Color.Empty,
        _ => ConvertToDrawingColor(color)
    };
    
    // Support for ImageSharp colors (if available)
    #if IMAGESHARP_SUPPORT
    public static implicit operator Color(SixLabors.ImageSharp.Color imageSharpColor)
    {
        var rgb = imageSharpColor.ToPixel<Rgb24>();
        return Rgb(rgb.R, rgb.G, rgb.B);
    }
    #endif
    
    // Support for SkiaSharp colors (if available)  
    #if SKIASHARP_SUPPORT
    public static implicit operator Color(SkiaSharp.SKColor skColor) =>
        Rgb(skColor.Red, skColor.Green, skColor.Blue);
    #endif
}
```

#### Professional Color Space Support

For applications requiring professional color handling, the system should support integration with color space libraries:

```csharp
namespace CycoTui.Style.ColorSpace
{
    public static class ColorSpaceConversions
    {
        // Linear sRGB to gamma-corrected sRGB conversion
        public static Color FromLinearSrgb(float r, float g, float b)
        {
            byte GammaCorrect(float linear) => (byte)(
                linear <= 0.0031308f 
                    ? Math.Max(0, Math.Min(255, linear * 12.92f * 255f))
                    : Math.Max(0, Math.Min(255, (1.055f * Math.Pow(linear, 1f/2.4f) - 0.055f) * 255f))
            );
            
            return Color.Rgb(GammaCorrect(r), GammaCorrect(g), GammaCorrect(b));
        }
        
        // Support for various color space inputs when color libraries are available
        #if COLORFUL_SUPPORT
        public static Color FromHsl(Colorful.HSL hsl)
        {
            var rgb = Colorful.Converters.HSLtoRGB.Convert(hsl);
            return Color.Rgb((byte)(rgb.R * 255), (byte)(rgb.G * 255), (byte)(rgb.B * 255));
        }
        
        public static Color FromLab(Colorful.LAB lab)
        {
            var rgb = Colorful.Converters.LABtoRGB.Convert(lab);
            return Color.Rgb((byte)(rgb.R * 255), (byte)(rgb.G * 255), (byte)(rgb.B * 255));
        }
        #endif
    }
}
```

#### Generic Color Conversion Framework

To support flexible integration with various color libraries:

```csharp
public interface IColorConverter<T>
{
    Color FromExternal(T externalColor);
    T ToExternal(Color color);
}

public static class ColorConversions
{
    private static readonly Dictionary<Type, object> Converters = new();
    
    public static void RegisterConverter<T>(IColorConverter<T> converter)
    {
        Converters[typeof(T)] = converter;
    }
    
    public static Color From<T>(T externalColor)
    {
        if (Converters.TryGetValue(typeof(T), out var converter))
        {
            return ((IColorConverter<T>)converter).FromExternal(externalColor);
        }
        
        throw new NotSupportedException($"No converter registered for type {typeof(T)}");
    }
    
    public static T To<T>(Color color)
    {
        if (Converters.TryGetValue(typeof(T), out var converter))
        {
            return ((IColorConverter<T>)converter).ToExternal(color);
        }
        
        throw new NotSupportedException($"No converter registered for type {typeof(T)}");
    }
}
```

### Style Struct Definition

The core `Style` struct must provide:

```csharp
public readonly struct Style
{
    public Color? Foreground { get; }
    public Color? Background { get; }
    public Color? UnderlineColor { get; } // Optional feature
    public TextModifier AddModifier { get; }
    public TextModifier SubModifier { get; }
    
    // Constructors
    public static Style New() { }
    public static Style Reset() { }
    
    // Color methods
    public Style Fg(Color color) { }
    public Style Bg(Color color) { }
    public Style UnderlineColor(Color color) { } // Optional feature
    
    // Modifier methods
    public Style Bold() { }
    public Style Italic() { }
    public Style Underlined() { }
    // ... other modifiers
    
    // Style composition
    public Style Patch(Style other) { }
}
```

### Color System Integration

Based on analysis of `ratatui-core/src/style/color.rs` and `ratatui-core/src/style/palette.rs`, the style system must provide comprehensive color support including predefined palettes:

#### Color Enum Definition
```csharp
public enum Color
{
    // Reset color
    Reset = 0,
    
    // Standard ANSI colors (30-37, 40-47)
    Black,
    Red, 
    Green,
    Yellow,
    Blue,
    Magenta,
    Cyan,
    Gray,
    
    // Bright ANSI colors (90-97, 100-107)
    DarkGray,
    LightRed,
    LightGreen,
    LightYellow,
    LightBlue,
    LightMagenta,
    LightCyan,
    White,
    
    // Extended color modes
    Rgb,     // 24-bit true color
    Indexed  // 8-bit 256 color
}

public readonly struct Color : IEquatable<Color>
{
    public ColorType Type { get; }
    public byte R { get; }      // RGB red component or indexed value
    public byte G { get; }      // RGB green component 
    public byte B { get; }      // RGB blue component
    
    // Factory methods for different color types
    public static Color Reset => new(ColorType.Reset);
    public static Color Black => new(ColorType.Black);
    public static Color Red => new(ColorType.Red);
    // ... other ANSI colors
    
    public static Color Rgb(byte r, byte g, byte b) => new(ColorType.Rgb, r, g, b);
    public static Color Indexed(byte index) => new(ColorType.Indexed, index, 0, 0);
    public static Color FromU32(uint value) => Rgb((byte)(value >> 16), (byte)(value >> 8), (byte)value);
    
    // String parsing with comprehensive format support
    public static Color Parse(string colorString);
    public static bool TryParse(string colorString, out Color color);
    
    // HSL/HSLuv conversion support (optional feature)
    public static Color FromHsl(float hue, float saturation, float lightness);  // Requires color library
    public static Color FromHsluv(float hue, float saturation, float lightness); // Requires color library
    
    // Conversion operators
    public static implicit operator Color((byte r, byte g, byte b) rgb) => Rgb(rgb.r, rgb.g, rgb.b);
    public static implicit operator Color(byte[] rgb) => Rgb(rgb[0], rgb[1], rgb[2]); // Length 3 or 4
}
```

#### Color String Parsing Requirements

The color parsing must support all formats used by Ratatui:

1. **ANSI Color Names**: Case-insensitive, with comprehensive alias support
   - Standard: "red", "green", "blue", etc.
   - Bright variants: "lightred", "light red", "light-red", "light_red", "brightred", "bright red"
   - Aliases: "grey"/"gray", "silver" → Gray, "lightblack" → DarkGray, "lightwhite"/"lightgray" → White

2. **Hex RGB Colors**: "#RRGGBB" format (case-insensitive)
   - Examples: "#FF0000", "#00ff00", "#0000FF"

3. **Indexed Colors**: Numeric string "0" to "255"
   - Examples: "10", "42", "255"

4. **Normalization Rules**:
   - Remove spaces, hyphens, underscores: "light-red" → "lightred"
   - Convert "bright" → "light": "brightred" → "lightred"
   - Handle spelling variants: "grey" → "gray"

#### Color Serialization Support (Optional Feature)

When JSON serialization is needed:

```csharp
[JsonConverter(typeof(ColorJsonConverter))]
public readonly struct Color
{
    // Implementation...
}

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return Color.Parse(reader.GetString()!);
        }
        
        // Support legacy format: {"Rgb": [255, 0, 0]} or {"Indexed": 10}
        // Implementation for backward compatibility...
    }
    
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
```

#### Color Palette System

Based on analysis of `ratatui-core/src/style/palette.rs`, the style system must provide predefined color palettes for common design systems:

##### Material Design Palette Support

```csharp
namespace CycoTui.Style.Palette.Material
{
    public readonly struct AccentedPalette
    {
        // Color variants from 50 (lightest) to 900 (darkest)
        public Color C50 { get; }
        public Color C100 { get; }
        public Color C200 { get; }
        public Color C300 { get; }
        public Color C400 { get; }
        public Color C500 { get; }   // Base color
        public Color C600 { get; }
        public Color C700 { get; }
        public Color C800 { get; }
        public Color C900 { get; }
        
        // Accent variants
        public Color A100 { get; }
        public Color A200 { get; }
        public Color A400 { get; }
        public Color A700 { get; }
        
        public static AccentedPalette FromVariants(uint[] variants)
        {
            // variants should be 14 elements: [c50, c100, ..., c900, a100, a200, a400, a700]
            return new AccentedPalette
            {
                C50 = Color.FromU32(variants[0]),
                C100 = Color.FromU32(variants[1]),
                // ... other variant assignments
                A100 = Color.FromU32(variants[10]),
                A200 = Color.FromU32(variants[11]),
                A400 = Color.FromU32(variants[12]),
                A700 = Color.FromU32(variants[13])
            };
        }
    }
    
    public readonly struct NonAccentedPalette
    {
        // Color variants from 50 (lightest) to 900 (darkest)
        public Color C50 { get; }
        public Color C100 { get; }
        public Color C200 { get; }
        public Color C300 { get; }
        public Color C400 { get; }
        public Color C500 { get; }   // Base color
        public Color C600 { get; }
        public Color C700 { get; }
        public Color C800 { get; }
        public Color C900 { get; }
        
        public static NonAccentedPalette FromVariants(uint[] variants)
        {
            // variants should be 10 elements: [c50, c100, ..., c900]
            return new NonAccentedPalette
            {
                C50 = Color.FromU32(variants[0]),
                C100 = Color.FromU32(variants[1]),
                // ... other variant assignments
                C900 = Color.FromU32(variants[9])
            };
        }
    }
    
    // Predefined Material Design color palettes
    public static class MaterialColors
    {
        // Accented palettes (with A100-A700 variants)
        public static readonly AccentedPalette Red = AccentedPalette.FromVariants(RedVariants);
        public static readonly AccentedPalette Pink = AccentedPalette.FromVariants(PinkVariants);
        public static readonly AccentedPalette Purple = AccentedPalette.FromVariants(PurpleVariants);
        public static readonly AccentedPalette DeepPurple = AccentedPalette.FromVariants(DeepPurpleVariants);
        public static readonly AccentedPalette Indigo = AccentedPalette.FromVariants(IndigoVariants);
        public static readonly AccentedPalette Blue = AccentedPalette.FromVariants(BlueVariants);
        public static readonly AccentedPalette LightBlue = AccentedPalette.FromVariants(LightBlueVariants);
        public static readonly AccentedPalette Cyan = AccentedPalette.FromVariants(CyanVariants);
        public static readonly AccentedPalette Teal = AccentedPalette.FromVariants(TealVariants);
        public static readonly AccentedPalette Green = AccentedPalette.FromVariants(GreenVariants);
        public static readonly AccentedPalette LightGreen = AccentedPalette.FromVariants(LightGreenVariants);
        public static readonly AccentedPalette Lime = AccentedPalette.FromVariants(LimeVariants);
        public static readonly AccentedPalette Yellow = AccentedPalette.FromVariants(YellowVariants);
        public static readonly AccentedPalette Amber = AccentedPalette.FromVariants(AmberVariants);
        public static readonly AccentedPalette Orange = AccentedPalette.FromVariants(OrangeVariants);
        public static readonly AccentedPalette DeepOrange = AccentedPalette.FromVariants(DeepOrangeVariants);
        
        // Non-accented palettes (only C50-C900 variants)
        public static readonly NonAccentedPalette Brown = NonAccentedPalette.FromVariants(BrownVariants);
        public static readonly NonAccentedPalette Gray = NonAccentedPalette.FromVariants(GrayVariants);
        public static readonly NonAccentedPalette BlueGray = NonAccentedPalette.FromVariants(BlueGrayVariants);
        
        // Black and white for completeness
        public static readonly Color Black = Color.FromU32(0x000000);
        public static readonly Color White = Color.FromU32(0xFFFFFF);
        
        // Private static arrays containing the actual color values
        private static readonly uint[] RedVariants = { /* ... color values ... */ };
        private static readonly uint[] PinkVariants = { /* ... color values ... */ };
        // ... other variant arrays
    }
}
```

##### Tailwind CSS Palette Support

Based on analysis of `ratatui-core/src/style/palette/tailwind.rs`, CycoTui must provide comprehensive Tailwind CSS color palette support with all 22 color palettes plus black and white constants.

```csharp
namespace CycoTui.Style.Palette.Tailwind
{
    /// <summary>
    /// Represents a Tailwind CSS color palette with 11 variants from 50 (lightest) to 950 (darkest).
    /// Based on the Tailwind CSS default color palette: https://tailwindcss.com/docs/customizing-colors#default-color-palette
    /// </summary>
    [Serializable]
    public readonly struct Palette
    {
        // Color variants from 50 (lightest) to 950 (darkest) 
        public Color C50 { get; init; }
        public Color C100 { get; init; }
        public Color C200 { get; init; }
        public Color C300 { get; init; }
        public Color C400 { get; init; }
        public Color C500 { get; init; }   // Base color
        public Color C600 { get; init; }
        public Color C700 { get; init; }
        public Color C800 { get; init; }
        public Color C900 { get; init; }
        public Color C950 { get; init; }   // Tailwind-specific darkest variant
        
        public static Palette FromU32Array(uint[] variants)
        {
            if (variants.Length != 11)
                throw new ArgumentException("Tailwind palette must have exactly 11 color variants", nameof(variants));
                
            return new Palette
            {
                C50 = Color.FromU32(variants[0]),
                C100 = Color.FromU32(variants[1]),
                C200 = Color.FromU32(variants[2]),
                C300 = Color.FromU32(variants[3]),
                C400 = Color.FromU32(variants[4]),
                C500 = Color.FromU32(variants[5]),
                C600 = Color.FromU32(variants[6]),
                C700 = Color.FromU32(variants[7]),
                C800 = Color.FromU32(variants[8]),
                C900 = Color.FromU32(variants[9]),
                C950 = Color.FromU32(variants[10])
            };
        }
    }
    
    /// <summary>
    /// Provides all Tailwind CSS color palettes as static constants.
    /// Includes 22 palettes plus black and white for completeness.
    /// </summary>
    public static class TailwindColors
    {
        // Neutral palettes
        public static readonly Palette Slate = Palette.FromU32Array(SlateVariants);
        public static readonly Palette Gray = Palette.FromU32Array(GrayVariants);
        public static readonly Palette Zinc = Palette.FromU32Array(ZincVariants);
        public static readonly Palette Neutral = Palette.FromU32Array(NeutralVariants);
        public static readonly Palette Stone = Palette.FromU32Array(StoneVariants);
        
        // Red spectrum
        public static readonly Palette Red = Palette.FromU32Array(RedVariants);
        public static readonly Palette Orange = Palette.FromU32Array(OrangeVariants);
        public static readonly Palette Amber = Palette.FromU32Array(AmberVariants);
        public static readonly Palette Yellow = Palette.FromU32Array(YellowVariants);
        
        // Green spectrum
        public static readonly Palette Lime = Palette.FromU32Array(LimeVariants);
        public static readonly Palette Green = Palette.FromU32Array(GreenVariants);
        public static readonly Palette Emerald = Palette.FromU32Array(EmeraldVariants);
        public static readonly Palette Teal = Palette.FromU32Array(TealVariants);
        
        // Blue spectrum
        public static readonly Palette Cyan = Palette.FromU32Array(CyanVariants);
        public static readonly Palette Sky = Palette.FromU32Array(SkyVariants);
        public static readonly Palette Blue = Palette.FromU32Array(BlueVariants);
        public static readonly Palette Indigo = Palette.FromU32Array(IndigoVariants);
        
        // Purple spectrum
        public static readonly Palette Violet = Palette.FromU32Array(VioletVariants);
        public static readonly Palette Purple = Palette.FromU32Array(PurpleVariants);
        public static readonly Palette Fuchsia = Palette.FromU32Array(FuchsiaVariants);
        public static readonly Palette Pink = Palette.FromU32Array(PinkVariants);
        public static readonly Palette Rose = Palette.FromU32Array(RoseVariants);
        
        // Black and white for completeness (matches terminal themes)
        public static readonly Color Black = Color.FromU32(0x000000);
        public static readonly Color White = Color.FromU32(0xffffff);
        
        // Private static arrays containing the actual Tailwind CSS color values
        // These must match the exact hex values from the Tailwind CSS specification
        private static readonly uint[] SlateVariants = { 
            0xf8fafc, 0xf1f5f9, 0xe2e8f0, 0xcbd5e1, 0x94a3b8, 
            0x64748b, 0x475569, 0x334155, 0x1e293b, 0x0f172a, 0x020617 
        };
        
        private static readonly uint[] GrayVariants = { 
            0xf9fafb, 0xf3f4f6, 0xe5e7eb, 0xd1d5db, 0x9ca3af, 
            0x6b7280, 0x4b5563, 0x374151, 0x1f2937, 0x111827, 0x030712 
        };
        
        private static readonly uint[] ZincVariants = { 
            0xfafafa, 0xf4f4f5, 0xe4e4e7, 0xd4d4d8, 0xa1a1aa, 
            0x71717a, 0x52525b, 0x3f3f46, 0x27272a, 0x18181b, 0x09090b 
        };
        
        private static readonly uint[] NeutralVariants = { 
            0xfafafa, 0xf5f5f5, 0xe5e5e5, 0xd4d4d4, 0xa3a3a3, 
            0x737373, 0x525252, 0x404040, 0x262626, 0x171717, 0x0a0a0a 
        };
        
        private static readonly uint[] StoneVariants = { 
            0xfafaf9, 0xf5f5f4, 0xe7e5e4, 0xd6d3d1, 0xa8a29e, 
            0x78716c, 0x57534e, 0x44403c, 0x292524, 0x1c1917, 0x0c0a09 
        };
        
        private static readonly uint[] RedVariants = { 
            0xfef2f2, 0xfee2e2, 0xfecaca, 0xfca5a5, 0xf87171, 
            0xef4444, 0xdc2626, 0xb91c1c, 0x991b1b, 0x7f1d1d, 0x450a0a 
        };
        
        private static readonly uint[] OrangeVariants = { 
            0xfff7ed, 0xffedd5, 0xfed7aa, 0xfdba74, 0xfb923c, 
            0xf97316, 0xea580c, 0xc2410c, 0x9a3412, 0x7c2d12, 0x431407 
        };
        
        private static readonly uint[] AmberVariants = { 
            0xfffbeb, 0xfef3c7, 0xfde68a, 0xfcd34d, 0xfbbf24, 
            0xf59e0b, 0xd97706, 0xb45309, 0x92400e, 0x78350f, 0x451a03 
        };
        
        private static readonly uint[] YellowVariants = { 
            0xfefce8, 0xfef9c3, 0xfef08a, 0xfde047, 0xfacc15, 
            0xeab308, 0xca8a04, 0xa16207, 0x854d0e, 0x713f12, 0x422006 
        };
        
        private static readonly uint[] LimeVariants = { 
            0xf7fee7, 0xecfccb, 0xd9f99d, 0xbef264, 0xa3e635, 
            0x84cc16, 0x65a30d, 0x4d7c0f, 0x3f6212, 0x365314, 0x1a2e05 
        };
        
        private static readonly uint[] GreenVariants = { 
            0xf0fdf4, 0xdcfce7, 0xbbf7d0, 0x86efac, 0x4ade80, 
            0x22c55e, 0x16a34a, 0x15803d, 0x166534, 0x14532d, 0x052e16 
        };
        
        private static readonly uint[] EmeraldVariants = { 
            0xecfdf5, 0xd1fae5, 0xa7f3d0, 0x6ee7b7, 0x34d399, 
            0x10b981, 0x059669, 0x047857, 0x065f46, 0x064e3b, 0x022c22 
        };
        
        private static readonly uint[] TealVariants = { 
            0xf0fdfa, 0xccfbf1, 0x99f6e4, 0x5eead4, 0x2dd4bf, 
            0x14b8a6, 0x0d9488, 0x0f766e, 0x115e59, 0x134e4a, 0x042f2e 
        };
        
        private static readonly uint[] CyanVariants = { 
            0xecfeff, 0xcffafe, 0xa5f3fc, 0x67e8f9, 0x22d3ee, 
            0x06b6d4, 0x0891b2, 0x0e7490, 0x155e75, 0x164e63, 0x083344 
        };
        
        private static readonly uint[] SkyVariants = { 
            0xf0f9ff, 0xe0f2fe, 0xbae6fd, 0x7dd3fc, 0x38bdf8, 
            0x0ea5e9, 0x0284c7, 0x0369a1, 0x075985, 0x0c4a6e, 0x082f49 
        };
        
        private static readonly uint[] BlueVariants = { 
            0xeff6ff, 0xdbeafe, 0xbfdbfe, 0x93c5fd, 0x60a5fa, 
            0x3b82f6, 0x2563eb, 0x1d4ed8, 0x1e40af, 0x1e3a8a, 0x172554 
        };
        
        private static readonly uint[] IndigoVariants = { 
            0xeef2ff, 0xe0e7ff, 0xc7d2fe, 0xa5b4fc, 0x818cf8, 
            0x6366f1, 0x4f46e5, 0x4338ca, 0x3730a3, 0x312e81, 0x1e1b4b 
        };
        
        private static readonly uint[] VioletVariants = { 
            0xf5f3ff, 0xede9fe, 0xddd6fe, 0xc4b5fd, 0xa78bfa, 
            0x8b5cf6, 0x7c3aed, 0x6d28d9, 0x5b21b6, 0x4c1d95, 0x2e1065 
        };
        
        private static readonly uint[] PurpleVariants = { 
            0xfaf5ff, 0xf3e8ff, 0xe9d5ff, 0xd8b4fe, 0xc084fc, 
            0xa855f7, 0x9333ea, 0x7e22ce, 0x6b21a8, 0x581c87, 0x3b0764 
        };
        
        private static readonly uint[] FuchsiaVariants = { 
            0xfdf4ff, 0xfae8ff, 0xf5d0fe, 0xf0abfc, 0xe879f9, 
            0xd946ef, 0xc026d3, 0xa21caf, 0x86198f, 0x701a75, 0x4a044e 
        };
        
        private static readonly uint[] PinkVariants = { 
            0xfdf2f8, 0xfce7f3, 0xfbcfe8, 0xf9a8d4, 0xf472b6, 
            0xec4899, 0xdb2777, 0xbe185d, 0x9d174d, 0x831843, 0x500724 
        };
        
        private static readonly uint[] RoseVariants = { 
            0xfff1f2, 0xffe4e6, 0xfecdd3, 0xfda4af, 0xfb7185, 
            0xf43f5e, 0xe11d48, 0xbe123c, 0x9f1239, 0x881337, 0x4c0519 
        };
    }
}
```

##### Palette Usage Examples

```csharp
// Using Material Design colors
var errorStyle = Style.New()
    .Fg(MaterialColors.Red.C500)
    .Bg(MaterialColors.Red.C50);

var accentButton = Style.New()
    .Fg(Color.White)
    .Bg(MaterialColors.Blue.A400);

// Using Tailwind colors  
var warningStyle = Style.New()
    .Fg(TailwindColors.Yellow.C700)
    .Bg(TailwindColors.Yellow.C100);

var darkTheme = Style.New()
    .Fg(TailwindColors.Gray.C200)
    .Bg(TailwindColors.Gray.C800);
```

#### Terminal Color Capability Detection

```csharp
public enum ColorSupport
{
    None,           // No color support
    Basic,          // 16 ANSI colors
    Extended,       // 256 indexed colors  
    TrueColor       // 24-bit RGB colors
}

public interface IColorCapabilityDetector
{
    ColorSupport DetectColorSupport();
    bool SupportsUnderlineColor();
}
```

### Widget-Specific Styling Requirements

#### Scrollbar Widget Styling

Based on scrollbar widget analysis, the style system must support multi-component styling for complex widgets:

```csharp
public class ScrollbarStyle
{
    public Style ThumbStyle { get; set; } = Style.Default;
    public Style TrackStyle { get; set; } = Style.Default;
    public Style BeginStyle { get; set; } = Style.Default;
    public Style EndStyle { get; set; } = Style.Default;
    
    // Fluent API for scrollbar-specific styling
    public ScrollbarStyle WithThumbStyle(Style style) => 
        new ScrollbarStyle { ThumbStyle = style, TrackStyle = this.TrackStyle, BeginStyle = this.BeginStyle, EndStyle = this.EndStyle };
    
    public ScrollbarStyle WithTrackStyle(Style style) =>
        new ScrollbarStyle { ThumbStyle = this.ThumbStyle, TrackStyle = style, BeginStyle = this.BeginStyle, EndStyle = this.EndStyle };
    
    // Apply same style to all components
    public ScrollbarStyle WithUniformStyle(Style style) =>
        new ScrollbarStyle { ThumbStyle = style, TrackStyle = style, BeginStyle = style, EndStyle = style };
}
```

Requirements for multi-component widget styling:
- Independent styling for each visual component (thumb, track, begin/end symbols)
- Fluent API for configuring component-specific styles  
- Uniform styling option for consistent appearance
- Integration with widget builder patterns

### Text Modifier System

```csharp
[Flags]
public enum TextModifier : uint
{
    None = 0,
    Bold = 1 << 0,
    Dim = 1 << 1,
    Italic = 1 << 2,
    Underlined = 1 << 3,
    SlowBlink = 1 << 4,
    RapidBlink = 1 << 5,
    Reversed = 1 << 6,
    Hidden = 1 << 7,
    CrossedOut = 1 << 8
}
```

### Style Composition
    public Style Bg(Color color) { }
    public Style UnderlineColor(Color color) { } // Optional
    
    // Modifier methods
    public Style AddModifier(TextModifier modifier) { }
    public Style RemoveModifier(TextModifier modifier) { }
    
    // Composition
    public Style Patch(Style other) { }
}
```

### Text Modifier Flags

Text modifiers must be implemented as flag enumeration:

```csharp
[Flags]
public enum TextModifier : ushort
{
    None = 0,
    Bold = 1 << 0,
    Dim = 1 << 1,
    Italic = 1 << 2,
    Underlined = 1 << 3,
    SlowBlink = 1 << 4,
    RapidBlink = 1 << 5,
    Reversed = 1 << 6,
    Hidden = 1 << 7,
    CrossedOut = 1 << 8
}
```

### Style Composition Algorithm

The `Patch` method must implement style merging with overflow handling for text that exceeds bar space:

1. **Color Properties**: Later style overrides earlier style
   ```csharp
   public Style Patch(Style other)
   {
       return new Style
       {
           Foreground = other.Foreground ?? this.Foreground,
           Background = other.Background ?? this.Background,
           UnderlineColor = other.UnderlineColor ?? this.UnderlineColor,
           Modifiers = this.Modifiers | other.Modifiers
       };
   }
   ```

2. **Text Overflow Handling**: When text exceeds available space, split text and apply different styles
   ```csharp
   public void RenderValueWithDifferentStyles(
       Buffer buffer, 
       Rect area, 
       int barLength, 
       Style defaultValueStyle, 
       Style barStyle)
   {
       if (!string.IsNullOrEmpty(text))
       {
           var style = defaultValueStyle.Patch(this.ValueStyle);
           
           // Render first part with value style
           buffer.SetStringN(area.X, area.Y, text, barLength, style);
           
           // Render overflow part with bar style if text is longer than bar
           if (text.Length > barLength)
           {
               // Find safe character boundary for split
               var splitIndex = GetCharacterBoundary(text, barLength);
               var firstPart = text.Substring(0, splitIndex);
               var secondPart = text.Substring(splitIndex);
               
               var overflowStyle = barStyle.Patch(this.Style);
               buffer.SetStringN(
                   area.X + firstPart.Length, 
                   area.Y, 
                   secondPart, 
                   area.Width - firstPart.Length, 
                   overflowStyle);
           }
       }
   }
   
   private int GetCharacterBoundary(string text, int maxLength)
   {
       // Use StringInfo to find proper grapheme boundaries
       var stringInfo = new StringInfo(text);
       int length = 0;
       for (int i = 0; i < stringInfo.LengthInTextElements; i++)
       {
           length += StringInfo.GetNextTextElement(text, length).Length;
           if (length > maxLength)
               return length - StringInfo.GetNextTextElement(text, length - 1).Length;
       }
       return length;
   }
   ```
   result.Foreground = other.Foreground ?? this.Foreground;
   result.Background = other.Background ?? this.Background;
   ```

2. **Modifier Handling**: Complex logic to handle add/remove conflicts
   ```csharp
   // Remove conflicting modifiers
   result.AddModifier = this.AddModifier & ~other.SubModifier;
   result.AddModifier |= other.AddModifier;
   
   // Update remove modifiers
   result.SubModifier = this.SubModifier & ~other.AddModifier;
   result.SubModifier |= other.SubModifier;
   ```

### Color Models

The system must support multiple color representations:

```csharp
public abstract class Color
{
    public static Color Reset { get; }
    public static Color Black { get; }
    public static Color Red { get; }
    // ... other basic colors
    
    public static Color Rgb(byte r, byte g, byte b) { }
    public static Color Indexed(byte index) { }
}
```

### Fluent API Extensions

Style shorthand methods must be available via extension methods:

```csharp
public static class StyleExtensions
{
    // Color extensions
    public static Style Red(this Style style) => style.Fg(Color.Red);
    public static Style OnBlue(this Style style) => style.Bg(Color.Blue);
    
    // Modifier extensions  
    public static Style Bold(this Style style) => style.AddModifier(TextModifier.Bold);
    public static Style NotBold(this Style style) => style.RemoveModifier(TextModifier.Bold);
}
```

### Conversion Operators

Implicit conversions must be supported for ergonomic usage:

```csharp
public static implicit operator Style(Color color) => Style.New().Fg(color);
public static implicit operator Style(TextModifier modifier) => Style.New().AddModifier(modifier);
public static implicit operator Style((Color fg, Color bg) colors) => Style.New().Fg(colors.fg).Bg(colors.bg);
```

## Technical Approach

### Immutability Strategy

The Style struct must be immutable with value semantics:
- Use `readonly struct` for the Style type
- All methods return new Style instances
- Properties are get-only
- Internal state uses private readonly fields

### Performance Considerations

1. **Struct Design**: Use value types to avoid allocations
2. **Method Chaining**: Support efficient builder patterns
3. **Nullable Colors**: Use nullable value types for optional colors
4. **Bitwise Operations**: Efficient modifier flag operations

### Optional Features

Handle optional features like underline color:

1. **Conditional Compilation**: Use preprocessor directives
2. **Feature Interfaces**: Define optional capability interfaces
3. **Runtime Detection**: Check terminal capabilities

## Platform-Specific Details

### Windows Considerations
- Ensure ANSI escape sequence support is enabled
- Provide fallbacks for legacy console applications
- Handle Windows Terminal vs ConHost differences

### Unix Considerations  
- Standard ANSI escape sequence support
- Handle terminal capability variations
- Support terminfo-based capability detection

## Examples

### Basic Usage
```csharp
// Explicit style creation
var headingStyle = Style.New()
    .Fg(Color.Black)
    .Bg(Color.Green)
    .AddModifier(TextModifier.Bold | TextModifier.Italic);

// Shorthand style creation
var sameStyle = Style.New().Black().OnGreen().Bold().Italic();

// Direct application
var span = new Span("Hello", headingStyle);
```

### Style Composition
```csharp
var baseStyle = Style.New().Bold();
var colorStyle = Style.New().Red().OnBlue();
var combinedStyle = baseStyle.Patch(colorStyle);
// Results in: Bold + Red foreground + Blue background
```

### String Extensions
```csharp
// Extension methods on strings
var styledText = "Hello World".Red().Bold().OnBlue();
```

## See Also

- **SPEC-BUFFER-002.md**: Cell implementation that uses styles
- **005-STYLE-SYSTEM-001.md**: Feature specification
- **CORE-STYLE-SYSTEM-001**: Implementation task
   
   public struct Color
   {
       public ColorType Type { get; }
       public byte Index { get; }  // For Basic, Bright, and Indexed
       public byte R { get; }      // For RGB
       public byte G { get; }
       public byte B { get; }
       
       // Static factory methods for different color types
       public static Color Reset { get; }
       public static Color Black { get; }
       public static Color Red { get; }
       // ... other basic colors
       public static Color Rgb(byte r, byte g, byte b) => /* implementation */;
       public static Color Indexed(byte index) => /* implementation */;
   }
   ```

### Text Modifiers

The style system must support text attributes through modifiers:

1. **Standard Modifiers**:
   - Bold, Italic, Underlined, Crossed, Reversed, Dim, Hidden

2. **Advanced Modifiers** (when supported):
   - Rapid Blink, Slow Blink, Double Underline

3. **Modifier Implementation**:
   ```csharp
   [Flags]
   public enum Modifier
   {
       None         = 0,
       Bold         = 1 << 0,
       Dim          = 1 << 1,
       Italic       = 1 << 2,
       Underlined   = 1 << 3,
       DoubleUnderlined = 1 << 4,
       SlowBlink    = 1 << 5,
       RapidBlink   = 1 << 6,
       Reversed     = 1 << 7,
       Hidden       = 1 << 8,
       CrossedOut   = 1 << 9
   }
   ```

### Style Composition

Styles must be composable to allow flexible styling:

1. **Style Structure**:
   ```csharp
   public struct Style
   {
       public Color? Foreground { get; }
       public Color? Background { get; }
       public Color? UnderlineColor { get; }  // Optional feature
       public Modifier Modifiers { get; }
       
       // Factory methods
       public static Style Default { get; }
       public static Style Reset { get; }
       
       // Composition methods
       public Style WithForeground(Color color) => /* implementation */;
       public Style WithBackground(Color color) => /* implementation */;
       public Style WithUnderlineColor(Color color) => /* implementation */;
       public Style WithModifier(Modifier modifier) => /* implementation */;
       public Style WithoutModifier(Modifier modifier) => /* implementation */;
       
       // Patch one style over another
       public Style Patch(Style other) => /* implementation */;
   }
   ```

2. **Style Resolution**:
   - Styles should follow a patching model where unset properties don't override existing ones
   - Modifiers should be combined with OR operations
   - Color properties should only override if explicitly set

### Style Inheritance

UI elements should inherit styles from parents with local overrides:

1. **Inheritance Model**:
   - Container widgets pass their style to children
   - Children can override specific aspects of the parent style
   - Style resolution follows the patching rules

2. **Default Styles**:
   - System should provide reasonable defaults
   - Application can define global theme styles
   - Individual widgets can override as needed

### Fluent Styling API

Based on analysis of the `stylize.rs` file, the system must provide a fluent API for styling:

```csharp
public static class StyleExtensions
{
    // Foreground colors
    public static T Black<T>(this T obj) where T : IStyled => /* implementation */;
    public static T Red<T>(this T obj) where T : IStyled => /* implementation */;
    public static T Green<T>(this T obj) where T : IStyled => /* implementation */;
    // ... other colors
    
    // Background colors
    public static T OnBlack<T>(this T obj) where T : IStyled => /* implementation */;
    public static T OnRed<T>(this T obj) where T : IStyled => /* implementation */;
    // ... other background colors
    
    // Modifiers
    public static T Bold<T>(this T obj) where T : IStyled => /* implementation */;
    public static T Italic<T>(this T obj) where T : IStyled => /* implementation */;
    // ... other modifiers
    
    // Generic style application
    public static T Style<T>(this T obj, Style style) where T : IStyled => /* implementation */;
}

// Interface for stylable objects
public interface IStyled
{
    Style Style { get; }
    IStyled WithStyle(Style style);
}
```

### Platform Adaptation

The style system must adapt to different terminal capabilities:

1. **Capability Detection**:
   - Detect color support level (16, 256, RGB)
   - Detect modifier support
   - Adapt to terminal type and environment

2. **Color Downsampling**:
   - Convert RGB colors to indexed or basic when needed
   - Use perceptual color mapping for best approximation

3. **Modifier Fallbacks**:
   - Provide fallbacks for unsupported modifiers
   - Document expected behavior across terminals

## Technical Approach

### Implementation Strategy

The style system will be implemented as:

1. **Core Components**:
   - `Color` struct for color representation
   - `Modifier` enum (flags) for text attributes
   - `Style` struct for combining colors and modifiers
   - Extension methods for fluent styling API

2. **Type Hierarchy**:
   - `IStyled` interface for stylable objects
   - Extension methods on `IStyled` for fluent styling
   - Implementation on key types like `Span`, `Line`, etc.

3. **Style Processing**:
   - Terminal backends will convert styles to platform-specific codes
   - Style resolution will patch styles together
   - Capability adaptation will happen at the backend level

### Color Conversions

The system will provide utilities for color space conversions:

1. **RGB to Indexed**:
   - Map RGB colors to the nearest color in the 256-color palette
   - Use perceptual color distance algorithms

2. **RGB to ANSI**:
   - Map RGB colors to the nearest ANSI color
   - Consider brightness and contrast for readability

3. **Color Spaces**:
   - Support conversion between RGB and other color spaces
   - Consider perceptual uniformity for mapping

### Capability Detection

The system will detect terminal capabilities:

1. **Detection Methods**:
   - Check environment variables (TERM, COLORTERM)
   - Query terminal capabilities when possible
   - Use platform-specific detection mechanisms

2. **Capability Levels**:
   - Basic (16 colors)
   - Extended (256 colors)
   - True Color (24-bit RGB)
   - Modifier support (which modifiers work)

### Fallback Mechanisms

For terminals with limited capabilities:

1. **Color Fallbacks**:
   - RGB → Indexed → Basic → None
   - Use the best available approximation

2. **Modifier Fallbacks**:
   - Skip unsupported modifiers
   - Provide alternative representation when sensible

## Examples

### Fluent Styling API

```csharp
// Styling text with extension methods
var text = "Error:".Red().Bold() + " Something went wrong".White();

// Creating a styled span
var header = new Span("Header").Blue().OnWhite().Underlined();

// Combining styles
var warning = new Paragraph("Warning")
    .Yellow()
    .Bold()
    .Style(new Style().WithBackground(Color.Black));

// Using RGB colors
var custom = new Span("Custom Color")
    .Style(new Style().WithForeground(Color.Rgb(128, 64, 192)));
```

### Style Composition

```csharp
// Default style
var defaultStyle = new Style()
    .WithForeground(Color.White)
    .WithBackground(Color.Black);

// Inherit and override
var highlightStyle = defaultStyle
    .WithForeground(Color.Yellow)
    .WithModifier(Modifier.Bold);

// Patch styles
var baseStyle = new Style().WithForeground(Color.Blue);
var specificStyle = new Style().WithBackground(Color.White);
var combined = baseStyle.Patch(specificStyle);
// combined has blue foreground and white background
```

#### Widget-Level Style Composition

Widgets handle style composition at different levels to achieve flexible styling:

##### Cell Widget Style Composition
Based on analysis of table cell styling (`ratatui-widgets/src/table/cell.rs`):

- **Two-Layer Styling Model**: Cell widgets implement a two-layer style composition system
  1. **Cell Area Style**: Applied to entire cell area (background, borders)
  2. **Content Style**: Applied to text content within the cell
  
- **Rendering Order**:
  ```csharp
  internal void Render(Rect area, Buffer buffer)
  {
      // Layer 1: Apply cell style to entire area
      buffer.SetStyle(area, _cellStyle);
      
      // Layer 2: Render text content with its own styles
      _textContent.Render(area, buffer);
  }
  ```

- **Style Precedence Rules**:
  - Cell style applies to entire cell area (backgrounds, area-wide effects)
  - Text content styles layer on top for content-specific styling (foreground, text attributes)
  - Text alignment handled through underlying Text widget properties
  - Style inheritance: Cell style → Text content styles for overlapping properties

- **Use Cases**:
  - Cell background colors while preserving text foreground colors
  - Area-wide selection highlighting with custom text styling
  - Border/padding styling independent of content styling

##### General Widget Style Composition
```

### Color Conversion System

Based on analysis of `ratatui-termwiz/src/lib.rs`, the style system must provide comprehensive color conversion capabilities for different terminal backends:

#### Backend Color Conversion Interface
```csharp
public interface IColorConverter<TBackendColor>
{
    TBackendColor ToBackendColor(Color color);
    Color FromBackendColor(TBackendColor backendColor);
}

// Example implementation for a hypothetical terminal library
public class TerminalColorConverter : IColorConverter<TerminalColor>
{
    public TerminalColor ToBackendColor(Color color) => color switch
    {
        { Type: ColorType.Reset } => TerminalColor.Default,
        { Type: ColorType.Basic, Value: var v } => ConvertBasicColor(v),
        { Type: ColorType.Indexed, Value: var i } => TerminalColor.Indexed(i),
        { Type: ColorType.Rgb, R: var r, G: var g, B: var b } => 
            TerminalColor.TrueColor(r, g, b),
        _ => TerminalColor.Default
    };
    
    public Color FromBackendColor(TerminalColor termColor) => termColor switch
    {
        { Type: TerminalColorType.Default } => Color.Reset,
        { Type: TerminalColorType.Ansi, Index: var i } => ConvertAnsiToBasic(i),
        { Type: TerminalColorType.Indexed, Index: var i } => Color.Indexed(i),
        { Type: TerminalColorType.Rgb, R: var r, G: var g, B: var b } => 
            Color.Rgb(r, g, b),
        _ => Color.Reset
    };
}
```

#### Color Space Conversion
Support for gamma correction and color space conversion:

```csharp
public static class ColorSpaceConverter
{
    // Convert from linear RGB to sRGB (gamma correction)
    public static Color FromLinearRgb(float r, float g, float b)
    {
        byte ConvertChannel(float value) => (byte)(
            value <= 0.0031308f 
                ? Math.Max(0, Math.Min(255, value * 12.92f * 255f))
                : Math.Max(0, Math.Min(255, (1.055f * Math.Pow(value, 1f/2.4f) - 0.055f) * 255f))
        );
        
        return Color.Rgb(ConvertChannel(r), ConvertChannel(g), ConvertChannel(b));
    }
    
    // Handle alpha channel (typically ignored in terminal contexts)
    public static Color FromSrgba(float r, float g, float b, float alpha)
    {
        // Alpha is typically ignored in terminal rendering
        return Color.Rgb(
            (byte)(r * 255f), 
            (byte)(g * 255f), 
            (byte)(b * 255f)
        );
    }
}
```

#### ANSI Color Mapping
Comprehensive mapping between different ANSI color representations:

```csharp
public static class AnsiColorMapping
{
    // Map standard ANSI colors to Ratatui equivalents
    private static readonly Dictionary<AnsiColor, Color> AnsiToColor = new()
    {
        { AnsiColor.Black, Color.Black },
        { AnsiColor.Maroon, Color.Red },
        { AnsiColor.Green, Color.Green },
        { AnsiColor.Olive, Color.Yellow },
        { AnsiColor.Navy, Color.Blue },
        { AnsiColor.Purple, Color.Magenta },
        { AnsiColor.Teal, Color.Cyan },
        { AnsiColor.Silver, Color.Gray },
        { AnsiColor.Grey, Color.DarkGray },
        { AnsiColor.Red, Color.LightRed },
        { AnsiColor.Lime, Color.LightGreen },
        { AnsiColor.Yellow, Color.LightYellow },
        { AnsiColor.Blue, Color.LightBlue },
        { AnsiColor.Fuchsia, Color.LightMagenta },
        { AnsiColor.Aqua, Color.LightCyan },
        { AnsiColor.White, Color.White }
    };
    
    public static Color FromAnsi(AnsiColor ansiColor) => 
        AnsiToColor.TryGetValue(ansiColor, out var color) ? color : Color.Reset;
}
```

#### Modifier Conversion
Support for converting different modifier representations:

```csharp
public static class ModifierConverter
{
    public static Modifier FromIntensity(Intensity intensity) => intensity switch
    {
        Intensity.Normal => Modifier.None,
        Intensity.Bold => Modifier.Bold,
        Intensity.Dim => Modifier.Dim,
        _ => Modifier.None
    };
    
    public static Modifier FromUnderline(UnderlineType underlineType) => underlineType switch
    {
        UnderlineType.None => Modifier.None,
        UnderlineType.Single or UnderlineType.Double or 
        UnderlineType.Curly or UnderlineType.Dashed or 
        UnderlineType.Dotted => Modifier.Underlined,
        _ => Modifier.None
    };
    
    public static Modifier FromBlink(BlinkType blinkType) => blinkType switch
    {
        BlinkType.None => Modifier.None,
        BlinkType.Slow => Modifier.SlowBlink,
        BlinkType.Rapid => Modifier.RapidBlink,
        _ => Modifier.None
    };
}
```

## See Also

- [VISION-CORE-001.md](../vision/VISION-CORE-001.md): Core vision
- [VISION-TECH-002.md](../vision/VISION-TECH-002.md): Technical vision
- [SPEC-BUFFER-002.md](SPEC-BUFFER-002.md): Buffer specification
- [005-STYLE-SYSTEM-001.md](../features/005-STYLE-SYSTEM-001.md): Style system feature