using Cinemachine;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Tracks")] private RoomPath[] _roomPaths;
    [SerializeField, FoldoutGroup("Tracks")] private Transform _roomLookTarget;
    [SerializeField, FoldoutGroup("Cart")] private CinemachineDollyCart _cart;
    [SerializeField, FoldoutGroup("Cart")] private CinemachineVirtualCamera _cameraBrain;
    [SerializeField, FoldoutGroup("Cart")] private float _cartSpeed;

    private RoomPart _currentRoomPart = RoomPart.Room;

    public void GoToPoint(RoomPart roomPart)
    {
        if (_currentRoomPart == roomPart)
            return;

        if (roomPart == RoomPart.Room)
        {
            _cart.m_Speed = -_cartSpeed;
            _cameraBrain.LookAt = _roomLookTarget;
            _currentRoomPart = roomPart;
            return;
        }

        if (_currentRoomPart != RoomPart.Room)
            return;

        _currentRoomPart = roomPart;
        RoomPath path = _roomPaths.FirstOrDefault(foundPath => foundPath.RoomPart == roomPart);
        if (path == null)
            return;

        _cart.m_Path = path.Path;
        if (path.LookTarget)
            _cameraBrain.LookAt = path.LookTarget;
        _cart.m_Speed = _cartSpeed;
        
    }

    [Button("Go to part")]
    private void GoToPointButton(RoomPart roomPart)
    {
        GoToPoint(roomPart);
    }

    [Button("GoToRoom")]
    private void GoToRoom()
    {
        GoToPoint(RoomPart.Room);
    }

}

[Serializable]
public class RoomPath
{
    public CinemachineSmoothPath Path;
    public bool LookOnTarget;
    [ShowIf(nameof(LookOnTarget))] public Transform LookTarget;
    public RoomPart RoomPart;
}