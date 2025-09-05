---
id: SPEC-MACROS-001
title: Macro System and Convenience API Specification
status: draft
date: 2023-11-28
---

# Macro System and Convenience API Specification

## Overview

This specification defines the architecture and implementation approach for CycoTui's convenience APIs and macro-equivalent functionality, translating Ratatui's declarative macro system into idiomatic C# patterns.

## Scope

This specification covers:
- Fluent API design patterns for text, layout, and table creation
- Builder pattern implementations for complex type construction
- Extension method strategies for enhanced convenience
- Source generator patterns for compile-time code generation
- Type conversion and implicit operator strategies

## Requirements

### Fluent API Architecture

#### Text Fluent APIs
```csharp
// Equivalent to text![] (empty text)
var text = Text.Empty();
var text = new Text();

// Equivalent to text!["line1", "line2"] (multiple lines)
var text = Text.From("line1", "line2");
var text = Text.Builder().AddLine("line1").AddLine("line2").Build();

// Equivalent to text![line!["hello", "world"]] (line from spans)
var text = Text.From(Line.From("hello", "world"));
var text = Text.Builder().AddLine(Line.From("hello", "world")).Build();

// Equivalent to text!["line"; 3] (repeated line)
var text = Text.Repeat("line", 3);
var text = Text.FromLine("line", count: 3);

// Collection initializer approach
var text = new Text { "line1", "line2" }; // Requires ICollection<Line> implementation

// Extension method approach
var text = new[] { "line1", "line2" }.ToText();
var text = "line".Repeat(3).ToText();

// Mixed content types (equivalent to text![line!["hello", "world"], span!["single"]])
var text = Text.From(
    Line.From("hello", "world"),
    Span.Raw("single").ToLine()
);

// Explicit line and span usage within text
var text = Text.Builder()
    .AddLine(Line.From("hello", "world"))
    .AddLine(Span.Raw("single"))
    .Build();

// Equivalent to span! macro patterns
// span!("hello world") - simple literal
var span = Span.Raw("hello world");
var span = "hello world".ToSpan();

// span!("hello {}", name) - format string
var span = Span.Raw($"hello {name}");
var span = Span.Raw("hello {0}", name);

// span!(variable) - expression
var span = Span.Raw(variable.ToString());
var span = variable.ToSpan();

// span!(Style::new().green(); "hello world") - styled with explicit style
var span = Span.Styled(Style.Default.Green(), "hello world");
var span = "hello world".Styled(Style.Default.Green());

// span!(Color::Green; "hello {}", name) - styled with color
var span = Span.Styled(Color.Green, $"hello {name}");
var span = $"hello {name}".Styled(Color.Green);

// span!(Modifier::BOLD; "hello") - styled with modifier
var span = Span.Styled(Modifier.Bold, "hello");
var span = "hello".Styled(Modifier.Bold);

// Fluent style building approach
var span = "hello world"
    .ToSpan()
    .WithColor(Color.Green)
    .WithModifier(Modifier.Bold);

// Builder pattern approach
var span = Span.Builder()
    .WithText("hello world")
    .WithColor(Color.Green)
    .WithModifier(Modifier.Bold)
    .Build();

// Extension method chaining
var span = "hello world".Green().Bold();
var span = $"hello {name}".Red().Italic();

// Equivalent to span!(Color::Green; "hello {name}")
var span = Span.Text("hello {name}").WithColor(Color.Green);
var span = Span.Styled(Color.Green, "hello {name}");

// Equivalent to line!["hello", span!(Color::Green; "world")]
var line = Line.From("hello", Span.Styled(Color.Green, "world"));
var line = Line.Builder().Add("hello").Add(Span.Styled(Color.Green, "world")).Build();

// Equivalent to line![] (empty line)
var line = Line.Empty();
var line = new Line();

// Equivalent to line!["hello"; 3] (repeated span)
var line = Line.Repeat("hello", 3);
var line = Line.FromSpan("hello", count: 3);

// Collection initializer approach
var line = new Line { "hello", "world" }; // Requires ICollection implementation

// Extension method approach
var line = new[] { "hello", "world" }.ToLine();
var line = "hello".Repeat(3).ToLine();

// Equivalent to text!["line1", "line2"]
var text = Text.From("line1", "line2");
var text = Text.Builder().AddLine("line1").AddLine("line2").Build();
```

