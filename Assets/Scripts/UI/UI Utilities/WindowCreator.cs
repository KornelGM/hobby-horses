using UnityEngine;

public class WindowCreator : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private WindowManager _windowManager;

    [SerializeField] private GameObject _window;

    private GameObject _currentWindow;

    public void CreateWindow()
    {
        if (_currentWindow != null) return;
        if(_window.TryGetComponent(out IWindow window))
            _windowManager.CreateWindow(window, WindowManager.PauseMenuBasePriority);
    }
}
