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
        public Button CreateFunctionButton(Action<Button> action, string categoryPath = "", string cardParentPath = "main",
           Color? color = null, string buttonName = "")
        {
            bool isLambda = action.GetMethodInfo().Name.Any(new char[] { '<', '>' }.Contains);

            Card cardParent = HandleCardPath(cardParentPath);

            GameObject spawnedObject = Instantiate(_remoteFuncitoButtonPrefab.gameObject, _panelParent);
            Button button = spawnedObject.GetComponent<Button>();

            button.onClick.AddListener(() => action(button));
            spawnedObject.GetComponent<Image>().color = new Color(color.Value.r, color.Value.g, color.Value.b, 1f);
            cardParent.AddChild(spawnedObject.transform);

            if (buttonName == "")
            {
                spawnedObject.GetComponentInChildren<TextMeshProUGUI>().text = isLambda ? "Button" : action.GetMethodInfo().Name;
            }
            else
            {
                spawnedObject.GetComponentInChildren<TextMeshProUGUI>().text = buttonName;
            }

            button.transform.SetParent(_panelParent);

            //change this
            AssignButtonToCategory(spawnedObject.transform, categoryPath, cardParent);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelParent.GetComponent<RectTransform>());

            DisplayCurrentCard();
            return button;
        }
        public Button CreateMethodButton(MethodInfo method, object parentObject, ParameterInfo[] parameters, string categoryPath,
            string cardPath, Color? color = null)
        {
            
            if (categoryPath != string.Empty)
            {
                categoryPath += "/";
            }
            categoryPath += method.Name;
            Card cardParent = HandleCardPath(cardPath);
            AccordionUI category = HandleCategoryPath(categoryPath.Split("/").ToArray(), cardParent);
            GameObject spawnedObject = Instantiate(_remoteFuncitoButtonPrefab.gameObject, _panelParent);
            Button button = spawnedObject.GetComponent<Button>();

            spawnedObject.GetComponent<Image>().color = new Color(color.Value.r, color.Value.g, color.Value.b, 1f);
            category.AddChild(spawnedObject.transform);
            spawnedObject.GetComponentInChildren<TextMeshProUGUI>().text = method.Name;
            List<TMP_InputField> inputFields = new();
            foreach (var par in parameters)
            {
                var panel = CreateNotConnectedPanel(par, category);
                inputFields.Add(panel.ValueField);
            }

            MethodButton methodButton = new MethodButton()
            { methodInfo = method, parentObject = parentObject, inputFields = inputFields, parameterTypes = parameters.ToList() };

            button.onClick.AddListener(() => methodButton.InvokeMethod());

            AssignButtonToCategory(button.transform,category);

            DisplayCurrentCard();
            return button;
        }
        public void AssignButtonToCategory(Transform button, string categoryPath, Card card)
        {
            if (categoryPath != string.Empty)
            {
                var category = HandleCategoryPath(categoryPath.Split("/").ToArray(), card);
                AssignButtonToCategory(button, category);
            }
        }
        public void AssignButtonToCategory(Transform button, AccordionUI category)
        {
            if (category != null)
            {
                category.AddChild(button);
            }
        }
    }
}
