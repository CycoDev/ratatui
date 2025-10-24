using System;
using System.Collections.Generic;

namespace CycoTui.Core.Features;

/// <summary>
/// Runtime feature flag snapshot. Build-time exclusion still relies on conditional compilation.
/// </summary>
public sealed class FeatureFlags
{
    private readonly HashSet<string> _flags;

    private FeatureFlags(HashSet<string> flags) => _flags = flags;

    public bool IsEnabled(string name) => _flags.Contains(name);

    public static FeatureFlags Empty { get; } = new(new HashSet<string>(StringComparer.OrdinalIgnoreCase));

    public static Builder CreateBuilder() => new();

    public sealed class Builder
    {
        private readonly HashSet<string> _flags = new(StringComparer.OrdinalIgnoreCase);

        public Builder Enable(string name)
        {
            if (!string.IsNullOrWhiteSpace(name)) _flags.Add(name.Trim());
            return this;
        }

        public FeatureFlags Build() => new(new HashSet<string>(_flags, StringComparer.OrdinalIgnoreCase));
    }
}
