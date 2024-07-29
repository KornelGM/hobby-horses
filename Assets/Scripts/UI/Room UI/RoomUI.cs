using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomUI : MonoBehaviour, IServiceLocatorComponent, IWindow
{
    public ServiceLocator MyServiceLocator { get; set; }

    public WindowManager Manager { get; set; }
    public GameObject MyObject => gameObject;
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; } = false;
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; } = true;
    public bool ShouldDeactivateCrosshair { get; set; }

    [ServiceLocatorComponent] private RoomCameraController _roomCameraController;

    [SerializeField, FoldoutGroup("Buttons")] private GameObject _buttonsPanel;
    [SerializeField, FoldoutGroup("Buttons")] private List<RoomPartButton> _partsButtons = new();

    private void Start()
    {
        _roomCameraController.OnRoomPartChange += UpdateButtons;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool isRoomView = _roomCameraController.CurrentRoomPart == RoomPart.Room;
        _buttonsPanel.SetActive(isRoomView);
    }

    public void AddListenerToButton(RoomPart roomPart, UnityAction actionToAdd)
    {
        RoomPartButton roomPartButton = _partsButtons.FirstOrDefault(button => button.RoomPart == roomPart);
        roomPartButton.AddListenerToButton(actionToAdd);
    }

    public void RemoveListenerFromButton(RoomPart roomPart, UnityAction actionToAdd)
    {
        RoomPartButton roomPartButton = _partsButtons.FirstOrDefault(button => button.RoomPart == roomPart);
        roomPartButton.RemoveListenerFromButton(actionToAdd);
    }
}
