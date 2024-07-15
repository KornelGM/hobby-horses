using System.Globalization;
using TMPro;
using UnityEngine;

public class SaveDataPanel<T,N> : MonoBehaviour
       where T : class, new()
        where N : BaseHeaderData, new()
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _date;
    [SerializeField] private TextMeshProUGUI _hour; 
    [SerializeField] private TextMeshProUGUI _gameTime;
    [SerializeField] private GameObject _checkmark;

    public SaveFileInfo<N> SaveDataInfo { get; private set; }
    private SaveDataPanelsController<T,N> _saveWindow;

    public void Initialize(SaveFileInfo<N> save, SaveDataPanelsController<T, N> saveWindow)
    {
        //if (save == null) return;

        SaveDataInfo = save;
        _saveWindow = saveWindow;
        _name.text = save.FileName;

        if (save.DateTime == null)
        {
            _date.text = "Old save";
            return;
        }

        _date.text = save.DateTime.ToString("yyyy.MM.dd");
        _hour.text = save.DateTime.ToString("hh:mm tt"); 
        _checkmark.SetActive(false);

        _gameTime.text = "--:--";//TODO: Implement game time counter
    }

    //Used by button
    public void OnDoubleClick()
    {
        _saveWindow.DoubleClickedPanel(this);
    }

    //Used by button
    public void OnClick()
    {
        _saveWindow.Select(this);
    }

    public void OnSelect()
    {
        _checkmark.SetActive(true);
    }

    public void OnDeselect()
    {
        _checkmark.SetActive(false);
    }
}
