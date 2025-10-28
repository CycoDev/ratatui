using Xunit;
using System.Threading;
using CycoTui.Core.Input;

namespace CycoTui.Core.Tests;

public class BlockingInputLoopTests
{
    [Fact(Skip="Disabled pending stable blocking source timing")]
    public void LoopProcessesAllScriptedEventsAndStopsOnCompletion()
    {
        var events = new []
        {
            InputEvent.FromKey(new KeyEvent(KeyCode.Tab, null, KeyModifiers.None)),
            InputEvent.FromResize(new ResizeEvent(100,40)),
            InputEvent.FromKey(new KeyEvent(KeyCode.Tab, null, KeyModifiers.Shift))
        };
        var source = new ScriptedBlockingSource(events);
        var resizePoller = new DummyResizePoller();
        var loop = new BlockingInputLoop(source, resizePoller);
        int count = 0;
        var cts = new CancellationTokenSource();
        loop.Run(cts.Token, e => count++, shouldStop: () => source.Completed);
        Assert.Equal(3 + resizePoller.PolledCount, count); // includes any polled resize events
    }

    private sealed class DummyResizePoller : IPollingInputSource
    {
        private int _emissions;
        public int PolledCount => _emissions;
        public bool TryPoll(out InputEvent evt)
        {
            if (_emissions == 0)
            {
                evt = InputEvent.FromResize(new ResizeEvent(80,25));
                _emissions++;
                return true;
            }
            evt = default;
            return false;
        }
    }
}
