using System;
using System.Threading;

namespace CycoTui.Core.Input;

/// <summary>
/// Synchronous input loop dispatching events to a handler.
/// </summary>
public sealed class InputLoop
{
    private readonly IInputSource _source;
    public InputLoop(IInputSource source) => _source = source;

    public void Run(CancellationToken token, Action<InputEvent> dispatch)
    {
        while (!token.IsCancellationRequested)
        {
            if (_source.TryRead(out var evt, token))
            {
                dispatch(evt);
            }
        }
    }
}
