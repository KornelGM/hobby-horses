using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save Load Info", menuName = "ScriptableObjects/Save/Save Load Info")]
public class CustomSaveLoadInfo : SaveLoadInfo<SaveData>
{
    public List<SaveData> NewGameDatasForScenes = new();

    public void AddNewSaveDataForScene(SaveData saveData)
    {
        SaveData foundSaveData = GetSaveDataForScene(saveData.SceneName);

        if (foundSaveData != null)
        {
            NewGameDatasForScenes.Remove(foundSaveData);
        }

        NewGameDatasForScenes.Add(saveData);

        saveData.IsNewGame = true;
        saveData.UnlockedPopups.Clear();
        saveData.CurrentCounterValue = 300;
        saveData.ItemsOnScene.DestroyedItems.Clear();
    }

    [CanBeNull]
    public SaveData GetSaveDataForScene(string scene)
    {
        foreach (SaveData saveData in NewGameDatasForScenes)
        {
            if (scene == saveData.SceneName)
                return saveData;
        }

        return null;
    }
}
