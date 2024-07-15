using I2.Loc;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NewGameStartupUI : MonoBehaviour, IServiceLocatorComponent, IWindow
{
    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }
    public GameObject MyObject { get => gameObject; }
    public int Priority { get; set; } = 100;
    public bool CanBeClosedByManager { get; set; } = true;
    public bool IsOnTop { get; set; } = true;
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; } = false;

    [ServiceLocatorComponent] private SettingsInitializerMainMenu _settingsInitializerMainMenu;

    [SerializeField] private NewGameStartup _startup;
    [SerializeField] private SpriteDatabase _spriteDatabase;

    [SerializeField] private int _maxFundsMultiplier;
    [SerializeField] private int _maxReputationMultiplier;

    [SerializeField] private Image _imageIcon;
    [SerializeField] private Slider _fundsSlider;
    [SerializeField] private Slider _reputationSlider;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _startButton;

    [SerializeField] private int _factoriesCount;
    [SerializeField] private List<LocalizedString> _factoryNames = new();

    private int _index;

    private void OnEnable()
    {
        Initialize();
        OnInputFieldChange();
    }

    public void OnInputFieldChange()
    {
        _startButton.interactable = _inputField.text.Length > 0;
    }

    private void Initialize()
    {
        _startup.SetDefaultVariables();

        RandomizeName();

        _fundsSlider.maxValue = _maxFundsMultiplier;
        _reputationSlider.maxValue = _maxReputationMultiplier;

        _index = 0;

        _imageIcon.sprite = _spriteDatabase.GetElementOfIndex(_index);
    }

    public void OnButtonPress()
    {
        SetStartup();

        if (!_settingsInitializerMainMenu.MainMenuWindow.MyServiceLocator.TryGetServiceLocatorComponent(out LoadingWindow loadingWindow))
            return;

        loadingWindow.OnNewGameSceneLoad();
        gameObject.SetActive(false);
    }

    private void RandomizeName()
    {
        int id = Random.Range(0, _factoryNames.Count);
        string name = _factoryNames[id];

        _inputField.text = name;
    }

    public void SetNewIcon(int value)
    {
        if (_index + value < 0)
            _index = _spriteDatabase.Entries.Count - 1;
        else if (_index + value > _spriteDatabase.Entries.Count - 1)
            _index = 0;
        else
            _index += value;

        _imageIcon.sprite = _spriteDatabase.GetElementOfIndex(_index);
    }

    public void SetStartup()
    {
        _startup.FactoryName = _inputField.text;
        _startup.FactoryIconIndex = _index;
        _startup.FundsMultiplier = _fundsSlider.value;
        _startup.ReputationMultiplier = _fundsSlider.value;
    }

    [Button("Get Factories Names")]
    private void GetFactoriesNames()
    {
        _factoryNames.Clear();

        for (int i = 1; i <= _factoriesCount; i++)
        {
            LocalizedString newName = new($"Factories/{i}");
            _factoryNames.Add(newName);
        }
    }
}
