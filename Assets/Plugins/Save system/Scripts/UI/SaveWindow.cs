using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class SaveWindow<T,N> : SerializedMonoBehaviour
        where T : class, new()
        where N : BaseHeaderData, new()
{
    protected abstract SaveLoadManager<T, N> _saveLoadManager { get; }
    protected abstract SaveDataPanelsController<T, N> _controller { get; }

    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int _maxBackupsAmount = 2;
    [SerializeField] private Button _saveFileWithNameButton;
    [SerializeField] private Button _resaveFileButton;

    private string GetBackupName(string fileName,  int id, string suffix = "Backup") => $"{fileName}_{suffix}_{id}";

    public void Start()
    {
        SetButtonInteractableness();
        _controller.OnPanelSelected += OnPanelSelected;
        _controller.OnPanelDeselected += OnPanelDeselected;
        _controller.OnDoubleClicked += OnDoubleClickedPanel;
        _inputField.onValueChanged.AddListener(OnInputFieldChanged);
        _saveFileWithNameButton.onClick.AddListener(SaveWithInputField);
        _resaveFileButton.onClick.AddListener(ResaveSelectedFile);
    }

    //Used by button
    public void ResaveSelectedFile()
    {
        ResaveFile(_controller.CurrentSelectedPanel);
    }

    //Used by button
    public void SaveWithInputField()
    {
        AskToSaveFile(_inputField.text, Cleanup, () => OnAlreadyNameExists(() => TryToSaveFile(_inputField.text, true, null)));      
    }

    protected abstract void OnAlreadyNameExists(Action action);

    void Cleanup() => _inputField.text = "";

    private void ResaveFile(SaveDataPanel<T, N> panel)
    {
        if (panel == null) return;

        AskToSaveFile(panel.SaveDataInfo.FileName, null, () => OnAlreadyExists(() => TryToSaveFile(panel.SaveDataInfo.FileName, true, null)));
    }

    protected abstract void OnAlreadyExists(Action action);


    private void SetButtonInteractableness()
    {
        _resaveFileButton.interactable = _controller.CurrentSelectedPanel != null;
        _saveFileWithNameButton.interactable = !string.IsNullOrEmpty(_inputField.text);
    }

    private void OnPanelDeselected()
    {
        _resaveFileButton.interactable = false;
    }

    private void OnInputFieldChanged(string value)
    {
        _saveFileWithNameButton.interactable = !string.IsNullOrEmpty(value);
    }

    private void OnPanelSelected(SaveDataPanel<T, N> saveDataPanel)
    {
        if (saveDataPanel == null)
        {
            OnPanelDeselected();
            return;
        }

        _resaveFileButton.interactable = true;
    }

    private void OnDoubleClickedPanel(SaveDataPanel<T, N> saveDataPanel)
    {
        ResaveFile(saveDataPanel);
    }

    private void AskToSaveFile(string fileName, Action onSuccess = null, Action onAlreadyExist = null)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        if (_controller.SaveFileExist(fileName))
        {
            onAlreadyExist?.Invoke();
            return;
        }

        TryToSaveFile(fileName, false, onSuccess);
    }

    private void TryToSaveFile(string fileName, bool createBackup, Action onSuccess = null)
    {
        try
        {
            if (createBackup)
            {
                if (!TryCreateBackup(fileName)) throw new Exception("Unexpected error. Couldn't handle backup");
            }
            _saveLoadManager.SaveDataToFile(fileName, false);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowModalForSaveError();
            return;
        }

        onSuccess?.Invoke();
        _controller.RefreshPanels();

        //Do you want to inform player about successfuly saved file?
        //_modalWindow.CreateModalWindowOK(_controller.RefreshPanels, "Saved successfuly");
    }

    protected abstract void ShowModalForSaveError();
  

    private bool TryCreateBackup(string fileName)
    {
        try
        {
            (T saveData, SaveFileInfo<N> saveFileInfo) = _saveLoadManager.GetSaveDataFile(fileName, false);

            if (saveFileInfo == null) throw new Exception($"Couldn't read header of type {typeof(SaveFileInfo<T>)} for purpose of creating backup");
            if (saveData == null) throw new Exception($"Couldn't read {typeof(T)} for purpose of creating backup");

            GetBackupsInfo(fileName, out SaveFileInfo<N> leastIDBackup, out int backupsCount, out int biggestID);

            saveFileInfo.HeaderData.PrimeBackupsFileName = fileName;
            saveFileInfo.HeaderData.BackupID = biggestID + 1;

            string backupName = GetBackupName(fileName, saveFileInfo.HeaderData.BackupID);

            if (!_saveLoadManager.SaveDataToFile(backupName, saveData, saveFileInfo.HeaderData, true))
            {
                throw new Exception("Couldn't save file");
            }

            if (backupsCount >= _maxBackupsAmount && leastIDBackup != null)
            {
                if(!_saveLoadManager.DeleteFile(leastIDBackup.FileName, leastIDBackup.HeaderData.IsBackup))
                {
                    throw new Exception("Couldn't delete old backup");
                }
            }
            return true;
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    private void GetBackupsInfo(string fileName, out SaveFileInfo<N> leastIDBackup, out int backupsCount, out int biggestID)
    {
        List<SaveFileInfo<N>> backupsForThisFileName = new();
        foreach (SaveFileInfo<N> saveFileInfo in _saveLoadManager.GetAllSaves())
        {
            if (!saveFileInfo.HeaderData.IsBackup) continue;
            if (saveFileInfo.HeaderData.PrimeBackupsFileName != fileName) continue;
            backupsForThisFileName.Add(saveFileInfo);
        }

        backupsCount = backupsForThisFileName.Count;
        if(backupsCount == 0)
        {
            leastIDBackup = null;
            biggestID = 0;
            return;
        }

        leastIDBackup = backupsForThisFileName.OrderBy(backup => backup.HeaderData.BackupID).ToList().First();
        biggestID = backupsForThisFileName.OrderByDescending(backup => backup.HeaderData.BackupID).ToList().First().HeaderData.BackupID;

        return;
    }

    private void OnDestroy()
    {
        _controller.OnPanelSelected -= OnPanelSelected;
        _controller.OnPanelDeselected -= OnPanelDeselected;
        _controller.OnDoubleClicked -= OnDoubleClickedPanel;
    }
}
