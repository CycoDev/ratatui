using System;
using Xunit;
using CycoAI.CycoTui.Core.Events;

namespace CycoTui.Tests.Events
{
    public class EventTests
    {
        [Fact]
        public void BaseEvent_Constructor_SetsPropertiesCorrectly()
        {
            var eventType = EventType.Key;
            var before = DateTime.UtcNow;
            var testEvent = new TestEvent(eventType);
            var after = DateTime.UtcNow;

            Assert.Equal(eventType, testEvent.Type);
            Assert.False(testEvent.IsHandled);
            Assert.True(testEvent.Timestamp >= before && testEvent.Timestamp <= after);
        }

        [Fact]
        public void BaseEvent_Handle_MarksEventAsHandled()
        {
            var testEvent = new TestEvent(EventType.Key);
            Assert.False(testEvent.IsHandled);

            testEvent.Handle();
            Assert.True(testEvent.IsHandled);
        }

        [Theory]
        [InlineData(10, 20)]
        [InlineData(0, 0)]
        [InlineData(80, 24)]
        public void ResizeEvent_Constructor_SetsPropertiesCorrectly(int width, int height)
        {
            var resizeEvent = new ResizeEvent(width, height);

            Assert.Equal(EventType.Resize, resizeEvent.Type);
            Assert.Equal(width, resizeEvent.Width);
            Assert.Equal(height, resizeEvent.Height);
            Assert.Equal((width, height), resizeEvent.Size);
            Assert.False(resizeEvent.IsHandled);
        }

        [Fact]
        public void ResizeEvent_ToString_ReturnsCorrectFormat()
        {
            var resizeEvent = new ResizeEvent(80, 24);
            var result = resizeEvent.ToString();

            Assert.Equal("ResizeEvent: 80x24", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FocusEvent_Constructor_SetsPropertiesCorrectly(bool gained)
        {
            var focusEvent = new FocusEvent(gained);

            Assert.Equal(EventType.Focus, focusEvent.Type);
            Assert.Equal(gained, focusEvent.Gained);
            Assert.Equal(!gained, focusEvent.Lost);
            Assert.False(focusEvent.IsHandled);
        }

        [Theory]
        [InlineData(true, "FocusEvent: Gained")]
        [InlineData(false, "FocusEvent: Lost")]
        public void FocusEvent_ToString_ReturnsCorrectFormat(bool gained, string expected)
        {
            var focusEvent = new FocusEvent(gained);
            var result = focusEvent.ToString();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Hello World")]
        [InlineData("")]
        [InlineData("A very long string that exceeds the 50 character limit for the preview")]
        public void PasteEvent_Constructor_SetsPropertiesCorrectly(string content)
        {
            var pasteEvent = new PasteEvent(content);

            Assert.Equal(EventType.Paste, pasteEvent.Type);
            Assert.Equal(content, pasteEvent.Content);
            Assert.False(pasteEvent.IsHandled);
        }

        [Fact]
        public void PasteEvent_Constructor_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PasteEvent(null!));
        }

        [Theory]
        [InlineData("Short", "PasteEvent: \"Short\"")]
        [InlineData("This is a very long string that will be truncated", "PasteEvent: \"This is a very long string that will be truncated\"")]
        public void PasteEvent_ToString_ReturnsCorrectFormat(string content, string expected)
        {
            var pasteEvent = new PasteEvent(content);
            var result = pasteEvent.ToString();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Key.A, KeyModifiers.None)]
        [InlineData(Key.Enter, KeyModifiers.Control)]
        [InlineData(Key.F1, KeyModifiers.Alt | KeyModifiers.Shift)]
        public void KeyEvent_Constructor_SetsPropertiesCorrectly(Key key, KeyModifiers modifiers)
        {
            var keyEvent = new KeyEvent(key, modifiers);

            Assert.Equal(EventType.Key, keyEvent.Type);
            Assert.Equal(key, keyEvent.Key);
            Assert.Equal(modifiers, keyEvent.Modifiers);
            Assert.False(keyEvent.IsHandled);
        }

        [Theory]
        [InlineData(Key.A, KeyModifiers.None, 'a', true)]
        [InlineData(Key.A, KeyModifiers.Shift, 'A', true)]
        [InlineData(Key.D1, KeyModifiers.None, '1', true)]
        [InlineData(Key.D1, KeyModifiers.Shift, '!', true)]
        [InlineData(Key.Space, KeyModifiers.None, ' ', true)]
        [InlineData(Key.Enter, KeyModifiers.None, '\n', false)]
        [InlineData(Key.Tab, KeyModifiers.None, '\t', false)]
        public void KeyEvent_Character_ReturnsCorrectCharacter(Key key, KeyModifiers modifiers, char expectedChar, bool expectedPrintable)
        {
            var keyEvent = new KeyEvent(key, modifiers);

            Assert.Equal(expectedChar, keyEvent.Character);
            Assert.Equal(expectedPrintable, keyEvent.IsPrintable);
        }

        [Theory]
        [InlineData(Key.F1)]
        [InlineData(Key.Up)]
        [InlineData(Key.Escape)]
        [InlineData(Key.Delete)]
        public void KeyEvent_NonPrintableKeys_ReturnsNullCharacter(Key key)
        {
            var keyEvent = new KeyEvent(key, KeyModifiers.None);

            Assert.Null(keyEvent.Character);
            Assert.False(keyEvent.IsPrintable);
        }

        [Theory]
        [InlineData(MouseEventKind.Down, MouseButton.Left, 10, 20, KeyModifiers.None)]
        [InlineData(MouseEventKind.ScrollUp, MouseButton.None, 0, 0, KeyModifiers.Control)]
        public void MouseEvent_Constructor_SetsPropertiesCorrectly(MouseEventKind kind, MouseButton button, int x, int y, KeyModifiers modifiers)
        {
            var mouseEvent = new MouseEvent(kind, button, x, y, modifiers);

            Assert.Equal(EventType.Mouse, mouseEvent.Type);
            Assert.Equal(kind, mouseEvent.Kind);
            Assert.Equal(button, mouseEvent.Button);
            Assert.Equal(x, mouseEvent.X);
            Assert.Equal(y, mouseEvent.Y);
            Assert.Equal(modifiers, mouseEvent.Modifiers);
            Assert.Equal((x, y), mouseEvent.Position);
            Assert.False(mouseEvent.IsHandled);
        }

        [Theory]
        [InlineData(MouseEventKind.Down, MouseButton.Left, 10, 20, KeyModifiers.None, "MouseEvent: Down Left at (10, 20)")]
        [InlineData(MouseEventKind.ScrollUp, MouseButton.None, 0, 0, KeyModifiers.Control, "MouseEvent: ScrollUp None at (0, 0) [Control]")]
        public void MouseEvent_ToString_ReturnsCorrectFormat(MouseEventKind kind, MouseButton button, int x, int y, KeyModifiers modifiers, string expected)
        {
            var mouseEvent = new MouseEvent(kind, button, x, y, modifiers);
            var result = mouseEvent.ToString();

            Assert.Equal(expected, result);
        }

        private class TestEvent : BaseEvent
        {
            public TestEvent(EventType type) : base(type) { }
        }
    }
}