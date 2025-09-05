---
id: SPEC-CANVAS-001
title: Canvas System Specification
status: draft
date: 2023-11-28
---

# Canvas System Specification

## Overview

This specification defines the technical implementation requirements for the Canvas drawing system in CycoTui. The Canvas provides a flexible drawing surface with multiple grid types, coordinate transformation, and layer-based rendering capabilities.

## Scope

This specification covers:
- Canvas widget implementation
- Grid type interfaces and implementations
- Coordinate system transformation
- Shape drawing interface
- Layer management system
- Text label overlay system

## Requirements

### Canvas Widget Interface

**Chart Widget Integration**: The Canvas serves as the primary rendering surface for Chart widgets, providing:
- Coordinate transformation from data space to terminal cell space
- High-precision point and line rendering using various marker symbols
- Layer-based rendering for multiple datasets
- Bounds-based clipping for data points outside visible area
- Background color support for chart areas

```csharp
public class Canvas<TDrawFunction> : IWidget
    where TDrawFunction : class // Action<ICanvasContext> or similar
{
    // Configuration properties
    public Block? Block { get; set; }
    public double[] XBounds { get; set; } // [left, right]
    public double[] YBounds { get; set; } // [bottom, top]
    public Color BackgroundColor { get; set; }
    public Marker MarkerType { get; set; }
    
    // Fluent configuration methods
    public Canvas<TDrawFunction> WithBlock(Block block);
    public Canvas<TDrawFunction> WithXBounds(double left, double right);
    public Canvas<TDrawFunction> WithYBounds(double bottom, double top);
    public Canvas<TDrawFunction> WithBackgroundColor(Color color);
    public Canvas<TDrawFunction> WithMarker(Marker marker);
    public Canvas<TDrawFunction> Paint(TDrawFunction drawFunction);
    
    // Widget implementation
    public void Render(Rect area, IBuffer buffer);
}
```

### Grid Type Interface

```csharp
public interface IGrid
{
    // Grid properties
    (double Width, double Height) Resolution { get; }
    
    // Grid operations
    void Paint(int x, int y, Color color);
    ILayer Save();
    void Reset();
}

public interface ILayer
{
    string Content { get; }
    IReadOnlyList<(Color Foreground, Color Background)> Colors { get; }
}
```

### Grid Implementations

#### BrailleGrid
- **Resolution**: 2x4 dots per terminal cell
- **Storage**: UTF-16 code points for Braille patterns (0x2800-0x28FF)
- **Color Support**: Foreground only (no individual dot background colors)
- **Memory Efficiency**: Most compact representation for high-resolution drawing

#### HalfBlockGrid  
- **Resolution**: 1x2 pixels per terminal cell
- **Storage**: Color matrix with per-pixel color information
- **Color Support**: Full foreground and background control per terminal cell
- **Character Selection**: Space, upper half block (▀), lower half block (▄), full block (█)

#### CharGrid
- **Resolution**: 1x1 character per terminal cell  
- **Storage**: Character array with associated colors
- **Color Support**: Foreground color per cell
- **Character Selection**: Configurable character for drawing (dot, block, bar, etc.)

### Coordinate System

```csharp
public interface ICanvasPainter
{
    // Coordinate transformation
    (int X, int Y)? GetGridPoint(double canvasX, double canvasY);
    
    // Drawing operations
    void Paint(int gridX, int gridY, Color color);
    
    // Canvas properties
    (double[] XBounds, double[] YBounds) Bounds { get; }
}
```

#### Coordinate Transformation Algorithm
```csharp
public (int X, int Y)? GetGridPoint(double canvasX, double canvasY)
{
    var (left, right) = (XBounds[0], XBounds[1]);
    var (bottom, top) = (YBounds[0], YBounds[1]);
    
    // Check bounds
    if (canvasX < left || canvasX > right || canvasY < bottom || canvasY > top)
        return null;
        
    var width = right - left;
    var height = top - bottom;
    
    if (width <= 0.0 || height <= 0.0)
        return null;
    
    // Transform to grid coordinates (origin top-left)
    var gridX = (int)Math.Round((canvasX - left) * (Resolution.Width - 1) / width);
    var gridY = (int)Math.Round((top - canvasY) * (Resolution.Height - 1) / height);
    
    return (gridX, gridY);
}
```

