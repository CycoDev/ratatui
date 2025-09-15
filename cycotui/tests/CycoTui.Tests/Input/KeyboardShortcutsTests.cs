using System;
using System.Collections.Generic;
using Xunit;
using CycoAI.CycoTui.Core.Events;
using CycoAI.CycoTui.Core.Input;

namespace CycoTui.Tests.Input
{
    public class KeyboardShortcutsTests
    {
        [Theory]
        [InlineData(Key.A, KeyModifiers.None)]
        [InlineData(Key.Enter, KeyModifiers.Control)]
        [InlineData(Key.F1, KeyModifiers.Alt | KeyModifiers.Shift)]
        public void KeyboardShortcut_Constructor_SetsPropertiesCorrectly(Key key, KeyModifiers modifiers)
        {
            var shortcut = new KeyboardShortcut(key, modifiers);

            Assert.Equal(key, shortcut.Key);
            Assert.Equal(modifiers, shortcut.Modifiers);
        }

        [Fact]
        public void KeyboardShortcut_FromKeyEvent_CreatesCorrectShortcut()
        {
            var keyEvent = new KeyEvent(Key.C, KeyModifiers.Control);
            var shortcut = KeyboardShortcut.FromKeyEvent(keyEvent);

            Assert.Equal(Key.C, shortcut.Key);
            Assert.Equal(KeyModifiers.Control, shortcut.Modifiers);
        }

        [Theory]
        [InlineData(Key.A, KeyModifiers.None, Key.A, KeyModifiers.None, true)]
        [InlineData(Key.A, KeyModifiers.Control, Key.A, KeyModifiers.Control, true)]
        [InlineData(Key.A, KeyModifiers.None, Key.B, KeyModifiers.None, false)]
        [InlineData(Key.A, KeyModifiers.None, Key.A, KeyModifiers.Control, false)]
        public void KeyboardShortcut_Matches_ReturnsCorrectResult(
            Key shortcutKey, KeyModifiers shortcutModifiers,
            Key eventKey, KeyModifiers eventModifiers,
            bool expectedMatch)
        {
            var shortcut = new KeyboardShortcut(shortcutKey, shortcutModifiers);
            var keyEvent = new KeyEvent(eventKey, eventModifiers);

            var result = shortcut.Matches(keyEvent);

            Assert.Equal(expectedMatch, result);
        }

