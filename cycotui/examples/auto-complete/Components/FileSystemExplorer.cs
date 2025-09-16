using System.Collections.Concurrent;
using CycoAI.CycoTui.Examples.AutoComplete.Models;

namespace CycoAI.CycoTui.Examples.AutoComplete.Components;

/// <summary>
/// Provides file system exploration and scanning capabilities for auto-completion.
/// </summary>
public class FileSystemExplorer : IDisposable
{
    private readonly ConcurrentDictionary<string, CachedDirectoryInfo> _directoryCache = new();
    private readonly SemaphoreSlim _scanSemaphore = new(1, 1);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _disposed = false;

    /// <summary>
    /// Maximum depth to scan recursively (to prevent performance issues).
    /// </summary>
    public int MaxScanDepth { get; set; } = 3;

    /// <summary>
    /// Maximum number of items to return (to prevent overwhelming the UI).
    /// </summary>
    public int MaxItems { get; set; } = 100;

    /// <summary>
    /// Whether to include hidden files and directories.
    /// </summary>
    public bool IncludeHidden { get; set; } = false;

    /// <summary>
    /// Whether to scan subdirectories recursively.
    /// </summary>
    public bool RecursiveScan { get; set; } = true;

    /// <summary>
    /// Cache expiration time for directory contents.
    /// </summary>
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets completion items from the file system based on the filter.
    /// </summary>
    /// <param name="filter">The filter text to match against file/directory names.</param>
    /// <param name="basePath">The base path to scan from (defaults to current directory).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of matching completion items.</returns>
    public async Task<List<CompletionItem>> GetCompletionItemsAsync(
        string filter = "",
        string? basePath = null,
        CancellationToken cancellationToken = default)
    {
        if (_disposed)
            return new List<CompletionItem>();

        basePath ??= Directory.GetCurrentDirectory();

        await _scanSemaphore.WaitAsync(cancellationToken);
        try
        {
            var allItems = await ScanDirectoryAsync(basePath, basePath, 0, cancellationToken);
            var filteredItems = FilterItems(allItems, filter);
            var sortedItems = SortItems(filteredItems);

            return sortedItems.Take(MaxItems).ToList();
        }
        finally
        {
            _scanSemaphore.Release();
        }
    }

