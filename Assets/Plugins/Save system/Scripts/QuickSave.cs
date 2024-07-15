using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class QuickSave<T,N> : MonoBehaviour
    where T : class, new()
    where N : BaseHeaderData, new()
{
    protected abstract SaveLoadManager<T,N> _saveLoadManager { get; }
    protected abstract string _autoSaveName { get; }
    protected abstract Action _sendNotificationAction { get; }

    [SerializeField] private int _maxQuickSavesAmount = 5;

    public bool Enabled { get; } = true;


    protected void TrySave()
    {
        try
        {
            GetAutoSavesInfo(out SaveFileInfo<N> leastIDAutoSave, out int AutoSavesCount, out int biggestID);

            N header = new();
            header.QuickSaveID = biggestID + 1;

            string atuosaveName = $"{_autoSaveName}_{header.QuickSaveID}";

            if (!_saveLoadManager.SaveDataToFile(atuosaveName, false))
            {
                throw new Exception("Couldn't save file");
            }

            if (AutoSavesCount >= _maxQuickSavesAmount && leastIDAutoSave != null)
            {
                if (!_saveLoadManager.DeleteFile(leastIDAutoSave.FileName, leastIDAutoSave.HeaderData.IsBackup))
                {
                    throw new Exception("Couldn't delete old backup");
                }
            }
            _sendNotificationAction?.Invoke();
            return;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            SaveErrorFeedback();
        }
    }

    private void GetAutoSavesInfo(out SaveFileInfo<N> leastIDAutoSave, out int AutoSavesCount, out int biggestID)
    {
        List<SaveFileInfo<N>> backupsForThisFileName = new();
        foreach (SaveFileInfo<N> saveFileInfo in _saveLoadManager.GetMainSaves())
        {
            if (!saveFileInfo.HeaderData.IsQuickSave) continue;
            backupsForThisFileName.Add(saveFileInfo);
        }

        AutoSavesCount = backupsForThisFileName.Count;
        if (AutoSavesCount == 0)
        {
            leastIDAutoSave = null;
            biggestID = 0;
            return;
        }

        leastIDAutoSave = backupsForThisFileName.OrderBy(backup => backup.HeaderData.QuickSaveID).ToList().First();
        biggestID = backupsForThisFileName.OrderByDescending(backup => backup.HeaderData.QuickSaveID).ToList().First().HeaderData.QuickSaveID;

        return;
    }

    protected abstract void SaveErrorFeedback();
}
