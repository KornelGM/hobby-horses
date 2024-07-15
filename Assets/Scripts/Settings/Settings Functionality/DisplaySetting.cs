using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplaySetting : DropdownSetting
{
    private readonly List<DisplayInfo> _displays = new();
    private bool _isMovingToChosenDisplay = false;

    private ResolutionSetting _resolutionSetting;
    
    public override void Initialize(SettingsTab tab)
    {
        base.Initialize(tab);
        FillUpDropdown();
        FindResolutionSetting();
    }

    public override void Apply()
    {
        base.Apply();
        ChangeDisplay(CurrentValue);
    }

    private void FindResolutionSetting() =>
        _resolutionSetting = (ResolutionSetting)SettingsTab.GetSetting(SettingsTab.GetBaseSetting<int>(SettingName.Resolution));

    private void FillUpDropdown()
    {
        Screen.GetDisplayLayout(_displays);

        if (_displays.Count <= 1)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        
        SettingDropdown.options.Clear();

        DisplayInfo currentDisplay = Screen.mainWindowDisplayInfo;
        int currentDisplayIndex = 0;

        for (int i = 0; i < _displays.Count; i++)
        {
            DisplayInfo displayInfo = _displays[i];
            SettingDropdown.options.Add(new TMP_Dropdown.OptionData{ text = displayInfo.name} );

            if (currentDisplay.Equals(_displays[i]))
                currentDisplayIndex = i;
        }
        
        SetCurrentValue(currentDisplayIndex);
        DefaultValue = currentDisplayIndex;
    }

    private void ChangeDisplay(int indexOfDisplay)
    {
        if (!CanChangeDisplay(indexOfDisplay)) 
            return;

        if (!_isMovingToChosenDisplay)
            StartCoroutine(MoveToChosenDisplay(indexOfDisplay));
    }

    private IEnumerator MoveToChosenDisplay(int indexOfDisplay)
    {
        _isMovingToChosenDisplay = true;
        Vector2Int targetPosition = new Vector2Int();
        
        try
        {
            DisplayInfo displayToMove = _displays[indexOfDisplay];
            
            if (Screen.fullScreenMode != FullScreenMode.Windowed)
            {
                targetPosition.x = displayToMove.width / 2;
                targetPosition.y = displayToMove.height / 2;
            }

            AsyncOperation moveToChosenDisplayOperation = Screen.MoveMainWindowTo(displayToMove, targetPosition);
            yield return moveToChosenDisplayOperation;
        }
        finally
        {
            _isMovingToChosenDisplay = false;
            _resolutionSetting.MatchToAnotherDisplay();
        }
    }

    private bool CanChangeDisplay(int indexOfDisplay)
    {
        if (_displays.Count <= 0)
            return false;

        if (indexOfDisplay > _displays.Count)
            return false;

        if (Screen.mainWindowDisplayInfo.Equals(_displays[indexOfDisplay]))
            return false;
        
        return true;
    }
}