using System;
using System.Collections.Generic;
using CycoAI.CycoTui.Core.Layout;
using CycoAI.CycoTui.Core.Style;

namespace CycoAI.CycoTui.Core.Buffer
{
    /// <summary>
    /// A view into a portion of another buffer that behaves like an independent buffer.
    /// Changes to the sub-buffer are reflected in the parent buffer.
    /// </summary>
    internal class SubBuffer : IBuffer
    {
        private readonly IBuffer _parent;
        private readonly Rect _area;

        /// <inheritdoc />
        public int Width => _area.Width;

        /// <inheritdoc />
        public int Height => _area.Height;

        /// <inheritdoc />
        public Rect Area => new Rect(0, 0, Width, Height);

        /// <summary>
        /// Initializes a new instance of the <see cref="SubBuffer"/> class.
        /// </summary>
        /// <param name="parent">The parent buffer.</param>
        /// <param name="area">The area within the parent buffer.</param>
        /// <exception cref="ArgumentNullException">Thrown when parent is null.</exception>
        public SubBuffer(IBuffer parent, Rect area)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _area = area.Intersect(parent.Area);
        }

        /// <inheritdoc />
        public Cell this[int x, int y]
        {
            get
            {
                if (!IsInBounds(x, y))
                    throw new IndexOutOfRangeException($"Coordinates ({x}, {y}) are outside sub-buffer bounds (0, 0, {Width}, {Height}).");

                return _parent[_area.X + x, _area.Y + y];
            }
            set
            {
                if (!IsInBounds(x, y))
                    throw new IndexOutOfRangeException($"Coordinates ({x}, {y}) are outside sub-buffer bounds (0, 0, {Width}, {Height}).");

                _parent[_area.X + x, _area.Y + y] = value;
            }
        }

        /// <inheritdoc />
        public Cell GetCellSafe(int x, int y)
        {
            if (!IsInBounds(x, y))
                return Cell.Empty;

            return _parent.GetCellSafe(_area.X + x, _area.Y + y);
        }

        /// <inheritdoc />
        public bool SetCellSafe(int x, int y, Cell cell)
        {
            if (!IsInBounds(x, y))
                return false;

            return _parent.SetCellSafe(_area.X + x, _area.Y + y, cell);
        }

        /// <inheritdoc />
        public void Clear()
        {
            Fill(Area, Cell.Empty);
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

            // Translate to parent coordinates
            var parentArea = new Rect(
                _area.X + clipped.X,
                _area.Y + clipped.Y,
                clipped.Width,
                clipped.Height);

            _parent.Fill(parentArea, cell);
        }

        /// <inheritdoc />
        public int SetString(int x, int y, string text, Color foreground = default, Color background = default, Modifier modifiers = Modifier.None)
        {
            if (string.IsNullOrEmpty(text) || !IsInBounds(x, y))
                return 0;

            return _parent.SetString(_area.X + x, _area.Y + y, text, foreground, background, modifiers);
        }

        /// <inheritdoc />
        public int SetStringWrapped(Rect area, string text, Color foreground = default, Color background = default, Modifier modifiers = Modifier.None)
        {
            if (string.IsNullOrEmpty(text) || area.IsEmpty)
                return 0;

            var clipped = area.Intersect(Area);
            if (clipped.IsEmpty)
                return 0;

            // Translate to parent coordinates
            var parentArea = new Rect(
                _area.X + clipped.X,
                _area.Y + clipped.Y,
                clipped.Width,
                clipped.Height);

            return _parent.SetStringWrapped(parentArea, text, foreground, background, modifiers);
        }

        /// <inheritdoc />
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <inheritdoc />
        public Cell[] GetCells()
        {
            var cells = new Cell[Width * Height];
            int index = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    cells[index++] = this[x, y];
                }
            }

            return cells;
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
                    yield return this[x, y];
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
        IBuffer IBuffer.SubBuffer(Rect area)
        {
            var clipped = area.Intersect(Area);
            if (clipped.IsEmpty)
                return new Buffer(0, 0);

            // Calculate the absolute area in the parent buffer
            var parentSubArea = new Rect(
                _area.X + clipped.X,
                _area.Y + clipped.Y,
                clipped.Width,
                clipped.Height);

            return _parent.SubBuffer(parentSubArea);
        }

        /// <inheritdoc />
        public void Resize(int width, int height)
        {
            throw new NotSupportedException("Cannot resize a sub-buffer. Resize the parent buffer instead.");
        }
    }
}