#### Layout Fluent APIs
```csharp
// Equivalent to constraint!(>=50)
var constraint = Constraint.Min(50);
var constraint = 50.Min();

// Equivalent to constraint!(<=100)
var constraint = Constraint.Max(100);
var constraint = 100.Max();

// Equivalent to constraint!(==30%)
var constraint = Constraint.Percentage(30);
var constraint = 30.Percent();

// Equivalent to constraint!(==1/3)
var constraint = Constraint.Ratio(1, 3);
var constraint = Constraint.Ratio(1, 3); // No convenient extension for this

// Equivalent to constraint!(==50)
var constraint = Constraint.Length(50);
var constraint = 50.Fixed();

// Equivalent to constraint!(*=1)
var constraint = Constraint.Fill(1);
var constraint = 1.Fill();

// Equivalent to constraints![==50, *=1, >=20]
var constraints = Constraints.From(
    Constraint.Length(50),
    Constraint.Fill(1),
    Constraint.Min(20)
);

// Alternative with extension methods
var constraints = new[] {
    50.Fixed(),
    1.Fill(),
    20.Min()
};

// Equivalent to vertical![==1, *=1, >=3]
var layout = Layout.Vertical()
    .WithConstraints(
        Constraint.Length(1),
        Constraint.Fill(1),
        Constraint.Min(3)
    );

// Alternative concise syntax
var layout = Layout.Vertical(
    1.Fixed(),
    1.Fill(),
    3.Min()
);

// Equivalent to horizontal![>=20, *=1, >=20]
var layout = Layout.Horizontal(
    20.Min(),
    1.Fill(),
    20.Min()
);
```

#### Table Fluent APIs
```csharp
// Equivalent to row![]
var row = Row.Empty();
var row = new Row();

// Equivalent to row!["hello", "world"]
var row = Row.From("hello", "world");
var row = Row.Builder().AddCell("hello").AddCell("world").Build();

// Equivalent to row!["hello"; 3]
var row = Row.Repeat("hello", 3);
var row = Row.FromCell("hello", count: 3);

// Mixed content types
var row = Row.From(
    Text.From("Line 1", "Line 2"),
    Span.Styled(Modifier.Bold, "Cell 2"),
    "Simple string"
);

// Collection initializer approach
var row = new Row { "hello", "world" }; // Requires ICollection implementation

// Extension method approach
var row = new[] { "hello", "world" }.ToRow();
var row = "hello".Repeat(3).ToRow();
```

### Builder Pattern Implementation

#### Core Builder Interface
```csharp
public interface IBuilder<T>
{
    T Build();
}

public interface IFluentBuilder<T, TBuilder> : IBuilder<T>
    where TBuilder : IFluentBuilder<T, TBuilder>
{
    // Fluent methods return TBuilder for chaining
}
```

