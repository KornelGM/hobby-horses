using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomPartButton : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private RoomCameraController _roomCameraController;

    [SerializeField, FoldoutGroup("References")] private TextMeshProUGUI _partName;

    private RoomPart _roomPart;

    public void Initialize(RoomPart roomPart)
    {
        _roomPart = roomPart;

        _partName.text = _roomPart.ToString();
    }

    public void OnButtonDown()
    {
        _roomCameraController.MoveCamera(_roomPart);
    }
}
