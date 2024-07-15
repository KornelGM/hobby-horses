using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class DeleteFileButton<T,N> : SerializedMonoBehaviour
        where T : class, new()
        where N : BaseHeaderData, new()
{
    protected abstract SaveLoadManager<T, N> _saveLoadManager { get; }
    protected abstract SaveDataPanelsController<T,N> _controller { get; }

    [SerializeField] private Button _removeButton;

    public static event Action OnDeleteSave;

    public void Start()
    {
        _removeButton.onClick.AddListener(AskToRemoveFile);

        _controller.OnPanelSelected += OnPanelSelected;
        _controller.OnPanelDeselected += OnPanelDeselected;
    }

    public void AskToRemoveFile()
    {
        SaveDataPanel<T,N> panel = _controller.CurrentSelectedPanel;
        if (panel == null)
        {
            SetButtonInteractableness();
            return;
        }

        ShowModalWindowForDelete(panel, () => RemoveFile(panel));
    }

    protected abstract void ShowModalWindowForDelete(SaveDataPanel<T, N> panel, Action remove);

    public void RemoveFile(SaveDataPanel<T, N> panel)
    {
        if (!_saveLoadManager.DeleteFile(panel.SaveDataInfo.FileName, panel.SaveDataInfo.HeaderData.IsBackup))
        {
            ShowModalWindowForDeleteError(panel);
            return;
        };

        ShowModalWindowForSuccesfullDelete(panel);
        _controller.RefreshPanels();
        OnDeleteSave?.Invoke();
    }

    private void OnPanelDeselected()
    {
        _removeButton.interactable = false;
    }

    private void OnPanelSelected(SaveDataPanel<T,N> saveDataPanel)
    {
        if (saveDataPanel == null)
        {
            OnPanelDeselected();
            return;
        }

        _removeButton.interactable = true;
    }

    private void SetButtonInteractableness()
    {
        _removeButton.interactable = _controller.CurrentSelectedPanel != null; ;
    }

    protected abstract void ShowModalWindowForSuccesfullDelete(SaveDataPanel<T, N> panel);

    protected abstract void ShowModalWindowForDeleteError(SaveDataPanel<T, N> panel);

    public void OnDestroy()
    {
        _removeButton.onClick.RemoveListener(AskToRemoveFile);
        _controller.OnPanelSelected -= OnPanelSelected;
        _controller.OnPanelDeselected -= OnPanelDeselected;
    }
}
