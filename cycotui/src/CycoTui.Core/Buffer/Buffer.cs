using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Buffer
{
    /// <summary>
    /// A high-performance terminal buffer implementation with dirty region tracking.
    /// Optimized for minimal terminal I/O by tracking which cells have changed.
    /// </summary>
    public class Buffer : IBuffer
    {
        private readonly Cell[] _cells;
        private readonly bool[] _dirty;
        private bool _allDirty;

        /// <inheritdoc />
        public int Width { get; }

        /// <inheritdoc />
        public int Height { get; }

        /// <inheritdoc />
        public Rect Area => new Rect(0, 0, Width, Height);

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer"/> class.
        /// </summary>
        /// <param name="width">The width of the buffer in characters.</param>
        /// <param name="height">The height of the buffer in characters.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height is negative.</exception>
        public Buffer(int width, int height)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), "Width cannot be negative.");
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height), "Height cannot be negative.");

            Width = width;
            Height = height;
            _cells = new Cell[width * height];
            _dirty = new bool[width * height];

            // Initialize with empty cells
            Clear();
        }

        /// <inheritdoc />
        public Cell this[int x, int y]
        {
            get
            {
                if (!IsInBounds(x, y))
                    throw new IndexOutOfRangeException($"Coordinates ({x}, {y}) are outside buffer bounds (0, 0, {Width}, {Height}).");

                return _cells[y * Width + x];
            }
            set
            {
                if (!IsInBounds(x, y))
                    throw new IndexOutOfRangeException($"Coordinates ({x}, {y}) are outside buffer bounds (0, 0, {Width}, {Height}).");

                var index = y * Width + x;
                if (!_cells[index].Equals(value))
                {
                    _cells[index] = value;
                    _dirty[index] = true;
                }
            }
        }

        /// <inheritdoc />
        public Cell GetCellSafe(int x, int y)
        {
            return IsInBounds(x, y) ? _cells[y * Width + x] : Cell.Empty;
        }

        /// <inheritdoc />
        public bool SetCellSafe(int x, int y, Cell cell)
        {
            if (!IsInBounds(x, y))
                return false;

            var index = y * Width + x;
            if (!_cells[index].Equals(cell))
            {
                _cells[index] = cell;
                _dirty[index] = true;
            }
            return true;
        }

        /// <inheritdoc />
        public void Clear()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i] = Cell.Empty;
                _dirty[i] = true;
            }
            _allDirty = true;
        }

        /// <inheritdoc />
        public void Clear(Rect area)
        {
            Fill(area, Cell.Empty);
        }

        /// <inheritdoc />
        public void Fill(Rect area, Cell cell)
        {
            var clipped = area.Intersect(Area);
            if (clipped.IsEmpty)
                return;

            for (int y = clipped.Y; y < clipped.Y + clipped.Height; y++)
            {
                for (int x = clipped.X; x < clipped.X + clipped.Width; x++)
                {
                    var index = y * Width + x;
                    if (!_cells[index].Equals(cell))
                    {
                        _cells[index] = cell;
                        _dirty[index] = true;
                    }
                }
            }
        }

        /// <inheritdoc />
        public int SetString(int x, int y, string text, Color foreground = default, Color background = default, Modifier modifiers = Modifier.None)
        {
            if (string.IsNullOrEmpty(text) || !IsInBounds(x, y))
                return 0;

            int written = 0;
            for (int i = 0; i < text.Length && x + i < Width; i++)
            {
                var cell = new Cell(text[i], foreground, background, modifiers);
                this[x + i, y] = cell;
                written++;
            }

            return written;
        }

        /// <inheritdoc />
        public int SetStringWrapped(Rect area, string text, Color foreground = default, Color background = default, Modifier modifiers = Modifier.None)
        {
            if (string.IsNullOrEmpty(text) || area.IsEmpty)
                return 0;

            var clipped = area.Intersect(Area);
            if (clipped.IsEmpty)
                return 0;

            int currentX = clipped.X;
            int currentY = clipped.Y;
            int linesUsed = 0;

            foreach (char c in text)
            {
                if (currentY >= clipped.Y + clipped.Height)
                    break;

                if (c == '\n' || currentX >= clipped.X + clipped.Width)
                {
                    currentX = clipped.X;
                    currentY++;
                    linesUsed++;

                    if (c == '\n')
                        continue;
                }

                if (currentY < clipped.Y + clipped.Height)
                {
                    var cell = new Cell(c, foreground, background, modifiers);
                    this[currentX, currentY] = cell;
                    currentX++;
                }
            }

            if (currentX > clipped.X)
                linesUsed++;

            return linesUsed;
        }

        /// <inheritdoc />
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <inheritdoc />
        public Cell[] GetCells()
        {
            var result = new Cell[_cells.Length];
            Array.Copy(_cells, result, _cells.Length);
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<Cell> GetCells(Rect area)
        {
            var clipped = area.Intersect(Area);
            if (clipped.IsEmpty)
                yield break;

            for (int y = clipped.Y; y < clipped.Y + clipped.Height; y++)
            {
                for (int x = clipped.X; x < clipped.X + clipped.Width; x++)
                {
                    yield return _cells[y * Width + x];
                }
            }
        }

        /// <inheritdoc />
        public void Merge(IBuffer source, int offsetX = 0, int offsetY = 0)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    var cell = source[x, y];
                    if (!cell.Equals(Cell.Empty))
                    {
                        SetCellSafe(x + offsetX, y + offsetY, cell);
                    }
                }
            }
        }

        /// <inheritdoc />
        public IBuffer SubBuffer(Rect area)
        {
            return new SubBuffer(this, area);
        }

        /// <inheritdoc />
        public void Resize(int width, int height)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), "Width cannot be negative.");
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height), "Height cannot be negative.");

            if (width == Width && height == Height)
                return;

            // Create new buffer and copy existing content
            var newBuffer = new Buffer(width, height);

            var copyArea = new Rect(0, 0, Math.Min(Width, width), Math.Min(Height, height));
            for (int y = 0; y < copyArea.Height; y++)
            {
                for (int x = 0; x < copyArea.Width; x++)
                {
                    newBuffer[x, y] = this[x, y];
                }
            }

            // Replace current buffer data
            var currentBuffer = (Buffer)this;
            Array.Copy(newBuffer._cells, currentBuffer._cells, Math.Min(_cells.Length, newBuffer._cells.Length));
            Array.Copy(newBuffer._dirty, currentBuffer._dirty, Math.Min(_dirty.Length, newBuffer._dirty.Length));
        }

        /// <summary>
        /// Gets all dirty cells since the last call to <see cref="ResetDirtyTracking"/>.
        /// This is used for efficient rendering by only updating changed cells.
        /// </summary>
        /// <returns>An enumerable of dirty cell coordinates and their values.</returns>
        public IEnumerable<(int X, int Y, Cell Cell)> GetDirtyCells()
        {
            if (_allDirty)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        yield return (x, y, _cells[y * Width + x]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _dirty.Length; i++)
                {
                    if (_dirty[i])
                    {
                        int x = i % Width;
                        int y = i / Width;
                        yield return (x, y, _cells[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the dirty tracking, marking all cells as clean.
        /// This should be called after rendering to the terminal.
        /// </summary>
        public void ResetDirtyTracking()
        {
            if (_allDirty)
            {
#if NETSTANDARD2_0 || NETSTANDARD2_1
                for (int i = 0; i < _dirty.Length; i++)
                {
                    _dirty[i] = false;
                }
#else
                Array.Fill(_dirty, false);
#endif
                _allDirty = false;
            }
            else
            {
                for (int i = 0; i < _dirty.Length; i++)
                {
                    _dirty[i] = false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the entire buffer is dirty.
        /// </summary>
        public bool IsAllDirty => _allDirty;

        /// <summary>
        /// Gets a value indicating whether any cells are dirty.
        /// </summary>
        public bool HasDirtyCells => _allDirty || _dirty.Any(d => d);

        /// <summary>
        /// Forces all cells to be marked as dirty.
        /// Useful when the terminal needs a complete refresh.
        /// </summary>
        public void MarkAllDirty()
        {
            _allDirty = true;
        }
    }
}