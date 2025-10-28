using Xunit;
using System.Threading;
using CycoTui.Core.Input;

namespace CycoTui.Core.Tests;

public class InputLoopNavigationTests
{
    private sealed class DummyFocusable : IFocusableWidget
    {
        public bool CanFocus { get; init; } = true;
        public int FocusGainedCount { get; private set; }
        public void OnFocusGained() => FocusGainedCount++;
        public void OnFocusLost() { }
    }

    [Fact]
    public void TabCyclesFocus()
    {
        var fm = new FocusManager();
        var a = new DummyFocusable();
        var b = new DummyFocusable();
        var c = new DummyFocusable();
        fm.Register(a); fm.Register(b); fm.Register(c);
        var nav = new KeyNavigationHandler(fm);
        var events = new []
        {
            InputEvent.FromKey(new KeyEvent(KeyCode.Tab, null, KeyModifiers.None)),
            InputEvent.FromKey(new KeyEvent(KeyCode.Tab, null, KeyModifiers.None)),
            InputEvent.FromKey(new KeyEvent(KeyCode.Tab, null, KeyModifiers.Shift))
        };
        var source = new ScriptedBlockingSource(events);
        var loop = new BlockingInputLoop(source);
        var cts = new CancellationTokenSource();
        int handled = 0;
        loop.Run(cts.Token, e => {
            if (nav.Handle(e)) handled++;
            if (handled == events.Length) source.Complete();
        }, shouldStop: () => source.Completed);
        Assert.Equal(3, handled);
        Assert.Equal(b, fm.Current); // two next, one previous => ends at b
    }
}
