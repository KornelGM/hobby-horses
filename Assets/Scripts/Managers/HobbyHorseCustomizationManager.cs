using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HobbyHorseCustomizationManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public Action OnPartUnlocked;

    public HobbyHorseCustomizationCategory[] HobbyHorsePartsCategories => _hobbyHorsePartsCategories;
    public HobbyHorseCustomizationPartInfo[] HobbyHorseParts
    {
        get
        {
            List<HobbyHorseCustomizationPartInfo> infos = new();
            foreach (var category in _hobbyHorsePartsCategories)
            {
                infos.AddRange(category.CustomizationPart);
            }
            return infos.ToArray();
        }
    }

    public CurrentHobbyHorseInfo CurrentHobbyHorseInfo => _currentHobbyHorseInfo;
    public CustomHobbyHorse CreatedHobbyHorse => _createdHobbyHorse;

    [ServiceLocatorComponent] private WindowManager _windowManager;
    [ServiceLocatorComponent] private FundsManager _fundsManager;
    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
    [ServiceLocatorComponent(canBeNull: true)] private RoomCameraController _roomCameraController;
    [ServiceLocatorComponent(canBeNull: true)] private RoomUIManager _roomUIManager;

    [SerializeField, FoldoutGroup("Customization Parts")] private HobbyHorseCustomizationCategory[] _hobbyHorsePartsCategories;

    [SerializeField, FoldoutGroup("Current Hobby Horse")] private CurrentHobbyHorseInfo _currentHobbyHorseInfo;

    [SerializeField, FoldoutGroup("Cameras")] private CustomizationCamera[] _customizationCameras;

    [SerializeField, FoldoutGroup("References")] private Transform _hobbyHorseHolder;
    [SerializeField, FoldoutGroup("References")] private CustomHobbyHorse _hobbyHorsePrefab;
    [SerializeField, FoldoutGroup("References")] private CustomizationUI _customizationUI;

    [SerializeField, FoldoutGroup("Settings")] private bool _spawnHorseOnStart;

    private List<string> _unlockedParts = new();
    private CustomHobbyHorse _createdHobbyHorse;
    private CustomizationUI _createdCustomizationUI;

    private void Start()
    {
        if(_roomUIManager != null)
            _roomUIManager.RoomUI.AddListenerToButton(RoomPart.Wardrobe, SubscribeStartCustomization);

        if (_spawnHorseOnStart)
        {
            _createdHobbyHorse = Instantiate(_hobbyHorsePrefab, _hobbyHorseHolder);
            LoadAppearance();
        }
    }

    private void SubscribeStartCustomization()
    {
        if(_roomCameraController != null)
            _roomCameraController.Cart.OnEndPath += StartCustomize;
    }

    public void StartCustomize()
    {
        _createdCustomizationUI = _windowManager.CreateWindow(_customizationUI).GetComponent<CustomizationUI>();
        _createdCustomizationUI.Initialize(_customizationCameras);
    }

    private void LoadAppearance()
    {
        if (_createdHobbyHorse == null)
            return;

        if (_currentHobbyHorseInfo.PartsGuids is { Count: > 0 })
            _createdHobbyHorse.LoadAppearance(_currentHobbyHorseInfo.PartsGuids.ToArray());
        else
            _createdHobbyHorse.LoadDefaultAppearance();
    }

    public void StopCustomize(Action onStopAction = null)
    {
        bool canSave = true;
        foreach (var guid in _createdHobbyHorse.HobbyHorsePartsGuids)
        {
            if (!CheckHobbyHorsePartIsUnlocked(guid))
            {
                canSave = false;
                break;
            }
        }

        if(canSave)
        {
            _currentHobbyHorseInfo.SetPartsGuids(_createdHobbyHorse.HobbyHorsePartsGuids.ToList());
            Exit();
        }
        else
        {
            _modalWindowManager.CreateModalWindowYesNo(Exit, null, "Current customization has unlocked elements - will not be saved", "Are you sure you want to stop customizing?");
        }

        void Exit()
        {
            LoadAppearance();
            onStopAction?.Invoke();
            _roomCameraController.Cart.OnEndPath -= StartCustomize;
            _windowManager.DeleteWindow(_createdCustomizationUI);
        }
    }

    public void ChangeHobbyHorsePart(HobbyHorseCustomizationPartInfo newPart)
    {
        _createdHobbyHorse.ChangeHobbyHorsePart(newPart);
    }

    public bool TryUnlockHobbyHorsePart(string guid)
    {
        if (CheckHobbyHorsePartIsUnlocked(guid)) return false;

        HobbyHorseCustomizationPartInfo partToUnlock = HobbyHorseParts.FirstOrDefault(part => part.Guid == guid);

        if (partToUnlock == null) return false;
        if (!_fundsManager.CanBePurchased(partToUnlock.Cost)) return false;

        _fundsManager.SubtractAmount(partToUnlock.Cost);
        UnlockHobbyHorsePart(partToUnlock);
        return true;
    }

    private void UnlockHobbyHorsePart(HobbyHorseCustomizationPartInfo partToUnlock)
    {
        _unlockedParts.Add(partToUnlock.Guid);
        OnPartUnlocked?.Invoke();
    }

    public bool CheckHobbyHorsePartIsUnlocked(string guid)
    {
        HobbyHorseCustomizationPartInfo info = HobbyHorseParts.FirstOrDefault(part => part.Guid == guid);
        if (info == null) return false;

        if (!info.CanUnlocked) return true;

        return _unlockedParts.Contains(guid);
    }

    private void OnApplicationQuit()
    {
        _currentHobbyHorseInfo.ClearOnQuit();
    }
}

[Serializable]
public class HobbyHorseCustomizationCategory
{
    public HobbyHorsePart HobbyHorsePart;
    public HobbyHorseCustomizationPartInfo[] CustomizationPart;

    [Button("Set Part To All Elements"), FoldoutGroup("Buttons")]
    private void SetPartToAll()
    {
        foreach (var part in CustomizationPart)
        {
            part.SetHobbyHorsePart(HobbyHorsePart);
        }
    }

    [Button("Set Guid To All Elements"), FoldoutGroup("Buttons")]
    private void SetGuidToAll()
    {
        foreach (var part in CustomizationPart)
        {
            part.SetGuid();
        }
    }
}

[Serializable]
public class HobbyHorseCustomizationPartInfo
{
    public HobbyHorseCustomize HobbyHorsePartPrefab;
    public Sprite Icon;
    public bool CanUnlocked;
    [ShowIf(nameof(CanUnlocked))] public int Cost;
    [Range(0, 5), ShowIf(nameof(CanUnlocked))] public int ReputationLevel;
    public bool HasStats;
    [ShowIfGroup(nameof(HasStats))] public HobbyHorseStats HobbyHorseStats;

    [field: SerializeField, ReadOnly] public HobbyHorsePart HobbyHorsePart;
    [field: SerializeField, ReadOnly] public string Guid;

    [InfoBox("The element does not have its GUID!!",
        InfoMessageType.Warning, visibleIfMemberName: nameof(_guidIsEmpty))]

    [Button("Set Hobby Horse Part")]
    public void SetHobbyHorsePart(HobbyHorsePart newPart)
    {
        HobbyHorsePart = newPart;
    }

    [Button("Set Guid")]
    public void SetGuid()
    {
        if (!string.IsNullOrEmpty(Guid))
            return;

        Guid = System.Guid.NewGuid().ToString();
    }

    private bool _guidIsEmpty { get { return string.IsNullOrEmpty(Guid); } }
}
