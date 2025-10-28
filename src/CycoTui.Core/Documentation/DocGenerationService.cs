using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CycoTui.Core.Documentation;

/// <summary>
/// Generates markdown documentation of the public API surface using reflection.
/// </summary>
public static class DocGenerationService
{
    public static string GenerateMarkdown(Assembly assembly, string commitHash)
        => GenerateMarkdown(new[] { assembly }, commitHash);

    public static string GenerateMarkdown(IEnumerable<Assembly> assemblies, string commitHash)
    {
        var asmList = assemblies.ToList();
        var types = asmList
            .SelectMany(a => a.GetExportedTypes())
            .Distinct()
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name)
            .ToList();

        var categories = new Dictionary<string, Func<Type, bool>>
        {
            ["Backend"]   = t => t.Namespace?.Contains("Backend") == true,
            ["Buffer"]    = t => t.Namespace?.Contains("Buffer") == true,
            ["Rendering"] = t => t.Namespace?.Contains("Terminal") == true || t.Name.Contains("Frame"),
            ["Style"]     = t => t.Namespace?.Contains("Style") == true,
            ["Layout"]    = t => t.Namespace?.Contains("Layout") == true,
            ["Widgets"]   = t => t.Namespace?.Contains("Widgets") == true,
            ["Text"]      = t => t.Namespace?.Contains("Text") == true,
            ["Input"]     = t => t.Namespace?.Contains("Input") == true
        };
        var categoryCounts = categories.ToDictionary(k => k.Key, k => types.Count(k.Value));

        var sb = new StringBuilder();
        sb.AppendLine("<!-- BEGIN-AUTO-DOC -->");
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Commit: {commitHash}");
        sb.AppendLine($"Assemblies: {string.Join(", ", asmList.Select(a => a.GetName().Name))}");
        sb.AppendLine($"Public Types: {types.Count}");
        sb.AppendLine();

        sb.AppendLine("### Category Summary");
        foreach (var kv in categoryCounts.OrderBy(c => c.Key))
            sb.AppendLine($"- {kv.Key}: {kv.Value}");
        sb.AppendLine();

        sb.AppendLine("### Type Index");
        foreach (var t in types) sb.AppendLine($"- {t.FullName}");
        sb.AppendLine();

        var enumCache = new Dictionary<Type, List<string>>();
        foreach (var nsGroup in types.GroupBy(t => t.Namespace ?? "<global>").OrderBy(g => g.Key))
        {
            sb.AppendLine($"<details><summary>Namespace {nsGroup.Key}</summary>");
            sb.AppendLine();
            foreach (var t in nsGroup)
                sb.AppendLine(FormatType(t, enumCache));
            sb.AppendLine("</details>");
            sb.AppendLine();
        }

        sb.AppendLine("<!-- END-AUTO-DOC -->");
        sb.AppendLine();
        return sb.ToString();
    }

    private static string FormatType(Type t, Dictionary<Type, List<string>> enumCache)
    {
        var kind = t.IsInterface
            ? "interface"
            : t.IsEnum
                ? "enum"
                : t.IsValueType
                    ? (IsRecord(t) ? "record struct" : "struct")
                    : IsRecord(t) ? "record" : "class";

        var sb = new StringBuilder();
        sb.AppendLine($"### {kind} {t.Name}");

        if (t.IsEnum)
        {
            if (!enumCache.TryGetValue(t, out var vals))
            {
                vals = Enum.GetNames(t).ToList();
                enumCache[t] = vals;
            }
            sb.AppendLine("- enum values: " + string.Join(", ", vals));
        }

        var members = t
            .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(m => m.MemberType is MemberTypes.Method or MemberTypes.Property or MemberTypes.Field or MemberTypes.Event)
            .Where(m => !m.Name.StartsWith("get_")
                     && !m.Name.StartsWith("set_")
                     && !m.Name.StartsWith("add_")
                     && !m.Name.StartsWith("remove_"))
            .OrderBy(m => m.Name)
            .ToList();

        if (members.Count == 0)
        {
            sb.AppendLine("(no public members declared)");
            return sb.ToString();
        }

        foreach (var m in members)
        {
            var formatted = FormatMember(m);
            if (!string.IsNullOrEmpty(formatted)) sb.AppendLine(formatted);
        }

        return sb.ToString();
    }

    private static bool IsRecord(Type t)
        => t.GetMethod("PrintMembers", BindingFlags.Instance | BindingFlags.NonPublic) != null;

    private static string FormatMember(MemberInfo m) => m.MemberType switch
    {
        MemberTypes.Method  => FormatMethod((MethodInfo)m),
        MemberTypes.Property => $"- property {m.Name}",
        MemberTypes.Field    => $"- field {m.Name}",
        MemberTypes.Event    => $"- event {m.Name}",
        _ => string.Empty
    };

    private static string FormatMethod(MethodInfo mi)
    {
        if (mi.IsSpecialName) return string.Empty;
        var pars = mi.GetParameters();
        var paramList = string.Join(", ", pars.Select(p => p.ParameterType.Name + " " + p.Name));
        return $"- method {mi.Name}({paramList})";
    }
}
