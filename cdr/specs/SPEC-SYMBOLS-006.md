---
id: SPEC-SYMBOLS-006
title: Symbols and Unicode Handling
status: draft
date: 2023-11-28
---

# Symbols and Unicode Handling

## Overview

The Symbols system in CycoTui provides a comprehensive collection of Unicode characters and symbols used for rendering UI elements in the terminal. These include box-drawing characters, block elements, braille patterns, markers, and other special symbols that enable rich terminal graphics.

Based on analysis of Ratatui's symbols implementation, this specification defines how CycoTui will organize, represent, and handle terminal symbols across different platforms and terminal capabilities.

## Scope

This specification covers:

1. Organization and categorization of terminal symbols
2. Unicode handling and compatibility
3. Fallback mechanisms for limited terminals
4. Symbol merging and composition
5. Integration with the rendering system

## Requirements

### Symbol Categories

The following symbol categories must be supported:

1. **Bar**: Characters for vertical bars with various heights
2. **Block**: Block characters of different horizontal widths
3. **Border**: Box-drawing characters for various border styles
4. **Braille**: Braille patterns for high-resolution "pixel" drawing
5. **Half Block**: Half-block characters for higher-resolution coloring
6. **Line**: Line-drawing characters for charts and graphs
7. **Marker**: Markers for points and indicators
8. **Scrollbar**: Specialized characters for scrollbar rendering
9. **Shade**: Shading characters with different densities

### Bar Symbols

The Bar symbol category provides characters for rendering bars with different heights. These are particularly useful for charts, gauges, and progress indicators.

```csharp
public static class Bar
{
    // Individual height levels
    public const string FULL = "█";
    public const string SEVEN_EIGHTHS = "▇";
    public const string THREE_QUARTERS = "▆";
    public const string FIVE_EIGHTHS = "▅";
    public const string HALF = "▄";
    public const string THREE_EIGHTHS = "▃";
    public const string ONE_QUARTER = "▂";
    public const string ONE_EIGHTH = "▁";
    public const string EMPTY = " ";
    
    // Sets of bar symbols with different granularity
    public static readonly BarSet NineLevels = new(
        EMPTY, ONE_EIGHTH, ONE_QUARTER, THREE_EIGHTHS, 
        HALF, FIVE_EIGHTHS, THREE_QUARTERS, SEVEN_EIGHTHS, FULL
    );
    
    public static readonly BarSet ThreeLevels = new(
        EMPTY, HALF, FULL
    );
}

// Immutable collection of bar symbols
public record BarSet
{
    private readonly string[] _bars;
    
    public BarSet(params string[] bars)
    {
        _bars = bars;
    }
    
    // Get bar symbol closest to the given ratio (0.0 to 1.0)
    public string GetBarSymbol(double ratio)
    {
        if (ratio <= 0.0) return _bars[0];
        if (ratio >= 1.0) return _bars[^1];
        
        double step = 1.0 / (_bars.Length - 1);
        int index = (int)Math.Round(ratio / step);
        return _bars[index];
    }
}
```

### Block Symbols

The Block symbol category provides characters for rendering horizontal blocks of different widths. These are useful for horizontal gauges and progress bars.

```csharp
public static class Block
{
    // Individual width levels
    public const string FULL = "█";
    public const string SEVEN_EIGHTHS = "▉";
    public const string THREE_QUARTERS = "▊";
    public const string FIVE_EIGHTHS = "▋";
    public const string HALF = "▌";
    public const string THREE_EIGHTHS = "▍";
    public const string ONE_QUARTER = "▎";
    public const string ONE_EIGHTH = "▏";
    public const string EMPTY = " ";
    
    // Sets with different granularity
    public static readonly BlockSet NineLevels = new()
    {
        Empty = EMPTY,
        OneEighth = ONE_EIGHTH,
        OneQuarter = ONE_QUARTER,
        ThreeEighths = THREE_EIGHTHS,
        Half = HALF,
        FiveEighths = FIVE_EIGHTHS,
        ThreeQuarters = THREE_QUARTERS,
        SevenEighths = SEVEN_EIGHTHS,
        Full = FULL
    };
    
    public static readonly BlockSet ThreeLevels = new()
    {
        Empty = EMPTY,
        Half = HALF,
        Full = FULL
    };
}

// Immutable collection of block symbols with named fields
public record BlockSet
{
    public string Empty { get; init; } = " ";
    public string OneEighth { get; init; } = "▏";
    public string OneQuarter { get; init; } = "▎";
    public string ThreeEighths { get; init; } = "▍";
    public string Half { get; init; } = "▌";
    public string FiveEighths { get; init; } = "▋";
    public string ThreeQuarters { get; init; } = "▊";
    public string SevenEighths { get; init; } = "▉";
    public string Full { get; init; } = "█";
    
    // Get block symbol closest to the given ratio (0.0 to 1.0)
    public string GetBlockSymbol(double ratio)
    {
        if (ratio <= 0.0) return Empty;
        if (ratio >= 1.0) return Full;
        
        if (ratio < 0.125) return Empty;
        if (ratio < 0.25) return OneEighth;
        if (ratio < 0.375) return OneQuarter;
        if (ratio < 0.5) return ThreeEighths;
        if (ratio < 0.625) return Half;
        if (ratio < 0.75) return FiveEighths;
        if (ratio < 0.875) return ThreeQuarters;
        return SevenEighths;
    }
}
```

### Border Symbols