#### Span Builder Implementation
```csharp
public class SpanBuilder : IFluentBuilder<Span, SpanBuilder>
{
    private string _content = "";
    private Style _style = Style.Default;

    public SpanBuilder WithText(string text)
    {
        _content = text;
        return this;
    }

    public SpanBuilder WithText(FormattableString formattableString)
    {
        _content = formattableString.ToString();
        return this;
    }

    public SpanBuilder WithStyle(Style style)
    {
        _style = style;
        return this;
    }

    public SpanBuilder WithColor(Color color)
    {
        _style = _style.WithForeground(color);
        return this;
    }

    public SpanBuilder WithModifier(Modifier modifier)
    {
        _style = _style.WithModifier(modifier);
        return this;
    }

    public Span Build() => _style == Style.Default 
        ? Span.Raw(_content) 
        : Span.Styled(_content, _style);
}

// Static factory methods for Span - equivalent to span! macro
public static class Span
{
    // span!("literal") or span!(expression)
    public static Span Raw(string content) => new Span(content);
    public static Span Raw(FormattableString content) => new Span(content.ToString());
    public static Span Raw(object expression) => new Span(expression?.ToString() ?? "");
    
    // span!(style; "content") patterns
    public static Span Styled(Style style, string content) => new Span(content, style);
    public static Span Styled(Style style, FormattableString content) => new Span(content.ToString(), style);
    public static Span Styled(Style style, object expression) => new Span(expression?.ToString() ?? "", style);
    
    // Overloads for types convertible to Style (Color, Modifier)
    public static Span Styled(Color color, string content) => new Span(content, Style.Default.WithForeground(color));
    public static Span Styled(Color color, FormattableString content) => new Span(content.ToString(), Style.Default.WithForeground(color));
    public static Span Styled(Modifier modifier, string content) => new Span(content, Style.Default.WithModifier(modifier));
    public static Span Styled(Modifier modifier, FormattableString content) => new Span(content.ToString(), Style.Default.WithModifier(modifier));
    
    public static SpanBuilder Builder() => new SpanBuilder();
}

// Extension methods for span! macro equivalent functionality
public static class SpanExtensions
{
    // Convert any object to Span (equivalent to span!(expression))
    public static Span ToSpan(this object obj) => Span.Raw(obj);
    public static Span ToSpan(this string text) => Span.Raw(text);
    public static Span ToSpan(this FormattableString fs) => Span.Raw(fs);
    
    // Style extension methods for fluent API
    public static Span Styled(this string text, Style style) => Span.Styled(style, text);
    public static Span Styled(this string text, Color color) => Span.Styled(color, text);
    public static Span Styled(this string text, Modifier modifier) => Span.Styled(modifier, text);
    public static Span Styled(this FormattableString text, Style style) => Span.Styled(style, text);
    public static Span Styled(this FormattableString text, Color color) => Span.Styled(color, text);
    public static Span Styled(this FormattableString text, Modifier modifier) => Span.Styled(modifier, text);
    
    // Quick color styling (equivalent to span!(Color::Green; "text"))
    public static Span Red(this string text) => Span.Styled(Color.Red, text);
    public static Span Green(this string text) => Span.Styled(Color.Green, text);
    public static Span Blue(this string text) => Span.Styled(Color.Blue, text);
    public static Span Yellow(this string text) => Span.Styled(Color.Yellow, text);
    public static Span Magenta(this string text) => Span.Styled(Color.Magenta, text);
    public static Span Cyan(this string text) => Span.Styled(Color.Cyan, text);
    public static Span White(this string text) => Span.Styled(Color.White, text);
    public static Span Black(this string text) => Span.Styled(Color.Black, text);
    
    // Quick modifier styling (equivalent to span!(Modifier::BOLD; "text"))
    public static Span Bold(this string text) => Span.Styled(Modifier.Bold, text);
    public static Span Italic(this string text) => Span.Styled(Modifier.Italic, text);
    public static Span Underlined(this string text) => Span.Styled(Modifier.Underlined, text);
    public static Span Dim(this string text) => Span.Styled(Modifier.Dim, text);
    public static Span SlowBlink(this string text) => Span.Styled(Modifier.SlowBlink, text);
    public static Span RapidBlink(this string text) => Span.Styled(Modifier.RapidBlink, text);
    public static Span Reversed(this string text) => Span.Styled(Modifier.Reversed, text);
    public static Span Hidden(this string text) => Span.Styled(Modifier.Hidden, text);
    public static Span Crossed(this string text) => Span.Styled(Modifier.CrossedOut, text);
}

// Implicit conversion support for convenience
public static class SpanConversions
{
    // Allow implicit conversion from string to Span (equivalent to span!(string))
    public static implicit operator Span(string text) => Span.Raw(text);
}
```

