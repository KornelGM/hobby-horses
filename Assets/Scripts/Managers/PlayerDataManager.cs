using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData Manager", menuName = "ScriptableObjects/Managers/PlayerData Manager")]
public class PlayerDataManager : ScriptableObject, IManager
{
    public int Money = 150; // Temporary value

    public void Reset()
    {
        Money = 150;
    }
}
