using UnityEngine;

public class UIStartCleaner : MonoBehaviour
{
    [SerializeField] private GameObject[] _objectsToRemove;

    private void Awake()
    {
        foreach (var obj in _objectsToRemove) 
        {
            Destroy(obj);
        }
    }
}
