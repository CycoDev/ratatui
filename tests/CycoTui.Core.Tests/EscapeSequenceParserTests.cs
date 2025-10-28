using Xunit;
using System.Linq;
using CycoTui.Core.Input;

namespace CycoTui.Core.Tests;

public class EscapeSequenceParserTests
{
    [Fact]
    public void ParsesArrowKeys()
    {
        var input = "\u001b[A\u001b[B\u001b[C\u001b[D";
        var events = EscapeSequenceParser.Parse(input).ToList();
        Assert.Equal(4, events.Count);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.ArrowUp);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.ArrowDown);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.ArrowRight);
        Assert.Contains(events, e => e.Key!.Value.Code == KeyCode.ArrowLeft);
    }

    [Fact]
    public void ParsesAltCharacter()
    {
        var input = "\u001bX"; // Alt+X
        var evt = EscapeSequenceParser.Parse(input).Single();
        Assert.Equal(KeyCode.Character, evt.Key!.Value.Code);
        Assert.Equal('X', evt.Key!.Value.Char);
        Assert.True((evt.Key!.Value.Modifiers & KeyModifiers.Alt) != 0);
    }
}
