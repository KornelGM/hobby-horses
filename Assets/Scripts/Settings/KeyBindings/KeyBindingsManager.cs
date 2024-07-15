using System.Collections.Generic;
using I2.Loc;
using Rewired;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyBindingsManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private KeyBindingSettingAssetCategory[] _keyBindingSettingAssetCategories;
    [SerializeField] private KeyBindingSetting _keybindingPrefab;
    [SerializeField] private KeyBindingCategory _keybindingCategoryPrefab;
    [SerializeField] private GameObject _waitingForInputWindow;

    [SerializeField] private Transform _keyboardAndMouseSettingsHolder;
    [SerializeField] private Transform _gamepeadSettingsHolder;
    [SerializeField] private ScrollRect _bindingsScrollRect;

    [SerializeField] private LocalizedString _resetSettingsModalTitle;

    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
    private List<KeyBindingSetting> _keyBindingSettings = new();
    private Player _player;

    private void Awake()
    {
        foreach (var category in _keyBindingSettingAssetCategories)
        {
            KeyBindingCategory instanceGamepad = Instantiate(_keybindingCategoryPrefab, _gamepeadSettingsHolder);
            KeyBindingCategory instanceKeyboardMouse =
                Instantiate(_keybindingCategoryPrefab, _keyboardAndMouseSettingsHolder);

            instanceGamepad.Initialize(category.DisplayedName);
            instanceKeyboardMouse.Initialize(category.DisplayedName);

            foreach (var asset in category.KeyBindingSettingsAssets)
            {
                if (asset.ControllerType == BindingControllerType.Both)
                {
                    KeyBindingSetting settingGamepad = Instantiate(_keybindingPrefab, _gamepeadSettingsHolder);
                    _keyBindingSettings.Add(settingGamepad);
                    settingGamepad.Initialize(this, category.CategoryId, asset, BindingControllerType.Gamepad);

                    KeyBindingSetting settingKeyboardMouse =
                        Instantiate(_keybindingPrefab, _keyboardAndMouseSettingsHolder);
                    _keyBindingSettings.Add(settingKeyboardMouse);
                    settingKeyboardMouse.Initialize(this, category.CategoryId, asset,
                        BindingControllerType.KeyboardAndMouse);
                }
                else
                {
                    Transform holderToSpawnIn = asset.ControllerType == BindingControllerType.Gamepad
                        ? _gamepeadSettingsHolder
                        : _keyboardAndMouseSettingsHolder;
                    KeyBindingSetting setting = Instantiate(_keybindingPrefab, holderToSpawnIn);
                    _keyBindingSettings.Add(setting);
                    setting.Initialize(this, category.CategoryId, asset, asset.ControllerType);
                }
            }
        }

        _player = ReInput.players.GetPlayer(0);
    }

    private void OnEnable()
    {
        HandleProperControllerSettingsActivation();
        InputSystem.onDeviceChange += DeviceChanged;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= DeviceChanged;
    }

    private void DeviceChanged(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
    {
        UpdateAllKeyBindingLabels();
    }

    private void Update()
    {
        HandleProperControllerSettingsActivation();
    }

    private void HandleProperControllerSettingsActivation()
    {
        var currentControllerType = ReInput.controllers.GetLastActiveController().type;
        if (currentControllerType == ControllerType.Joystick)
        {
            _keyboardAndMouseSettingsHolder.gameObject.SetActive(false);
            _gamepeadSettingsHolder.gameObject.SetActive(true);
            _bindingsScrollRect.content = _gamepeadSettingsHolder as RectTransform;
            _keyBindingSettings.ForEach(binding => binding.UpdateKeybindingsLabels());
        }
        else
        {
            _bindingsScrollRect.content = _keyboardAndMouseSettingsHolder as RectTransform;
            _keyboardAndMouseSettingsHolder.gameObject.SetActive(true);
            _gamepeadSettingsHolder.gameObject.SetActive(false);
        }
    }

    public void UpdateAllKeyBindingLabels()
    {
        _keyBindingSettings.ForEach(setting => setting.UpdateKeybindingsLabels());
    }

    public void StartMappingButtonsLock(LocalizedString title)
    {
        _keyBindingSettings.ForEach(setting => setting.SetKeybindingButtonsInteractable(false));
        _waitingForInputWindow.SetActive(true);
    }

    public void EndMappingButtonsLock()
    {
        _keyBindingSettings.ForEach(setting => setting.SetKeybindingButtonsInteractable(true));
        _waitingForInputWindow.SetActive(false);
        UpdateAllKeyBindingLabels();
    }

    public void ResetCurrentControllerKeybindings()
    {
        _modalWindowManager.CreateModalWindowYesNo(() =>
        {
            var currentControllerType = ReInput.controllers.GetLastActiveController().type;
            if (currentControllerType == ControllerType.Joystick)
            {
                ResetJoystickKeybindings();
            }
            else
            {
                ResetKeyboardAndMouseKeybindings();
            }
        }, () => { },
            _resetSettingsModalTitle);
    }

    private void ResetKeyboardAndMouseKeybindings()
    {
        _player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
        _player.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);

        foreach (var keyBindingSetting in
                 _keyboardAndMouseSettingsHolder.GetComponentsInChildren<KeyBindingSetting>())
            keyBindingSetting.UpdateKeybindingsLabels();

#if !UNITY_EDITOR
        ReInput.userDataStore.Save();
#endif
    }

    private void ResetJoystickKeybindings()
    {
        _player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
        foreach (var keyBindingSetting in _gamepeadSettingsHolder.GetComponentsInChildren<KeyBindingSetting>())
            keyBindingSetting.UpdateKeybindingsLabels();

#if !UNITY_EDITOR
        ReInput.userDataStore.Save();
#endif
    }
}

