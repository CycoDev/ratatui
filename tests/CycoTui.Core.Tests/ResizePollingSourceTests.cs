using Xunit;
using CycoTui.Core.Input;
using CycoTui.Core.Backend;
using System.Threading;

namespace CycoTui.Core.Tests;

using System;

public class ResizePollingSourceTests
{
    private sealed class MockBackend : ITerminalBackend
    {
        public BackendCapabilities Capabilities => BackendCapabilities.Minimal;
        private int _w; private int _h;
        public MockBackend(int w, int h){ _w=w; _h=h; }
        public void SetSize(int w,int h){ _w=w; _h=h; }
        public Size GetSize() => new(_w,_h);
        public WindowSize GetWindowSize() => new(new Size(_w,_h), Size.Empty);
        public void Draw(System.Collections.Generic.IEnumerable<CellUpdate> updates) {}
        public void WriteRaw(string sequence) {}
        public void Flush() {}
        public void HideCursor() {}
        public void ShowCursor() {}
        public Position GetCursorPosition() => new(0,0);
        public void SetCursorPosition(Position position) {}
        public void Clear(ClearType type = ClearType.All) {}
        public void AppendLines(int count = 1) {}
        public void ScrollRegionUp(System.Range region, int lineCount) {}
        public void ScrollRegionDown(System.Range region, int lineCount) {}
        public void Dispose() {}
    }

    [Fact]
    public void EmitsResizeEventOnSizeChange()
    {
        var backend = new MockBackend(80,25);
        var source = new ResizePollingSource(backend);
        backend.SetSize(100,30);
        Assert.True(source.TryPoll(out var evt));
        Assert.Equal(InputEventType.Resize, evt.Type);
        Assert.Equal(100, evt.Resize!.Value.Width);
        Assert.Equal(30, evt.Resize!.Value.Height);
    }
}
