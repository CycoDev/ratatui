using System;
using System.Threading;
using System.Collections.Generic;

namespace CycoTui.Core.Input;

/// <summary>
/// Blocking input loop consuming a primary blocking source plus optional polling sources.
/// </summary>
public sealed class BlockingInputLoop
{
    private readonly IBlockingInputSource _primary;
    private readonly IReadOnlyList<IPollingInputSource> _pollers;

    public BlockingInputLoop(IBlockingInputSource primary, params IPollingInputSource[] pollers)
    {
        _primary = primary;
        _pollers = pollers;
    }

    public void Run(CancellationToken token, Action<InputEvent> dispatch, Func<bool>? shouldStop = null)
    {
        while (!token.IsCancellationRequested)
        {
            if (shouldStop?.Invoke() == true) break;
            if (!_primary.TryReadBlocking(out var evt, token)) break; // cancellation or completion
            dispatch(evt);
            foreach (var p in _pollers)
            {
                if (p.TryPoll(out var pevt)) dispatch(pevt);
            }
        }
    }
}
