namespace CycoAI.CycoTui.Examples.AutoComplete.Models;

/// <summary>
/// Represents a file or directory completion item.
/// </summary>
public class CompletionItem
{
    /// <summary>
    /// The display name of the file or directory.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The full path to the file or directory.
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// The relative path from the current working directory.
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// The type of the completion item.
    /// </summary>
    public CompletionItemType Type { get; set; } = CompletionItemType.File;

    /// <summary>
    /// The size of the file in bytes (0 for directories).
    /// </summary>
    public long Size { get; set; } = 0;

    /// <summary>
    /// The last modified time of the file or directory.
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Whether this item is hidden (starts with dot on Unix or has hidden attribute on Windows).
    /// </summary>
    public bool IsHidden { get; set; } = false;

    /// <summary>
    /// The depth level from the scanning root (0 = direct child, 1 = grandchild, etc.).
    /// </summary>
    public int Depth { get; set; } = 0;

    /// <summary>
    /// Gets the display text for this completion item.
    /// </summary>
    public string DisplayText
    {
        get
        {
            var prefix = Type switch
            {
                CompletionItemType.Directory => "📁 ",
                CompletionItemType.File => GetFileIcon(),
                _ => "   "
            };

            var suffix = Type == CompletionItemType.Directory ? "/" : "";
            var displayName = string.IsNullOrEmpty(RelativePath) ? Name : RelativePath;
            return $"{prefix}{displayName}{suffix}";
        }
    }

    /// <summary>
    /// Gets the display text without emojis for UI display.
    /// </summary>
    public string SimpleDisplayText
    {
        get
        {
            var suffix = Type == CompletionItemType.Directory ? "/" : "";
            var displayName = string.IsNullOrEmpty(RelativePath) ? Name : RelativePath;
            return $"{displayName}{suffix}";
        }
    }

    /// <summary>
    /// Gets the text that should be inserted when this completion is selected.
    /// </summary>
    public string CompletionText
    {
        get
        {
            // Use relative path for completion
            var path = RelativePath;

            // Normalize path separators for the current platform
            path = path.Replace('\\', Path.DirectorySeparatorChar)
                      .Replace('/', Path.DirectorySeparatorChar);

            // Add trailing separator for directories
            if (Type == CompletionItemType.Directory && !path.EndsWith(Path.DirectorySeparatorChar))
                path += Path.DirectorySeparatorChar;

            return path;
        }
    }

    /// <summary>
    /// Gets a file icon based on the file extension.
    /// </summary>
    private string GetFileIcon()
    {
        var extension = Path.GetExtension(Name).ToLowerInvariant();

        return extension switch
        {
            ".txt" or ".md" or ".readme" => "📄 ",
            ".cs" or ".vb" or ".fs" => "🔷 ",
            ".js" or ".ts" or ".jsx" or ".tsx" => "🟨 ",
            ".py" or ".pyw" => "🐍 ",
            ".java" or ".kt" or ".scala" => "☕ ",
            ".cpp" or ".c" or ".h" or ".hpp" => "🔧 ",
            ".rs" => "🦀 ",
            ".go" => "🐹 ",
            ".html" or ".htm" or ".xml" => "🌐 ",
            ".css" or ".scss" or ".sass" or ".less" => "🎨 ",
            ".json" or ".yaml" or ".yml" or ".toml" => "⚙️ ",
            ".sql" or ".db" or ".sqlite" => "🗄️ ",
            ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".ico" => "🖼️ ",
            ".mp3" or ".wav" or ".flac" or ".ogg" => "🎵 ",
            ".mp4" or ".avi" or ".mkv" or ".mov" => "🎬 ",
            ".pdf" => "📕 ",
            ".zip" or ".7z" or ".rar" or ".tar" or ".gz" => "📦 ",
            ".exe" or ".msi" or ".app" or ".deb" or ".rpm" => "⚡ ",
            ".sh" or ".bat" or ".cmd" or ".ps1" => "📜 ",
            ".log" => "📋 ",
            ".config" or ".ini" or ".conf" => "⚙️ ",
            _ => "📄 "
        };
    }

    /// <summary>
    /// Creates a completion item from a file system entry.
    /// </summary>
    /// <param name="fullPath">The full path to the file or directory.</param>
    /// <param name="basePath">The base path to calculate relative path from.</param>
    /// <param name="depth">The depth level from the scanning root.</param>
    /// <returns>A new completion item.</returns>
    public static CompletionItem FromPath(string fullPath, string basePath, int depth = 0)
    {
        var info = new FileInfo(fullPath);
        var isDirectory = Directory.Exists(fullPath);

        // Calculate relative path
        var relativePath = Path.GetRelativePath(basePath, fullPath);

        var item = new CompletionItem
        {
            Name = Path.GetFileName(fullPath),
            FullPath = fullPath,
            RelativePath = relativePath,
            Type = isDirectory ? CompletionItemType.Directory : CompletionItemType.File,
            Depth = depth,
            IsHidden = IsHiddenPath(fullPath)
        };

        try
        {
            if (isDirectory)
            {
                var dirInfo = new DirectoryInfo(fullPath);
                item.LastModified = dirInfo.LastWriteTime;
            }
            else if (info.Exists)
            {
                item.Size = info.Length;
                item.LastModified = info.LastWriteTime;
            }
        }
        catch
        {
            // Ignore errors getting file info (permissions, etc.)
        }

        return item;
    }

    /// <summary>
    /// Determines if a path represents a hidden file or directory.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns>True if the path is hidden.</returns>
    private static bool IsHiddenPath(string path)
    {
        var name = Path.GetFileName(path);

        // Unix-style hidden files (start with dot)
        if (name.StartsWith('.'))
            return true;

        try
        {
            // Windows-style hidden files (hidden attribute)
            var attributes = File.GetAttributes(path);
            return (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Enumeration of completion item types.
/// </summary>
public enum CompletionItemType
{
    /// <summary>
    /// A regular file.
    /// </summary>
    File,

    /// <summary>
    /// A directory.
    /// </summary>
    Directory
}