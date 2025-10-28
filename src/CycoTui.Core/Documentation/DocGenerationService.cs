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
    {
        var types = assembly.GetExportedTypes().OrderBy(t => t.Namespace).ThenBy(t => t.Name).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("<!-- BEGIN-AUTO-DOC -->");
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Commit: {commitHash}");
        sb.AppendLine($"Assembly: {assembly.GetName().Name}");
        sb.AppendLine($"Public Types: {types.Count}");
        sb.AppendLine();

        var nsGroups = types.GroupBy(t => t.Namespace ?? "<global>").OrderBy(g => g.Key);
        foreach (var g in nsGroups)
        {
            sb.AppendLine($"## Namespace {g.Key}");
            foreach (var t in g)
            {
                sb.AppendLine(FormatType(t));
            }
            sb.AppendLine();
        }
        sb.AppendLine("<!-- END-AUTO-DOC -->");
        return sb.ToString();
    }

    private static string FormatType(Type t)
    {
        var kind = t.IsInterface ? "interface" : t.IsEnum ? "enum" : t.IsValueType ? (IsRecord(t) ? "record struct" : "struct") : IsRecord(t) ? "record" : "class";
        var sb = new StringBuilder();
        sb.AppendLine($"### {kind} {t.Name}");
        var members = t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                       .Where(m => m.MemberType == MemberTypes.Method || m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Event)
                       .OrderBy(m => m.Name)
                       .ToList();
        if (members.Count == 0)
        {
            sb.AppendLine("(no public members declared)");
            return sb.ToString();
        }
        foreach (var m in members)
        {
            sb.AppendLine(FormatMember(m));
        }
        return sb.ToString();
    }

    private static bool IsRecord(Type t) => t.GetMethod("PrintMembers", BindingFlags.Instance | BindingFlags.NonPublic) != null;

    private static string FormatMember(MemberInfo m)
    {
        return m.MemberType switch
        {
            MemberTypes.Method => FormatMethod((MethodInfo)m),
            MemberTypes.Property => $"- property {m.Name}",
            MemberTypes.Field => $"- field {m.Name}",
            MemberTypes.Event => $"- event {m.Name}",
            _ => string.Empty
        };
    }

    private static string FormatMethod(MethodInfo mi)
    {
        if (mi.IsSpecialName) return string.Empty; // skip property accessor, operator overload for brevity now
        var pars = mi.GetParameters();
        var paramList = string.Join(", ", pars.Select(p => p.ParameterType.Name + " " + p.Name));
        return $"- method {mi.Name}({paramList})";
    }
}
