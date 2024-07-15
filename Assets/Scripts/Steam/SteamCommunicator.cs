using Sirenix.OdinInspector;
using Steamworks;
using UnityEngine;

[CreateAssetMenu(fileName = "Steam Communicator", menuName = "ScriptableObjects/Steam/Steam Communicator")]
public class SteamCommunicator : ScriptableObject
{
    [SerializeField] private PadlockList _achievementLocks;
    public void AddToStat(string statName, int value = 1)
    {
        if (SteamNotInitialized()) return;
        if (AchievementLocked()) return;

#if !UNITY_EDITOR
        string message = "Add To Stat";
        message += statName != null ? $" - Stat name: {statName}" : " - Stat name: UNAVAILABLE";
        message += $", Value: {value}";
        Debug.Log(message);
#endif

        SteamUserStats.GetStat(statName, out int amount);
        SteamUserStats.SetStat(statName, amount + value);
        SteamUserStats.StoreStats();
    }

    public void AddToStat(string statName, float value)
    {
        if (SteamNotInitialized()) return;
        if (AchievementLocked()) return;

        SteamUserStats.GetStat(statName, out float amount);
        SteamUserStats.SetStat(statName, amount + value);
        SteamUserStats.StoreStats();
    }

    public void SetStat(string statName, int value)
    {
        if (SteamNotInitialized()) return;
        if (AchievementLocked()) return;

#if !UNITY_EDITOR
        string message = "Set Stat";
        message += statName != null ? $" - Stat name: {statName}" : " - Stat name: UNAVAILABLE";
        message += $", Value: {value}";
        Debug.Log(message);
#endif

        SteamUserStats.SetStat(statName, value);
        SteamUserStats.StoreStats();
    }

    public void SetStat(string statName, float value)
    {
        if (SteamNotInitialized()) return;
        if (AchievementLocked()) return;

#if !UNITY_EDITOR
        string message = "Set Stat";
        message += statName != null ? $" - Stat name: {statName}" : " - Stat name: UNAVAILABLE";
        message += $", Value: {value}";
        Debug.Log(message);
#endif
        SteamUserStats.SetStat(statName, value);
        SteamUserStats.StoreStats();
    }

    public float GetFloatStat(string statName)
    {
        if (SteamNotInitialized()) return 0;

        SteamUserStats.GetStat(statName, out float amount);

#if !UNITY_EDITOR
        string message = "Get Float State";
        message += statName != null ? $" - Stat name: {statName}" : " - Stat name: UNAVAILABLE";
        message += $", Amount: {amount}";
        Debug.Log(message);
#endif

        return amount;
    }

    public int GetIntStat(string statName)
    {
        if (SteamNotInitialized()) return 0;

        SteamUserStats.GetStat(statName, out int amount);

#if !UNITY_EDITOR
        string message = "Get Int State";
        message += statName != null ? $" - Stat name: {statName}" : " - Stat name: UNAVAILABLE";
        message += $", Amount: {amount}";
        Debug.Log(message);
#endif

        return amount;
    }

    public void SetAchievement(string statName)
    {
        if (SteamNotInitialized()) return;
        if (AchievementLocked()) return;

#if !UNITY_EDITOR
        string message = "Set Achievement";
        message += statName != null ? $" - Stat name: {statName}" : " - Stat name: UNAVAILABLE";
        Debug.Log(message);
#endif

        SteamUserStats.SetAchievement(statName);
        SteamUserStats.StoreStats();
    }

    [Button("ResetAllStats")]
    public void ResetAllStats()
    {
        if (SteamNotInitialized()) return;

#if !UNITY_EDITOR
        Debug.Log("Reset All Stats");
#endif
        SteamUserStats.ResetAllStats(true);
        SteamUserStats.StoreStats();
    }

    private bool AchievementLocked()
    {
        return _achievementLocks.IsAnyPadlockLocked();
    }

    private bool SteamNotInitialized()
    {
        if (SteamManager.Initialized)
            return false;

		Debug.LogWarning("Trying to use Steam API, but Steam is not initialized.");

        return true;
    }

    public bool GetAchievement(string statName)
    {
        if (SteamNotInitialized()) return false;

        SteamUserStats.GetAchievement(statName, out bool enabled);

#if !UNITY_EDITOR
        string message = "Get Achievement";
        message += statName != null ? $" - Stat name: {statName}" : " - Stat name: UNAVAILABLE";
        message += $", Enabled: {enabled}";
        Debug.Log(message);
#endif

        return enabled;
    }
}
