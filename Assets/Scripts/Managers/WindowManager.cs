using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.Utilities;
using UnityEngine;
using VisualDebug;

public class WindowManager : MonoBehaviour, IManager, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private CursorManager _cursorManager;

    [SerializeField] private GameObject _mainCanvasPrefab;
    [SerializeField] private ModalWindowController modalWindowPrefab;

    public GameObject MainCanvas { get; private set; }
    private List<IWindow> _windows = new List<IWindow>();
    private Canvas _canvas;
    
    public static int PopUpBasePriority = 10;
    public static int PauseMenuBasePriority = 50;
    public static int ModalBasePriority = 100;
    public void CustomReset()
    {
        for (int i = _windows.Count - 1; i >= 0; i--)
        {
            DeleteWindow(_windows[i]);
        }
    }

    private void TryToInitializeMainCanvas()
    {
        if(MainCanvas == null) MainCanvas = Instantiate(_mainCanvasPrefab);
        _canvas = MainCanvas.GetComponent<Canvas>();
    }

    public void SetMainCanvasOrder(int order = 50)
    {
        if (_canvas == null)
            return;

        _canvas.sortingOrder = order;
    }

    public GameObject CreateWindow(IWindow window, int priority = 0)
    {
        TryToInitializeMainCanvas();
        GameObject newPanel = Instantiate(window.MyObject, MainCanvas.transform);
        IWindow newWindow = newPanel.GetComponent<IWindow>();
        newWindow.Priority = priority;
        newWindow.Manager = this;
        _windows.Add(newWindow);
        
        if(window.ShouldActivateCursor) _cursorManager.ActivateCursor();
        
        DetermineTopWindows();
        HandleHidingCursor();
        newPanel.SetActive(true);
        return newPanel;
    }

    public GameObject RegisterExistingWindow(IWindow existingWindow, bool setActive = false)
    {
        if (existingWindow == null) return null;
        if (existingWindow.MyObject == null)
        {
            Debug.LogError("Window you're trying to register does not exist.");
            return null;
        }
        existingWindow.Manager = this;

        existingWindow.MyObject.SetActive(setActive);
        _windows.Add(existingWindow);

        TryToInitializeMainCanvas();
        existingWindow.MyObject.transform.SetParent(MainCanvas.transform);

        return existingWindow.MyObject;
    }

    public void DeleteWindow(IWindow window)
    {
        if (window.Equals(null)) return;
        
        window.OnDeleteWindow();
        _windows.Remove(window);
        
        if (window.MyObject != null) Destroy(window.MyObject);
        
        DetermineTopWindows();
        HandleHidingCursor();
    }

    public IWindow DetachWindow(IWindow window)
    {
        _windows.Remove(window);
        return window;
    }
    public bool TryToCloseTopWindow()
    {
        IWindow topWindow = TopPriorityWindow();
        
        if (topWindow == null) return false;
        if (topWindow.CanBeClosedByManager == false) return false;        
        
        topWindow.DeleteWindow();
        return true;
    }

    [CanBeNull]
    public IWindow TopPriorityWindow()
    {
        if(_windows.Count == 0) return null;
        int maxPriority = _windows.Max(window => window.Priority);
        return _windows.FirstOrDefault(window => window.Priority == maxPriority);
    }

    public void DetermineTopWindows()
    {
        if (_windows.Count == 0) return;
        _windows = _windows.Where(window => window?.MyObject != null).ToList();

        _windows.ForEach(window => window.IsOnTop = false);
        
        _windows
            .OrderByDescending(window => window.Priority)
            .ForEach(window => window.MyObject.transform.SetAsFirstSibling());

        int maxPriority = _windows.Max(window => window.Priority);
        _windows
            .Where(window => window.Priority == maxPriority)
            .ForEach(window => window.IsOnTop = true);
    }
    
    private void HandleHidingCursor()
    {
        IWindow topPriorityWindow = TopPriorityWindow();
        if (topPriorityWindow == null || topPriorityWindow.ShouldActivateCursor == false)
        {
            _cursorManager.DeactivateCursor();
        }
    }

    public void ToggleMainCanvas(bool toggleValue)
    {
        IWindow[] transforms = MainCanvas.GetComponentsInChildren<IWindow>();

        foreach (var window in transforms)
        {
            StartCoroutine(ScaleCoroutine(window.MyObject.transform, toggleValue ? Vector3.one : Vector3.zero, 0.1f));
        }
    }

    private IEnumerator ScaleCoroutine(Transform transform, Vector3 finalScale, float duration)
    {
        float time = 0;
        while (time <= duration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, finalScale, (time / duration));

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = finalScale;
    }
}