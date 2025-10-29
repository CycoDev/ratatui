using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CycoTui.Core.Documentation;

public static class DocGenerationService
{
    public static string GenerateMarkdown(Assembly assembly, string commitHash) => GenerateMarkdown(new[] { assembly }, commitHash);

    public static string GenerateMarkdown(IEnumerable<Assembly> assemblies, string commitHash)
    {
        var types = assemblies
            .SelectMany(a => a.GetExportedTypes())
            .Distinct()
            .OrderBy(t => t.FullName)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("<!-- BEGIN-AUTO-DOC -->");
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Commit: {commitHash}");
        sb.AppendLine($"Assemblies: {string.Join(", ", assemblies.Select(a => a.GetName().Name))}");
        sb.AppendLine($"Public Types: {types.Count}");
        sb.AppendLine();
        sb.AppendLine("### Type Index");
        foreach (var t in types) sb.AppendLine("- " + t.FullName);
        sb.AppendLine();
        sb.AppendLine("<!-- END-AUTO-DOC -->");
        return sb.ToString();
    }
}
