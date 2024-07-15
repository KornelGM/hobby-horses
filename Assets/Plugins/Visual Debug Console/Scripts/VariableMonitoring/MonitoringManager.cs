using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VisualDebug
{
    public class MonitoringManager : MonoBehaviour //! uncomment for animal shelter 2
    //public class MonitoringManager : Singleton<MonitoringManager>  
    {
        [SerializeField] private float _tickRate = 0;
        [SerializeField] private VisualConsoleUI _uiManager = null;

        private Dictionary<object, List<string>> _monitoredVariables = new Dictionary<object, List<string>>();
        private float _timer = 0f;

        private const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;


        void LateUpdate()
        {
            Tick();
        }

        private void Tick()
        {
            if (_timer <= _tickRate)
            {
                _timer += Time.deltaTime;
                return;
            }

            _timer = 0f;

            foreach (KeyValuePair<object, List<string>> pair in _monitoredVariables)
            {
                UpdateAllVariablesUI(pair.Key, pair.Value);
            }
        }

        public void AddMonitoredVariable(object parentObject, string variableName, string categoryPath = "",
            string card = "main")
        {
            if(!VisualConsoleUI.CheckParent(parentObject))
            {
                return;
            }

            var panel = _uiManager.CreatePanel(variableName, parentObject);
            // AddCategory(category, card);
            //_uiManager.AssignPanelToCategory(variableName, parentObject, category);
            Card parent = _uiManager.AssignPanelToCard(variableName, parentObject, card, panel);
            if (categoryPath != string.Empty)
            {
                var parentCategory = AddCategory(categoryPath, parent);
                _uiManager.AssignPanelToCategory(variableName, parentObject, categoryPath.Split("/").ToArray(), parent,panel);
            }

            if (_monitoredVariables.ContainsKey(parentObject))
            {
                if (_monitoredVariables[parentObject].Contains(variableName))
                {
                    Debug.LogError("Variable already registered");
                    return;
                }

                _monitoredVariables[parentObject].Add(variableName);
                return;
            }

            _monitoredVariables.Add(parentObject, new List<string>());
            _monitoredVariables[parentObject].Add(variableName);
        }

        // public void ListAllFields(object parentObject)
        // {
        //     foreach (FieldInfo field in parentObject.GetType().GetFields(bindingFlags))
        //     {
        //         _uiManager.CreatePanel(field.Name, parentObject);
        //         _uiManager.AssignPanelToCategory(field.Name, parentObject, parentObject.GetType().Name);
        //         if (field.FieldType.GetInterface(nameof(IEnumerable)) == null)
        //         {
        //             DisplayPanelSimpleVariable(parentObject, field);
        //         }
        //         else
        //         {
        //             DisplayPanelCollection(parentObject, field, GetMemberEnumerator<FieldInfo>(field, parentObject));
        //         }
        //     }
        // }

        // public void ListAllProperties(object parentObject)
        // {
        //     foreach (PropertyInfo property in parentObject.GetType().GetProperties(bindingFlags))
        //     {
        //         _uiManager.CreatePanel(property.Name, parentObject);
        //         _uiManager.AssignPanelToCategory(property.Name, parentObject, parentObject.GetType().Name);
        //         if (property.PropertyType.GetInterface(nameof(IEnumerable)) == null)
        //         {
        //             DisplayPanelSimpleVariable(parentObject, property);
        //         }
        //         else
        //         {
        //             DisplayPanelCollection(parentObject, property,
        //                 GetMemberEnumerator<PropertyInfo>(property, parentObject));
        //         }
        //     }
        // }

        public AccordionUI AddCategory(string categoryPath, Card cardParent)
        {
            if (categoryPath == null) return null;
            return _uiManager.AddCategory(categoryPath, cardParent);
        }

        public void AssignVariableToCategory(object parentObject, string variableName, string categoryPath)
        {
            _uiManager.AssignPanelToCategory(variableName, parentObject, categoryPath.Split("/"));
        }

        // public void ListAll(object parentObject, string cardParent = "main")
        // {
        //     AddCategory(parentObject.GetType().Name, cardParent);
        //     ListAllFields(parentObject);
        //     ListAllProperties(parentObject);
        // }

        public void RemoveMonitoredVariable(object parentObject, object variableToWatch)
        {
            if (!_monitoredVariables.ContainsKey(parentObject))
            {
                Debug.LogError("No such object registered");
                return;
            }

            if (!_monitoredVariables[parentObject].Contains(nameof(variableToWatch)))
            {
                Debug.LogError("No such variable registered to watch");
                return;
            }

            _monitoredVariables[parentObject].Remove(nameof(variableToWatch));
        }

        private void UpdateAllVariablesUI(object parentObject, List<string> variableNames)
        {
            foreach (string variableName in variableNames)
            {
                FieldInfo field = GetField(parentObject, variableName);
                PropertyInfo propertyInfo = GetPropertyInfo(parentObject, variableName);

                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType.GetInterface(nameof(IEnumerable)) == null||propertyInfo.PropertyType == typeof(string))
                    {
                        DisplayPanelSimpleVariable(parentObject, propertyInfo);
                    }
                    else
                    {
                        DisplayPanelCollection(parentObject, propertyInfo,
                            GetMemberEnumerator<PropertyInfo>(propertyInfo, parentObject));
                    }
                }

                if (field == null) continue;
                if (field.FieldType.GetInterface(nameof(IEnumerable)) == null||field.FieldType==typeof(string))
                {
                    DisplayPanelSimpleVariable(parentObject, field);
                }
                else
                {
                    DisplayPanelCollection(parentObject, field, GetMemberEnumerator<FieldInfo>(field, parentObject));
                }
            }
        }

        private void DisplayPanelSimpleVariable(object parentObject, FieldInfo field)
        {
            object fieldValue = field.GetValue(parentObject);
            _uiManager.UpdatePanel(field.Name, fieldValue, parentObject);
        }

        private void DisplayPanelSimpleVariable(object parentObject, PropertyInfo property)
        {
            object fieldValue = property.GetValue(parentObject);
            _uiManager.UpdatePanel(property.Name, fieldValue, parentObject);
        }


        private void DisplayPanelCollection(object parentObject, MemberInfo property, IEnumerator fieldList)
        {
            string stringToPanel = "[ ";

            while (fieldList.MoveNext())
            {
                string valueToString = System.Convert.ToString(fieldList.Current);
                stringToPanel += $"{valueToString},";
            }

            stringToPanel += " ]";
            fieldList.Reset();

            _uiManager.UpdatePanel(property.Name, stringToPanel, parentObject);
        }

        private IEnumerator GetMemberEnumerator<T>(T property, object parentObject)
        {
            if (typeof(T) == typeof(FieldInfo))
            {
                FieldInfo field = property as FieldInfo;
                return ((IEnumerable)field.GetValue(parentObject)).GetEnumerator();
            }

            if (typeof(T) == typeof(PropertyInfo))
            {
                PropertyInfo propertyInfo = property as PropertyInfo;
                return ((IEnumerable)propertyInfo.GetValue(parentObject)).GetEnumerator();
            }

            Debug.LogError("Getting Ienumerator from member unsuccesful");
            return null;
        }

        PropertyInfo GetPropertyInfo(object parentObject, string variableName)
        {
            PropertyInfo property = parentObject.GetType().GetProperty(variableName, bindingFlags);

            if (property != null) return property;
            return null;
        }

        FieldInfo GetField(object parentObject, string variableName)
        {
            FieldInfo field = parentObject.GetType().GetField(variableName, bindingFlags);

            if (field != null) return field;
            return null;
        }
    }
}
