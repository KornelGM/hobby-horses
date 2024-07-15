using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AutoSave<T,N> : MonoBehaviour
    where T: class, new()
    where N: BaseHeaderData, new()
{

    protected abstract SaveLoadManager<T, N> _saveLoadManager { get; }
    protected abstract string _autoSaveName { get; }
    protected abstract Action _sendNotificationAction { get; }

    [SerializeField] protected float _secondsBetweenSaves = 600;
    [SerializeField] private int _maxAutoSavesAmount = 5;

    public virtual void CustomStart()
    {
        StartCoroutine(LoopingAutoSave());
    }

    protected virtual IEnumerator LoopingAutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(_secondsBetweenSaves);
            TrySave();
        }
    }

    protected void TrySave()
    {
        try
        {            
            GetAutoSavesInfo(out SaveFileInfo<N> leastIDAutoSave, out int AutoSavesCount, out int biggestID);

            N header = new();
            header.AutoSaveID = biggestID + 1;

            string atuosaveName = $"{_autoSaveName}_{header.AutoSaveID}";

            if (!_saveLoadManager.SaveDataToFile(atuosaveName, false))
            {
                throw new Exception("Couldn't save file");
            }

            if (AutoSavesCount >= _maxAutoSavesAmount && leastIDAutoSave != null)
            {
                if (!_saveLoadManager.DeleteFile(leastIDAutoSave.FileName, leastIDAutoSave.HeaderData.IsBackup))
                {
                    throw new Exception("Couldn't delete old backup");
                }
            }
            _sendNotificationAction?.Invoke();
            return;
        }
        catch(Exception e)
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
            if (!saveFileInfo.HeaderData.IsAutoSave) continue;
            backupsForThisFileName.Add(saveFileInfo);
        }

        AutoSavesCount = backupsForThisFileName.Count;
        if (AutoSavesCount == 0)
        {
            leastIDAutoSave = null;
            biggestID = 0;
            return;
        }

        leastIDAutoSave = backupsForThisFileName.OrderBy(backup => backup.HeaderData.AutoSaveID).ToList().First();
        biggestID = backupsForThisFileName.OrderByDescending(backup => backup.HeaderData.AutoSaveID).ToList().First().HeaderData.AutoSaveID;

        return;
    }

    protected abstract void SaveErrorFeedback();
}