The Border symbol category provides box-drawing characters for various border styles. These are essential for creating boxes, panels, and tables.

```csharp
public static class Border
{
    // Border characters for different styles
    public static readonly BorderSet Plain = new()
    {
        TopLeft = "┌",
        TopRight = "┐",
        BottomLeft = "└",
        BottomRight = "┘",
        Horizontal = "─",
        Vertical = "│",
        Top = "┬",
        Bottom = "┴",
        Left = "├",
        Right = "┤",
        Cross = "┼"
    };
    
    public static readonly BorderSet Rounded = new()
    {
        TopLeft = "╭",
        TopRight = "╮",
        BottomLeft = "╰",
        BottomRight = "╯",
        Horizontal = "─",
        Vertical = "│",
        Top = "┬",
        Bottom = "┴",
        Left = "├",
        Right = "┤",
        Cross = "┼"
    };
    
    public static readonly BorderSet Double = new()
    {
        TopLeft = "╔",
        TopRight = "╗",
        BottomLeft = "╚",
        BottomRight = "╝",
        Horizontal = "═",
        Vertical = "║",
        Top = "╦",
        Bottom = "╩",
        Left = "╠",
        Right = "╣",
        Cross = "╬"
    };
    
    public static readonly BorderSet Thick = new()
    {
        TopLeft = "┏",
        TopRight = "┓",
        BottomLeft = "┗",
        BottomRight = "┛",
        Horizontal = "━",
        Vertical = "┃",
        Top = "┳",
        Bottom = "┻",
        Left = "┣",
        Right = "┫",
        Cross = "╋"
    };
    
    // Additional border styles available: Hidden, QuadrantInside, QuadrantOutside, etc.
}

// Immutable collection of border symbols with named fields
public record BorderSet
{
    // Corner characters
    public string TopLeft { get; init; } = "┌";
    public string TopRight { get; init; } = "┐";
    public string BottomLeft { get; init; } = "└";
    public string BottomRight { get; init; } = "┘";
    
    // Edge characters
    public string Horizontal { get; init; } = "─";
    public string Vertical { get; init; } = "│";
    
    // Junction characters
    public string Top { get; init; } = "┬";
    public string Bottom { get; init; } = "┴";
    public string Left { get; init; } = "├";
    public string Right { get; init; } = "┤";
    public string Cross { get; init; } = "┼";
    
    // Helper methods to get characters based on position
    public string GetHorizontalChar(bool isTop, bool isLeft, bool isRight)
    {
        if (isLeft && isRight) return Horizontal;
        if (isLeft) return isTop ? TopLeft : BottomLeft;
        if (isRight) return isTop ? TopRight : BottomRight;
        return isTop ? Top : Bottom;
    }
    
    public string GetVerticalChar(bool isLeft, bool isTop, bool isBottom)
    {
        if (isTop && isBottom) return Vertical;
        if (isTop) return isLeft ? TopLeft : TopRight;
        if (isBottom) return isLeft ? BottomLeft : BottomRight;
        return isLeft ? Left : Right;
    }
}
```

### Braille Symbols

The Braille symbol category provides Unicode Braille patterns for high-resolution "pixel" drawing. Each Braille character contains 8 dots arranged in a 2×4 grid, allowing for 256 different patterns.

```csharp
public static class Braille
{
    // Base code point for empty Braille pattern
    public const ushort BLANK = 0x2800;
    
    // Bit patterns for each dot position in the Braille cell
    // Arranged in a 2×4 grid (2 columns × 4 rows)
    public static readonly ushort[,] DOTS = new ushort[4, 2]
    {
        { 0x0001, 0x0008 }, // Row 1: Dots 1 and 4
        { 0x0002, 0x0010 }, // Row 2: Dots 2 and 5
        { 0x0004, 0x0020 }, // Row 3: Dots 3 and 6
        { 0x0040, 0x0080 }  // Row 4: Dots 7 and 8
    };
    
    // Convert coordinates to Braille pattern
    public static char GetPattern(bool[,] dots)
    {
        ushort pattern = BLANK;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                if (dots[y, x]) pattern += DOTS[y, x];
            }
        }
        return (char)pattern;
    }
    
    // Set a specific dot in a pattern
    public static char SetDot(char pattern, int x, int y)
    {
        if (x < 0 || x > 1 || y < 0 || y > 3) return pattern;
        return (char)((ushort)pattern | DOTS[y, x]);
    }
    
    // Create a pattern from a set of points
    public static char FromPoints(IEnumerable<(int X, int Y)> points)
    {
        ushort pattern = BLANK;
        foreach (var (x, y) in points)
        {
            if (x >= 0 && x <= 1 && y >= 0 && y <= 3)
            {
                pattern |= DOTS[y, x];
            }
        }
        return (char)pattern;
    }
}
```

Example usage:

```csharp
// Creating a line with Braille patterns
public static string CreateBrailleLine(int length)
{
    var builder = new StringBuilder();
    for (int i = 0; i < length; i++)
    {
        // Create a pattern with random dots
        var pattern = Braille.BLANK;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                if (Random.Shared.Next(2) == 1)
                {
                    pattern |= Braille.DOTS[y, x];
                }
            }
        }
        builder.Append((char)pattern);
    }
    return builder.ToString();
}
```

### Half Block Symbols

The Half Block symbol category provides Unicode block characters that split a character cell horizontally. These are particularly useful for high-resolution color rendering by using both foreground and background colors in a single character.

