using CycoTui.Core.Widgets;
using Xunit;

namespace CycoTui.Core.Tests;

public class ListStateNavigationTests
{
    [Fact]
    public void NextAndPreviousNavigateWithinBounds()
    {
        var state = new ListState(count: 3);
        state.Next(viewportHeight:3); // select first
        Assert.Equal(0, state.Selected);
        state.Next(viewportHeight:3);
        Assert.Equal(1, state.Selected);
        state.Next(viewportHeight:3);
        Assert.Equal(2, state.Selected);
        state.Next(viewportHeight:3); // stays at last
        Assert.Equal(2, state.Selected);
        state.Previous(viewportHeight:3);
        Assert.Equal(1, state.Selected);
        state.Previous(viewportHeight:3);
        Assert.Equal(0, state.Selected);
        state.Previous(viewportHeight:3); // stays at first
        Assert.Equal(0, state.Selected);
    }
}
