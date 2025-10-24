using CycoTui.Core.Widgets;
using Xunit;

namespace CycoTui.Core.Tests;

public class ListStateNavigationTests
{
    [Fact]
    public void NextAndPreviousNavigateWithinBounds()
    {
        var state = new ListState(count: 3);
        state.Next(); // select first
        Assert.Equal(0, state.Selected);
        state.Next();
        Assert.Equal(1, state.Selected);
        state.Next();
        Assert.Equal(2, state.Selected);
        state.Next(); // stays at last
        Assert.Equal(2, state.Selected);
        state.Previous();
        Assert.Equal(1, state.Selected);
        state.Previous();
        Assert.Equal(0, state.Selected);
        state.Previous(); // stays at first
        Assert.Equal(0, state.Selected);
    }
}
