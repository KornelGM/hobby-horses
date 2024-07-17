using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUI : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private RoomCameraController _roomCameraController;

    [SerializeField, FoldoutGroup("References")] private Transform _buttonsContent;
    [SerializeField, FoldoutGroup("References")] private RoomPartButton _roomPartButtonPrefab;
    [SerializeField, FoldoutGroup("Buttons Settings")] private RoomPart[] _buttonsToSpawn;

    private RoomPartButton _backButton;
    private List<RoomPartButton> _partsButtons = new();

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        _backButton = Instantiate(_roomPartButtonPrefab, _buttonsContent);
        _backButton.Initialize(RoomPart.Room);

        foreach (var part in _buttonsToSpawn)
        {
            RoomPartButton newButton = Instantiate(_roomPartButtonPrefab, _buttonsContent);
            newButton.Initialize(part);
            _partsButtons.Add(newButton);
        }

        _roomCameraController.OnRoomPartChange += UpdateButtons;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool isRoomView = _roomCameraController.CurrentRoomPart == RoomPart.Room;

        _backButton.gameObject.SetActive(!isRoomView);

        foreach (var button in _partsButtons)
        {
            button.gameObject.SetActive(isRoomView);
        }
    }
}
