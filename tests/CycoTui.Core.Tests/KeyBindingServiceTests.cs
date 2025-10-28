using Xunit;
using CycoTui.Core.Input;

namespace CycoTui.Core.Tests;

public class KeyBindingServiceTests
{
    [Fact]
    public void HandlesBoundKey()
    {
        var svc = new KeyBindingService();
        bool invoked = false;
        svc.Bind(KeyCode.F5, KeyModifiers.Ctrl, () => invoked = true);
        var evt = InputEvent.FromKey(new KeyEvent(KeyCode.F5, null, KeyModifiers.Ctrl));
        Assert.True(svc.TryHandle(evt));
        Assert.True(invoked);
    }

    [Fact]
    public void IgnoresUnboundKey()
    {
        var svc = new KeyBindingService();
        var evt = InputEvent.FromKey(new KeyEvent(KeyCode.F5, null, KeyModifiers.None));
        Assert.False(svc.TryHandle(evt));
    }
}
