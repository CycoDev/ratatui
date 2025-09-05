# Style System Implementation

## Overview

Implement the core style system for CycoTui based on analysis of `ratatui-core/src/style.rs`. This includes the `Style` struct, `TextModifier` flags, color handling, style composition, and fluent API extensions.

## Implementation Approach

### 1. Define TextModifier Flags
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

### 2. Create Style Struct
```csharp
public readonly struct Style : IEquatable<Style>
{
    public Color? Foreground { get; }
    public Color? Background { get; }
    public Color? UnderlineColor { get; }
    public TextModifier AddModifier { get; }
    public TextModifier SubModifier { get; }
    
    // Private constructor for immutability
    private Style(Color? fg, Color? bg, Color? underline, 
                  TextModifier add, TextModifier sub) { }
    
    public static Style New() => new();
    public static Style Reset() => new(/* reset values */);
}
```

### 3. Implement Style Methods
```csharp
public Style Fg(Color color) => new(color, Background, UnderlineColor, AddModifier, SubModifier);
public Style Bg(Color color) => new(Foreground, color, UnderlineColor, AddModifier, SubModifier);
public Style AddModifier(TextModifier modifier) => new(Foreground, Background, UnderlineColor, 
    (AddModifier & ~modifier) | modifier, SubModifier & ~modifier);
```

### 4. Implement Style Patching
```csharp
public Style Patch(Style other)
{
    var fg = other.Foreground ?? Foreground;
    var bg = other.Background ?? Background;
    var underline = other.UnderlineColor ?? UnderlineColor;
    
    var addMod = (AddModifier & ~other.SubModifier) | other.AddModifier;
    var subMod = (SubModifier & ~other.AddModifier) | other.SubModifier;
    
    return new Style(fg, bg, underline, addMod, subMod);
}
```

### 5. Create Extension Methods
```csharp
public static class StyleExtensions
{
    // Color extensions
    public static Style Red(this Style style) => style.Fg(Color.Red);
    public static Style OnBlue(this Style style) => style.Bg(Color.Blue);
    
    // Modifier extensions
    public static Style Bold(this Style style) => style.AddModifier(TextModifier.Bold);
    public static Style NotBold(this Style style) => style.RemoveModifier(TextModifier.Bold);
    
    // String extensions
    public static Span Red(this string text) => new Span(text, Style.New().Red());
}
```

### 6. Add Implicit Conversions
```csharp
public static implicit operator Style(Color color) => Style.New().Fg(color);
public static implicit operator Style(TextModifier modifier) => Style.New().AddModifier(modifier);
public static implicit operator Style((Color fg, Color bg) colors) => 
    Style.New().Fg(colors.fg).Bg(colors.bg);
```

## Key Challenges

### Immutability with Performance
- Use `readonly struct` to ensure value semantics
- Minimize allocations through efficient struct design
- Consider using in-place operations where possible

### Fluent API Design
- Extension methods must feel natural in C#
- Method chaining should be efficient
- IntelliSense experience should be excellent

### Style Composition Logic
- Modifier flag handling requires careful bitwise operations
- Color precedence must follow Ratatui's behavior exactly
- Edge cases in style merging need comprehensive testing

## Related Components

### Dependencies
- **Color**: Must be defined before Style can be implemented
- **Span/Text**: Style will be used by text rendering components

### Integration Points
- **Buffer/Cell**: Cells will store and apply Style objects
- **Widgets**: All widgets will use Style for rendering
- **Backend**: Terminal backends must render Style properties as ANSI sequences

## Testing Approach

### Unit Tests
```csharp
[Test]
public void Style_Patch_MergesCorrectly()
{
    var base = Style.New().Bold().Red();
    var overlay = Style.New().Blue().Italic();
    var result = base.Patch(overlay);
    
    Assert.AreEqual(Color.Blue, result.Foreground);
    Assert.IsTrue(result.AddModifier.HasFlag(TextModifier.Bold));
    Assert.IsTrue(result.AddModifier.HasFlag(TextModifier.Italic));
}
```

### Performance Tests
- Benchmark style creation and modification operations
- Measure memory allocation in typical usage patterns
- Verify constant-time performance for style patching

### Integration Tests
- Test with actual terminal output
- Verify ANSI sequence generation
- Test cross-platform behavior

## Acceptance Criteria

- ✅ Style struct is immutable and uses value semantics
- ✅ TextModifier flags support bitwise operations correctly
- ✅ Style patching matches Ratatui's behavior exactly
- ✅ Extension methods provide fluent API for common operations
- ✅ Implicit conversions work for ergonomic usage
- ✅ Performance is suitable for real-time terminal applications
- ✅ Unit tests cover all style composition edge cases
- ✅ API documentation is complete and includes examples

