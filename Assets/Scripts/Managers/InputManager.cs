using UnityEngine;
using Rewired;

public class InputManager : MonoBehaviour, IManager, IInput, IAwake, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public Player PlayerInput { get; private set; } = null;

    [field: SerializeField] public string GameplayCategoryName { get; private set; } = "Gameplay";
    [field: SerializeField] public string DefaultCategoryName { get; private set; } = "Default";
    [field: SerializeField] public string PauseCategoryName { get; private set; } = "Pause";
    [field: SerializeField] public string BookCategoryName { get; private set; } = "Book";
    [field: SerializeField] public string DesignCategoryName { get; private set; } = "Design";

    public void CustomAwake()
    {
        PlayerInput = ReInput.players.GetPlayer(0);

        PlayerInput.controllers.maps.SetMapsEnabled(true, GameplayCategoryName);

        PlayerInput.controllers.maps.SetMapsEnabled(false, DefaultCategoryName);
        PlayerInput.controllers.maps.SetMapsEnabled(false, PauseCategoryName);
        PlayerInput.controllers.maps.SetMapsEnabled(false, BookCategoryName);
        PlayerInput.controllers.maps.SetMapsEnabled(false, DesignCategoryName);
    }

    public void CustomReset()
    {
        PlayerInput = null;
    }
}
