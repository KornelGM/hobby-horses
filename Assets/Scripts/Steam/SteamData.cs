using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Steam Data", menuName = "ScriptableObjects/Steam/Steam Data")]
public class SteamData : ScriptableObject
{
	private enum GameType
    {
		Final,
		Demo,
		Prologue,
		Static,
		Playtest,
    }

	public uint DemoID => _demoID;
	public uint PrologueID => _prologueID;
	public uint FinalVersionID => _finalVersionID;

	[SerializeField]private GameType _gameType = GameType.Final;
	[SerializeField] private uint _staticID = 480;
	[SerializeField] private uint _demoID = 480;
	[SerializeField] private uint _prologueID = 480;
	[SerializeField] private uint _playtest = 480;
	[SerializeField] private uint _finalVersionID = 480;

	public uint GetID()
    {
        switch (_gameType)
        {
            case GameType.Demo: return _demoID;
            case GameType.Final: return _finalVersionID;
            case GameType.Prologue: return _prologueID;
            case GameType.Static: return _staticID;
            case GameType.Playtest: return _playtest;
            default: break;
        }

		return 480;
    }
}