#### Text Builder Implementation
```csharp
public class TextBuilder : IFluentBuilder<Text, TextBuilder>
{
    private readonly List<Line> _lines = new();

    public TextBuilder AddLine(Line line)
    {
        _lines.Add(line);
        return this;
    }

    public TextBuilder AddLine(string text)
    {
        _lines.Add(new Line(text));
        return this;
    }

    public TextBuilder AddLine(Span span)
    {
        _lines.Add(new Line(span));
        return this;
    }

    public TextBuilder AddLines(IEnumerable<Line> lines)
    {
        _lines.AddRange(lines);
        return this;
    }

    public TextBuilder RepeatLine(Line line, int count)
    {
        _lines.AddRange(Enumerable.Repeat(line, count));
        return this;
    }

    public TextBuilder RepeatLine(string text, int count)
    {
        _lines.AddRange(Enumerable.Repeat(new Line(text), count));
        return this;
    }

    public Text Build() => new Text(_lines);
}

// Static factory methods for Text - equivalent to text! macro
public static class Text
{
    // text![] (empty text)
    public static Text Empty() => new Text();
    
    // text!["line1", "line2", ...] (multiple lines)
    public static Text From(params object[] lines) => new Text(lines.Select(l => l.ToLine()));
    
    // text!["line"; count] (repeated line)
    public static Text Repeat(Line line, int count) => new Text(Enumerable.Repeat(line, count));
    public static Text Repeat(string text, int count) => new Text(Enumerable.Repeat(new Line(text), count));
    public static Text FromLine(object line, int count) => new Text(Enumerable.Repeat(line.ToLine(), count));
    
    public static TextBuilder Builder() => new TextBuilder();
}

// Extension methods for Text construction
public static class TextExtensions
{
    public static Text ToText(this IEnumerable<Line> lines) => new Text(lines);
    public static Text ToText(this IEnumerable<string> texts) => new Text(texts.Select(t => new Line(t)));
    public static Text ToText(this IEnumerable<object> objects) => new Text(objects.Select(o => o.ToLine()));
    
    // Convert single items to Text
    public static Text ToText(this Line line) => new Text(new[] { line });
    public static Text ToText(this string text) => new Text(new[] { new Line(text) });
    public static Text ToText(this Span span) => new Text(new[] { new Line(span) });
}

// Implicit conversion support for convenience
public static class TextConversions
{
    // Allow implicit conversion from string to Text (equivalent to text!["string"])
    public static implicit operator Text(string text) => Text.From(text);
    
    // Allow implicit conversion from Line to Text
    public static implicit operator Text(Line line) => new Text(new[] { line });
}
```

#### Row Builder Implementation
```csharp
public class RowBuilder : IFluentBuilder<Row, RowBuilder>
{
    public RowBuilder AddCell(Cell cell);
    public RowBuilder AddCell(string text);
    public RowBuilder AddCell(Text text);
    public RowBuilder AddCell(Line line);
    public RowBuilder AddCell(Span span);
    public RowBuilder AddCellStyled(Style style, string text);
    public RowBuilder RepeatCell(Cell cell, int count);
    public RowBuilder RepeatCell(string text, int count);
    public Row Build();
}

// Static factory methods for Row
public static class Row
{
    public static Row Empty() => new Row();
    public static Row From(params object[] cells) => new Row(cells.Select(c => c.ToCell()));
    public static Row Repeat(Cell cell, int count) => new Row(Enumerable.Repeat(cell, count));
    public static Row Repeat(string text, int count) => new Row(Enumerable.Repeat(new Cell(text), count));
    public static Row FromCell(object cell, int count) => new Row(Enumerable.Repeat(cell.ToCell(), count));
    public static RowBuilder Builder() => new RowBuilder();
}

// Extension methods for Row construction
public static class RowExtensions
{
    public static Row ToRow(this IEnumerable<Cell> cells) => new Row(cells);
    public static Row ToRow(this IEnumerable<string> texts) => new Row(texts.Select(t => new Cell(t)));
    public static Row ToRow(this IEnumerable<object> objects) => new Row(objects.Select(o => o.ToCell()));
}
```

