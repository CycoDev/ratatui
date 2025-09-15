using System;
using System.Collections.Generic;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;

namespace CycoAI.CycoTui.Core.Buffer
{
    /// <summary>
    /// Defines the interface for a terminal buffer that stores character cells.
    /// Buffers are used to build up the complete screen content before rendering.
    /// </summary>
    public interface IBuffer
    {
        /// <summary>
        /// Gets the width of the buffer in characters.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the buffer in characters.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the total area covered by this buffer.
        /// </summary>
        Rect Area { get; }

        /// <summary>
        /// Gets or sets the cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate (column).</param>
        /// <param name="y">The y-coordinate (row).</param>
        /// <returns>The cell at the specified coordinates.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when coordinates are outside the buffer bounds.</exception>
        Cell this[int x, int y] { get; set; }

        /// <summary>
        /// Gets the cell at the specified coordinates, or returns an empty cell if coordinates are out of bounds.
        /// </summary>
        /// <param name="x">The x-coordinate (column).</param>
        /// <param name="y">The y-coordinate (row).</param>
        /// <returns>The cell at the specified coordinates, or <see cref="Cell.Empty"/> if out of bounds.</returns>
        Cell GetCellSafe(int x, int y);

        /// <summary>
        /// Sets the cell at the specified coordinates if the coordinates are within bounds.
        /// </summary>
        /// <param name="x">The x-coordinate (column).</param>
        /// <param name="y">The y-coordinate (row).</param>
        /// <param name="cell">The cell to set.</param>
        /// <returns>true if the cell was set; false if coordinates were out of bounds.</returns>
        bool SetCellSafe(int x, int y, Cell cell);

        /// <summary>
        /// Clears the entire buffer, filling it with empty cells.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears the specified rectangular area, filling it with empty cells.
        /// </summary>
        /// <param name="area">The area to clear.</param>
        void Clear(Rect area);

        /// <summary>
        /// Fills the specified rectangular area with the given cell.
        /// </summary>
        /// <param name="area">The area to fill.</param>
        /// <param name="cell">The cell to fill with.</param>
        void Fill(Rect area, Cell cell);

        /// <summary>
        /// Sets a string of text starting at the specified coordinates with the given style.
        /// </summary>
        /// <param name="x">The starting x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="text">The text to set.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="modifiers">The text modifiers.</param>
        /// <returns>The number of characters actually written.</returns>
        int SetString(int x, int y, string text, Color foreground = default, Color background = default, Modifier modifiers = Modifier.None);

        /// <summary>
        /// Sets a string of text starting at the specified coordinates, automatically wrapping to the next line.
        /// </summary>
        /// <param name="area">The area within which to render the text.</param>
        /// <param name="text">The text to set.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="modifiers">The text modifiers.</param>
        /// <returns>The number of lines used.</returns>
        int SetStringWrapped(Rect area, string text, Color foreground = default, Color background = default, Modifier modifiers = Modifier.None);

        /// <summary>
        /// Determines whether the specified coordinates are within the buffer bounds.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>true if the coordinates are within bounds; otherwise, false.</returns>
        bool IsInBounds(int x, int y);

        /// <summary>
        /// Gets all cells in the buffer as a flat array (row-major order).
        /// </summary>
        /// <returns>An array containing all cells in the buffer.</returns>
        Cell[] GetCells();

        /// <summary>
        /// Gets all cells in the specified area.
        /// </summary>
        /// <param name="area">The area to get cells from.</param>
        /// <returns>An enumerable of cells in the specified area.</returns>
        IEnumerable<Cell> GetCells(Rect area);

        /// <summary>
        /// Merges another buffer into this buffer at the specified offset.
        /// Only non-empty cells from the source buffer are copied.
        /// </summary>
        /// <param name="source">The source buffer to merge.</param>
        /// <param name="offsetX">The x-offset where to place the source buffer.</param>
        /// <param name="offsetY">The y-offset where to place the source buffer.</param>
        void Merge(IBuffer source, int offsetX = 0, int offsetY = 0);

        /// <summary>
        /// Creates a sub-buffer view of the specified area.
        /// Changes to the sub-buffer will be reflected in the original buffer.
        /// </summary>
        /// <param name="area">The area to create a sub-buffer for.</param>
        /// <returns>A sub-buffer view of the specified area.</returns>
        IBuffer SubBuffer(Rect area);

        /// <summary>
        /// Resizes the buffer to the specified dimensions.
        /// Existing content is preserved where possible.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        void Resize(int width, int height);
    }
}