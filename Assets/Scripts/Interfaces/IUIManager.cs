using UnityEngine;

public interface IUIManager
{
    public void EnableUIMap(bool state);
    public bool IsOverlayUIOpen();
    public void AddOverlayUI(GameObject uiOverlay);
    public void RemoveOverlayUI(GameObject uiOverlay);
    public void RemoveLastOverlayUI();
    public GameObject GetActiveOverlayUI();
}