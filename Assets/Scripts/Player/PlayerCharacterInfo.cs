using Steamworks;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacterInfo", menuName = "ScriptableObjects/Player/Info")]
public class PlayerCharacterInfo : ScriptableObject
{
    [SerializeField] private string _playerCharacterName;
    private ulong _playerSteamID;

    public ulong PlayerSteamID => _playerSteamID;
    public string PlayerCharacterName => SteamManager.Initialized ? SteamFriends.GetPersonaName() : _playerCharacterName;

    public void SetPlayerSteamID(ulong steamID) => _playerSteamID = steamID;
}