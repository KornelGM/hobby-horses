using LMirman.RewiredGlyphs;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class BookTooltip : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private TutorialManager _tutorialManager;

    [SerializeField] private int _actionId = 16;
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _objectToDisable;

    private ControllerType _currentControllerType;

    public void CustomStart()
    {
        if(!_tutorialManager.CanOpenBook)
        {
            SetActive(false);
        }
    }

    private void SetActive(bool active)
    {
        _objectToDisable.SetActive(active);
    }    

    private void Update()
    {
        if (!_tutorialManager.CanOpenBook)
            return;

        if (!_objectToDisable.activeInHierarchy)
            SetActive(true);

        ControllerType controllerType = ReInput.controllers.GetLastActiveController().type;

        if (controllerType == _currentControllerType)
            return;

        _currentControllerType = controllerType;
        UpdateIcon();  
    }

    private void UpdateIcon()
    {
        _icon.sprite = FindIcon();
    }

    private Sprite FindIcon()
    {
        Glyph foundGlyph = InputGlyphs.GetCurrentGlyph(_actionId, Pole.Positive, out _);

        if(foundGlyph == null)
            return null;

        return foundGlyph.FullSprite;
    }


}
