using Xunit;
using CycoTui.Core.Documentation;
using System.Reflection;
using System.IO;

namespace CycoTui.Core.Tests;

public class DocGenerationSmokeTests
{
    [Fact]
    public void GeneratesNonEmptyMarkdown()
    {
        var asm = typeof(CycoTui.Core.Terminal.Terminal).Assembly;
        var md = DocGenerationService.GenerateMarkdown(asm, "TEST-COMMIT");
        Assert.Contains("Public Types:", md);
        Assert.Contains("Namespace", md);
        // Persist portion into README.generated.md between markers
        var path = Path.Combine(Directory.GetCurrentDirectory(), "README.generated.md");
        if (File.Exists(path))
        {
            var existing = File.ReadAllText(path);
            int start = existing.IndexOf("<!-- BEGIN-AUTO-DOC -->");
            int end = existing.IndexOf("<!-- END-AUTO-DOC -->");
            var newBlock = md;
            if (start >= 0 && end > start)
            {
                var updated = existing.Substring(0, start) + newBlock + existing.Substring(end + "<!-- END-AUTO-DOC -->".Length);
                File.WriteAllText(path, updated);
            }
        }
    }
}
