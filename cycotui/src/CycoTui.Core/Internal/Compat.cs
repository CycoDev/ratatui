using System;

namespace CycoAI.CycoTui.Core.Internal
{
    /// <summary>
    /// Compatibility helpers for .NET Standard.
    /// </summary>
    internal static class Compat
    {
        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        /// <summary>
        /// Combines hash codes for multiple objects.
        /// </summary>
        public static int CombineHashCodes(params object[] objects)
        {
            unchecked
            {
                int hash = 17;
                foreach (var obj in objects)
                {
                    hash = hash * 31 + (obj?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }
    }
}