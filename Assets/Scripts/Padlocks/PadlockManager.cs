using System;
using UnityEngine;

public class PadlockManager : MonoBehaviour, IServiceLocatorComponent, IAwake, IManager, IStartable
{
    public event Action<Padlock> OnUnlock;
    public event Action<Padlock> OnLock;

    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField] private PadlocksDatabase _padlocksDatabase;

    [ServiceLocatorComponent] private StatsManager _statsManager;

    public void CustomAwake()
    {
        foreach (Padlock padlock in _padlocksDatabase.EntryList)
        {
            padlock.OnLock += OnLock;
            padlock.OnUnlock += OnUnlock;
            _statsManager.OnStatisticAdded += padlock.PlayerMadeAction;
        }
    }

    public void CustomStart()
    {
        foreach (Padlock padlock in _padlocksDatabase.EntryList)
            padlock.Initialize();
    }

    private void OnApplicationQuit()
    {
        foreach (Padlock padlock in _padlocksDatabase.EntryList)
        {
            padlock.OnLock -= OnLock;
            padlock.OnUnlock -= OnUnlock;
            _statsManager.OnStatisticAdded -= padlock.PlayerMadeAction;
            padlock.RevertToDefaultValue();
        }
    }


    public void Lock(Padlock padlock)
    {
        padlock.Lock();
    }

    public void Unlock(Padlock padlock)
    {
        padlock.Lock();
    }

    public bool GetPadlockState(Padlock padlock)
    {
        return _padlocksDatabase.GetEntry(padlock.Guid).Locked;
    }
}
