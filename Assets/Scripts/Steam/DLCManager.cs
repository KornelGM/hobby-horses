using UnityEngine;

public class DLCManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField]private DLCDatabase _dlcDatabase;

    public bool TryInitializeDLCPadlock(Padlock padlock)
    {
        foreach(DLCData dlcData in _dlcDatabase.DLCData)
        {
            if(dlcData.Padlock == padlock)
            {
                if (dlcData.IsAvailable())
                {
                    padlock.Unlock();
                }
                else
                {
                    padlock.Lock();
                }
                return true;
            }
        }
        return false;
    }
}
