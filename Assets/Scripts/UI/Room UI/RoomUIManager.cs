using UnityEngine;

public class RoomUIManager : MonoBehaviour, IServiceLocatorComponent
{
    public RoomUI RoomUI => _createdRoomUI;
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private WindowManager _windowManager;

    [SerializeField] private RoomUI _roomUIPrefab;

    private RoomUI _createdRoomUI;

    public void Awake()
    {
        _createdRoomUI = _windowManager.CreateWindow(_roomUIPrefab).GetComponent<RoomUI>();
    }
}