```csharp
public static class HalfBlock
{
    // Half block characters
    public const char UPPER = '▀';  // Upper half block
    public const char LOWER = '▄';  // Lower half block
    public const char FULL = '█';   // Full block
}
```

#### High-Resolution Rendering Technique

Half blocks enable a powerful technique for doubling the vertical resolution of terminal graphics. By using the upper half block with different foreground and background colors, you can effectively display two different colors in a single character cell.

Example usage:

```csharp
// Render two pixels vertically in one character cell
public static Cell RenderTwoPixels(Color upperColor, Color lowerColor)
{
    return new Cell
    {
        Content = HalfBlock.UPPER,  // Upper half block
        Foreground = upperColor,    // Color for the upper half
        Background = lowerColor     // Color for the lower half
    };
}

// Create a high-resolution color canvas
public static Cell[,] CreateHighResCanvas(Color[,] pixels)
{
    int height = pixels.GetLength(0);
    int width = pixels.GetLength(1);
    
    // Output canvas has half the height (2 pixels per cell)
    Cell[,] canvas = new Cell[height / 2, width];
    
    for (int y = 0; y < height; y += 2)
    {
        for (int x = 0; x < width; x++)
        {
            // Get colors for upper and lower half
            Color upperColor = pixels[y, x];
            Color lowerColor = (y + 1 < height) ? pixels[y + 1, x] : Color.Black;
            
            canvas[y / 2, x] = RenderTwoPixels(upperColor, lowerColor);
        }
    }
    
    return canvas;
}
```

### Block Symbols

The Block symbol category provides characters for rendering horizontal blocks with different widths. These are useful for horizontal gauges and charts.

```csharp
public static class Block
{
    // Individual width levels
    public const string FULL = "█";
    public const string SEVEN_EIGHTHS = "▉";
    public const string THREE_QUARTERS = "▊";
    public const string FIVE_EIGHTHS = "▋";
    public const string HALF = "▌";
    public const string THREE_EIGHTHS = "▍";
    public const string ONE_QUARTER = "▎";
    public const string ONE_EIGHTH = "▏";
    public const string EMPTY = " ";
    
    // Sets of block symbols with different granularity
    public static readonly BlockSet NineLevels = new(
        Empty: EMPTY,
        OneEighth: ONE_EIGHTH,
        OneQuarter: ONE_QUARTER,
        ThreeEighths: THREE_EIGHTHS,
        Half: HALF,
        FiveEighths: FIVE_EIGHTHS,
        ThreeQuarters: THREE_QUARTERS,
        SevenEighths: SEVEN_EIGHTHS,
        Full: FULL
    );
    
    public static readonly BlockSet ThreeLevels = new(
        Empty: EMPTY,
        Half: HALF,
        Full: FULL
    );
}

// Immutable collection of block symbols with named fields
public record BlockSet(
    string Empty,
    string OneEighth,
    string OneQuarter,
    string ThreeEighths,
    string Half,
    string FiveEighths,
    string ThreeQuarters,
    string SevenEighths,
    string Full
)
{
    // Constructor for three-level set
    public BlockSet(string Empty, string Half, string Full)
        : this(Empty, Empty, Empty, Empty, Half, Full, Full, Full, Full)
    {
    }
    
    // Get block symbol closest to the given ratio (0.0 to 1.0)
    public string GetBlockSymbol(double ratio)
    {
        if (ratio <= 0.0) return Empty;
        if (ratio < 0.125) return OneEighth;
        if (ratio < 0.25) return OneQuarter;
        if (ratio < 0.375) return ThreeEighths;
        if (ratio < 0.5) return Half;
        if (ratio < 0.625) return FiveEighths;
        if (ratio < 0.75) return ThreeQuarters;
        if (ratio < 0.875) return SevenEighths;
        return Full;
    }
}
```

### Half Block Symbols

The Half Block symbol category provides characters for higher-resolution color rendering. By using upper and lower half blocks with different colors, you can effectively double the vertical resolution of the terminal.

```csharp
public static class HalfBlock
{
    public const string UPPER = "▀";  // Upper half block
    public const string LOWER = "▄";  // Lower half block
    public const string FULL = "█";   // Full block
    
    // Helper method to create a half-block pixel with different colors
    public static Cell CreateDualColorCell(Color topColor, Color bottomColor)
    {
        return new Cell
        {
            Character = UPPER,
            Foreground = topColor,
            Background = bottomColor
        };
    }
}
```

#### Example Half Block Usage

```csharp
// Creating a cell with different colors for top and bottom half
var cell = HalfBlock.CreateDualColorCell(Color.Red, Color.Blue);

// Rendering an image with double vertical resolution
for (int y = 0; y < image.Height; y += 2)
{
    for (int x = 0; x < image.Width; x++)
    {
        Color top = y < image.Height ? image[x, y] : Color.Black;
        Color bottom = y + 1 < image.Height ? image[x, y + 1] : Color.Black;
        buffer.SetCell(x, y / 2, HalfBlock.CreateDualColorCell(top, bottom));
    }
}
```

### Line Symbols

The Line symbol category provides box-drawing characters for rendering borders, frames, and tables with various styles.

