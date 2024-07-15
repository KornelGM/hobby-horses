using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace VisualDebug
{
    public class RemoteFunctionOperator : MonoBehaviour //!uncomment for animal shelter 2
    //public class RemoteFunctionOperator : Singleton<RemoteFunctionOperator>
    {
        [SerializeField] private VisualConsoleUI _uiManager = null;
        private Dictionary<object, List<Action<Button>>> _actions = new Dictionary<object, List<Action<Button>>>();

        public void CreateComboBox(Dictionary<string, Action> boxDictionary, string categoryPath = "", string cardPath = "main", string boxName = "ComboBox")
        {
            _uiManager.CreateComboBox(boxDictionary, categoryPath, cardPath, boxName);
        }

        public Button RegisterRemoteFunction(object parentObject, Action<Button> remoteFunction, string categoryPath = "", string cardPath = "main", Color? color = null, String buttonName = "Button")
        {
            if(!VisualConsoleUI.CheckParent(parentObject))
            {
                return null;
            }

            color ??= new Color(0.4046991f, 0.7855105f, 0.9245283f, 1f);

            if (!_actions.ContainsKey(parentObject))
            {
                _actions.Add(parentObject, new List<Action<Button>>() { remoteFunction });
            }
            else
            {
                _actions[parentObject].Add(remoteFunction);
            }

            return _uiManager.CreateFunctionButton(remoteFunction, categoryPath, cardPath, color, buttonName);
        }
        public Button RegisterParametrizedFunction(object parentObject, string methodName, string categoryPath = "", string cardPath = "main", Color? color = null)
        {
            if(!VisualConsoleUI.CheckParent(parentObject))
            {
                return null;
            }


            color ??= new Color(0.8f,0,0.8f, 1f);
            MethodInfo methodInfo = parentObject.GetType().GetMethod(methodName);
            if (methodInfo == null)
            {
                Debug.Log($"Could not find method with name{methodName} in {parentObject.GetType()}");
                return null;
            }
            ParameterInfo[] parameters = methodInfo.GetParameters();

            foreach (var par in parameters)
            {
                if (!VisualConsoleUI.AllowedTypes.Contains(par.ParameterType))
                {
                    Debug.Log($"One or more parameters from {methodName} method is not in allowed type now allowed type: {par.ParameterType}");
                    return null;
                }
            }

            return _uiManager.CreateMethodButton(methodInfo,parentObject, parameters, categoryPath, cardPath, color);
        }
    }
}
