using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;

public class AdditionalOutline : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    
    public bool IsEnabled => _outlinable != null && _outlinable.enabled;
    public bool CanBeEnabled => _outlinable != null && _outlinable.OutlineTargets.Count > 0;
    public bool IsInitialized { get; private set; }

    [ServiceLocatorComponent] private DreamParableLogger.Logger _logger;
    
    [SerializeField, FoldoutGroup("Settings")] private bool _initializeOnAwake = false;
    
    private Outlinable _outlinable;

    private void Awake()
    {
        _outlinable = GetComponentInChildren<AdditionalOutlineObject>(includeInactive: true)?.GetComponent<Outlinable>();
        
        if (_outlinable == null)
        {
            _logger?.Log(LogType.Error, "AdditionalOutlineController", this, 
                $"No outlinable found for {this} ({transform.parent.name})");
            
            return;
        }
        
        if (_initializeOnAwake) Initialize();
    }

    public void SetState(bool state)
    {
        if (!IsInitialized) return;
        if (!CanBeEnabled) return;
        
        _outlinable.enabled = state;
    }

    public void SetColor(Color? color)
    {
        if (!IsInitialized) return;
        if (color == null) return;
        
        _outlinable.OutlineParameters.Color = color.Value;
    }
    
    public void SetOutline(bool state = true, Color? color = null)
    {
        if (!IsInitialized)
        {
            Initialize();
        }

        if (!IsInitialized)
        {
            _logger?.Log(LogType.Error, "AdditionalOutlineController", this, 
                $"Failed to initialize {this} ({transform.parent.name})");
            
            return;
        }
        
        if (!CanBeEnabled)
        {
            _logger?.Log(LogType.Error, "AdditionalOutlineController", this, 
                $"No outline targets found for {this} ({transform.parent.name})");
            
            return;
        }
        
        SetColor(color);
        SetState(state);
    }

    public void Initialize(bool force = false)
    {
        if (IsInitialized && !force) return;
        
        if (_outlinable == null)
        {
            _logger?.Log(LogType.Error, "AdditionalOutlineController", this, 
                $"No outlinable found for {this} ({transform.parent.name})");
            
            return;
        }
        
        AddTargets(_outlinable);
        
        if (_outlinable.OutlineTargets is not { Count: > 0 })
        {
            _logger?.Log(LogType.Log, "AdditionalOutlineController", this, 
                $"No outline targets found for {this} ({transform.parent.name})");
            return;
        }
        
        IsInitialized = true;
    }

    public void RefreshTargets()
    {
        Initialize(force: true);
    }

    private void AddTargets(Outlinable outlinable)
    {
        if (!ClearTargets(outlinable)) return;
        
        Outlinable[] outlinables = GetComponentsInChildren<Outlinable>();

        if (outlinables is not { Length: > 0 }) return;
        
        foreach (Outlinable outlineableObject in outlinables)
        {
            if (outlineableObject == null) continue;
            if (outlineableObject == _outlinable) continue;

            foreach (OutlineTarget target in outlineableObject.OutlineTargets)
            {
                _outlinable.TryAddTarget(target);
            }
        }
    }
    
    private bool ClearTargets(Outlinable outlinable)
    {
        if (outlinable == null) return false;
        if (outlinable.OutlineTargets is { Count: > 0 })
        {
            foreach (OutlineTarget target in outlinable.OutlineTargets)
            {
                outlinable.RemoveTarget(target);
            }
        }
        
        return outlinable.OutlineTargets is { Count: 0 };
    }
}