## See Also
- **SPEC-STYLE-005.md**: Detailed technical specification
- **005-STYLE-SYSTEM-001.md**: Feature requirements
- **ratatui-core-src-style-ANALYSIS.md**: Source code analysis
       public byte G { get; }
       public byte B { get; }
       
       // Constructors for different color types
       private Color(ColorType type, byte index = 0, byte r = 0, byte g = 0, byte b = 0) 
           => /* implementation */
       
       // Static factory methods
       public static Color Reset => new Color(ColorType.Reset);
       public static Color Black => new Color(ColorType.Basic, 0);
       public static Color Red => new Color(ColorType.Basic, 1);
       // ... other basic colors
       
       public static Color Rgb(byte r, byte g, byte b) 
           => new Color(ColorType.Rgb, 0, r, g, b);
       
       public static Color Indexed(byte index) 
           => new Color(ColorType.Indexed, index);
       
       // Equality, hash code, and ToString implementation
   }
   ```

2. **Implement text modifiers**:
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

3. **Create the Style struct**:
   ```csharp
   public readonly struct Style : IEquatable<Style>
   {
       // Style properties
       public Color? Foreground { get; }
       public Color? Background { get; }
       public Color? UnderlineColor { get; }
       public Modifier Modifiers { get; }
       
       // Constructors
       public Style(Color? fg = null, Color? bg = null, 
                   Color? ul = null, Modifier mods = Modifier.None)
           => /* implementation */
       
       // Static properties
       public static Style Default => new Style();
       public static Style Reset => new Style(Color.Reset, Color.Reset);
       
       // Modification methods (returns new Style)
       public Style WithForeground(Color color) => /* implementation */
       public Style WithBackground(Color color) => /* implementation */
       public Style WithUnderlineColor(Color color) => /* implementation */
       public Style WithModifier(Modifier modifier) => /* implementation */
       public Style WithoutModifier(Modifier modifier) => /* implementation */
       
       // Patching/merging styles
       public Style Patch(Style other) => /* implementation */
       
       // Equality, hash code, and ToString implementation
   }
   ```

4. **Implement the IStyled interface**:
   ```csharp
   public interface IStyled
   {
       Style Style { get; }
       IStyled WithStyle(Style style);
   }
   ```

5. **Create extension methods for fluent styling**:
   ```csharp
   public static class StyleExtensions
   {
       // Foreground color extensions
       public static T Black<T>(this T obj) where T : IStyled 
           => (T)obj.WithStyle(obj.Style.WithForeground(Color.Black));
       
       public static T Red<T>(this T obj) where T : IStyled 
           => (T)obj.WithStyle(obj.Style.WithForeground(Color.Red));
       
       // ... other foreground colors
       
       // Background color extensions
       public static T OnBlack<T>(this T obj) where T : IStyled 
           => (T)obj.WithStyle(obj.Style.WithBackground(Color.Black));
       
       public static T OnRed<T>(this T obj) where T : IStyled 
           => (T)obj.WithStyle(obj.Style.WithBackground(Color.Red));
       
       // ... other background colors
       
       // Modifier extensions
       public static T Bold<T>(this T obj) where T : IStyled 
           => (T)obj.WithStyle(obj.Style.WithModifier(Modifier.Bold));
       
       public static T Italic<T>(this T obj) where T : IStyled 
           => (T)obj.WithStyle(obj.Style.WithModifier(Modifier.Italic));
       
       // ... other modifiers
       
       // Generic style application
       public static T Style<T>(this T obj, Style style) where T : IStyled 
           => (T)obj.WithStyle(obj.Style.Patch(style));
   }
   ```

6. **Implement color conversion utilities**:
   ```csharp
   public static class ColorConversion
   {
       // RGB to ANSI 16-color conversion
       public static Color ToAnsi16(Color color) => /* implementation */
       
       // RGB to 256-color conversion
       public static Color ToIndexed(Color color) => /* implementation */
       
       // Get the nearest color in a palette
       public static Color NearestColor(Color color, IReadOnlyList<Color> palette) 
           => /* implementation */
       
       // Color distance calculation (for finding nearest colors)
       public static double ColorDistance(Color a, Color b) => /* implementation */
   }
   ```

7. **Add capability detection**:
   ```csharp
   public enum ColorCapability { None, Basic, Indexed, Rgb }
   
   public static class CapabilityDetection
   {
       // Detect terminal color capability
       public static ColorCapability DetectColorCapability() => /* implementation */
       
       // Check modifier support
       public static bool SupportsModifier(Modifier modifier) => /* implementation */
       
       // Get environment information
       public static string GetTermVariable() => /* implementation */
   }
   ```

## Key Challenges

1. **Extension Method Design**:
   - Ensuring the fluent API is intuitive and consistent
   - Handling generic constraints correctly
   - Maintaining type safety while allowing chaining

2. **Color Conversion**:
   - Implementing accurate color distance algorithms
   - Providing good approximations for color downsampling
   - Handling terminal-specific color quirks

3. **Capability Detection**:
   - Reliably detecting terminal capabilities
   - Providing appropriate fallbacks
   - Cross-platform consistency

4. **Style Composition**:
   - Ensuring correct style patching behavior
   - Handling modifier composition
   - Managing null/optional colors

## Related Components

- **Buffer System**: Will use styles for rendering cells
- **Terminal Backend**: Will convert styles to terminal-specific codes
- **Text Rendering**: Will apply styles to text spans and lines

## Testing Approach

1. **Unit Tests**:
   - Test color models and conversions
   - Verify style composition and patching
   - Test extension method behavior

2. **Visual Tests**:
   - Create sample renders with different styles
   - Verify colors and modifiers appear correctly
   - Test in different terminal emulators

## Acceptance Criteria

1. All color models (basic, indexed, RGB) are implemented
2. Style struct with foreground, background, and modifiers is complete
3. Text modifiers (bold, italic, etc.) are implemented
4. Fluent styling API with extension methods is working
5. Style composition and patching works correctly
6. Color conversion utilities are implemented
7. Capability detection for colors and modifiers is in place
8. Documentation is complete for all public APIs
9. Unit tests verify all functionality
10. Visual tests confirm correct rendering

## See Also

- [SPEC-STYLE-005](../../specs/SPEC-STYLE-005.md): Style system specification
- [005-STYLE-SYSTEM-001](../../features/005-STYLE-SYSTEM-001.md): Style system feature
- [VISION-TECH-002](../../vision/VISION-TECH-002.md): Technical vision