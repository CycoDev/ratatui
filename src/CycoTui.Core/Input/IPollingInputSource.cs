namespace CycoTui.Core.Input;

/// <summary>
/// Non-blocking poll source for auxiliary events (resize, timers, etc.).
/// </summary>
public interface IPollingInputSource
{
    bool TryPoll(out InputEvent evt);
}
