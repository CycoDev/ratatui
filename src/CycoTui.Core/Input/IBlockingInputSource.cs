using System.Threading;

namespace CycoTui.Core.Input;

/// <summary>
/// Blocking input source: returns true when an event is produced; false only on cancellation.
/// </summary>
public interface IBlockingInputSource
{
    bool TryReadBlocking(out InputEvent evt, CancellationToken token);
}
