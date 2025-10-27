using System.Linq;
using Xunit;
using CycoTui.Core.Terminal;
using CycoTui.Core.Backend;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;

namespace CycoTui.Core.Tests;

public class StyleCompressionTests
{
    [Fact]
    public void ModifierSequencesAreCompressed()
    {
        var backend = new TestBackend();
        var term = new TerminalType(backend, new LoggingContext(null));
        term.Draw(f =>
        {
            f.SetCell(0,0,"A", StyleType.Empty.Add(TextModifier.Bold));
            f.SetCell(1,0,"B", StyleType.Empty.Add(TextModifier.Bold | TextModifier.Italic));
            f.SetCell(2,0,"C", StyleType.Empty.Add(TextModifier.Bold | TextModifier.Italic | TextModifier.Underline));
        });
        // Look for combined sequence 1;3;4 (bold;italic;underline) or subset; ensure not three separate bold/italic/underline starts
        var modifierSeqs = backend.RawSequences.Where(s => s.Contains("[1") || s.Contains("[3") || s.Contains("[4")).ToList();
        // There will be at most 2: one intermediate and one final compressed; enforce <=2
        Assert.True(modifierSeqs.Count <= 2, $"Expected compressed modifier emission (<=2 sequences), got {modifierSeqs.Count}");
    }
}