```csharp
public static class Line
{
    // Horizontal lines
    public const string HORIZONTAL = "─";
    public const string HORIZONTAL_DOUBLE = "═";
    public const string HORIZONTAL_THICK = "━";
    public const string HORIZONTAL_DASHED = "╌";
    public const string HORIZONTAL_DASHED_DOUBLE = "╍";
    
    // Vertical lines
    public const string VERTICAL = "│";
    public const string VERTICAL_DOUBLE = "║";
    public const string VERTICAL_THICK = "┃";
    public const string VERTICAL_DASHED = "╎";
    public const string VERTICAL_DASHED_DOUBLE = "╏";
    
    // Corners
    public const string TOP_LEFT = "┌";
    public const string TOP_RIGHT = "┐";
    public const string BOTTOM_LEFT = "└";
    public const string BOTTOM_RIGHT = "┘";
    
    // Rounded corners
    public const string TOP_LEFT_ROUNDED = "╭";
    public const string TOP_RIGHT_ROUNDED = "╮";
    public const string BOTTOM_LEFT_ROUNDED = "╰";
    public const string BOTTOM_RIGHT_ROUNDED = "╯";
    
    // Double line corners
    public const string TOP_LEFT_DOUBLE = "╔";
    public const string TOP_RIGHT_DOUBLE = "╗";
    public const string BOTTOM_LEFT_DOUBLE = "╚";
    public const string BOTTOM_RIGHT_DOUBLE = "╝";
    
    // Thick line corners
    public const string TOP_LEFT_THICK = "┏";
    public const string TOP_RIGHT_THICK = "┓";
    public const string BOTTOM_LEFT_THICK = "┗";
    public const string BOTTOM_RIGHT_THICK = "┛";
    
    // Junctions
    public const string VERTICAL_LEFT = "┤";
    public const string VERTICAL_RIGHT = "├";
    public const string HORIZONTAL_DOWN = "┬";
    public const string HORIZONTAL_UP = "┴";
    public const string CROSS = "┼";
    
    // Predefined line sets
    public static readonly LineSet Normal = new(
        TopLeft: TOP_LEFT,
        TopRight: TOP_RIGHT,
        BottomLeft: BOTTOM_LEFT,
        BottomRight: BOTTOM_RIGHT,
        Horizontal: HORIZONTAL,
        Vertical: VERTICAL,
        VerticalLeft: VERTICAL_LEFT,
        VerticalRight: VERTICAL_RIGHT,
        HorizontalDown: HORIZONTAL_DOWN,
        HorizontalUp: HORIZONTAL_UP,
        Cross: CROSS
    );
    
    public static readonly LineSet Rounded = new(
        TopLeft: TOP_LEFT_ROUNDED,
        TopRight: TOP_RIGHT_ROUNDED,
        BottomLeft: BOTTOM_LEFT_ROUNDED,
        BottomRight: BOTTOM_RIGHT_ROUNDED,
        Horizontal: HORIZONTAL,
        Vertical: VERTICAL,
        VerticalLeft: VERTICAL_LEFT,
        VerticalRight: VERTICAL_RIGHT,
        HorizontalDown: HORIZONTAL_DOWN,
        HorizontalUp: HORIZONTAL_UP,
        Cross: CROSS
    );
    
    public static readonly LineSet Double = new(
        TopLeft: TOP_LEFT_DOUBLE,
        TopRight: TOP_RIGHT_DOUBLE,
        BottomLeft: BOTTOM_LEFT_DOUBLE,
        BottomRight: BOTTOM_RIGHT_DOUBLE,
        Horizontal: HORIZONTAL_DOUBLE,
        Vertical: VERTICAL_DOUBLE,
        VerticalLeft: "╣",
        VerticalRight: "╠",
        HorizontalDown: "╦",
        HorizontalUp: "╩",
        Cross: "╬"
    );
    
    public static readonly LineSet Thick = new(
        TopLeft: TOP_LEFT_THICK,
        TopRight: TOP_RIGHT_THICK,
        BottomLeft: BOTTOM_LEFT_THICK,
        BottomRight: BOTTOM_RIGHT_THICK,
        Horizontal: HORIZONTAL_THICK,
        Vertical: VERTICAL_THICK,
        VerticalLeft: "┫",
        VerticalRight: "┣",
        HorizontalDown: "┳",
        HorizontalUp: "┻",
        Cross: "╋"
    );
}

// Immutable collection of line symbols for box drawing
public record LineSet(
    string TopLeft,
    string TopRight,
    string BottomLeft,
    string BottomRight,
    string Horizontal,
    string Vertical,
    string VerticalLeft,
    string VerticalRight,
    string HorizontalDown,
    string HorizontalUp,
    string Cross
)
{
    // Create a box string with specified width and height
    public string DrawBox(int width, int height)
    {
        if (width < 2 || height < 2)
            return string.Empty;
            
        var result = new StringBuilder();
        
        // Top border
        result.Append(TopLeft);
        result.Append(string.Concat(Enumerable.Repeat(Horizontal, width - 2)));
        result.AppendLine(TopRight);
        
        // Middle rows
        for (int i = 0; i < height - 2; i++)
        {
            result.Append(Vertical);
            result.Append(string.Concat(Enumerable.Repeat(" ", width - 2)));
            result.AppendLine(Vertical);
        }
        
        // Bottom border
        result.Append(BottomLeft);
        result.Append(string.Concat(Enumerable.Repeat(Horizontal, width - 2)));
        result.Append(BottomRight);
        
        return result.ToString();
    }
}
```

### Braille Symbols

The Braille symbol category provides Unicode Braille patterns for high-resolution drawing. These patterns can be used to create detailed graphics with 8 dots per character (2 columns × 4 rows).

