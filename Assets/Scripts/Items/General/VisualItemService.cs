using System.Collections.Generic;
using UnityEngine;
using EPOOutline;
using System.Linq;
using Sirenix.Utilities;

public class VisualItemService : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

    public GameObject Model => _models;

    [SerializeField] private GameObject _models;
    [SerializeField] private GameObject _visuals; 

    private List<Outlinable> _outlines = new List<Outlinable>();

    public void CustomAwake()
    {
        GetAllOutlinables();
    }

    public void Toggle(bool enabled)
    {
        ToggleModels(enabled);
        ToggleVisuals(enabled);
        ToggleOutlines(enabled);
    }

    public void ToggleModels(bool enabled)
    {
        _models.SetActive(enabled);
    }

    public void ToggleVisuals(bool enabled)
    {
        _visuals.SetActive(enabled);
    }

    public void ToggleOutlines(bool enabled)
    {
        if(_outlines is { Count: > 0})
            _outlines.Where(outline => outline != null).ForEach(outline => outline.enabled = enabled);
    }

    public void ToggleOutlineColor(Color color)
    {
        if (_outlines is { Count: > 0 })
            _outlines.Where(outline => outline != null).ForEach(outline => outline.OutlineParameters.Color = color);
    }

    public void GetAllOutlinables()
    {
        _outlines = new(GetComponentsInChildren<Outlinable>().ToList());
    }
}
