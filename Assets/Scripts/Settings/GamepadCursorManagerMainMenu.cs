using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using Mouse = UnityEngine.InputSystem.Mouse;

public class GamepadCursorManagerMainMenu : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

    public Action<PlayerServiceLocator> OnLocalPlayerSet;
    public Vector2 GamepadCursorPosition => _gamepadCursorPosition;
    [field: SerializeField] public float CursorSensitivity { get; set; } = 10f;
    [field: SerializeField] public float ScrollSensitivity { get; set; } = 50f;

    [SerializeField] private PlayerInputReader _playerInputReader;
    
    private Vector2 _gamepadCursorPosition;

    public void CustomAwake()
    {
        _playerInputReader.OnGamepadUILeftButtonPerformed += () => SimulateUILeftPressed(true);
        _playerInputReader.OnGamepadUILeftButtonCancelled += () => SimulateUILeftPressed(false);
    }

    private void Update()
    {
        HandleGamepadCursorMovement();
        SimulateUIScroll();
    }

    private void HandleGamepadCursorMovement()
    {
        if (ReInput.controllers.GetLastActiveController().type != ControllerType.Joystick) return;

        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        _gamepadCursorPosition = currentMousePosition;
        _gamepadCursorPosition += _playerInputReader.GamepadCursor * CursorSensitivity;
        Mouse.current.WarpCursorPosition(_gamepadCursorPosition);
        
    }

    private void SimulateUIScroll()
    {
        if (ReInput.controllers.GetLastActiveController().type != ControllerType.Joystick) return;

        Vector2 scroll = _playerInputReader.GamepadScroll * ScrollSensitivity;

        if (scroll == Vector2.zero)
            return;

        using (StateEvent.From(Mouse.current, out var eventPtr))
        {
            Mouse.current.scroll.WriteValueIntoEvent(scroll, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }

    private void SimulateUILeftPressed(bool shouldBeDown)
    {
        if (ReInput.controllers.GetLastActiveController().type != ControllerType.Joystick) return;

        using (StateEvent.From(Mouse.current,out var eventPtr))
        {
            Mouse.current.press.WriteValueIntoEvent( shouldBeDown ? 1f : 0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }
}
