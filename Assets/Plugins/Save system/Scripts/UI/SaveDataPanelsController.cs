using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public abstract class SaveDataPanelsController<T,N> : SerializedMonoBehaviour
    where T : class, new()
    where N : BaseHeaderData, new()
{
    protected abstract SaveLoadManager<T, N> _saveLoadManager { get; }

    public GameObject MyObject => gameObject;

    public SaveDataPanel<T, N> CurrentSelectedPanel { get; private set; }
    public List<SaveFileInfo<N>> CurrentLoadedSaves { get; private set; } = new();
    public event Action<SaveDataPanel<T, N>> OnPanelSelected;
    public event Action<SaveDataPanel<T, N>> OnDoubleClicked;
    public event Action OnPanelDeselected;

    [SerializeField] private SaveDataPanel<T, N> _savePanelPrefab;
    [SerializeField] private Transform _savePanelsLayout;
    [SerializeField] private bool _showBackups = false;
    
    private IObjectPool<SaveDataPanel<T, N>> _pool;
    private List<SaveDataPanel<T, N>> _activePanels = new();

    public virtual void Awake()
    {
        _pool = new ObjectPool<SaveDataPanel<T, N>>(CreatePooledSaveDataPanel, OnTakeFromPool, OnReturnedToPool);
    }

    protected virtual void OnEnable()
    {
        OnWindowOpened();
    }

    public void RefreshPanels()
    {
        CleanupSaves();

        List<SaveFileInfo<N>> saveFileInfos = _showBackups ? _saveLoadManager.GetAllSaves() : _saveLoadManager.GetMainSaves();
        CurrentLoadedSaves = saveFileInfos;
        ShowPanels(saveFileInfos);
        DeselectPanel();
    }

    private void OnWindowOpened()
    {
        RefreshPanels();
    }

    private void ShowPanels(List<SaveFileInfo<N>> saves)
    {
        saves = saves
            .OrderBy(save => save.HeaderData.IsBackup)
            .ThenByDescending(save => save.DateTime).ToList();
        
        foreach(SaveFileInfo<N> save in saves)
        {
            ShowPanel(save);
        }
    }

    private void ShowPanel(SaveFileInfo<N> save)
    {
        SaveDataPanel<T, N> panel = _pool.Get();
        panel.Initialize(save, this);
    }

    private void CleanupSaves()
    {
        CurrentLoadedSaves.Clear();
        for (int i = _activePanels.Count - 1; i >=0 ; i--)
        {
            SaveDataPanel<T, N> panel = _activePanels[i];
            _pool.Release(panel);
        }
    }

    public void DeselectPanel()
    {
        if (CurrentSelectedPanel != null) CurrentSelectedPanel.OnDeselect();
        CurrentSelectedPanel = null;
        OnPanelDeselected?.Invoke();
    }

    public void Select(SaveDataPanel<T,N> savePanel)
    {
        if (CurrentSelectedPanel != null) CurrentSelectedPanel.OnDeselect();
        CurrentSelectedPanel = savePanel;
        CurrentSelectedPanel.OnSelect();
        OnPanelSelected?.Invoke(CurrentSelectedPanel);
    }

    public bool SaveFileExist(string saveFileName)
    {
        return _activePanels.Exists(saveDataPanel => 
        saveDataPanel != null &&
        !string.IsNullOrEmpty(saveDataPanel.SaveDataInfo.FileName) &&
        saveDataPanel.SaveDataInfo.FileName == saveFileName);
    }

    private SaveDataPanel<T, N> CreatePooledSaveDataPanel()
    {
        return Instantiate(_savePanelPrefab, _savePanelsLayout);
    }

    private void OnReturnedToPool(SaveDataPanel<T, N> panel)
    {
        _activePanels.Remove(panel);
        panel.gameObject.SetActive(false);
    }

    void OnTakeFromPool(SaveDataPanel<T, N> panel)
    {
        _activePanels.Add(panel);
        panel.gameObject.SetActive(true);
    }

    public void DoubleClickedPanel(SaveDataPanel<T, N> panel)
    {
        OnDoubleClicked?.Invoke(panel);
    }
}
