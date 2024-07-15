using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamLobbyListUI : MonoBehaviour
{
    [SerializeField] private Padlock _padlock;

    private void Start()
    {
        _padlock.OnUnlock += CheckVisibility;
        _padlock.OnLock += CheckVisibility;
        CheckVisibility(_padlock);
    }

    private void OnDestroy()
    {
        _padlock.OnUnlock -= CheckVisibility;
        _padlock.OnLock -= CheckVisibility;
    }

    void CheckVisibility(Padlock padlock)
    {
        gameObject.SetActive(!_padlock.Locked);
    }

}