### Shape Drawing Interface

The Canvas system must support drawing geometric shapes through a common interface:

```csharp
public interface IShape
{
    void Draw(IPainter painter);
}

public interface IPainter
{
    void Paint(int x, int y, Color color);
    (int x, int y)? GetPoint(double worldX, double worldY);
}
```

#### Circle Shape Implementation

```csharp
public class Circle : IShape
{
    public double X { get; set; }           // Center X coordinate
    public double Y { get; set; }           // Center Y coordinate  
    public double Radius { get; set; }      // Circle radius
    public Color Color { get; set; }        // Circle color

    public Circle(double x, double y, double radius, Color color)
    {
        X = x;
        Y = y;
        Radius = radius;
        Color = color;
    }

    public void Draw(IPainter painter)
    {
        // Implementation details in WIDGET-CANVAS-CIRCLE-001 task
    }
}
```

**Circle Rendering Requirements**:
- Must use parametric circle equation for point generation
- Should sample 360 points around circumference for smooth rendering
- Must handle coordinate transformation from world to screen coordinates
- Should gracefully handle points outside drawable area
- Must use high-precision floating-point operations where available

#### Points Shape Implementation

```csharp
public class Points : IShape
{
    public IReadOnlyList<(double X, double Y)> Coordinates { get; set; }
    public Color Color { get; set; }

    public Points(IReadOnlyList<(double x, double y)> coordinates, Color color)
    {
        Coordinates = coordinates;
        Color = color;
    }

    public void Draw(IPainter painter)
    {
        foreach (var (x, y) in Coordinates)
        {
            if (painter.GetPoint(x, y) is var (gridX, gridY))
            {
                painter.Paint(gridX, gridY, Color);
            }
        }
    }
}
```

**Points Rendering Requirements**:
- Must efficiently process collections of coordinate tuples
- Should use readonly collections for memory efficiency
- Must apply single color to all points in collection
- Should use coordinate transformation for each point
- Must gracefully handle out-of-bounds coordinates via painter filtering
- Should optimize for large point collections where possible
- Must preserve floating-point precision during coordinate processing

```csharp
public interface IShape
{
    void Draw(ICanvasPainter painter);
}

// Example shape implementations
public class Line : IShape
{
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
    public Color Color { get; set; }
    
    public void Draw(ICanvasPainter painter) { /* Bresenham line algorithm */ }
}

public class Rectangle : IShape
{
    public double X { get; set; }        // X position (bottom-left corner)
    public double Y { get; set; }        // Y position (bottom-left corner)
    public double Width { get; set; }    // Rectangle width
    public double Height { get; set; }   // Rectangle height
    public Color Color { get; set; }     // Rectangle border color
    
    public Rectangle(double x, double y, double width, double height, Color color)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = color;
    }
    
    public void Draw(ICanvasPainter painter)
    {
        // Decompose rectangle into 4 line segments
        var lines = new[]
        {
            new Line(X, Y, X, Y + Height, Color),                    // Left edge
            new Line(X, Y + Height, X + Width, Y + Height, Color),   // Top edge  
            new Line(X + Width, Y, X + Width, Y + Height, Color),    // Right edge
            new Line(X, Y, X + Width, Y, Color)                      // Bottom edge
        };
        
        foreach (var line in lines)
        {
            line.Draw(painter);
        }
    }
}

### Map Shape Implementation

```csharp
public class Map : IShape
{
    public MapResolution Resolution { get; set; }
    public Color Color { get; set; }
    
