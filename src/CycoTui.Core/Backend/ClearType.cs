namespace CycoTui.Core.Backend;

/// <summary>
/// Indicates which region should be cleared when invoking <see cref="ITerminalBackend.Clear"/>.
/// </summary>
public enum ClearType
{
    /// <summary>Entire screen.</summary>
    All,
    /// <summary>All content after (below/right of) cursor.</summary>
    AfterCursor,
    /// <summary>All content before (above/left of) cursor.</summary>
    BeforeCursor,
    /// <summary>The current line.</summary>
    CurrentLine,
    /// <summary>From cursor position to end of line.</summary>
    UntilNewLine
}
