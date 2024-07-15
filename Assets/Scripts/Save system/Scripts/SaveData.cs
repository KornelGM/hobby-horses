using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string SceneName = "Main";
	public bool IsNewGame;
	public ServerPlayerSaveData PlayerSaveData = new();
	public ItemsSaveData ItemsSaveData = new();
	public ItemsSaveData ItemsOnScene = new();

	public int Funds = 0;

	public Reputation Reputation = new();

    public AchievementsSaveData AchievementsSaveData = new();

	public List<string> PastEventsGuid = new();
	public string CurrentEventGuid = "";
	public int CurrentCounterValue = 0;

	public List<FactorySaveData> RegionalFactories = new();
	public List<FactorySaveData> NationalFactories = new();
	public List<FactorySaveData> WorldFactories = new();
    public bool ReceivedRegionalAward = false;
    public bool ReceivedNationalAward = false;
    public bool ReceivedWorldAward = false;

    public bool IsTutorial = false;
	public bool CanOpenBook = false;
	public bool FirstChocolateNougat = false;
	public bool ReputationBlocking = false;
	public bool BlockGenerateOrders = false;

	public List<PopupType> UnlockedPopups = new();

	public string FactoryName = "My Factory";
	public int FactoryIconIndex= 0;
	public float FundsMultiplier = 1;
	public float ReputationMultiplier = 1;
}


[Serializable]
public class SaveVector3
{
	public float X = 0;
	public float Y = 0;
	public float Z = 0;

	public SaveVector3() { }

	public SaveVector3(Vector3 vector)
	{
		Vector3 = vector;
	}

	public Vector3 Vector3
	{
		get
		{
			return new Vector3(X, Y, Z);
		}
		set
		{
			X = value.x;
			Y = value.y;
			Z = value.z;
		}
    }
}

[System.Serializable]
public class LocalizationSaveInfo
{
	public SaveVector3 StartPostion = new SaveVector3();
	public SaveQuaternion StartRotation = new SaveQuaternion();

	public LocalizationSaveInfo() { }
	public LocalizationSaveInfo(Transform transform)
    {
		StartPostion = new SaveVector3(transform.position);
		StartRotation = new SaveQuaternion(transform.rotation);
    }
}

[System.Serializable]
public class ServerPlayerSaveData
{
	public LocalizationSaveInfo PlayerLocalization = new();
	public LocalizationSaveInfo PlayerCameraLocalization = new();
	public QuestsSaveData Quests = new();
    [field: SerializeField] public InventorySaveInfo InventorySaveData = new();
    public Dictionary<string, List<AActionStatData>> Statistics = new();
}

[Serializable]
public class AchievementsSaveData
{
    public float GameplayTime;
}

[System.Serializable]
public class ClientPlayerSaveData
{
	public InventorySaveInfo InventorySaveInfo = new();
}

[Serializable]
public class SaveColor
{
	public string HexValue;

	public SaveColor(Color color)
	{
		Color = color;
	}

	public Color Color
	{
		get
		{
            if (ColorUtility.TryParseHtmlString(HexValue, out Color newCol))
            {
                return newCol;
            }
            return Color.clear;
		}
		set
		{
			HexValue = "#" + ColorUtility.ToHtmlStringRGBA(value);
		}
	}
}


[Serializable]
public class SaveEnumValue
{
	public int Value = 0;

	public SaveEnumValue(int id)
    {
		this.Value = id;
    }
}


[Serializable]
public class SaveQuaternion
{
	public float X = 0;
	public float Y = 0;
	public float Z = 0;
	public float W = 0;

	public SaveQuaternion() { }

	public SaveQuaternion(Quaternion quaternion)
	{
		Quaternion = quaternion;
	}

	public Quaternion Quaternion
	{
		get
		{
			return new Quaternion(X, Y, Z, W);
		}
		set
		{
			X = value.x;
			Y = value.y;
			Z = value.z;
			W = value.w;
		}
	}
}

[Serializable]
public class QuestsSaveData
{
	public string ActiveQuestGuid;
	public List<SaveQuest> CompletedQuests = new();
	public List<SaveQuest> InProgressQuests = new();
}

[Serializable]
public abstract class ASaveQuestTask { }

[Serializable]
public class SaveQuest
{
	public string Guid;
	[SerializeReference] public List<ASaveQuestTask> QuestTasks = new();
	public bool Activated = false;
}


[Serializable]
public class ItemsSaveData
{
	[SerializeReference] public List<ItemSaveData> Items = new();

    public List<string> DestroyedItems = new();
}

[Serializable]
public class ItemSaveData
{
	public string ItemDataGUID = "";
	public string PersonalGUID = "";
	public LocalizationSaveInfo Localization;

	public ItemSaveData() { }
	public ItemSaveData(string GUID, string personalGUID, Transform transform)
    {
        ItemDataGUID = GUID;
        PersonalGUID = personalGUID;
        Localization = new LocalizationSaveInfo(transform);
    }
}
