using Xunit;
using CycoTui.Core.Documentation;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace CycoTui.Core.Tests;

public class DocGenerationSmokeTests
{
    [Fact]
    public void GeneratesNonEmptyMarkdown()
    {
        var assemblies = new List<Assembly> { typeof(CycoTui.Core.Terminal.Terminal).Assembly };
        TryAddAssembly("CycoTui.Backend.Unix.UnixTerminalBackend", assemblies);
        TryAddAssembly("CycoTui.Backend.Windows.WindowsTerminalBackend", assemblies);
        var commit = GetGitCommitHash();
        var md = DocGenerationService.GenerateMarkdown(assemblies, commit);

        Assert.Contains("Public Types:", md);
        Assert.Contains("### Category Summary", md);
        Assert.Contains("### Type Index", md);
        Assert.Contains(commit, md);

        var path = FindRepoRootReadme();
        File.WriteAllText(path, md);
        var persisted = File.ReadAllText(path);
        Assert.Contains(commit, persisted);
        Assert.Contains("### Category Summary", persisted);
    }

    private static void TryAddAssembly(string typeFullName, List<Assembly> list)
    {
        var t = Type.GetType(typeFullName);
        if (t != null) list.Add(t.Assembly);
    }

    private static string GetGitCommitHash()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse --short HEAD",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using var p = System.Diagnostics.Process.Start(psi);
            return p!.StandardOutput.ReadToEnd().Trim();
        }
        catch { return "UNKNOWN"; }
    }

    private static string FindRepoRootReadme()
    {
        var dir = Directory.GetCurrentDirectory();
        for (int i = 0; i < 10; i++)
        {
            var candidate = Path.Combine(dir, "README.generated.md");
            if (File.Exists(candidate)) return candidate;
            var parent = Directory.GetParent(dir);
            if (parent == null) break;
            dir = parent.FullName;
        }
        return Path.Combine(Directory.GetCurrentDirectory(), "README.generated.md");
    }
}
