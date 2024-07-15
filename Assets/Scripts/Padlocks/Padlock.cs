using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class PadlockList
{
    public List<Padlock> Padlocks => _padlocks;
    [SerializeField] private List<Padlock> _padlocks = new();

    public bool IsAnyPadlockLocked()
    {
        return _padlocks.Any(padlock => padlock != null && padlock.Locked);
    }
}

[CreateAssetMenu(fileName = "Padlock", menuName = "ScriptableObjects/Padlock/Padlock")]
public class Padlock : DatabaseElement<ActionStat>
{
    public event Action<Padlock> OnUnlock;
    public event Action<Padlock> OnLock;
    private bool _locked = true;
    public bool Locked => _locked;
    public bool Unlocked => !_locked;

    private bool _lockedDefault = true;
    public bool LockedDefault => _lockedDefault;
    public bool UnlockedDefault => !_lockedDefault;

    public override List<string> GetPath()
    {
        return new List<string>();
    }

    public void PlayerMadeAction(ActionStat stat)
    {
        if (Unlocked) return;
        if (Entries.ContainsKey(stat.Guid)) Unlock();
    }

    public void Initialize()
    {
        if (_lockedDefault) Lock();
        else Unlock();
    }

    public void RevertToDefaultValue()
    {
        _locked = _lockedDefault;
    }

    [GUIColor("green")]
    [ShowIf(nameof(CanLock))]
    [Button("Unlocked", ButtonSizes.Gigantic)]
    public void Lock()
    {
        _locked = true;
        OnLock?.Invoke(this);
    }

    [GUIColor("red")]
    [ShowIf(nameof(CanUnlock))]
    [Button("Locked", ButtonSizes.Gigantic)]
    public void Unlock()
    {
        _locked = false;
        OnUnlock?.Invoke(this);
    }

    [InfoBox("Entry List tells which player actions unlocks the padlock")]
    [GUIColor("green")]
    [ShowIf(nameof(CanLockDefaultLock))]
    [Button("Default Unlocked", ButtonSizes.Gigantic)]
    public void LockDefault()
    {
        _lockedDefault = true;
    }

    [InfoBox("Entry List tells which player actions unlocks the padlock")]
    [GUIColor("red")]
    [ShowIf(nameof(CanUnlockDefaultLock))]
    [Button("Default Locked", ButtonSizes.Gigantic)]
    public void UnlockDefault()
    {
        _lockedDefault = false;
    }

    public bool CanLockDefaultLock()
    {
        if (Application.isPlaying) return false;
        return !_lockedDefault;
    }

    public bool CanUnlockDefaultLock()
    {
        if (Application.isPlaying) return false;
        return _lockedDefault;
    }

    public bool CanLock()
    {
        if (!Application.isPlaying) return false;
        return !_locked;
    }

    public bool CanUnlock()
    {
        if (!Application.isPlaying) return false;
        return _locked;
    }
}


