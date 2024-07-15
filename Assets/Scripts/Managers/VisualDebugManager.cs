using UnityEngine;
using UnityEngine.Serialization;
using VisualDebug;
public class VisualDebugManager : MonoBehaviour, IManager, IServiceLocatorComponent, IAwake, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    
    public RemoteFunctionOperator functionOperator;
    public MonitoringManager monitoringManager;
    public LabelManager labelManager;
    private Canvas _canvas;
    
    [ServiceLocatorComponent] private PlayerManager _playerManager;
    [ServiceLocatorComponent] private CursorManager _cursorManager;
    
    [field: SerializeField] public bool Enabled { get; set; }
    [SerializeField] private KeyCode toogleVisualDebugKey;
    [SerializeField] private GameObject _visualConsolePrefab;

    private PlayerInputBlocker _playerInputBlocker;
    private bool _opened;
    
    public void CustomAwake()
    {
        if (!Enabled) return;

        var spawnedConsole = Instantiate(_visualConsolePrefab);

        functionOperator = spawnedConsole.GetComponentInChildren<RemoteFunctionOperator>();
        monitoringManager = spawnedConsole.GetComponentInChildren<MonitoringManager>();
        labelManager = spawnedConsole.GetComponentInChildren<LabelManager>();
        _canvas = spawnedConsole.GetComponentInChildren<Canvas>();

        SetCanvasVisibility(false);
    }

    public void CustomStart()
    {
        if (!Enabled) return;
        
        if (_playerInputBlocker == null)
        {
            _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _playerInputBlocker);
        }
    }

    private void Update()
    {
        if (!Enabled) return;
        
        if (Input.GetKeyDown(toogleVisualDebugKey))
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        SetCanvasVisibility(!_canvas.enabled);
        if (_opened)
        {
            if (_playerInputBlocker.TryUnblock(this))
            {
                _cursorManager.DeactivateCursor();
                _playerInputBlocker.TryUnblock(this);
            }
        }
        else
        {
            if (_playerInputBlocker.BlockersQueue.Count == 0)
            {
                PlayerInputPauseState inputPauseState = new PlayerInputPauseState(_playerInputBlocker.InputManager);
                 _playerInputBlocker.Block(new(this, inputPauseState));
            }
            _cursorManager.ActivateCursor();
        }

        _opened = !_opened;
    }
    
    private void SetCanvasVisibility(bool visibility)
    {
        _canvas.enabled = visibility;
    }
}