        [Theory]
        [InlineData(Key.A, KeyModifiers.None, "A")]
        [InlineData(Key.C, KeyModifiers.Control, "Control+C")]
        [InlineData(Key.F1, KeyModifiers.Alt | KeyModifiers.Shift, "Shift, Alt+F1")]
        public void KeyboardShortcut_ToString_ReturnsCorrectFormat(Key key, KeyModifiers modifiers, string expected)
        {
            var shortcut = new KeyboardShortcut(key, modifiers);
            var result = shortcut.ToString();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void KeyboardShortcut_Equals_WorksCorrectly()
        {
            var shortcut1 = new KeyboardShortcut(Key.A, KeyModifiers.Control);
            var shortcut2 = new KeyboardShortcut(Key.A, KeyModifiers.Control);
            var shortcut3 = new KeyboardShortcut(Key.B, KeyModifiers.Control);

            Assert.Equal(shortcut1, shortcut2);
            Assert.NotEqual(shortcut1, shortcut3);
            Assert.True(shortcut1 == shortcut2);
            Assert.True(shortcut1 != shortcut3);
        }

        [Fact]
        public void KeyboardShortcut_GetHashCode_IsConsistent()
        {
            var shortcut1 = new KeyboardShortcut(Key.A, KeyModifiers.Control);
            var shortcut2 = new KeyboardShortcut(Key.A, KeyModifiers.Control);

            Assert.Equal(shortcut1.GetHashCode(), shortcut2.GetHashCode());
        }

        [Fact]
        public void KeyboardShortcut_CommonShortcuts_AreDefinedCorrectly()
        {
            Assert.Equal(new KeyboardShortcut(Key.C, KeyModifiers.Control), KeyboardShortcut.CtrlC);
            Assert.Equal(new KeyboardShortcut(Key.V, KeyModifiers.Control), KeyboardShortcut.CtrlV);
            Assert.Equal(new KeyboardShortcut(Key.Escape), KeyboardShortcut.Escape);
            Assert.Equal(new KeyboardShortcut(Key.Tab, KeyModifiers.Shift), KeyboardShortcut.ShiftTab);
        }

        [Fact]
        public void ShortcutContext_Constructor_SetsPropertiesCorrectly()
        {
            var keyEvent = new KeyEvent(Key.A);
            var data = new Dictionary<string, object> { { "test", "value" } };

            var context = new ShortcutContext(keyEvent, data);

            Assert.Same(keyEvent, context.KeyEvent);
            Assert.Same(data, context.Data);
        }

        [Fact]
        public void ShortcutContext_WithNullData_UsesEmptyDictionary()
        {
            var keyEvent = new KeyEvent(Key.A);

            var context = new ShortcutContext(keyEvent);

            Assert.Same(keyEvent, context.KeyEvent);
            Assert.Empty(context.Data);
        }

        [Fact]
        public void DelegateShortcutAction_WithFuncAction_ExecutesCorrectly()
        {
            var executed = false;
            var action = new DelegateShortcutAction(
                ctx => { executed = true; return true; },
                "Test action");

            var keyEvent = new KeyEvent(Key.A);
            var context = new ShortcutContext(keyEvent);
            var result = action.Execute(context);

            Assert.True(result);
            Assert.True(executed);
            Assert.Equal("Test action", action.Description);
        }

        [Fact]
        public void DelegateShortcutAction_WithSimpleAction_ExecutesCorrectly()
        {
            var executed = false;
            var action = new DelegateShortcutAction(
                () => executed = true,
                "Test action");

            var keyEvent = new KeyEvent(Key.A);
            var context = new ShortcutContext(keyEvent);
            var result = action.Execute(context);

            Assert.True(result);
            Assert.True(executed);
            Assert.Equal("Test action", action.Description);
        }

        [Fact]
        public void DelegateShortcutAction_WithNullAction_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DelegateShortcutAction((Func<IShortcutContext, bool>)null!, "Test"));

            Assert.Throws<ArgumentNullException>(() =>
                new DelegateShortcutAction((Action)null!, "Test"));
        }

