using UnityEngine;

public class DoorToUnlock : MonoBehaviour
{
    [SerializeField] private Padlock _padlock;
    private void Awake()
    {
        _padlock.OnUnlock += Unlock;
        _padlock.OnLock += Lock;
    }

    private void Lock(Padlock _)
    {
        gameObject.SetActive(true);
    }


    private void Unlock(Padlock _)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _padlock.OnUnlock -= Unlock;
        _padlock.OnLock -= Lock;
    }
}