#### Line Builder Implementation
```csharp
public class LineBuilder : IFluentBuilder<Line, LineBuilder>
{
    public LineBuilder Add(Span span);
    public LineBuilder Add(string text);
    public LineBuilder AddStyled(Style style, string text);
    public LineBuilder Repeat(Span span, int count);
    public LineBuilder Repeat(string text, int count);
    public Line Build();
}

// Static factory methods for Line
public static class Line
{
    public static Line Empty() => new Line();
    public static Line From(params object[] spans) => new Line(spans.Select(s => s.ToSpan()));
    public static Line Repeat(Span span, int count) => new Line(Enumerable.Repeat(span, count));
    public static Line Repeat(string text, int count) => new Line(Enumerable.Repeat(new Span(text), count));
    public static LineBuilder Builder() => new LineBuilder();
}

// Extension methods for Line construction
public static class LineExtensions
{
    public static Line ToLine(this IEnumerable<Span> spans) => new Line(spans);
    public static Line ToLine(this IEnumerable<string> texts) => new Line(texts.Select(t => new Span(t)));
    public static IEnumerable<T> Repeat<T>(this T item, int count) => Enumerable.Repeat(item, count);
}
```

### Extension Method Strategy

#### Constraint Extensions
```csharp
public static class ConstraintExtensions
{
    // Symbolic operator equivalents to Ratatui constraint! macro
    public static Constraint Fixed(this int value) => Constraint.Length(value);
    public static Constraint Percent(this int value) => Constraint.Percentage(value);
    public static Constraint Fill(this int value) => Constraint.Fill(value);
    public static Constraint Min(this int value) => Constraint.Min(value);
    public static Constraint Max(this int value) => Constraint.Max(value);
}

public static class ConstraintBuilder
{
    // For ratios that can't be expressed as extension methods
    public static Constraint Ratio(int numerator, int denominator) => 
        Constraint.Ratio((uint)numerator, (uint)denominator);
}

// Usage: 
// 50.Fixed()     // equivalent to constraint!(==50)
// 30.Percent()   // equivalent to constraint!(==30%)
// 1.Fill()       // equivalent to constraint!(*=1)
// 20.Min()       // equivalent to constraint!(>=20)
// 100.Max()      // equivalent to constraint!(<=100)
// ConstraintBuilder.Ratio(1, 3)  // equivalent to constraint!(==1/3)
```

#### Style Extensions
```csharp
public static class StyleExtensions
{
    public static Style WithForeground(this Style style, Color color);
    public static Style WithBackground(this Style style, Color color);
    public static Style WithModifier(this Style style, Modifier modifier);
    public static Style Bold(this Style style);
    public static Style Italic(this Style style);
}
```

### Compile-Time Safety Considerations

#### Format String Safety
The Rust span! macro provides compile-time validation of format strings. In C#, we can achieve similar safety through:

```csharp
// Option 1: Use FormattableString for compile-time validation
public static Span Raw(FormattableString fs) => new Span(fs.ToString());
public static Span Styled(Style style, FormattableString fs) => new Span(fs.ToString(), style);

// Usage with compile-time safety:
var name = "World";
var span = Span.Raw($"Hello {name}!"); // Compile-time validated interpolation

// Option 2: Use string interpolation with custom handler (C# 10+)
[InterpolatedStringHandler]
public ref struct SpanInterpolatedStringHandler
{
    // Custom handler for span creation with compile-time validation
}

// Option 3: Roslyn analyzer for format string validation
// Custom analyzer to validate format strings at compile time
```

#### Style Type Safety
Prevent invalid style combinations at compile time:

```csharp
// Use method overloading to ensure only valid style types are accepted
public static Span Styled(Color color, string text) => /* ... */;
public static Span Styled(Modifier modifier, string text) => /* ... */;
public static Span Styled(Style style, string text) => /* ... */;

// Prevent invalid patterns that would cause compile_error! in Rust
// Achieved through method signature design rather than macro pattern matching
```