```csharp
public static class Braille
{
    // Base Unicode value for blank Braille pattern
    public const char BLANK = '\u2800';
    
    // Bit values for each dot position in a Braille cell
    // The layout follows the standard Braille pattern:
    // 0 3
    // 1 4
    // 2 5
    // 6 7
    public static readonly int[,] DOTS = new int[4, 2] {
        { 0x01, 0x08 },
        { 0x02, 0x10 },
        { 0x04, 0x20 },
        { 0x40, 0x80 }
    };
    
    // Get Braille character for a set of dots
    public static char GetDots(params int[] dots)
    {
        int value = 0;
        foreach (int dot in dots)
        {
            if (dot >= 0 && dot < 8)
            {
                value |= 1 << dot;
            }
        }
        return (char)(BLANK + value);
    }
    
    // Get Braille character from a 2x4 bitmap
    public static char FromBitmap(bool[,] bitmap)
    {
        int value = 0;
        for (int y = 0; y < 4 && y < bitmap.GetLength(0); y++)
        {
            for (int x = 0; x < 2 && x < bitmap.GetLength(1); x++)
            {
                if (bitmap[y, x])
                {
                    value |= DOTS[y, x];
                }
            }
        }
        return (char)(BLANK + value);
    }
    
    // Set a specific dot in a Braille character
    public static char SetDot(char braille, int row, int col)
    {
        if (row >= 0 && row < 4 && col >= 0 && col < 2)
        {
            return (char)(braille | DOTS[row, col]);
        }
        return braille;
    }
}
```

#### Example Braille Usage

```csharp
// Creating patterns directly with dot indices
char dot0 = Braille.GetDots(0);    // ⠁
char dots03 = Braille.GetDots(0, 3); // ⠉

// Creating patterns from a bitmap
bool[,] bitmap = new bool[4, 2] {
    { true, false },
    { true, false },
    { false, false },
    { false, false }
};
char pattern = Braille.FromBitmap(bitmap); // ⠃

// Building patterns incrementally
char c = Braille.BLANK;
c = Braille.SetDot(c, 0, 0);
c = Braille.SetDot(c, 1, 0);
// c is now ⠃
```

### Half Block Symbols

The Half Block symbol category provides characters for creating higher-resolution color output by combining foreground and background colors in a single character position.

```csharp
public static class HalfBlock
{
    // Basic half block symbols
    public const string UPPER = "▀"; // Upper half block
    public const string LOWER = "▄"; // Lower half block
    public const string FULL = "█";  // Full block
    
    // Helper method for creating a cell with different colors for upper and lower half
    public static Cell CreateDualColorCell(Color upperColor, Color lowerColor)
    {
        return new Cell
        {
            Content = UPPER,
            Foreground = upperColor,
            Background = lowerColor
        };
    }
    
    // Helper method to map a 2x1 area of colors to a single cell
    public static Cell FromColors(Color[,] colors)
    {
        if (colors.GetLength(0) >= 2 && colors.GetLength(1) >= 1)
        {
            return CreateDualColorCell(colors[0, 0], colors[1, 0]);
        }
        return new Cell { Content = " " };
    }
}
```

#### Half Block Usage Example

Half blocks are particularly useful for doubling the vertical resolution of your terminal output. By using the upper half block character with different foreground and background colors, you can effectively display two different colors in the space of a single character:

```csharp
// Regular character approach - 1x1 resolution
Buffer.SetCell(x, y, new Cell { Content = "X", Foreground = Colors.Red });

// Half block approach - 2x1 resolution
Buffer.SetCell(x, y, HalfBlock.CreateDualColorCell(Colors.Red, Colors.Blue));
```

This technique is commonly used for:
- Color gradients with smoother transitions
- Higher resolution graphics
- Images rendered in the terminal
- Color-based heatmaps

### Border Symbols

The Border symbol category provides characters for drawing boxes and borders with different styles. These are essential for creating panels, windows, and other UI containers.

```csharp
public static class Border
{
    // Border set record type
    public record BorderSet(
        string TopLeft,
        string TopRight,
        string BottomLeft,
        string BottomRight,
        string Horizontal,
        string Vertical,
        string Top = null,        // Optional specialized chars
        string Bottom = null,
        string Left = null,
        string Right = null
    )
    {
        // Initialize optional fields with defaults if not provided
        public BorderSet(
            string TopLeft, 
            string TopRight, 
            string BottomLeft, 
            string BottomRight, 
            string Horizontal, 
            string Vertical
        ) : this(
            TopLeft, 
            TopRight, 
            BottomLeft, 
            BottomRight, 
            Horizontal, 
            Vertical, 
            Horizontal, 
            Horizontal, 
            Vertical, 
            Vertical
        ) { }
    }
    
    // Predefined border styles
    public static readonly BorderSet Plain = new(
        "┌", "┐", "└", "┘", "─", "│"
    );
    
    public static readonly BorderSet Rounded = new(
        "╭", "╮", "╰", "╯", "─", "│"
    );
    
    public static readonly BorderSet Double = new(
        "╔", "╗", "╚", "╝", "═", "║"
    );
    
    public static readonly BorderSet Thick = new(
        "┏", "┓", "┗", "┛", "━", "┃"
    );
    
    public static readonly BorderSet Quadrant = new(
        "▘", "▝", "▖", "▗", "▀", "▌"
    );
    
    // Additional styles (hidden, dashed, etc.)
    public static readonly BorderSet Hidden = new(
        " ", " ", " ", " ", " ", " "
    );
    
    public static readonly BorderSet Dashed = new(
        "┌", "┐", "└", "┘", "╌", "╎"
    );
    
    // Helper to create a specific border corner
    public static string GetCorner(BorderSet set, bool isTop, bool isLeft)
    {
        if (isTop)
        {
            return isLeft ? set.TopLeft : set.TopRight;
        }
        else
        {
            return isLeft ? set.BottomLeft : set.BottomRight;
        }
    }
}
```