        [Fact]
        public void KeyboardShortcutManager_RegisterShortcut_WithAction_RegistersCorrectly()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A, KeyModifiers.Control);
            var executed = false;
            var action = new DelegateShortcutAction(() => executed = true, "Test");

            manager.RegisterShortcut(shortcut, action, "test-shortcut");

            var keyEvent = new KeyEvent(Key.A, KeyModifiers.Control);
            var result = manager.ProcessKeyEvent(keyEvent);

            Assert.True(result);
            Assert.True(executed);

            var retrievedShortcut = manager.GetShortcut("test-shortcut");
            Assert.Equal(shortcut, retrievedShortcut);
        }

        [Fact]
        public void KeyboardShortcutManager_RegisterShortcut_WithDelegate_RegistersCorrectly()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A, KeyModifiers.Control);
            var executed = false;

            manager.RegisterShortcut(shortcut, () => executed = true, "Test action");

            var keyEvent = new KeyEvent(Key.A, KeyModifiers.Control);
            var result = manager.ProcessKeyEvent(keyEvent);

            Assert.True(result);
            Assert.True(executed);
        }

        [Fact]
        public void KeyboardShortcutManager_RegisterShortcut_WithNullAction_ThrowsArgumentNullException()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A);

            Assert.Throws<ArgumentNullException>(() =>
                manager.RegisterShortcut(shortcut, (IShortcutAction)null!));
        }

        [Fact]
        public void KeyboardShortcutManager_UnregisterShortcut_WithAction_RemovesAction()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A);
            var action = new DelegateShortcutAction(() => { }, "Test");

            manager.RegisterShortcut(shortcut, action);
            var result = manager.UnregisterShortcut(shortcut, action);

            Assert.True(result);

            var keyEvent = new KeyEvent(Key.A);
            var processResult = manager.ProcessKeyEvent(keyEvent);
            Assert.False(processResult);
        }

        [Fact]
        public void KeyboardShortcutManager_UnregisterShortcut_WithShortcut_RemovesAllActions()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A);

            manager.RegisterShortcut(shortcut, () => { }, "Action 1");
            manager.RegisterShortcut(shortcut, () => { }, "Action 2");

            var result = manager.UnregisterShortcut(shortcut);

            Assert.True(result);

            var keyEvent = new KeyEvent(Key.A);
            var processResult = manager.ProcessKeyEvent(keyEvent);
            Assert.False(processResult);
        }

        [Fact]
        public void KeyboardShortcutManager_UnregisterShortcut_WithName_RemovesShortcut()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A);

            manager.RegisterShortcut(shortcut, () => { }, "Test", "test-shortcut");

            var result = manager.UnregisterShortcut("test-shortcut");

            Assert.True(result);
            Assert.Null(manager.GetShortcut("test-shortcut"));
        }

        [Fact]
        public void KeyboardShortcutManager_ProcessKeyEvent_WithMultipleActions_ExecutesAll()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A);
            var executed1 = false;
            var executed2 = false;

            manager.RegisterShortcut(shortcut, () => executed1 = true, "Action 1");
            manager.RegisterShortcut(shortcut, () => executed2 = true, "Action 2");

            var keyEvent = new KeyEvent(Key.A);
            var result = manager.ProcessKeyEvent(keyEvent);

            Assert.True(result);
            Assert.True(executed1);
            Assert.True(executed2);
        }

        [Fact]
        public void KeyboardShortcutManager_ProcessKeyEvent_WithNoMatchingShortcut_ReturnsFalse()
        {
            var manager = new KeyboardShortcutManager();
            var keyEvent = new KeyEvent(Key.A);

            var result = manager.ProcessKeyEvent(keyEvent);

            Assert.False(result);
        }

        [Fact]
        public void KeyboardShortcutManager_GetAllShortcuts_ReturnsAllRegistered()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut1 = new KeyboardShortcut(Key.A);
            var shortcut2 = new KeyboardShortcut(Key.B);

            manager.RegisterShortcut(shortcut1, () => { }, "Action 1");
            manager.RegisterShortcut(shortcut2, () => { }, "Action 2");

            var shortcuts = manager.GetAllShortcuts();

            Assert.Equal(2, shortcuts.Count);
            Assert.Contains(shortcut1, shortcuts);
            Assert.Contains(shortcut2, shortcuts);
        }

        [Fact]
        public void KeyboardShortcutManager_GetActions_ReturnsCorrectActions()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A);
            var action1 = new DelegateShortcutAction(() => { }, "Action 1");
            var action2 = new DelegateShortcutAction(() => { }, "Action 2");

            manager.RegisterShortcut(shortcut, action1);
            manager.RegisterShortcut(shortcut, action2);

            var actions = manager.GetActions(shortcut);

            Assert.Equal(2, actions.Count);
            Assert.Contains(action1, actions);
            Assert.Contains(action2, actions);
        }

        [Fact]
        public void KeyboardShortcutManager_Clear_RemovesAllShortcuts()
        {
            var manager = new KeyboardShortcutManager();
            var shortcut = new KeyboardShortcut(Key.A);

            manager.RegisterShortcut(shortcut, () => { }, "Test", "test-shortcut");
            manager.Clear();

            var shortcuts = manager.GetAllShortcuts();
            Assert.Empty(shortcuts);

            var namedShortcut = manager.GetShortcut("test-shortcut");
            Assert.Null(namedShortcut);
        }
    }
}