using System;
using System.Collections.Generic;
using CycoTui.Core.Logging;

namespace CycoTui.Core.Layout;

/// <summary>
/// Distributes space among constraints producing child rectangles.
/// </summary>
public sealed class LayoutEngine
{
    private readonly LoggingContext _logging;

    public LayoutEngine(LoggingContext logging) => _logging = logging;

    /// <summary>
    /// Distribute the <paramref name="area"/> among the given <paramref name="constraints"/>.
    /// Applies margin/padding if provided. Returns list of child rects in order.
    /// </summary>
    public IReadOnlyList<Rect> Distribute(
        Rect area,
        IReadOnlyList<Constraint> constraints,
        LayoutDirection direction,
        Margin? margin = null,
        Padding? padding = null,
        AlignmentMode alignment = AlignmentMode.Start)
    {
        var logger = _logging.GetLogger<LayoutEngine>();
        if (constraints == null || constraints.Count == 0) return Array.Empty<Rect>();

        // Apply margin then padding
        if (margin.HasValue) area = margin.Value.Apply(area);
        if (padding.HasValue) area = padding.Value.Apply(area);

        int primarySize = direction == LayoutDirection.Horizontal ? area.Width : area.Height;
        var lengths = new int[constraints.Count];

        // First pass: satisfy Length constraints
        int used = 0;
        for (int i = 0; i < constraints.Count; i++)
        {
            var c = constraints[i];
            if (c.Kind == ConstraintKind.Length)
            {
                lengths[i] = Math.Min(c.Value, primarySize - used);
                used += lengths[i];
            }
        }

        // Second pass: Min constraints (ensure minimum if length not set)
        for (int i = 0; i < constraints.Count; i++)
        {
            var c = constraints[i];
            if (c.Kind == ConstraintKind.Min && lengths[i] < c.Value)
            {
                var add = Math.Min(c.Value - lengths[i], primarySize - used);
                lengths[i] += add;
                used += add;
            }
        }

        // Third pass: allocate Percentage
        for (int i = 0; i < constraints.Count; i++)
        {
            var c = constraints[i];
            if (c.Kind == ConstraintKind.Percentage)
            {
                int alloc = (int)Math.Round(primarySize * (c.Value / 100.0));
                alloc = Math.Min(alloc, primarySize - used);
                lengths[i] += alloc;
                used += alloc;
            }
        }

        // Fourth pass: Ratio constraints share remaining proportionally
        int remaining = primarySize - used;
        int ratioSum = 0;
        for (int i = 0; i < constraints.Count; i++) if (constraints[i].Kind == ConstraintKind.Ratio) ratioSum += constraints[i].Value;
        if (ratioSum > 0 && remaining > 0)
        {
            int allocated = 0;
            for (int i = 0; i < constraints.Count; i++)
            {
                var c = constraints[i];
                if (c.Kind == ConstraintKind.Ratio)
                {
                    int alloc = (int)Math.Floor(remaining * (c.Value / (double)ratioSum));
                    lengths[i] += alloc;
                    allocated += alloc;
                }
            }
            remaining -= allocated;
        }

        // Fifth pass: Fill constraints consume remaining equally or by order
        var fillIndices = new List<int>();
        for (int i = 0; i < constraints.Count; i++) if (constraints[i].Kind == ConstraintKind.Fill) fillIndices.Add(i);
        if (fillIndices.Count > 0 && remaining > 0)
        {
            // Simple equal distribution
            int per = remaining / fillIndices.Count;
            int leftover = remaining % fillIndices.Count;
            foreach (var idx in fillIndices)
            {
                lengths[idx] += per + (leftover > 0 ? 1 : 0);
                if (leftover > 0) leftover--;
            }
            remaining = 0;
        }

        // Sixth pass: enforce Max constraints (shrink if needed, redistribute excess to fills if present)
        int reclaimed = 0;
        for (int i = 0; i < constraints.Count; i++)
        {
            var c = constraints[i];
            if (c.Kind == ConstraintKind.Max && lengths[i] > c.Value)
            {
                reclaimed += (lengths[i] - c.Value);
                lengths[i] = c.Value;
            }
        }
        if (reclaimed > 0)
        {
            var fillAgain = new List<int>();
            for (int i = 0; i < constraints.Count; i++) if (constraints[i].Kind == ConstraintKind.Fill) fillAgain.Add(i);
            if (fillAgain.Count > 0)
            {
                int per = reclaimed / fillAgain.Count;
                int leftover = reclaimed % fillAgain.Count;
                foreach (var idx in fillAgain)
                {
                    lengths[idx] += per + (leftover > 0 ? 1 : 0);
                    if (leftover > 0) leftover--;
                }
                reclaimed = 0;
            }
        }

        // Alignment and positioning
        var rects = new Rect[constraints.Count];
        int startOffset = 0;
        int totalUsed = 0;
        for (int i = 0; i < lengths.Length; i++) totalUsed += lengths[i];
        int freeSpace = primarySize - totalUsed;

        switch (alignment)
        {
            case AlignmentMode.Center:
                startOffset = freeSpace / 2; break;
            case AlignmentMode.End:
                startOffset = freeSpace; break;
            case AlignmentMode.SpaceBetween:
                // distribute gaps between items
                // if only one item, treat like Start
                if (constraints.Count > 1)
                {
                    int gap = freeSpace / (constraints.Count - 1);
                    int extra = freeSpace % (constraints.Count - 1);
                    int cursor = direction == LayoutDirection.Horizontal ? area.X : area.Y;
                    for (int i = 0; i < constraints.Count; i++)
                    {
                        int len = lengths[i];
                        rects[i] = direction == LayoutDirection.Horizontal
                            ? new Rect(cursor, area.Y, len, area.Height)
                            : new Rect(area.X, cursor, area.Width, len);
                        cursor += len;
                        if (i < constraints.Count - 1)
                            cursor += gap + (extra-- > 0 ? 1 : 0);
                    }
                    return rects;
                }
                break;
            case AlignmentMode.SpaceAround:
                if (constraints.Count > 0)
                {
                    int gaps = constraints.Count;
                    int gap = freeSpace / gaps;
                    int extra = freeSpace % gaps;
                    startOffset = gap/2 + (extra > 0 ? 1 : 0);
                }
                break;
            case AlignmentMode.SpaceEvenly:
                if (constraints.Count > 0)
                {
                    int gaps = constraints.Count + 1;
                    int gap = freeSpace / gaps;
                    int extra = freeSpace % gaps;
                    startOffset = gap + (extra > 0 ? 1 : 0);
                }
                break;
        }

        int pos = direction == LayoutDirection.Horizontal ? area.X + startOffset : area.Y + startOffset;
        for (int i = 0; i < lengths.Length; i++)
        {
            int len = lengths[i];
            rects[i] = direction == LayoutDirection.Horizontal
                ? new Rect(pos, area.Y, len, area.Height)
                : new Rect(area.X, pos, area.Width, len);
            pos += len;
        }

        return rects;
    }
}