#### Border Style Examples

```
┌────────┐  ╭────────╮  ┏━━━━━━━━┓  ╔════════╗
│ Plain  │  │ Rounded│  ┃ Thick  ┃  ║ Double ║
└────────┘  ╰────────╯  ┗━━━━━━━━┛  ╚════════╝

┌╌╌╌╌╌╌╌╌┐
╎ Dashed ╎
└╌╌╌╌╌╌╌╌┘
```

### Symbol Merging

When borders or lines intersect or connect, their symbols need to merge coherently. CycoTui provides a symbol merging system to handle these cases:

```csharp
public enum MergeStrategy
{
    // Simply replaces the previous symbol with the new one
    Replace,
    
    // Only merges if an exact match exists for the combination
    Exact,
    
    // Uses best-effort approximation if exact match doesn't exist
    Fuzzy
}

public static class SymbolMerge
{
    // Merge two border symbols based on the strategy
    public static string Merge(string current, string next, MergeStrategy strategy = MergeStrategy.Fuzzy)
    {
        // Implementation depends on strategy
        if (strategy == MergeStrategy.Replace)
        {
            return next;
        }
        
        // Decompose symbols into components
        var currentComponents = DecomposeSymbol(current);
        var nextComponents = DecomposeSymbol(next);
        
        // Merge components
        var mergedComponents = MergeComponents(currentComponents, nextComponents);
        
        // Find the symbol that matches the merged components
        return FindMatchingSymbol(mergedComponents, strategy);
    }
    
    // Helper methods (implementation details)
    private static BorderComponents DecomposeSymbol(string symbol) { /* ... */ }
    private static BorderComponents MergeComponents(BorderComponents a, BorderComponents b) { /* ... */ }
    private static string FindMatchingSymbol(BorderComponents components, MergeStrategy strategy) { /* ... */ }
}
```

#### Border Merging Examples

```
Before merging:            After merging:
┌────┐ ┌────┐              ┌────┬────┐
│    │ │    │      →       │    │    │
└────┘ └────┘              └────┴────┘

┌────┐                     ┌────┐
│    │                     │    │
└────┘      →       ┌──────┼────┘
┌────┐               │      │
│    │               └──────┘
└────┘
```

The merging algorithm is particularly useful when:
- Creating complex layouts with nested borders
- Connecting lines in drawings or diagrams
- Building tables with internal borders
- Creating UI elements that need to connect visually

### Scrollbar Symbols

The Scrollbar symbol category provides characters for rendering scrollbars in both vertical and horizontal orientations.

```csharp
public static class Scrollbar
{
    // Scrollbar set record type
    public record ScrollbarSet(
        string Begin,     // Beginning of scrollbar
        string End,       // End of scrollbar
        string Track,     // Track along which the thumb moves
        string Thumb      // Moving indicator showing current position
    );
    
    // Predefined scrollbar styles
    
    // Standard vertical scrollbar (▲│▼█)
    public static readonly ScrollbarSet Vertical = new(
        Begin: "▲",
        End: "▼",
        Track: "│",
        Thumb: "█"
    );
    
    // Standard horizontal scrollbar (◄─►█)
    public static readonly ScrollbarSet Horizontal = new(
        Begin: "◄",
        End: "►",
        Track: "─",
        Thumb: "█"
    );
    
    // Double-line vertical scrollbar (╦║╩█)
    public static readonly ScrollbarSet DoubleVertical = new(
        Begin: "╦",
        End: "╩",
        Track: "║",
        Thumb: "█"
    );
    
    // Double-line horizontal scrollbar (╠═╣█)
    public static readonly ScrollbarSet DoubleHorizontal = new(
        Begin: "╠",
        End: "╣",
        Track: "═",
        Thumb: "█"
    );
}
```

#### Scrollbar Components

```
Vertical Scrollbar:        Horizontal Scrollbar:
    ▲ (Begin)              ◄───────────────► (Begin, Track, End)
    │                                  
    │ (Track)                          █ (Thumb)
    │                                  
    █ (Thumb)              
    │                      
    │                      
    ▼ (End)                
```

Scrollbars are commonly used for:
- Lists that exceed available space
- Text areas with multiple lines
- Panels with overflow content
- Any widget that needs to show a portion of larger content

### Marker Symbols

The Marker symbol category provides characters for data visualization points, indicators, and chart markers.

```csharp
public static class Marker
{
    // Common marker symbol
    public const string DOT = "•";
    
    // Marker types enum
    public enum MarkerType
    {
        // Standard dot marker (•)
        Dot,
        
        // Block marker (█)
        Block,
        
        // Bar marker (|)
        Bar,
        
        // Braille marker (⠒)
        Braille,
        
        // Half-block marker (▄)
        HalfBlock
    }
    
    // Get marker symbol from type
    public static string GetMarker(MarkerType markerType)
    {
        return markerType switch
        {
            MarkerType.Dot => DOT,
            MarkerType.Block => "█",
            MarkerType.Bar => "|",
            MarkerType.Braille => "⠒",
            MarkerType.HalfBlock => "▄",
            _ => DOT // Default to dot
        };
    }
    
    // Parse string to marker type
    public static bool TryParse(string value, out MarkerType markerType)
    {
        return Enum.TryParse(value, true, out markerType);
    }
}
```

