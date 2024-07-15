using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class VisualDebugger : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    protected const string slash = "/";
    [SerializeField] protected VisualDebugData visualDebugData;
    [SerializeField] protected string objectName;

    [SerializeField] private bool useLabel;

    private string _cardPath;
    protected static VisualDebugManager _debugManager = null;
    public virtual void CustomStart()
    {
        if (_debugManager == null)
        {
            if (!MyServiceLocator.TryGetServiceLocatorComponent(out _debugManager, true))
            {
                return;
            }
        }

        if (objectName == string.Empty)
        {
            objectName = "object #" + UnityEngine.Random.Range(0, 100000);
        }

        visualDebugData.IsNotNull(this, nameof(visualDebugData));

        _cardPath = visualDebugData.parentCard + slash + objectName;

        if (useLabel)
        {
            SetupLabel();
        }
    }

    private void SetupLabel()
    {
        _debugManager?.labelManager?.CreateLabelOverTransform(this.transform, objectName, objectName, followTransform: true);
    }

    private String HandleCardPath(string cardPath)
    {
        if (cardPath == string.Empty)
        {
            return _cardPath;
        }
        return _cardPath + slash + cardPath;
    }

    public void AddVariable(object parent, string variableName, string cardPath, string categoryPath)
    {
        _debugManager?.monitoringManager?.AddMonitoredVariable(parent, variableName, categoryPath, HandleCardPath(cardPath));
    }

    public void AddButton(object parent, Action<Button> action, string cardPath = "", string categoryPath = "", string buttonName = "", Color? color = null)
    {
        _debugManager?.functionOperator?.RegisterRemoteFunction(parent, action, categoryPath, HandleCardPath(cardPath), color, buttonName);
    }

    public void AddButtonForMethod(object parent, string methodName, string cardPath = "", string categoryPath = "", Color? color = null)
    {
        _debugManager?.functionOperator?.RegisterParametrizedFunction(parent, methodName, categoryPath, HandleCardPath(cardPath), color);
    }

    public void AddComboBox(Dictionary<string, Action> actionDictionary, string cardPath = "", string categoryPath = "", String boxName = "")
    {
        _debugManager?.functionOperator?.CreateComboBox(actionDictionary, categoryPath, HandleCardPath(cardPath),
                                                        boxName == string.Empty ? "Combo Box" : boxName);
    }
}
