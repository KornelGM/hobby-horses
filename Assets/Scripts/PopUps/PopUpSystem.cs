using UnityEngine;

public class PopUpSystem : MonoBehaviour
{
    public PopUp[] PopUps { get; private set; } 

    public void Initialize()
    {
        PopUps = GetComponentsInChildren<PopUp>(true);
    }
}
