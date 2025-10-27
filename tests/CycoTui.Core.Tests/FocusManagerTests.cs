using Xunit;
using CycoTui.Core.Input;

namespace CycoTui.Core.Tests;

public class FocusManagerTests
{
    private sealed class TestFocusable : IFocusableWidget
    {
        public bool CanFocus { get; init; } = true;
        public bool Gained { get; private set; }
        public bool Lost { get; private set; }
        public void OnFocusGained() => Gained = true;
        public void OnFocusLost() => Lost = true;
    }

    [Fact]
    public void RegistersAndCyclesFocus()
    {
        var fm = new FocusManager();
        var a = new TestFocusable();
        var b = new TestFocusable();
        fm.Register(a);
        fm.Register(b);
        Assert.Equal(a, fm.Current);
        fm.Next();
        Assert.Equal(b, fm.Current);
        Assert.True(a.Lost && b.Gained);
        fm.Previous();
        Assert.Equal(a, fm.Current);
    }
}
