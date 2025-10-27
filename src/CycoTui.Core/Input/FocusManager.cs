using System.Collections.Generic;
using System.Linq;

namespace CycoTui.Core.Input;

/// <summary>
/// Manages focus traversal among focusable widgets.
/// </summary>
public sealed class FocusManager
{
    private readonly List<IFocusableWidget> _widgets = new();
    private int _index = -1;

    public IReadOnlyList<IFocusableWidget> Widgets => _widgets;
    public IFocusableWidget? Current => _index >=0 && _index < _widgets.Count ? _widgets[_index] : null;

    public void Register(IFocusableWidget widget)
    {
        if (widget.CanFocus)
        {
            _widgets.Add(widget);
            if (_index == -1) SetIndex(0);
        }
    }

    public void Next()
    {
        if (_widgets.Count == 0) return;
        int next = (_index + 1) % _widgets.Count;
        SetIndex(next);
    }

    public void Previous()
    {
        if (_widgets.Count == 0) return;
        int prev = (_index - 1 + _widgets.Count) % _widgets.Count;
        SetIndex(prev);
    }

    public void SetIndex(int idx)
    {
        if (idx < 0 || idx >= _widgets.Count) return;
        var old = Current;
        if (old != null) old.OnFocusLost();
        _index = idx;
        var nw = Current;
        if (nw != null) nw.OnFocusGained();
    }

    public void SetWidget(IFocusableWidget widget)
    {
        var idx = _widgets.IndexOf(widget);
        if (idx >= 0) SetIndex(idx);
    }
}
