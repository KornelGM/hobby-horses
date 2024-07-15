using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallParent : MonoBehaviour
{
    List<ChangeColorAction> _colorChangers = new List<ChangeColorAction>();

    private void Awake()
    {
        _colorChangers = GetComponentsInChildren<ChangeColorAction>().ToList();
    }

    private void OnEnable()
    {
        _colorChangers.ForEach(colorChanger => colorChanger.OnColorChanged += ColorChanged);
    }

    private void OnDisable()
    {
        foreach (ChangeColorAction action in _colorChangers)
        {
            if (action == null) continue;
            action.OnColorChanged -= ColorChanged;
        }
    }

    void ColorChanged(ChangeColorAction actionRef, int index)
    {
        foreach (ChangeColorAction action in _colorChangers)
        {
            if (action == actionRef) continue;
            action.ChangeWithoutNotify(index);
        }
    }
}
