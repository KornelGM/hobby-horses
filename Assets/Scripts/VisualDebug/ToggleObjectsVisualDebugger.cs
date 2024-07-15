using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ToggleObjectsVisualDebugger : VisualDebugger
{
    [Serializable]
    private class ToggleableGameObject
    {
        public string Name => _name;
        public List<GameObject> GameObjects => _gameObjects;

        [SerializeField] private string _name;
        [SerializeField] private List<GameObject> _gameObjects;
    }

    [SerializeField, FoldoutGroup("Visuals")] private Color _enabled = Color.green;
    [SerializeField, FoldoutGroup("Visuals")] private Color _disabled = Color.red;
    
    [SerializeField, FoldoutGroup("Game Objects")] private List<ToggleableGameObject> _toggleableGameObjects = new();

    [ServiceLocatorComponent] private WindowManager _windowManager;

    private bool hudDisabled = false;
    
    public override void CustomStart()
    {
        base.CustomStart();
        
        ToggleableObject[] toggleableObjects = FindObjectsByType<ToggleableObject>(FindObjectsSortMode.None);

            AddButton(
                parent: this,
                action: button => ToggleHUD(),
                cardPath: "",
                buttonName: "HUD",
                color: GetColor(!hudDisabled));

        if (toggleableObjects.Length > 0)
        {
            foreach (ToggleableObject toggleable in toggleableObjects)
            {
                AddButton(
                    parent: this,
                    action: button => ToggleObject(toggleable, button),
                    cardPath: "",
                    buttonName: toggleable.name,
                    color: GetColor(toggleable.Enabled));
            }
        }

        if (_toggleableGameObjects.Count > 0)
        {
            foreach (ToggleableGameObject toggleable in _toggleableGameObjects)
            {
                AddButton(
                    parent: this,
                    action: button => ToggleObjects(toggleable, button),
                    cardPath: "",
                    buttonName: toggleable.Name,
                    color: GetColor(toggleable.GameObjects[0].activeSelf));
            }
        }
    }

    private void ToggleHUD()
    {
        _windowManager.ToggleMainCanvas(hudDisabled);
        hudDisabled = !hudDisabled;
    }

    private void ToggleObject(ToggleableObject toggleableObject, Button button)
    {
        toggleableObject.Toggle();
        button.image.color = GetColor(toggleableObject.Enabled);
    }
    
    private void ToggleObjects(ToggleableGameObject toggleableGameObject, Button button)
    {
        foreach (GameObject toggleable in toggleableGameObject.GameObjects)
        {
            toggleable.SetActive(!toggleable.activeSelf);
            button.image.color = GetColor(toggleable.activeSelf);
        }
    }
    
    private Color GetColor(bool isEnabled)
    {
        return isEnabled ? Color.green : Color.red;
    }
}
