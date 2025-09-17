namespace CycoTui {
    // Interface for terminal backend functionality
    public interface ITerminalBackend {
        void Initialize();
        void ClearScreen();
        void SetCursorPosition(int left, int top);
        void Write(string text);
    }
}