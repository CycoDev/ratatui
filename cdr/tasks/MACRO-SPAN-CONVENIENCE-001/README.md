# Span Convenience API Implementation

## Overview

Implement C# equivalents for Ratatui's span! macro, providing multiple approaches for creating styled and unstyled Span objects with a focus on convenience, type safety, and performance.

## Implementation Approach

### Static Factory Methods
Create static methods on the Span class that mirror span! macro functionality:

```csharp
public static class Span
{
    // Raw span creation (equivalent to span!("text") or span!(expression))
    public static Span Raw(string content);
    public static Span Raw(FormattableString content);
    public static Span Raw(object expression);
    
    // Styled span creation (equivalent to span!(style; "text"))
    public static Span Styled(Style style, string content);
    public static Span Styled(Style style, FormattableString content);
    public static Span Styled(Color color, string content);
    public static Span Styled(Modifier modifier, string content);
    
    public static SpanBuilder Builder();
}
```

### Extension Methods
Provide fluent extension methods for common styling patterns:

```csharp
public static class SpanExtensions
{
    // Basic conversion methods
    public static Span ToSpan(this string text);
    public static Span ToSpan(this object obj);
    
    // Style application methods
    public static Span Styled(this string text, Style style);
    public static Span Styled(this string text, Color color);
    public static Span Styled(this string text, Modifier modifier);
    
    // Quick color methods
    public static Span Red(this string text);
    public static Span Green(this string text);
    public static Span Blue(this string text);
    // ... other colors
    
    // Quick modifier methods  
    public static Span Bold(this string text);
    public static Span Italic(this string text);
    public static Span Underlined(this string text);
    // ... other modifiers
}
```

### Builder Pattern
Implement a fluent builder for complex span construction:

```csharp
public class SpanBuilder : IFluentBuilder<Span, SpanBuilder>
{
    private string _content = "";
    private Style _style = Style.Default;

    public SpanBuilder WithText(string text);
    public SpanBuilder WithText(FormattableString formattableString);
    public SpanBuilder WithStyle(Style style);
    public SpanBuilder WithColor(Color color);
    public SpanBuilder WithModifier(Modifier modifier);
    public Span Build();
}
```

### Implicit Conversions
Support convenient implicit conversions where semantically clear:

```csharp
public static class SpanConversions
{
    // Allow string to Span conversion (equivalent to span!(string))
    public static implicit operator Span(string text) => Span.Raw(text);
}
```

## Key Challenges

### Format String Safety
- **Challenge**: Rust span! macro provides compile-time format string validation
- **Solution**: Use FormattableString parameter overloads for compile-time safety
- **Alternative**: Consider Roslyn analyzers for additional validation

### Style Type Flexibility  
- **Challenge**: span! accepts Color, Modifier, or Style - need equivalent flexibility
- **Solution**: Method overloads for each type, plus implicit conversion operators on Style class

### Error Prevention
- **Challenge**: Rust macro prevents invalid syntax patterns at compile time
- **Solution**: Method signature design and XML documentation to guide correct usage
- **Alternative**: Obsolete methods with clear error messages for common mistakes

### Performance Considerations
- **Challenge**: Avoid allocation overhead compared to direct construction
- **Solution**: Inline simple factory methods, optimize builder pattern for minimal allocations

## Related Components

- **Span class**: Core text span implementation
- **Style system**: Color and modifier types
- **Text hierarchy**: Line and Text classes that contain spans
- **Builder interfaces**: Common builder pattern contracts

## Integration Points

### With Line Creation
Span convenience methods should integrate seamlessly with Line factory methods:

```csharp
var line = Line.From(
    "Hello ".Green(),
    "World".Bold().Red()
);
```

### With Text Creation
Should work within Text construction patterns:

```csharp
var text = Text.From(
    Line.From("Title".Bold().Underlined()),
    Line.From("Status: ".Dim(), "OK".Green())
);
```

### With Widget Rendering
Spans created via convenience APIs should work identically to explicitly constructed spans in all widget contexts.

## Testing Approach

### Unit Tests
Test all factory method and extension method combinations:

```csharp
[Test]
public void Span_Raw_WithString_CreatesCorrectSpan()
{
    var span = Span.Raw("hello");
    Assert.That(span.Content, Is.EqualTo("hello"));
    Assert.That(span.Style, Is.EqualTo(Style.Default));
}

[Test]
public void String_Green_Extension_CreatesStyledSpan()
{
    var span = "hello".Green();
    Assert.That(span.Content, Is.EqualTo("hello"));
    Assert.That(span.Style.Foreground, Is.EqualTo(Color.Green));
}
```

### Integration Tests
Verify convenience APIs work in real widget scenarios:

```csharp
[Test]
public void Paragraph_WithConvenienceSpans_RendersCorrectly()
{
    var paragraph = Paragraph.New(Text.From(
        Line.From("Hello ".Green(), "World".Bold())
    ));
    
    // Test rendering produces expected styled output
}
```

### Performance Tests
Ensure convenience APIs don't introduce significant overhead:

```csharp
[Test]
public void Span_ConvenienceAPI_Performance_ComparedToDirectConstruction()
{
    // Benchmark convenience vs direct construction
}
```

## Acceptance Criteria

- [ ] All span! macro patterns have C# equivalents with similar conciseness
- [ ] Extension methods provide fluent, discoverable styling APIs
- [ ] Format string integration works with string interpolation
- [ ] Performance is within 5% of direct Span constructor calls
- [ ] API prevents common mistakes through design rather than runtime errors
- [ ] IntelliSense shows helpful method signatures and documentation
- [ ] Color and modifier extension methods cover all enum values
- [ ] Builder pattern supports complex multi-step span construction
- [ ] Implicit conversion from string works in appropriate contexts
- [ ] Integration with Line and Text convenience APIs is seamless

## See Also

- 010-MACRO-SYSTEM-001: Overall macro system feature requirements
- SPEC-MACROS-001: Detailed macro system specification
- TEXT-SPAN-001: Core Span implementation task
- MACRO-EXTENSION-METHODS-001: General extension method implementation