using CycoTui.Core.Text;
using Xunit;

namespace CycoTui.Core.Tests;

public class MaskedTextTests
{
    [Fact]
    public void RendersMaskWithSameLength()
    {
        var m = new MaskedText("SecretValue", '#');
        var rendered = m.Render();
        Assert.Equal(m.Original.Length, rendered.Length);
        Assert.All(rendered, c => Assert.Equal('#', c));
    }
}
