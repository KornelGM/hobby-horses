using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PopUpGameOver : PopUp
{
    [SerializeField] private LoadingWindow _loadingWindow;

    private void OnEnable()
    {
        CanBeClosedByManager = false;
    }

    public override void OnDeleteWindow()
    {
        
    }

    public override void OnExitButton()
    {
        pages[_currentPage].SetActive(false);
        _loadingWindow.OnMainMenuLoad();
    }
}
