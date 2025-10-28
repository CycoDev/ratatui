using System;
using System.Threading;
using CycoTui.Core.Backend;

namespace CycoTui.Core.Input;

/// <summary>
/// Simple polling input source that emits ResizeEvent when terminal size changes.
/// </summary>
public sealed class ResizePollingSource : IPollingInputSource
{
    private readonly ITerminalBackend _backend;
    private (int W, int H) _lastSize;

    public ResizePollingSource(ITerminalBackend backend)
    {
        _backend = backend;
        var s = backend.GetSize();
        _lastSize = (s.Width, s.Height);
    }

    public bool TryPoll(out InputEvent evt)
    {
        var s = _backend.GetSize();
        if (s.Width != _lastSize.W || s.Height != _lastSize.H)
        {
            _lastSize = (s.Width, s.Height);
            evt = InputEvent.FromResize(new ResizeEvent(s.Width, s.Height));
            return true;
        }
        evt = default;
        return false;
    }
}
