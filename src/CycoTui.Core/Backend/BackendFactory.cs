using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CycoTui.Core.Backend;

/// <summary>
/// Factory responsible for creating terminal backend instances with fallback logic.
/// Supports registration of additional backend providers without modifying core code.
/// </summary>
public static class BackendFactory
{
    private static readonly Dictionary<BackendPreference, Func<ILogger?, ITerminalBackend>> Registry =
        new()
        {
            { BackendPreference.Windows, logger => new Stubs.WindowsBackendStub(logger) },
            { BackendPreference.Unix, logger => new Stubs.UnixBackendStub(logger) },
            { BackendPreference.Minimal, logger => new Stubs.MinimalBackendStub(logger) }
        };

    /// <summary>
    /// Registers or replaces a backend creation delegate for a preference key.
    /// </summary>
    public static void Register(BackendPreference preference, Func<ILogger?, ITerminalBackend> factory)
    {
        Registry[preference] = factory;
    }

    public static ITerminalBackend Create(BackendPreference preference, ILogger? logger = null)
    {
        if (preference == BackendPreference.Auto)
        {
            // Proposed fallback order per implementation decisions
            if (OperatingSystem.IsWindows() && Registry.TryGetValue(BackendPreference.Windows, out var win))
                return win(logger);
            if ((OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) && Registry.TryGetValue(BackendPreference.Unix, out var nix))
                return nix(logger);
            return Registry[BackendPreference.Minimal](logger);
        }

        if (Registry.TryGetValue(preference, out var factory))
        {
            if (preference == BackendPreference.Windows && !OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("Windows backend requested on non-Windows platform.");
            if (preference == BackendPreference.Unix && !(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()))
                throw new PlatformNotSupportedException("Unix backend requested on non-Unix platform.");
            return factory(logger);
        }

        throw new PlatformNotSupportedException($"Backend '{preference}' not registered or not supported on this platform.");
    }
}

public enum BackendPreference
{
    Auto,
    Windows,
    Unix,
    Minimal
}