#### Marker Usage Example

Markers are commonly used in charts, graphs, and data visualization:

```csharp
// Create a scatter plot with dot markers
var chart = new Chart()
    .AddSeries(data, Marker.MarkerType.Dot);
    
// Create a line chart with custom markers
var lineChart = new Chart()
    .AddSeries(data1, Marker.MarkerType.Block)
    .AddSeries(data2, Marker.MarkerType.Braille);
```

Different marker types are suitable for different visualization needs:
- **Dot**: Best for most scatter plots and general data points
- **Block**: High visibility, good for sparse data
- **Bar**: Works well with horizontal or vertical lines
- **Braille**: Smaller, less intrusive points
- **HalfBlock**: Good for dual-color representation

### Line Symbols

The Line symbol category provides box-drawing characters for creating lines, grids, and borders with various styles.

```csharp
public static class Line
{
    // Normal line characters
    public const string HORIZONTAL = "─";
    public const string VERTICAL = "│";
    public const string TOP_LEFT = "┌";
    public const string TOP_RIGHT = "┐";
    public const string BOTTOM_LEFT = "└";
    public const string BOTTOM_RIGHT = "┘";
    public const string VERTICAL_LEFT = "┤";
    public const string VERTICAL_RIGHT = "├";
    public const string HORIZONTAL_DOWN = "┬";
    public const string HORIZONTAL_UP = "┴";
    public const string CROSS = "┼";
    
    // Double line characters
    public const string DOUBLE_HORIZONTAL = "═";
    public const string DOUBLE_VERTICAL = "║";
    // ... more double line constants ...
    
    // Thick line characters
    public const string THICK_HORIZONTAL = "━";
    public const string THICK_VERTICAL = "┃";
    // ... more thick line constants ...
    
    // Line set record type
    public record LineSet(
        string HorizontalLine,
        string VerticalLine,
        string TopLeftCorner,
        string TopRightCorner,
        string BottomLeftCorner,
        string BottomRightCorner,
        string VerticalLeftLine,
        string VerticalRightLine,
        string HorizontalDownLine,
        string HorizontalUpLine,
        string CrossLine
    );
    
    // Predefined line sets
    public static readonly LineSet Normal = new(
        HORIZONTAL, VERTICAL, 
        TOP_LEFT, TOP_RIGHT,
        BOTTOM_LEFT, BOTTOM_RIGHT,
        VERTICAL_LEFT, VERTICAL_RIGHT,
        HORIZONTAL_DOWN, HORIZONTAL_UP,
        CROSS
    );
    
    public static readonly LineSet Double = new(
        DOUBLE_HORIZONTAL, DOUBLE_VERTICAL,
        // ... other double line characters ...
    );
    
    public static readonly LineSet Thick = new(
        THICK_HORIZONTAL, THICK_VERTICAL,
        // ... other thick line characters ...
    );
    
    public static readonly LineSet Rounded = new(
        HORIZONTAL, VERTICAL,
        "╭", "╮", // Rounded corners
        "╰", "╯",
        // ... other normal characters ...
    );
    
    // Helper methods
    public static string GetHorizontal(LineSet set) => set.HorizontalLine;
    public static string GetVertical(LineSet set) => set.VerticalLine;
    public static string GetCorner(LineSet set, bool isTop, bool isLeft)
    {
        if (isTop)
        {
            return isLeft ? set.TopLeftCorner : set.TopRightCorner;
        }
        else
        {
            return isLeft ? set.BottomLeftCorner : set.BottomRightCorner;
        }
    }
}
```

#### Line Style Examples

```
Normal:        Double:        Thick:         Rounded:
┌─┬─┐          ╔═╦═╗          ┏━┳━┓          ╭─┬─╮
│ │ │          ║ ║ ║          ┃ ┃ ┃          │ │ │ 
├─┼─┤          ╠═╬═╣          ┣━╋━┫          ├─┼─┤
│ │ │          ║ ║ ║          ┃ ┃ ┃          │ │ │
└─┴─┘          ╚═╩═╝          ┗━┻━┛          ╰─┴─╯
```

### Scrollbar Symbols

The Scrollbar symbol category provides characters for creating scrollbars in terminal UIs.

```csharp
public static class Scrollbar
{
    // Scrollbar set record type
    public record ScrollbarSet(
        string Begin,    // Start of scrollbar
        string End,      // End of scrollbar
        string Track,    // Track/background
        string Thumb     // Moving indicator
    );
    
    // Predefined scrollbar sets
    public static readonly ScrollbarSet Vertical = new(
        "▲",  // Up arrow
        "▼",  // Down arrow
        "│",  // Vertical line
        "█"   // Block
    );
    
    public static readonly ScrollbarSet Horizontal = new(
        "◄",  // Left arrow
        "►",  // Right arrow
        "─",  // Horizontal line
        "█"   // Block
    );
    
    public static readonly ScrollbarSet DoubleVertical = new(
        "╦",  // Double top
        "╩",  // Double bottom
        "║",  // Double vertical
        "█"   // Block
    );
    
    public static readonly ScrollbarSet DoubleHorizontal = new(
        "╠",  // Double left
        "╣",  // Double right
        "═",  // Double horizontal
        "█"   // Block
    );
}
```

