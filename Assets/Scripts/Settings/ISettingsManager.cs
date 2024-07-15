using UnityEngine.Rendering;

public interface ISettingsManager
{
    public Volume GetGlobalVolume();
    public VolumeProfile GetDefaultProfile();
}
