
using UnityEngine;

public class GameplayInitializer : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private SceneSaveService _saveService;
    [ServiceLocatorComponent] private DreamParableLogger.Logger _logger;

    [SerializeField] private SaveLoadInfo<SaveData> _saveLoadInfo;
    [SerializeField] private SteamData _steamData;

    private void Awake()
    {
        SetAppID(_steamData.GetID());
    }

    public void Start()
    {
        if (_saveService != null) _saveService.Initialize(_saveLoadInfo?.CurrentData);
        MyServiceLocator.CustomStart();
        
        CollectSystemInfo();
    }

    public void SetAppID(uint id)
    {
        SteamManager steamManager = SteamManager.Instance;
        if (steamManager == null)
        {
            Debug.LogException(new System.Exception("Cannot initialized SteamManager"));
            Application.Quit();
            return;
        }

        steamManager.CheckAppID(id);
    }

    private void CollectSystemInfo()
    {
        if (_logger == null)
        {
            Debug.LogWarning("Logger is not initialized in GameplayInitializer");
            return;
        }
        
        string systemInfo =
            $"Operating system: {SystemInfo.operatingSystem}, {SystemInfo.operatingSystemFamily}\n" +
            $"Processor: {SystemInfo.processorType}, {SystemInfo.processorFrequency}Hz, System memory size {SystemInfo.systemMemorySize}MB\n" +
            $"Graphics card: {SystemInfo.graphicsDeviceName}, ID {SystemInfo.graphicsDeviceID}, {SystemInfo.graphicsDeviceType}, " +
            $"{SystemInfo.graphicsDeviceVersion}v, {SystemInfo.graphicsMemorySize}MB, shader level: {SystemInfo.graphicsShaderLevel}";

        _logger?.Log(LogType.Log, "System Information", this, "System Info", systemInfo + "\r\n");
        _logger?.Log(LogType.Log, "Game Version", this, $"Game Version: {Application.version}");
    }
}