#### Scrollbar Examples

```
Vertical:    Horizontal:
   ▲            ◄════════█═══►
   █
   │
   │
   │
   │
   ▼
```

### Shade Symbols

The Shade symbol category provides block characters with different densities for creating visual gradients and shading effects.

```csharp
public static class Shade
{
    // Shade characters with increasing density
    public const string EMPTY = " ";   // No shade (space)
    public const string LIGHT = "░";   // Light shade
    public const string MEDIUM = "▒";  // Medium shade
    public const string DARK = "▓";    // Dark shade
    public const string FULL = "█";    // Full block
    
    // Array of shade characters for easy indexing
    public static readonly string[] ALL = { EMPTY, LIGHT, MEDIUM, DARK, FULL };
    
    // Get shade based on ratio (0.0 to 1.0)
    public static string GetShade(double ratio)
    {
        if (ratio <= 0.0) return EMPTY;
        if (ratio < 0.25) return LIGHT;
        if (ratio < 0.5) return MEDIUM;
        if (ratio < 0.75) return DARK;
        return FULL;
    }
    
    // Get shade index based on ratio (0.0 to 1.0)
    public static int GetShadeIndex(double ratio)
    {
        return (int)Math.Round(ratio * (ALL.Length - 1));
    }
}
```

#### Shade Examples

```
Visual density gradient:
 ░▒▓█

Shade usage in a progress bar:
[█████▓▒░        ]

Shade usage in a heatmap:
█ High
▓
▒
░ Low
```

### Organization Structure

The symbols system must be organized as:

```csharp
namespace CycoTui.Symbols
{
    public static class Bar { /* ... */ }
    public static class Block { /* ... */ }
    public static class Border { /* ... */ }
    public static class Braille { /* ... */ }
    public static class HalfBlock { /* ... */ }
    public static class Line { /* ... */ }
    public static class Marker { /* ... */ }
    public static class Scrollbar { /* ... */ }
    public static class Shade { /* ... */ }
    
    // Common symbols re-exported at the namespace level
    public static class Common { /* ... */ }
}
```

### Unicode Compatibility

For Unicode handling, the system must:

1. Use UTF-8 encoding for all symbols
2. Properly handle surrogate pairs and combining characters
3. Account for different terminal font capabilities
4. Provide fallback mechanisms for terminals with limited Unicode support

### Symbol Merging

For operations that merge symbols (e.g., when lines cross):

1. Define rules for combining different symbols
2. Handle special cases like line intersections
3. Maintain visual consistency when symbols are combined

## Technical Approach

### Implementation Strategy

Symbols will be implemented as:

1. **Static Classes**: Each category as a static class with constants
2. **String Constants**: Individual symbols as string constants
3. **Helper Methods**: Methods for symbol selection and composition

Example:

```csharp
public static class Border
{
    // Individual symbols
    public const string TopLeft = "┌";
    public const string TopRight = "┐";
    public const string BottomLeft = "└";
    public const string BottomRight = "┘";
    public const string Horizontal = "─";
    public const string Vertical = "│";
    
    // Border sets
    public static readonly BorderSet Normal = new(TopLeft, TopRight, BottomLeft, BottomRight, Horizontal, Vertical);
    public static readonly BorderSet Rounded = new("╭", "╮", "╰", "╯", "─", "│");
    public static readonly BorderSet Double = new("╔", "╗", "╚", "╝", "═", "║");
    public static readonly BorderSet Thick = new("┏", "┓", "┗", "┛", "━", "┃");
    
    // Helper methods
    public static string GetCorner(bool isTop, bool isLeft) => 
        isTop ? (isLeft ? TopLeft : TopRight) : 
               (isLeft ? BottomLeft : BottomRight);
}
```

### Fallback Mechanisms

For terminals with limited Unicode support:

1. Detect terminal capabilities at runtime
2. Provide ASCII fallback symbols when needed
3. Define a consistent fallback strategy for each symbol category

Example fallbacks:

```csharp
public static class BorderFallbacks
{
    public static readonly BorderSet Ascii = new("+", "+", "+", "+", "-", "|");
}
```

### Testing Strategy

The symbols system will be tested with:

1. Unit tests for all symbol definitions
2. Visual tests showing symbol rendering in different terminals
3. Tests for symbol merging and composition
4. Tests with various terminal capabilities (UTF-8, ASCII, etc.)

## Examples

### Border Styles Example

```csharp
// Normal borders
┌────────┐
│ Normal │
└────────┘

// Rounded borders
╭────────╮
│ Rounded│
╰────────╯

// Double-line borders
╔════════╗
║ Double ║
╚════════╝

// Thick borders
┏━━━━━━━━┓
┃ Thick  ┃
┗━━━━━━━━┛
```

### Braille Example

```
⠁⠂⠄⠈⠐⠠⡀⢀
⠃⠅⠆⠉⠑⠡⡁⢁
⠇⠏⠗⠟⠯⠿⡿⣿
```

## See Also

- [VISION-CORE-001.md](../vision/VISION-CORE-001.md): Core vision
- [SPEC-BUFFER-002.md](SPEC-BUFFER-002.md): Buffer specification
- [SPEC-STYLE-005.md](SPEC-STYLE-005.md): Style specification
- [005-STYLE-SYSTEM-001.md](../features/005-STYLE-SYSTEM-001.md): Style system feature