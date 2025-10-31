using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Scans workspace directory for files, excluding common build/dependency folders.
/// </summary>
public static class WorkspaceFileScanner
{
    private static readonly HashSet<string> ExcludedDirectories = new(StringComparer.OrdinalIgnoreCase)
    {
        "bin", "obj", "node_modules", ".git", ".vs", ".vscode", ".idea",
        "packages", "dist", "build", "out", "target", ".next", ".nuxt"
    };

    private static readonly HashSet<string> ExcludedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".dll", ".exe", ".pdb", ".cache", ".tmp", ".log", ".lock"
    };

    /// <summary>
    /// Scans the workspace directory and returns relative file paths.
    /// </summary>
    /// <param name="workspaceRoot">Absolute path to workspace root directory.</param>
    /// <param name="maxFiles">Maximum number of files to return (default 1000).</param>
    /// <returns>List of relative file paths from workspace root.</returns>
    public static IReadOnlyList<string> ScanFiles(string workspaceRoot, int maxFiles = 1000)
    {
        if (!Directory.Exists(workspaceRoot))
            return Array.Empty<string>();

        var files = new List<string>();
        ScanDirectoryRecursive(workspaceRoot, workspaceRoot, files, maxFiles);
        return files.OrderBy(f => f).ToList();
    }

    private static void ScanDirectoryRecursive(
        string rootPath,
        string currentPath,
        List<string> files,
        int maxFiles)
    {
        if (files.Count >= maxFiles)
            return;

        try
        {
            // Get all files in current directory
            foreach (var filePath in Directory.EnumerateFiles(currentPath))
            {
                if (files.Count >= maxFiles)
                    return;

                var extension = Path.GetExtension(filePath);
                if (ExcludedExtensions.Contains(extension))
                    continue;

                var relativePath = Path.GetRelativePath(rootPath, filePath);
                files.Add(relativePath);
            }

            // Recursively scan subdirectories
            foreach (var dirPath in Directory.EnumerateDirectories(currentPath))
            {
                if (files.Count >= maxFiles)
                    return;

                var dirName = Path.GetFileName(dirPath);
                if (ExcludedDirectories.Contains(dirName))
                    continue;

                ScanDirectoryRecursive(rootPath, dirPath, files, maxFiles);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have permission to access
        }
        catch (IOException)
        {
            // Skip directories with I/O errors
        }
    }
}
