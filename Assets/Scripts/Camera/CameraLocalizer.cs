using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocalizer : Localizer, ISaveable<ServerPlayerSaveData>
{
    public ServerPlayerSaveData CollectData(ServerPlayerSaveData data)
    {
        CollectData(data.PlayerCameraLocalization);
        return data;
    }

    public void Initialize(ServerPlayerSaveData save)
    {
        if (save == null) return;
        SetPositionAndRotationOfObject(save.PlayerCameraLocalization);
    }


    protected override void SetPositionAndRotationOfObject(LocalizationSaveInfo save)
    {
        var camera = GetComponent<CinemachineFreeLook>();
        camera.ForceCameraPosition(save.StartPostion.Vector3, save.StartRotation.Quaternion);

    }

    protected override void GetPositionAndRotationOfObject(LocalizationSaveInfo data)
    {
        data.StartPostion = new(transform.position);
        data.StartRotation = new(transform.rotation);
    }
}
