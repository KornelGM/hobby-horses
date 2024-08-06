using Cinemachine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizationUI : MonoBehaviour, IServiceLocatorComponent, IWindow
{
    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; }
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; }
    public bool ShouldDeactivateCrosshair { get; set; }

    [ServiceLocatorComponent] private HobbyHorseCustomizationManager _customizationManager;

    [SerializeField, FoldoutGroup("References")] private GameObject _confirmButton;
    [SerializeField, FoldoutGroup("References")] private HobbyHorsePartButton _partButtonPrefab;
    [SerializeField, FoldoutGroup("References")] private Transform _partButtonsContent;
    [SerializeField, FoldoutGroup("References")] private RoomPartButton _backButton;

    private CustomizationCamera[] _customizationCameras;

    private List<HobbyHorsePartButton> _spawnedPanels = new();
    private List<HobbyHorsePartButton> _activePanels = new();

    private HobbyHorseCustomizationPartInfo _currentHobbyHorsePart;

    private HobbyHorsePart _currentCategory = HobbyHorsePart.Head;

    bool _hide = false;

    int _index = 0;

    public void Initialize(CustomizationCamera[] customizationCameras)
    {
        _customizationCameras = customizationCameras;
        SetCategory(_currentCategory);

        _currentHobbyHorsePart = _customizationManager.HobbyHorsePartsCategories.
            FirstOrDefault(category => category.HobbyHorsePart == _currentCategory).CustomizationPart[0];

        RefreshConfirmButton();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            KeyBoardChangePart(1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            KeyBoardChangePart(-1);
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmButton();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideUI(!_hide);
        }
    }

    private void KeyBoardChangePart(int value)
    {
        _index += value;

        HobbyHorseCustomizationCategory foundCategory = _customizationManager.HobbyHorsePartsCategories.
            FirstOrDefault(category => category.HobbyHorsePart == _currentCategory);

        _currentHobbyHorsePart = null;

        if (_index >= foundCategory.CustomizationPart.Length)
            _index = 0;
        else if (_index < 0)
            _index = foundCategory.CustomizationPart.Length - 1;

        HobbyHorseCustomizationPartInfo newPart = foundCategory.CustomizationPart[_index];

        SetHobbyHorsePart(newPart);
    }

    private void HideUI(bool hide)
    {
        if (hide)
            transform.localScale = Vector3.zero;
        else
            transform.localScale = Vector3.one;

        _hide = !_hide;
    }

    public void SetHobbyHorsePart(HobbyHorseCustomizationPartInfo newPart)
    {
        _currentHobbyHorsePart = newPart;
        _customizationManager.ChangeHobbyHorsePart(_currentHobbyHorsePart);
        RefreshConfirmButton();
    }

    public void ConfirmButton()
    {
        _customizationManager.TryUnlockHobbyHorsePart(_currentHobbyHorsePart.Guid);
        RefreshConfirmButton();
    }

    private void RefreshConfirmButton()
    {
        _confirmButton.SetActive(!_customizationManager.CheckHobbyHorsePartIsUnlocked(_currentHobbyHorsePart.Guid));
    }

    public void SetCategory(HobbyHorsePart newCategory)
    {
        _currentCategory = newCategory;
        _index = 0;

        HobbyHorseCustomizationCategory foundCategory = _customizationManager.HobbyHorsePartsCategories.
            FirstOrDefault(category => category.HobbyHorsePart == _currentCategory);

        _currentHobbyHorsePart = null;
        foreach (var guid in _customizationManager.CreatedHobbyHorse.HobbyHorsePartsGuids)
        {
            _currentHobbyHorsePart = foundCategory.CustomizationPart.FirstOrDefault(part => part.Guid == guid);

            if (_currentHobbyHorsePart != null)
                break;
        }

        if (_currentHobbyHorsePart == null)
            return;

        _customizationManager.ChangeHobbyHorsePart(_currentHobbyHorsePart);

        RefreshConfirmButton();
        CreateButtons(_currentCategory);
        SwitchCamera(_currentCategory);
    }

    public void StopCustomize()
    {
        _customizationManager.StopCustomize(_backButton.OnButtonDown);
    }

    private void SwitchCamera(HobbyHorsePart newCategory)
    {
        foreach (var camera in _customizationCameras)
        {
            camera.Camera.Priority = 0;
        }

        CustomizationCamera currentCamera = _customizationCameras.FirstOrDefault(cam => cam.HobbyHorsePart == newCategory);
        if (currentCamera == null) return;

        currentCamera.Camera.Priority = 12;
    }

    public void OnDeleteWindow() 
    {
        foreach (var camera in _customizationCameras)
        {
            camera.Camera.Priority = 0;
        }
    }

    private void CreateButtons(HobbyHorsePart newCategory)
    {
        HobbyHorseCustomizationCategory categoryToSpawn = _customizationManager.HobbyHorsePartsCategories.FirstOrDefault(category => category.HobbyHorsePart == newCategory);

        if (categoryToSpawn == null)
            return;

        if (categoryToSpawn.CustomizationPart.Length != _activePanels.Count)
            Utilities.PoolFields<HobbyHorsePartButton>(categoryToSpawn.CustomizationPart.Length, _spawnedPanels, _activePanels, _partButtonPrefab, _partButtonsContent);

        for (int i = 0; i < categoryToSpawn.CustomizationPart.Length; i++)
        {
            _activePanels[i].Initialize(categoryToSpawn.CustomizationPart[i]);

            _activePanels[i].gameObject.SetActive(true);
        }
    }
}

[Serializable]
public class CustomizationCamera
{
    public CinemachineVirtualCamera Camera;
    public HobbyHorsePart HobbyHorsePart;
}
