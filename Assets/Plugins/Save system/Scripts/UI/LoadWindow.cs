using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class LoadWindow<T,N> : SerializedMonoBehaviour
            where T : class, new()
        where N : BaseHeaderData, new()
{
    protected abstract SaveLoadManager<T, N> _saveLoadManager { get; }
    protected abstract SaveDataPanelsController<T, N> _controller { get; }

    [SerializeField] protected SaveLoadInfo<T> _saveLoadInfo;
    [SerializeField] private Button _loadButton;

    public virtual void Start()
    {
        _controller.OnDoubleClicked += OnDoubleClickedPanel;
        _controller.OnPanelDeselected += OnPanelDeselected;
        _controller.OnPanelSelected += OnPanelSelected;
        OnPanelSelected(_controller.CurrentSelectedPanel);
    }

    //Used by button
    public void AskToLoadSelected()
    {
        AskToLoad(_controller.CurrentSelectedPanel);
    }

    private void OnPanelDeselected()
    {
        _loadButton.interactable = false;
    }

    private void OnPanelSelected(SaveDataPanel<T, N> panel)
    {
        if(panel == false)
        {
            OnPanelDeselected();
            return;
        }

        _loadButton.interactable = true;
    }

    private void AskToLoad(SaveDataPanel<T, N> panel)
    {
        if (panel == null)
        {
            ShowModalForUnSelectingFile();
            return;
        }

        bool isMainMenu = SceneManager.GetActiveScene().name == "Main Menu";

        if(isMainMenu)
        {
            TryLoadSelected(panel);
        }
        else
        {
            ShowModalForBeSureToLoad(panel, () => TryLoadSelected(panel));
        }
    }

    protected abstract void ShowModalForBeSureToLoad(SaveDataPanel<T, N> panel, Action action);

    protected abstract void ShowModalForUnSelectingFile();

    private void OnDoubleClickedPanel(SaveDataPanel<T,N> saveDataPanel)
    {
        AskToLoad(saveDataPanel);
    }

    protected virtual void TryLoadSelected(SaveDataPanel<T, N> panel)
    {
        SaveFileInfo<N> fileInfo = panel.SaveDataInfo;
        _saveLoadInfo.CurrentData = _saveLoadManager.GetSave(fileInfo.FileName, fileInfo.HeaderData.IsBackup);
    }

    protected abstract string GetSceneName(T saveData);

    private void OnDestroy()
    {
        _controller.OnDoubleClicked -= OnDoubleClickedPanel;
        _controller.OnPanelDeselected -= OnPanelDeselected;
        _controller.OnPanelSelected -= OnPanelSelected;
    }

    public abstract void DeleteWindow();
}
