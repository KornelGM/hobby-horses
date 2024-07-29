using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomPartButton : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public RoomPart RoomPart => _roomPart;

    [ServiceLocatorComponent] private RoomCameraController _roomCameraController;

    [SerializeField, FoldoutGroup("References")] private TextMeshProUGUI _partName;
    [SerializeField, FoldoutGroup("References")] private Button _button;

    [SerializeField] private RoomPart _roomPart;

    public void Start()
    {
        _partName.text = _roomPart.ToString();
    }

    public void OnButtonDown()
    {
        _roomCameraController.MoveCamera(_roomPart);
    }

    public void AddListenerToButton(UnityAction actionToAdd)
    {
        _button.onClick.AddListener(actionToAdd);
    }

    public void RemoveListenerFromButton(UnityAction actionToAdd)
    {
        _button.onClick.RemoveListener(actionToAdd);
    }
}
