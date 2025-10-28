using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace CycoTui.Core.Input;

/// <summary>
/// Blocking scripted source for tests. Dequeues events; waits when empty until completion signaled.
/// </summary>
public sealed class ScriptedBlockingSource : IBlockingInputSource
{
    private readonly ConcurrentQueue<InputEvent> _events = new();
    private readonly ManualResetEventSlim _available = new(false);
    private volatile bool _completed;

    public bool Completed => _completed && _events.IsEmpty;

    public ScriptedBlockingSource(IEnumerable<InputEvent> events)
    {
        foreach (var e in events) _events.Enqueue(e);
        if (!_events.IsEmpty) _available.Set();
    }

    public void Enqueue(InputEvent evt)
    {
        _events.Enqueue(evt);
        _available.Set();
    }

    public void Complete()
    {
        _completed = true;
        _available.Set();
    }

    public bool TryReadBlocking(out InputEvent evt, CancellationToken token)
    {
        while (!_events.TryDequeue(out evt))
        {
            if (Completed || token.IsCancellationRequested)
            {
                evt = default;
                return false;
            }
            _available.Wait(token);
            _available.Reset();
        }
        return true;
    }
}
