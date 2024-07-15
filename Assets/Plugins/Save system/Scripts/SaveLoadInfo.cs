using UnityEngine;

public class SaveLoadInfo<T> : ScriptableObject where T : class
{
	[SerializeField]public T CurrentData = null;
	public string LoadingScene = "";
}