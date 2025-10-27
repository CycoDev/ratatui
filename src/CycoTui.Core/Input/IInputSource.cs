using System.Threading;

namespace CycoTui.Core.Input;

/// <summary>
/// Abstract source of raw input events (keyboard, mouse, resize).
/// </summary>
public interface IInputSource
{
    bool TryRead(out InputEvent evt, CancellationToken cancellationToken);
}