#### Error Message Clarity
Provide clear error messages for common mistakes:

```csharp
// Use method attributes and XML documentation to guide correct usage
/// <summary>
/// Creates a styled span. Use semicolon syntax equivalent: 
/// <c>text.Styled(style)</c> instead of <c>Span.Create(style, text)</c>
/// </summary>
/// <param name="style">The style to apply</param>
/// <param name="text">The text content</param>
[Obsolete("Use text.Styled(style) for better readability", true)]
public static Span Create(Style style, string text) => throw new NotSupportedException();
```

#### Constraint DSL Generator
```csharp
// Source generator that transforms:
[Constraints("==50, >=20, *=1")]
public static readonly Constraint[] MyConstraints;

// Into:
public static readonly Constraint[] MyConstraints = new[]
{
    Constraint.Length(50),
    Constraint.Min(20),
    Constraint.Fill(1)
};
```

#### Alternative Constraint DSL Approaches
```csharp
// Option 1: String parsing at runtime
var constraints = ConstraintParser.Parse("==50, >=20, *=1, ==30%, ==1/3");

// Option 2: Fluent constraint builder
var constraints = ConstraintBuilder
    .Length(50)
    .Min(20)
    .Fill(1)
    .Percentage(30)
    .Ratio(1, 3)
    .Build();

// Option 3: Collection initializer with extensions
var constraints = new ConstraintCollection
{
    50.Fixed(),    // ==50
    20.Min(),      // >=20
    1.Fill(),      // *=1
    30.Percent(),  // ==30%
    ConstraintBuilder.Ratio(1, 3)  // ==1/3
};
```

#### Text Template Generator
```csharp
// Source generator for complex text templates
[TextTemplate(@"
Name: {name:bold}
Status: {status:green}
Count: {count:red}
")]
public static Text CreateStatusText(string name, string status, int count);
```

## Technical Approach

### Type Conversion Strategy
- Implement implicit operators where semantically clear
- Use explicit From() methods for complex conversions
- Provide overloaded methods for common parameter types
- Support collection initializer syntax where appropriate

### Performance Considerations
- Builder pattern should have minimal allocation overhead
- Fluent APIs should inline to direct constructor calls when possible
- Source generators should produce optimal code
- Extension methods should have zero runtime overhead

### API Design Principles
- Prioritize discoverability through IntelliSense
- Maintain type safety throughout the fluent chain
- Provide both terse and explicit API variants
- Follow C# naming conventions consistently
- Support method chaining where beneficial

## Terms and Definitions

- **Fluent API**: Method chaining interface that reads like natural language
- **Builder Pattern**: Creational pattern for constructing complex objects step by step
- **Source Generator**: Compile-time code generation in C#
- **Extension Method**: Static method that appears as instance method on extended type

## Examples

### Complete Text Creation Example
```csharp
// Rust macro equivalent: text![line!["Name: ".bold(), name.italic()], "Status: OK".green()]
var text = Text.Builder()
    .AddLine(Line.From(
        "Name: ".Bold(),
        name.Italic()
    ))
    .AddLine("Status: OK".Green())
    .Build();

// Alternative fluent syntax
var text = Text.From(
    Line.From("Name: ".Bold(), name.Italic()),
    "Status: OK".Green()
);
```

### Complete Layout Creation Example
```csharp
// Rust macro equivalent: Layout::vertical(constraints![==1, *=1, ==3])
var layout = Layout.Vertical(
    1.Fixed(),
    1.Fill(),
    3.Fixed()
);

// Alternative explicit syntax
var layout = Layout.Vertical()
    .WithConstraints(
        Constraint.Length(1),
        Constraint.Fill(1),
        Constraint.Length(3)
    );
```

## See Also

- 010-MACRO-SYSTEM-001: Macro system feature requirements
- SPEC-API-DESIGN-001: Overall API design principles
- SPEC-TEXT-001: Text system specification
- SPEC-LAYOUT-004: Layout engine specification