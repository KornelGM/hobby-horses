using Cinemachine;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

public class RoomCameraController : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public RoomPart CurrentRoomPart => _currentRoomPart;

    public Action OnRoomPartChange;

    public TrackMovement Cart => _cart;

    [SerializeField, FoldoutGroup("Tracks")] private RoomPath[] _roomPaths;
    [SerializeField, FoldoutGroup("Tracks")] private Transform _roomLookTarget;
    [SerializeField, FoldoutGroup("Cart")] private TrackMovement _cart;
    [SerializeField, FoldoutGroup("Cart")] private CinemachineVirtualCamera _cameraBrain;
    [SerializeField, FoldoutGroup("Camera")] private float _cameraRotationSpeed;

    private Coroutine _cameraLookCoroutine;
    private Transform _cameraTarget;

    private RoomPart _currentRoomPart = RoomPart.Room;
    private Track _currentTrack;

    private void Update()
    {
        if (_cameraTarget != null)
            CameraFollowTarget(_cameraTarget);
    }

    public void MoveCamera(RoomPart roomPart)
    {
        if (_currentRoomPart == roomPart)
            return;

        if (roomPart == RoomPart.Room)
        {
            GoBack();
            return;
        }

        if (_currentRoomPart != RoomPart.Room)
            return;

        GoToPart(roomPart);
    }

    private void GoBack()
    {
        if (_currentTrack == null)
            return;

        _cameraTarget = _roomLookTarget;
        _currentRoomPart = RoomPart.Room;
        _cart.Move(_currentTrack, true);
        OnRoomPartChange?.Invoke();
    }

    private void GoToPart(RoomPart roomPart)
    {
        _currentRoomPart = roomPart;
        RoomPath path = _roomPaths.FirstOrDefault(foundPath => foundPath.RoomPart == roomPart);
        if (path == null)
            return;

        _currentTrack = path.Path;

        if (path.LookOnTarget)
            _cameraTarget = path.LookTarget;

        _cart.Move(_currentTrack);

        OnRoomPartChange?.Invoke();
    }

    private void CameraFollowTarget(Transform target)
    {
        if (target == null)
            return;

        Vector3 direction = target.position - _cameraBrain.transform.position;
        Quaternion finalRotation = Quaternion.LookRotation(direction);
        _cameraBrain.transform.rotation = Quaternion.Slerp(_cameraBrain.transform.rotation, finalRotation, _cameraRotationSpeed);
    }
}

[Serializable]
public class RoomPath
{
    public Track Path;
    public bool LookOnTarget;
    [ShowIf(nameof(LookOnTarget))] public Transform LookTarget;
    public RoomPart RoomPart;
}