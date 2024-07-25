using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerInputReader : MonoBehaviour, IServiceLocatorComponent, IEarlyUpdate, IAwake, IVirtualController
{
    public ServiceLocator MyServiceLocator { get; set; }
    public bool FlipVerticalAxis { get; set; } = false;
    public bool VerticalPouring { get; set; } = false;

    [ServiceLocatorComponent] private InputManager _inputManager;

    public InputManager InputManager => _inputManager;

    public Vector3 Movement 
    {
        get
        {
            Vector3 vector = new Vector3(GetAxis("Horizontal"), 0, GetAxis("Vertical"));
            return vector.normalized;
        }
        set { } 
    }

    public Vector2 Mouse
    {
        get => new Vector2(GetAxis("Mouse X"), GetAxis("Mouse Y") * (FlipVerticalAxis ? -1 : 1));
        set { }
    }

    public Vector3 DesignMovement
    {
        get
        {
            Vector3 vector = new Vector3(GetAxis("Design Move Horizontal"), 0, GetAxis("Design Move Vertical"));
            return vector.normalized;
        }
        set { }
    }

    public Vector2 DesignMouse
    {
        get => new Vector2(GetAxis("Design Mouse X"), GetAxis("Design Mouse X") * (FlipVerticalAxis ? -1 : 1));
        set { }
    }

    public Vector2 GamepadCursor => new Vector2(GetAxis("Gamepad Cursor X"), GetAxis("Gamepad Cursor Y"));
    public Vector2 GamepadScroll => new Vector2(GetAxis("Gamepad Scroll X"), GetAxis("Gamepad Scroll Y"));

    public float HologramRotatingAxis 
    {
        get => GetAxis("Hologram Rotating Axis");
        set { }
    }

    public bool IsSprint 
    {
        get => GetInteraction("Sprint") && !IsWalk;
        set { }
    }

    public bool IsWalk 
    {
        get => GetInteraction("Walk");
        set { }
    }
    public Action OnJumpPerformed { get; set; }
    public Action OnJumpCancelled { get; set; }
    public Action OnFirstInteractionPerformed { get; set; }
    public Action OnSecondInteractionPerformed { get; set; }
    public Action OnAdditiveInteractionPerformed { get; set; }
    public Action OnMoreInfoInteractionPerformed { get; set; }
    public Action OnFirstInteractionCancelled { get; set; }
    public Action OnSecondInteractionCancelled { get; set; }
    public Action OnAdditiveInteractionCancelled { get; set; }
    public Action OnMoreInfoInteractionCancelled { get; set; }
    public Action OnToggleHologramGridSnappingPerformed { get; set; }
    public Action OnPausePerformed { get; set; }
    public Action OnUnpausePerformed { get; set; }
    public Action OnQuickSavePerformed { get; set; }

    public Action OnOpenBookPerformed { get; set; }
    public Action OnCloseBookPerformed { get; set; }

    public Action OnSetInventory0 { get; set; }
    public Action OnSetInventory1 { get; set; }
    public Action OnSetInventory2 { get; set; }
    public Action OnSetInventory3 { get; set; }
    public Action OnSetInventory4 { get; set; }
    public Action OnSetInventory5 { get; set; }
    public Action OnSetInventory6 { get; set; }
    public Action OnSetInventory7 { get; set; }
    public Action OnSetInventory8 { get; set; }
    public Action OnSetInventory9 { get; set; }
    public Action OnScrollInventoryUp { get; set; }
    public Action OnScrollInventoryDown { get; set; }

    public Action OnGamepadUILeftButtonPerformed { get; set; }
    public Action OnGamepadUILeftButtonCancelled { get; set; }
    public Action OnGamepadScrollInventoryUpPerformed { get; set; }
    public Action OnGamepadScrollInventoryDownPerformed { get; set; }

    public Action DesignOnFirstInteractionPerformed { get; set; }
    public Action DesignOnSecondInteractionPerformed { get; set; }
    public Action DesignCancel { get; set; }

    public Action<string> OnButtonDown;
    public Action<string> OnButtonUp;
    public Action<string> OnButton;
    public Action<string, float> OnAxis;

    private Dictionary<string, Action> _buttonsDown;
    private Dictionary<string, Action> _buttonsUp;
    private Dictionary<string, bool> _buttons = new();
    private Dictionary<string, float> _axis = new();

    private static List<string> _buttonsToRead = new()
    {
        "Sprint",
        "Walk", 
    };

    private static List<string> _axisesToRead = new()
    {
        "Mouse Y",
        "Mouse X",
        "Gamepad Cursor X",
        "Gamepad Cursor Y",
        "Gamepad Scroll X",
        "Gamepad Scroll Y",
        "Horizontal",
        "Vertical",
        "Hologram Rotating Axis",
        "Design Move Horizontal",
        "Design Move Vertical",
        "Design Mouse X",
        "Design Mouse Y",

    };

    private bool _isDisabled = false;

    public void CustomAwake()
    {
        _buttonsDown = new Dictionary<string, Action>()
        {
            {"Jump", () => OnJumpPerformed?.Invoke()},
            {"Mouse Left", () => OnFirstInteractionPerformed?.Invoke()},
            {"Mouse Right",()=> OnSecondInteractionPerformed?.Invoke()},
            {"Additive Interaction", ()=>OnAdditiveInteractionPerformed?.Invoke()},
            {"More Info", ()=>OnMoreInfoInteractionPerformed?.Invoke()},
            {"Toggle Hologram Grid Snapping", ()=>OnToggleHologramGridSnappingPerformed?.Invoke()},
            {"Pause", ()=>OnPausePerformed?.Invoke()},
            {"Unpause", ()=>OnUnpausePerformed?.Invoke()},
            {"QuickSave", ()=> OnQuickSavePerformed?.Invoke() },
            {"OpenBook", ()=>OnOpenBookPerformed?.Invoke()},
            {"CloseBook", ()=>OnCloseBookPerformed?.Invoke()},
            {"Set Inventory 0", ()=>OnSetInventory0?.Invoke()},
            {"Set Inventory 1", ()=>OnSetInventory1?.Invoke()},
            {"Set Inventory 2", ()=>OnSetInventory2?.Invoke()},
            {"Set Inventory 3", ()=>OnSetInventory3?.Invoke()},
            {"Set Inventory 4", ()=>OnSetInventory4?.Invoke()},
            {"Set Inventory 5", ()=>OnSetInventory5?.Invoke()},
            {"Set Inventory 6", ()=>OnSetInventory6?.Invoke()},
            {"Set Inventory 7", ()=>OnSetInventory7?.Invoke()},
            {"Set Inventory 8", ()=>OnSetInventory8?.Invoke()},
            {"Set Inventory 9", ()=>OnSetInventory9?.Invoke()},
            {"Scrolling Inventory Up", ()=>OnScrollInventoryUp?.Invoke()},
            {"Scrolling Inventory Down", ()=>OnScrollInventoryDown?.Invoke()},
            {"Gamepad UI Left Button", () => OnGamepadUILeftButtonPerformed?.Invoke() },
            {"Gamepad Scroll Inventory Up", () => OnGamepadScrollInventoryUpPerformed?.Invoke() },
            {"Design Mouse Left", () => DesignOnFirstInteractionPerformed?.Invoke() },
            {"Design Mouse Right", () => DesignOnSecondInteractionPerformed?.Invoke() },
            {"Design Cancel", () => DesignCancel?.Invoke() },
        };

        _buttonsUp = new Dictionary<string, Action>()
        {
            {"Mouse Left", ()=> OnFirstInteractionCancelled?.Invoke()},
            {"Mouse Right", ()=> OnSecondInteractionCancelled?.Invoke()},
            {"Additive Interaction", ()=> OnAdditiveInteractionCancelled?.Invoke()},
            {"More Info", ()=> OnMoreInfoInteractionCancelled?.Invoke()},
            {"Gamepad UI Left Button", ()=> OnGamepadUILeftButtonCancelled?.Invoke()},
            {"Jump", ()=> OnJumpCancelled?.Invoke()}
        };
    }

    public void CustomEarlyUpdate()
    {   
        if (_isDisabled) return;
        ListeningInput();
    }

    private void LateUpdate()
    {
        _buttons.Clear();
        _axis.Clear();
    }

    private void ListeningInput()
    {
        foreach (string axisName in _axisesToRead)
        {
            SetAxis(axisName, GetAxisValue(axisName));
        }

        foreach (string buttonName in _buttonsToRead)
        {
            if (GetButton(buttonName))SimulateButton(buttonName);
        }

        foreach (string buttonName in _buttonsDown.Keys)
        {
            if (GetButtonDown(buttonName)) SimulateButtonDown(buttonName);
        }

        foreach (string buttonName in _buttonsUp.Keys)
        {
            if (GetButtonUp(buttonName)) SimulateButtonUp(buttonName);
        }

        bool GetButtonDown(string button) => _inputManager.PlayerInput.GetButtonDown(button);
        bool GetButtonUp(string button) => _inputManager.PlayerInput.GetButtonUp(button);
        bool GetButton(string button) => _inputManager.PlayerInput.GetButton(button);
        float GetAxisValue(string axis) => _inputManager.PlayerInput.GetAxis(axis);
    }

    public void SimulateButtonDown(string buttonName)
    {
        if (!_buttonsDown.ContainsKey(buttonName))
        {
            Debug.LogError($"There is no declared button with name {buttonName}");
            return;
        }

        _buttonsDown[buttonName]?.Invoke();
        OnButtonDown?.Invoke(buttonName);
    }

    public void SimulateButtonUp(string buttonName)
    {
        if (!_buttonsUp.ContainsKey(buttonName))
        {
            Debug.LogError($"There is no declared button with name {buttonName}");
            return;
        }

        _buttonsUp[buttonName]?.Invoke();
        OnButtonUp?.Invoke(buttonName);
    }

    public void SimulateButton(string buttonName) 
    {
        if (_buttons.ContainsKey(buttonName))_buttons[buttonName] = true;
        else _buttons.Add(buttonName, true);      

        OnButton?.Invoke(buttonName);
    }

    public bool GetInteraction(string axis)
    {
        if (!_buttons.ContainsKey(axis)) return false;
        return _buttons[axis];
    }

    public void SetAxis(string axis, float value)
    {
        if (_axis.ContainsKey(axis))_axis[axis] = value;
        else _axis.Add(axis, value);

        if (axis == "Horizontal" || 
            axis == "Vertical" ||
            axis == "Hologram Rotating Axis") value *= Time.deltaTime;
        OnAxis?.Invoke(axis, value);
    }

    public float GetAxis(string axis)
    {
        if (!_axis.ContainsKey(axis)) return 0;
        return _axis[axis];
    }

    public void ToggleController(bool shouldBeDisabled)
    {
        _isDisabled = shouldBeDisabled;
    }
}