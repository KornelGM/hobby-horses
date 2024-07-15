using UnityEngine;

public class DestroyOnLoad : MonoBehaviour
{
    public void Perform()
    {
        DestroyImmediate(gameObject);
    }
}
