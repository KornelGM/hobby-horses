using UnityEngine;

public class CustomizeCategoryButton : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private CustomizationUI _customizationUI;

    [SerializeField] private HobbyHorsePart _partCategory;
    [SerializeField] private KeyCode _key;

    private void Update()
    {
        if (Input.GetKeyDown(_key))
            OnButtonDown();
    }

    public void OnButtonDown()
    {
        _customizationUI.SetCategory(_partCategory);
    }
}
