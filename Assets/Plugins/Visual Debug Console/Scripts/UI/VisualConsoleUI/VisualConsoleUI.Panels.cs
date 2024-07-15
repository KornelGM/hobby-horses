using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace VisualDebug
{
    public partial class VisualConsoleUI
    {
        public PanelConnectedWithValue CreatePanel(string propertyName, object parentObject, object value = null)
        {
            PanelConnectedWithValue foundPanel = _panels.Find(x => x.PropertyName == propertyName && x.ParentObject == parentObject);

            if (foundPanel != null) return foundPanel;

            GameObject panel = Instantiate(_propertyPanel, _panelParent);
            PanelConnectedWithValue createdPanel = new PanelConnectedWithValue()
            { Panel = panel, PropertyName = propertyName };

            createdPanel.ValueField = panel.transform.Find("Value Text").GetComponent<TMP_InputField>();
            createdPanel.ValueField.onSubmit.AddListener((string input) =>
            {
                TrySetValue(parentObject, propertyName, input);
            });

            FieldInfo fieldInfo = parentObject.GetType().GetField(propertyName, bindingFlags);
            if (fieldInfo != null && !AllowedTypes.Contains(fieldInfo.FieldType))
            {
                createdPanel.ValueField.interactable = false;
            }

            PropertyInfo propertyInfo = parentObject.GetType().GetProperty(propertyName, bindingFlags);
            if (propertyInfo != null && !AllowedTypes.Contains(propertyInfo.PropertyType))
            {
                createdPanel.ValueField.interactable = false;
            }

            createdPanel.NameText = panel.transform.Find("Name Text").GetComponent<TextMeshProUGUI>();
            createdPanel.NameText.text = propertyName;

            createdPanel.ParentObject = parentObject;
            _panels.Add(createdPanel);
            DisplayCurrentCard();
            return createdPanel;
        }

        private PanelConnectedWithValue CreateNotConnectedPanel(ParameterInfo parameterInfo, AccordionUI parent)
        {
            GameObject panel = Instantiate(_propertyPanel, _panelParent);
            PanelConnectedWithValue createdPanel = new PanelConnectedWithValue()
            { Panel = panel };
            createdPanel.ValueField = panel.transform.Find("Value Text").GetComponent<TMP_InputField>();
            createdPanel.NameText = panel.transform.Find("Name Text").GetComponent<TextMeshProUGUI>();
            createdPanel.NameText.text = ConvertTypeToString(parameterInfo.ParameterType) +" "+ parameterInfo.Name;
            parent.AddChild(panel.transform);
            return createdPanel;
        }
        private String ConvertTypeToString(Type type)
        {
            Dictionary<Type, string> TypeAlliases = new Dictionary<Type, string>
            {
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(string), "string" },
            };

            return TypeAlliases.GetValueOrDefault(type, type.ToString());
        }

        public Card AssignPanelToCard(string propertyName, object parentObject, string cardName, PanelConnectedWithValue panel = null)
        {
            Card parent = HandleCardPath(cardName);
            if (panel == null)
            {
                panel = GetPanel(propertyName, parentObject);
            }

            if (parent == null) return null;
            parent.AddChild(panel.Panel.transform);
            DisplayCard(_currentCard);
            return parent;
        }

        public void AssignPanelToCategory(string propertyName, object parentObject, string[] categoryPath, Card parentCard = null,
                 PanelConnectedWithValue panel = null)
        {

            if (parentCard == null)
            {
                parentCard = _cardsTree.root.item;
            }
            if (panel == null)
            {
                panel = GetPanel(propertyName, parentObject);
            }

            if (categoryPath.Length == 0)
            {
                panel.Panel.transform.SetParent(_panelParent);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_panelParent.GetComponent<RectTransform>());
                return;
            }

            AccordionUI parent = HandleCategoryPath(categoryPath, parentCard);
            if (parent == null) return;
            parent.AddChild(panel.Panel.transform);

        }
        public void AssignPanelToCategory(string propertyName, object parentObject, AccordionUI category)
        {

            // if (rootParent == null) return;

            // rootParent.AddChild(GetPanel(propertyName, parentObject).Panel.transform);

            // foreach (var pathFragment in path)
            // {
            //     GetCategoryByName(pathFragment).AddChild(GetPanel(propertyName, parentObject).Panel.transform);
            // }
        }
        private PanelConnectedWithValue GetPanel(string propertyName, object parentObject)
        {
            PanelConnectedWithValue toReturn =
                _panels.FirstOrDefault(x => x.PropertyName == propertyName && x.ParentObject == parentObject);
            return toReturn ?? null;
        }

        public void UpdatePanel(string propertyName, object value, object parentObject)
        {
            string valueToString = System.Convert.ToString(value);

            var panel = GetPanel(propertyName, parentObject);

            if (panel == null) return;

            if (panel.ValueField.isFocused) return;

            panel.ValueField.text = valueToString;
        }

        private void TrySetValue(object parentObject, string variableName, string value)
        {
            if (TrySetFieldValue(parentObject, variableName, value)) return;
            TrySetPropertyValue(parentObject, variableName, value);
        }

        private bool TrySetFieldValue(object parentObject, string variableName, string value)
        {
            FieldInfo fieldInfo = parentObject.GetType().GetField(variableName, bindingFlags);
            if (fieldInfo != null && AllowedTypes.Contains(fieldInfo.FieldType))
            {
                fieldInfo.SetValue(parentObject, Convert.ChangeType(value, fieldInfo.FieldType));
                return true;
            }

            return false;
        }

        private bool TrySetPropertyValue(object parentObject, string variableName, string value)
        {
            PropertyInfo propertyInfo = parentObject.GetType().GetProperty(variableName, bindingFlags);

            if (propertyInfo == null || !AllowedTypes.Contains(propertyInfo.PropertyType)) return false;
            propertyInfo.SetValue(parentObject, Convert.ChangeType(value, propertyInfo.PropertyType));
            return true;
        }
    }
}
