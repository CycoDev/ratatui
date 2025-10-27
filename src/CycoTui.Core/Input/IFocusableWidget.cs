namespace CycoTui.Core.Input;

public interface IFocusableWidget
{
    bool CanFocus { get; }
    void OnFocusGained();
    void OnFocusLost();
}
