using Steamworks;
using UnityEngine;

[CreateAssetMenu(fileName = "DLC", menuName = "ScriptableObjects/Steam/DLC")]
public class DLCData : ScriptableObject
{
	public Padlock Padlock => _padlock;

	public bool AllowToLoadWithoutThis { get => _allowToLoadWithoutThis; }
	public bool BuiltIn { get => _builtIn; }
	public bool TestInEditor { get => _testInEditor; }
	public AppId_t AppID { get => _appID; }

	[SerializeField] private bool _allowToLoadWithoutThis = false;
	[SerializeField] private bool _testInEditor = false;
	[SerializeField] private bool _builtIn;
	[SerializeField] private AppId_t _appID = AppId_t.Invalid;
	[SerializeField] private Padlock _padlock;

	public bool IsAvailable()
    {
		if (_builtIn) return true;

#if UNITY_EDITOR
		if (_testInEditor) return true;
#endif

		if (!SteamManager.Initialized) return false;

		return SteamApps.BIsDlcInstalled(AppID);
	}
}
