namespace DocGenProbe;

/// <summary>
/// Simple probe type used to verify XML doc generation.
/// </summary>
public class Probe
{
    /// <summary>
    /// Returns a greeting.
    /// </summary>
    public string Hello(string name) => $"Hello, {name}";
}
