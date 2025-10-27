using System;
using System.Collections.Generic;
using CycoTui.Core.Logging;

namespace CycoTui.Core.Layout;

/// <summary>
/// Distributes space among constraints, producing child rectangles horizontally or vertically.
/// </summary>
/// <remarks>
/// Precedence order: Length -> Min -> Percentage -> Ratio -> Fill -> Max enforcement.
/// Overflow policy: proportional shrink of Min first, then Length if needed.
/// Alignment applied after sizing.
/// </remarks>
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

        // First pass: satisfy Length constraints (mandatory)
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

        // Overflow detection (mandatory = lengths from Length + Min targets)
        int mandatory = 0;
        for (int i = 0; i < constraints.Count; i++)
        {
            var c = constraints[i];
            if (c.Kind == ConstraintKind.Length) mandatory += lengths[i];
            else if (c.Kind == ConstraintKind.Min) mandatory += Math.Max(lengths[i], c.Value);
        }
        if (mandatory > primarySize)
        {
            // Proportionally shrink Min allocations first
            int minTotal = 0;
            for (int i = 0; i < constraints.Count; i++) if (constraints[i].Kind == ConstraintKind.Min) minTotal += Math.Max(lengths[i], constraints[i].Value);
            int excess = mandatory - primarySize;
            int shrinkTarget = Math.Min(excess, minTotal);
            if (shrinkTarget > 0 && minTotal > 0)
            {
                int shrunk = 0;
                for (int i = 0; i < constraints.Count; i++)
                {
                    var c = constraints[i];
                    if (c.Kind == ConstraintKind.Min)
                    {
                        int cur = Math.Max(lengths[i], c.Value);
                        int proportional = (int)Math.Floor(shrinkTarget * (cur / (double)minTotal));
                        int newVal = cur - proportional;
                        if (newVal < 0) newVal = 0;
                        lengths[i] = newVal;
                        shrunk += (cur - newVal);
                    }
                }
                excess -= shrunk;
            }
            // If still excess, shrink Length constraints proportionally
            if (excess > 0)
            {
                int lengthTotal = 0;
                for (int i = 0; i < constraints.Count; i++) if (constraints[i].Kind == ConstraintKind.Length) lengthTotal += lengths[i];
                if (lengthTotal > 0)
                {
                    int shrunk2 = 0;
                    for (int i = 0; i < constraints.Count; i++)
                    {
                        var c = constraints[i];
                        if (c.Kind == ConstraintKind.Length)
                        {
                            int cur = lengths[i];
                            int proportional = (int)Math.Floor(excess * (cur / (double)lengthTotal));
                            int newVal = cur - proportional;
                            if (newVal < 0) newVal = 0;
                            lengths[i] = newVal;
                            shrunk2 += (cur - newVal);
                        }
                    }
                }
            }
            // Recompute used after shrink
            used = 0;
            for (int i = 0; i < lengths.Length; i++) used += lengths[i];
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
        double ratioWeightSum = 0.0;
        for (int i = 0; i < constraints.Count; i++) if (constraints[i].Kind == ConstraintKind.Ratio) ratioWeightSum += constraints[i].Value / (double)constraints[i].Denominator;
        if (ratioWeightSum > 0 && remaining > 0)
        {
            int allocated = 0;
            var ratioAllocations = new int[constraints.Count];
            for (int i = 0; i < constraints.Count; i++)
            {
                var c = constraints[i];
                if (c.Kind == ConstraintKind.Ratio)
                {
                    double weight = c.Value / (double)c.Denominator;
                    int alloc = (int)Math.Floor(remaining * (weight / ratioWeightSum));
                    ratioAllocations[i] = alloc;
                    allocated += alloc;
                }
            }
            // Distribute leftover from flooring
            int leftover = remaining - allocated;
            for (int i = 0; i < constraints.Count && leftover > 0; i++)
            {
                if (constraints[i].Kind == ConstraintKind.Ratio)
                {
                    ratioAllocations[i] += 1;
                    leftover--;
                }
            }
            // Apply allocations
            for (int i = 0; i < constraints.Count; i++) if (constraints[i].Kind == ConstraintKind.Ratio) lengths[i] += ratioAllocations[i];
            remaining = 0;
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
                    int cursor = area.X + gap + (extra > 0 ? 1 : 0);
                    if (extra > 0) extra--;
                    for (int i = 0; i < constraints.Count; i++)
                    {
                        int len = lengths[i];
                        rects[i] = direction == LayoutDirection.Horizontal
                            ? new Rect(cursor, area.Y, len, area.Height)
                            : new Rect(area.X, cursor, area.Width, len);
                        cursor += len;
                        int addGap = gap + (extra > 0 ? 1 : 0);
                        if (extra > 0) extra--;
                        cursor += addGap;
                    }
                    return rects;
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
