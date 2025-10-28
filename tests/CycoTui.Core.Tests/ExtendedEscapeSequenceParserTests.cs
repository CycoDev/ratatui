using Xunit;
using System.Linq;
using CycoTui.Core.Input;

namespace CycoTui.Core.Tests;

public class ExtendedEscapeSequenceParserTests
{
    [Fact]
    public void ParsesHomeEndPageFunctionKeys()
    {
        var input = "\u001b[1~\u001b[4~\u001b[5~\u001b[6~\u001b[15~"; // Home End PageUp PageDown F5
        var events = EscapeSequenceParser.Parse(input).ToList();
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.Home);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.End);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.PageUp);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.PageDown);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.F5);
    }

    [Fact]
    public void ParsesMouseScroll()
    {
        var input = "\u001b[<64;10;5M\u001b[<65;10;5M"; // scroll up then scroll down
        var events = EscapeSequenceParser.Parse(input).ToList();
        Assert.Contains(events, e => e.Mouse!.Value.Kind == MouseEventKind.ScrollUp);
        Assert.Contains(events, e => e.Mouse!.Value.Kind == MouseEventKind.ScrollDown);
    }
}
