using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using I2.Loc;
using LMirman.RewiredGlyphs;
using Rewired;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindingSetting : MonoBehaviour, IServiceLocatorComponent
{
    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
    [ServiceLocatorComponent] private CursorManager _cursorManager;

    /// <summary>
    ///  Assigment buttons are assigned specific index based on its type
    ///  <para/> 0 - Button for Keyboard
    ///  <para/> 1 - Button for Mouse
    ///  <para/> 2 - Button for Joystick
    /// </summary>
    /// 
    [SerializeField] private GameObject[] _assignmentButtonsHolders = new GameObject[3];
    [SerializeField] private Button[] _assignementButtons = new Button[3];
    [SerializeField] private Image[] _buttonLabels = new Image[3];
    [SerializeField] private TextMeshProUGUI _keyButtonLabelText;
    [SerializeField] private Localize _label;

    [SerializeField, HideInEditorMode, ReadOnly] private KeyBindingSettingAsset _settingAsset;
    [SerializeField] private LocalizedString _overrideModalTitle;

    private KeyBindingsManager _keyBindingsManager;
    private Player _player;
    private int _categoryId;
    private bool _isMapping = false;
    private bool _initialized = false;
    private BindingControllerType _controllerType;

    public void Initialize(KeyBindingsManager manager, int categoryId, KeyBindingSettingAsset asset, BindingControllerType controllerType)
    {
        _keyBindingsManager = manager;
        _categoryId = categoryId;
        _settingAsset = asset;
        _controllerType = controllerType;
        _player = ReInput.players.GetPlayer(0);
        _label.Term = asset.DisplayedName.mTerm;

        UpdateKeybindingsLabels();

        _initialized = true;
    }

    private void OnEnable()
    {
        if (_initialized)
            UpdateKeybindingsLabels();
    }

    public void UpdateKeybindingsLabels()
    {
        _assignmentButtonsHolders.ForEach(holder => holder.SetActive(false));

        if (_controllerType == BindingControllerType.KeyboardAndMouse)
        {
            UpdateLabelForGivenController(ControllerType.Keyboard);
            UpdateLabelForGivenController(ControllerType.Mouse);
            return;
        }

        if (_controllerType == BindingControllerType.Gamepad)
        {
            UpdateLabelForGivenController(ControllerType.Joystick);
            return;
        }
    }

    private void UpdateLabelForGivenController(ControllerType controllerType)
    {
        var controllerMap = _player.controllers.maps.GetAllMaps().FirstOrDefault(map => map.controllerType == controllerType);

        var elements = controllerMap?.GetElementMapsWithAction(_settingAsset.ActionId);
        Image buttonLabel = _buttonLabels[ControllerTypeToButtonIndex(controllerType)];
        GameObject buttonHolder = _assignmentButtonsHolders[ControllerTypeToButtonIndex(controllerType)];
        buttonHolder.SetActive(true);

        if (elements == null || elements.Length == 0)
        {
            buttonLabel.gameObject.SetActive(false);
            return;
        }
        buttonLabel.gameObject.SetActive(true);

        Pole contributionAxis = _settingAsset.IsAxis && _settingAsset.AxisRange == AxisRange.Negative
            ? Pole.Negative
            : Pole.Positive;

        Glyph glyphToDisplay;
        if (controllerType == ControllerType.Keyboard)
        {
            glyphToDisplay = InputGlyphs.GetKeyboardGlyph(_settingAsset.ActionId, contributionAxis, out _);

            _keyButtonLabelText.text = "";

            if (glyphToDisplay.FullSprite == InputGlyphs.NullGlyph.FullSprite)
            {
                KeyCode? keyCode = elements.FirstOrDefault(element =>
                    element.axisContribution == AxisToPole(_settingAsset.AxisRange)
                )?.keyCode;

                if (keyCode != null && !string.IsNullOrEmpty(keyCode.ToString()))
                    _keyButtonLabelText.text = KeyboardStringsUpdater.Keys[keyCode.Value];

                if (string.IsNullOrEmpty(keyCode.ToString()))
                {
                    glyphToDisplay = InputGlyphs.UnboundGlyph;
                }
            }
        }
        else if (controllerType == ControllerType.Mouse)
            glyphToDisplay = InputGlyphs.GetMouseGlyph(_settingAsset.ActionId, contributionAxis, out _);
        else
            glyphToDisplay = InputGlyphs.GetJoystickGlyph(_settingAsset.ActionId, ReInput.controllers.GetLastActiveController<Joystick>(), contributionAxis, out _);

        buttonLabel.sprite = glyphToDisplay?.GetSprite(_settingAsset.AxisRange);

        if (buttonLabel.sprite == null)
        {
            buttonLabel.gameObject.SetActive(false);
        }

        RectTransform rectTransform = buttonLabel.GetComponent<RectTransform>();

        if (buttonLabel.sprite != null)
        {
            float imageWidth = buttonLabel.sprite.rect.width * rectTransform.sizeDelta.y / buttonLabel.sprite.rect.height;
            rectTransform.sizeDelta = new(imageWidth, rectTransform.sizeDelta.y);
            buttonLabel.rectTransform.anchoredPosition = Vector2.zero;
        }

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    public void TryToAssignNewButton(int assigmentButtonIndex)
    {
        if (_isMapping) return;

        var controllerMap = _player.controllers.maps.GetAllMaps((ControllerType)assigmentButtonIndex).First();
        _isMapping = true;
        _keyBindingsManager.StartMappingButtonsLock(_settingAsset.DisplayedName.mTerm);
        InputMapper.Context context = new()
        {
            actionId = _settingAsset.ActionId,
            controllerMap = controllerMap,
            actionRange = _settingAsset.AxisRange,
        };

        if (_settingAsset.IsAxis)
        {
            context.actionElementMapToReplace = controllerMap
                .GetElementMapsWithAction(_settingAsset.ActionId, false)
                .FirstOrDefault(element => element.axisContribution == AxisToPole(_settingAsset.AxisRange));
        }
        else
            context.actionElementMapToReplace = controllerMap.GetFirstElementMapWithAction(_settingAsset.ActionId);

        _cursorManager.DeactivateCursor();

        InputMapper inputMapper = new();
        inputMapper.options.ignoreMouseXAxis = true;
        inputMapper.options.ignoreMouseYAxis = true;
        inputMapper.options.timeout = 5f;
        inputMapper.options.checkForConflictsWithSystemPlayer = false;
        inputMapper.options.checkForConflicts = _controllerType != BindingControllerType.Gamepad;
        inputMapper.options.holdDurationToMapKeyboardModifierKeyAsPrimary = 0;
        inputMapper.Start(context);
        inputMapper.InputMappedEvent += OnInputMapped;
        inputMapper.ConflictFoundEvent += OnMappingConflict;
        inputMapper.TimedOutEvent += TimedOut;
        inputMapper.CanceledEvent += Canceled;
    }

    private void Canceled(InputMapper.CanceledEventData data)
    {
        StartCoroutine(AbortMapping());
    }

    private void TimedOut(InputMapper.TimedOutEventData eventData)
    {
        StartCoroutine(AbortMapping());
    }

    private void OnInputMapped(InputMapper.InputMappedEventData eventData)
    {
        StartCoroutine(CompleteMapping());
    }

    private IEnumerator AbortMapping()
    {
        yield return null;

        _isMapping = false;
        _keyBindingsManager.EndMappingButtonsLock();
        _cursorManager.ActivateCursor();
    }

    private IEnumerator CompleteMapping()
    {
        yield return null;

        _isMapping = false;
        _keyBindingsManager.EndMappingButtonsLock();
        //UpdateKeybindingsLabels();
        _cursorManager.ActivateCursor();

#if !UNITY_EDITOR
        if(ReInput.userDataStore != null)
            ReInput.userDataStore.Save();
#endif
    }

    private void OnMappingConflict(InputMapper.ConflictFoundEventData eventData)
    {
        StartCoroutine(AddOverlayUI(eventData));
    }

    private IEnumerator AddOverlayUI(InputMapper.ConflictFoundEventData eventData)
    {
        yield return null;

        if (eventData.conflicts.All(c => c.keyCode == KeyCode.Escape))
        {
            eventData.responseCallback(InputMapper.ConflictResponse.Cancel);
            StartCoroutine(AbortMapping());
            yield break;
        }

        _cursorManager.ActivateCursor();

        _modalWindowManager
            .CreateModalWindowYesNo(
                () =>
                {
                    eventData.responseCallback(InputMapper.ConflictResponse.Replace);
                    OnInputMapped(null);
                    _keyBindingsManager.UpdateAllKeyBindingLabels();
                },
                () =>
                {
                    eventData.responseCallback(InputMapper.ConflictResponse.Cancel);
                    OnInputMapped(null);
                },
                _overrideModalTitle);

    }
    private Pole AxisToPole(AxisRange axisRange) => axisRange == AxisRange.Negative ? Pole.Negative : Pole.Positive;

    private int ControllerTypeToButtonIndex(ControllerType controllerType) => (int)controllerType;

    public void SetKeybindingButtonsInteractable(bool value)
    {
        _assignementButtons.ForEach(button => button.interactable = value);
    }

    public ServiceLocator MyServiceLocator { get; set; }
}