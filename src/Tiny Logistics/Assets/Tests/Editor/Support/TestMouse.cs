using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Tests.Editor.Support {
    internal sealed class TestMouse : IDisposable {
        private readonly InputSettings.BackgroundBehavior _background = InputSystem.settings.backgroundBehavior;
        private readonly InputSettings.EditorInputBehaviorInPlayMode _editorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
        private readonly Mouse _previousMouse = Mouse.current;
        private readonly Mouse _mouse;

        public TestMouse() {
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            _mouse = InputSystem.AddDevice<Mouse>("TinyLogisticsTestMouse");
            _mouse.MakeCurrent();
        }

        public void MoveTo(Vector2 position) => InputSystem.QueueStateEvent(_mouse, new MouseState { position = position });

        public void PressAt(Vector2 position) => InputSystem.QueueStateEvent(_mouse, new MouseState { position = position }.WithButton(MouseButton.Left));

        public void Dispose() {
            if (_mouse.added) {
                InputSystem.RemoveDevice(_mouse);
            }

            if (_previousMouse != null && _previousMouse.added) {
                _previousMouse.MakeCurrent();
            }

            InputSystem.settings.backgroundBehavior = _background;
            InputSystem.settings.editorInputBehaviorInPlayMode = _editorInput;
        }
    }
}
