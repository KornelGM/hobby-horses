using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class RaceSelectorManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public RaceInfo[] RaceInfo => _racesInofs;

    [ServiceLocatorComponent] private WindowManager _windowManager;
    [ServiceLocatorComponent] private ModalWindowManager _modalWindowManager;
    [ServiceLocatorComponent(canBeNull: true)] private RoomCameraController _roomCameraController;
    [ServiceLocatorComponent(canBeNull: true)] private RoomUIManager _roomUIManager;

    [SerializeField, FoldoutGroup("References")] private RaceSelectorUI _raceSelectorUI;
    [SerializeField, FoldoutGroup("Races")] private RaceInfo[] _racesInofs;
    
    private RaceSelectorUI _createdRaceSelectorUI;

    private void Start()
    {
        if (_roomUIManager != null)
            _roomUIManager.RoomUI.AddListenerToButton(RoomPart.Door, SubscribeStartCustomization);
    }

    private void SubscribeStartCustomization()
    {
        if (_roomCameraController != null)
            _roomCameraController.Cart.OnEndPath += StartSelecting;
    }

    public void StartSelecting()
    {
        _createdRaceSelectorUI = _windowManager.CreateWindow(_raceSelectorUI).GetComponent<RaceSelectorUI>();
        _createdRaceSelectorUI.Initialize();
    }

    public void StopSelecting(Action onStopAction = null)
    {
        onStopAction?.Invoke();
        _roomCameraController.Cart.OnEndPath -= StartSelecting;
        _windowManager.DeleteWindow(_createdRaceSelectorUI);
    }

    public void StartRace(RaceInfo raceInfo, LoadingWindow loadingWindow)
    {
        loadingWindow.transform.SetParent(_windowManager.MainCanvas.transform);
        loadingWindow.OnSceneLoad(raceInfo.SceneName);
        _windowManager.DeleteWindow(_createdRaceSelectorUI);
    }
}

[Serializable]
public class RaceInfo
{
    public string RaceName;
    public string SceneName;
    public Sprite RaceIcon;
}