    public Map(MapResolution resolution = MapResolution.High, Color color = default)
    {
        Resolution = resolution;
        Color = color;
    }
    
    public void Draw(ICanvasPainter painter)
    {
        var coordinates = Resolution switch
        {
            MapResolution.High => WorldMapData.HighResolution,
            MapResolution.Low => WorldMapData.LowResolution,
            _ => throw new ArgumentException($"Unsupported resolution: {Resolution}")
        };
        
        foreach (var (longitude, latitude) in coordinates)
        {
            if (painter.GetPoint(longitude, latitude) is var (x, y))
            {
                painter.Paint(x, y, Color);
            }
        }
    }
}

public enum MapResolution
{
    Low,    // ~1000 coordinate points
    High    // ~5000 coordinate points  
}

public static class WorldMapData
{
    /// <summary>
    /// High-resolution world map coordinate data containing 5125 longitude/latitude pairs.
    /// Data source: http://www.gnuplotting.org/plotting-the-world-revisited
    /// </summary>
    public static readonly (double Longitude, double Latitude)[] HighResolution = new[]
    {
        (-163.7128, -78.5956),
        (-163.1058, -78.2233),
        // ... additional 5123 coordinate pairs
    };
    
    /// <summary>
    /// Low-resolution world map coordinate data for performance-sensitive scenarios.
    /// Subset of high-resolution data with approximately 1000 points.
    /// </summary>
    public static readonly (double Longitude, double Latitude)[] LowResolution = new[]
    {
        // Subset of high-resolution coordinates for basic world outline
    };
}
```

**Map Shape Requirements**:
- Must support two resolution levels with different point densities
- Should use pre-computed world coordinate data arrays containing longitude/latitude pairs
- Must integrate with canvas coordinate transformation system to convert geographic coordinates to screen coordinates
- Should handle coordinate points outside drawable area gracefully via painter bounds checking
- Must support color customization through standard Color enum
- Should optimize for performance with large coordinate datasets (5125+ points)
- Must preserve geographic accuracy from original data source
- Should provide low-resolution alternative for performance-sensitive applications
```

### Canvas Context

```csharp
public interface ICanvasContext
{
    // Drawing operations
    void Draw<TShape>(TShape shape) where TShape : IShape;
    
    // Layer management
    void SaveLayer();
    
    // Text labels
    void PrintText(double x, double y, string text);
    void PrintText(double x, double y, ILine line);
}
```

### Layer Management System

```csharp
internal class CanvasContext : ICanvasContext
{
    private IGrid _grid;
    private List<ILayer> _layers;
    private List<TextLabel> _labels;
    private bool _isDirty;
    
    public void SaveLayer()
    {
        if (_isDirty)
        {
            _layers.Add(_grid.Save());
            _grid.Reset();
            _isDirty = false;
        }
    }
    
    public void Draw<TShape>(TShape shape) where TShape : IShape
    {
        _isDirty = true;
        var painter = new CanvasPainter(this, _grid);
        shape.Draw(painter);
    }
}
```

## Technical Approach

### Grid Factory Pattern
```csharp
public static class GridFactory
{
    public static IGrid CreateGrid(Marker marker, int width, int height)
    {
        return marker switch
        {
            Marker.Braille => new BrailleGrid(width, height),
            Marker.HalfBlock => new HalfBlockGrid(width, height),
            Marker.Dot => new CharGrid(width, height, '•'),
            Marker.Block => new CharGrid(width, height, '█'),
            Marker.Bar => new CharGrid(width, height, '▄'),
            _ => throw new ArgumentException($"Unsupported marker: {marker}")
        };
    }
}
```

