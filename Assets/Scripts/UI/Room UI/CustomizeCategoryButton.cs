using UnityEngine;

public class CustomizeCategoryButton : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private CustomizationUI _customizationUI;

    [SerializeField] private HobbyHorsePart _partCategory;

    public void OnButtonDown()
    {
        _customizationUI.SetCategory(_partCategory);
    }
}