    /// <summary>
    /// Scans a directory and returns all file and directory items.
    /// </summary>
    /// <param name="directoryPath">The directory to scan.</param>
    /// <param name="depth">Current recursion depth.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of completion items found in the directory.</returns>
    private async Task<List<CompletionItem>> ScanDirectoryAsync(
        string directoryPath,
        string originalBasePath,
        int depth,
        CancellationToken cancellationToken)
    {
        var items = new List<CompletionItem>();

        try
        {
            // Check cache first
            if (_directoryCache.TryGetValue(directoryPath, out var cachedInfo))
            {
                if (DateTime.UtcNow - cachedInfo.LastScanned < CacheExpiration)
                {
                    return cachedInfo.Items.ToList();
                }
            }

            // Check if directory exists and is accessible
            if (!Directory.Exists(directoryPath))
                return items;

            var directoryInfo = new DirectoryInfo(directoryPath);

            // Scan files in current directory
            await Task.Run(() =>
            {
                try
                {
                    foreach (var fileInfo in directoryInfo.EnumerateFiles())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var item = CompletionItem.FromPath(fileInfo.FullName, originalBasePath, depth);

                        // Skip hidden files if not included
                        if (!IncludeHidden && item.IsHidden)
                            continue;

                        items.Add(item);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip directories we can't access
                }
                catch (DirectoryNotFoundException)
                {
                    // Directory was deleted during scan
                }
            }, cancellationToken);

            // Scan subdirectories
            await Task.Run(() =>
            {
                try
                {
                    foreach (var subDirectoryInfo in directoryInfo.EnumerateDirectories())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var item = CompletionItem.FromPath(subDirectoryInfo.FullName, originalBasePath, depth);

                        // Skip hidden directories if not included
                        if (!IncludeHidden && item.IsHidden)
                            continue;

                        items.Add(item);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip directories we can't access
                }
                catch (DirectoryNotFoundException)
                {
                    // Directory was deleted during scan
                }
            }, cancellationToken);

            // Recursively scan subdirectories if enabled and within depth limit
            if (RecursiveScan && depth < MaxScanDepth)
            {
                var subdirectoryItems = items
                    .Where(item => item.Type == CompletionItemType.Directory)
                    .ToList();

                var recursiveTasks = subdirectoryItems.Select(async dirItem =>
                {
                    try
                    {
                        var subItems = await ScanDirectoryAsync(dirItem.FullPath, originalBasePath, depth + 1, cancellationToken);
                        return subItems;
                    }
                    catch
                    {
                        return new List<CompletionItem>();
                    }
                });

                var recursiveResults = await Task.WhenAll(recursiveTasks);
                foreach (var subItems in recursiveResults)
                {
                    items.AddRange(subItems);
                }
            }

            // Cache the results
            _directoryCache[directoryPath] = new CachedDirectoryInfo
            {
                Items = items.ToList(),
                LastScanned = DateTime.UtcNow
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // Log error in a real implementation
            // For now, just return empty list
        }

        return items;
    }

    /// <summary>
    /// Filters completion items based on the filter text.
    /// </summary>
    /// <param name="items">The items to filter.</param>
    /// <param name="filter">The filter text.</param>
    /// <returns>Filtered completion items.</returns>
    private List<CompletionItem> FilterItems(List<CompletionItem> items, string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return items;

        var normalizedFilter = filter.ToLowerInvariant();

        return items
            .Where(item =>
            {
                // Exact match at start gets highest priority
                if (item.Name.ToLowerInvariant().StartsWith(normalizedFilter))
                    return true;

                // Substring match
                if (item.Name.ToLowerInvariant().Contains(normalizedFilter))
                    return true;

                // Path match for nested items
                if (item.RelativePath.ToLowerInvariant().Contains(normalizedFilter))
                    return true;

                return false;
            })
            .ToList();
    }

    /// <summary>
    /// Sorts completion items by relevance and type.
    /// </summary>
    /// <param name="items">The items to sort.</param>
    /// <returns>Sorted completion items.</returns>
    private List<CompletionItem> SortItems(List<CompletionItem> items)
    {
        return items
            .OrderBy(item => item.Depth) // Closer items first
            .ThenBy(item => item.Type == CompletionItemType.Directory ? 0 : 1) // Directories first
            .ThenBy(item => item.Name.ToLowerInvariant()) // Alphabetical
            .ToList();
    }

    /// <summary>
    /// Clears the directory cache.
    /// </summary>
    public void ClearCache()
    {
        _directoryCache.Clear();
    }

    /// <summary>
    /// Refreshes the cache for a specific directory.
    /// </summary>
    /// <param name="directoryPath">The directory path to refresh.</param>
    public void RefreshDirectory(string directoryPath)
    {
        _directoryCache.TryRemove(directoryPath, out _);
    }

    /// <summary>
    /// Gets statistics about the current cache.
    /// </summary>
    /// <returns>Cache statistics.</returns>
    public CacheStatistics GetCacheStatistics()
    {
        var now = DateTime.UtcNow;
        var validEntries = _directoryCache.Values
            .Where(entry => now - entry.LastScanned < CacheExpiration)
            .ToList();

        return new CacheStatistics
        {
            TotalEntries = _directoryCache.Count,
            ValidEntries = validEntries.Count,
            TotalItems = validEntries.Sum(entry => entry.Items.Count),
            CacheHitRatio = _directoryCache.Count > 0 ? (double)validEntries.Count / _directoryCache.Count : 0
        };
    }

    /// <summary>
    /// Disposes of resources used by the FileSystemExplorer.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _scanSemaphore.Dispose();
            _directoryCache.Clear();
            _disposed = true;
        }
    }

    /// <summary>
    /// Information about cached directory contents.
    /// </summary>
    private class CachedDirectoryInfo
    {
        public List<CompletionItem> Items { get; set; } = new();
        public DateTime LastScanned { get; set; }
    }
}

/// <summary>
/// Statistics about the file system cache.
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Total number of cached directory entries.
    /// </summary>
    public int TotalEntries { get; set; }

    /// <summary>
    /// Number of valid (non-expired) cache entries.
    /// </summary>
    public int ValidEntries { get; set; }

    /// <summary>
    /// Total number of cached completion items.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Cache hit ratio (0.0 to 1.0).
    /// </summary>
    public double CacheHitRatio { get; set; }
}