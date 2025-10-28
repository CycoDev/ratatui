using System.Collections.Generic;
using System.Threading;

namespace CycoTui.Core.Input;

/// <summary>
/// Scripted input source for unit tests.
/// </summary>
public sealed class TestInputSource : IInputSource
{
    private readonly Queue<InputEvent> _events;
    public TestInputSource(IEnumerable<InputEvent> events) => _events = new Queue<InputEvent>(events);

    public bool TryRead(out InputEvent evt, CancellationToken cancellationToken)
    {
        if (_events.Count > 0)
        {
            evt = _events.Dequeue();
            return true;
        }
        evt = default;
        return false;
    }
}
