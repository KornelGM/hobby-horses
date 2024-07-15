using UnityEngine;

public class MainMenuInicializer : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private WindowManager _windowManager;
    [SerializeField] private SaveLoadInfo<SaveData> _saveLoadInfo;
    [SerializeField] private PlayerInputReader _playerInputReader;

    private void Start()
    {
        if(MyServiceLocator.TryGetServiceLocatorComponent(out SceneSaveService saveService))
            saveService.Initialize(_saveLoadInfo?.CurrentData);

        MyServiceLocator.CustomStart();
    }

    private void OnEnable()
    {
        _playerInputReader.OnPausePerformed += TryCloseWindow;
    }

    private void OnDisable()
    {
        _playerInputReader.OnPausePerformed -= TryCloseWindow;
    }

    private void TryCloseWindow()
    {
        _windowManager.TryToCloseTopWindow();
    }

}