### Unicode Pattern Handling
```csharp
internal static class BraillePatterns
{
    private const ushort BRAILLE_BLANK = 0x2800;
    
    // 2x4 dot pattern masks
    private static readonly ushort[,] DotMasks = new ushort[4, 2]
    {
        { 0x01, 0x08 }, // Row 0: dots 1, 4
        { 0x02, 0x10 }, // Row 1: dots 2, 5  
        { 0x04, 0x20 }, // Row 2: dots 3, 6
        { 0x40, 0x80 }  // Row 3: dots 7, 8
    };
    
    public static ushort GetPattern(int x, int y)
    {
        return DotMasks[y % 4, x % 2];
    }
}
```

### Performance Optimizations
- **Bounds Checking**: Early rejection of out-of-bounds coordinates
- **Memory Pooling**: Reuse grid instances where possible
- **Lazy Evaluation**: Defer layer rendering until final output
- **Efficient Unicode**: Direct UTF-16 manipulation for Braille patterns

### Shape System

```csharp
public interface IShape
{
    void Draw(IPainter painter);
}

public class Line : IShape
{
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
    public Color Color { get; set; }
    
    public Line(double x1, double y1, double x2, double y2, Color color);
    public void Draw(IPainter painter);
}
```

### Painter Interface

```csharp
public interface IPainter
{
    (double[], double[]) Bounds();
    (int, int)? GetPoint(double worldX, double worldY);
    void Paint(int x, int y, Color color);
}
```

### Line Drawing Algorithm Requirements

- **Bresenham's Algorithm**: Implement efficient line drawing using integer arithmetic
- **Clipping**: Use Cohen-Sutherland algorithm or equivalent for viewport clipping
- **Coordinate Transformation**: Convert from world coordinates (double) to screen coordinates (int)
- **Performance**: Optimize inner loops for pixel-level operations

### Geometric Algorithms

- **Line Clipping**: Cohen-Sutherland algorithm implementation
  - Classify endpoints using outcodes
  - Handle completely visible, invisible, and partially visible lines
  - Clip to rectangular viewport bounds
- **Line Drawing**: Bresenham-based algorithm
  - Separate handling for shallow slopes (dy < dx) and steep slopes (dy >= dx)
  - Direct iteration for horizontal and vertical lines
  - Use saturating arithmetic to prevent coordinate overflow

## Platform-Specific Details

### Windows Considerations
- **Font Support**: Verify Braille and half-block character rendering
- **Console API**: Ensure proper Unicode output through Console API
- **Color Handling**: Test foreground/background color combinations

### Unix/Linux Considerations  
- **Terminal Capabilities**: Detect Unicode and color support
- **Font Dependencies**: Document required font capabilities
- **Character Encoding**: Ensure UTF-8 terminal encoding

## Examples

### Basic Canvas Usage
```csharp
var canvas = new Canvas<Action<ICanvasContext>>()
    .WithXBounds(-10.0, 10.0)
    .WithYBounds(-5.0, 5.0)
    .WithMarker(Marker.Braille)
    .Paint(ctx =>
    {
        ctx.Draw(new Line { X1 = -5, Y1 = 0, X2 = 5, Y2 = 0, Color = Color.White });
        ctx.Draw(new Circle { X = 0, Y = 0, Radius = 3, Color = Color.Red });
        ctx.PrintText(0, -4, "Origin");
    });
```

### Multi-Layer Drawing
```csharp
var canvas = new Canvas<Action<ICanvasContext>>()
    .WithXBounds(0.0, 100.0)
    .WithYBounds(0.0, 100.0)
    .Paint(ctx =>
    {
        // Background layer
        ctx.Draw(new Rectangle { X = 0, Y = 0, Width = 100, Height = 100, Color = Color.Gray });
        ctx.SaveLayer();
        
        // Foreground layer
        ctx.Draw(new Circle { X = 50, Y = 50, Radius = 20, Color = Color.Blue });
    });
```

## See Also

- [007-CANVAS-SYSTEM-001](../features/007-CANVAS-SYSTEM-001.md): Canvas system features
- [SPEC-WIDGET-003](SPEC-WIDGET-003.md): Widget implementation specification
- [SPEC-STYLE-005](SPEC-STYLE-005.md): Style system specification