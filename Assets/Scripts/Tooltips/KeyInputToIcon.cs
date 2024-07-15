using System;
using System.Collections.Generic;
using System.Linq;
using LMirman.RewiredGlyphs;
using Rewired;
using UnityEngine;

[CreateAssetMenu(fileName = "Key Input To Icon", menuName = "ScriptableObjects/Input/Key Input To Icon")]
public class KeyInputToIcon : ScriptableObject
{
    [System.Serializable]
    private class InputIconKey
    {
        public InteractionType InteractionType;
        public int ActionId = 0;
    }

    [SerializeField] private List<InputIconKey> _inputIconKeys;

    public ToolTipIcon GetIcon(InteractionType type)
    {
        InputIconKey inputIconKey = _inputIconKeys.Find(key => key.InteractionType == type);
        if (inputIconKey == null)
        {
            Debug.LogError($"There is no icon for {type}");
            return new();
        }

        Glyph foundGlyph = InputGlyphs.GetCurrentGlyph(inputIconKey.ActionId, Pole.Positive, out _);
        ToolTipIcon icon = new();

        icon.Sprite = foundGlyph.FullSprite;
        if (foundGlyph.FullSprite != InputGlyphs.NullGlyph.FullSprite)
        {
            return icon;
        }

        icon.Text = ReInput.players.GetPlayer(0).controllers.maps
            .GetAllMaps()
            .FirstOrDefault(map => map.controllerType == ControllerType.Keyboard)?
            .GetElementMapsWithAction(inputIconKey.ActionId)
            .FirstOrDefault()?
            .keyCode.ToString();
        return icon;
    }
}

public class ToolTipIcon
{
    public Sprite Sprite = null;
    public string Text = String.Empty;
}
