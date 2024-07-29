using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomHobbyHorse : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private HobbyHorseCustomizationManager _customizeManager;

    public CustomizationHolder[] CustomizationHolders => _customizationHolders;

    public string[] HobbyHorsePartsGuids
    {
        get
        {
            List<string> parts = new();
            foreach (var holder in _customizationHolders)
            {
                if (holder.CurrentPart == null)
                    continue;

                parts.Add(holder.PartInfo.Guid);
            }
            return parts.ToArray();
        }
    }

    [SerializeField, FoldoutGroup("Holders")] private CustomizationHolder[] _customizationHolders;

    public void LoadAppearance(string[] loadGuids)
    {
        foreach (string guid in loadGuids)
        {
            HobbyHorseCustomizationPartInfo loadedPart = _customizeManager.HobbyHorseParts.
                FirstOrDefault(part => part.Guid == guid);

            if (loadedPart == null) continue;

            ChangeHobbyHorsePart(loadedPart);
        }
    }

    public void LoadDefaultAppearance()
    {
        foreach (var category in _customizeManager.HobbyHorsePartsCategories)
        {
            ChangeHobbyHorsePart(category.CustomizationPart[0]);
        }
    }

    public void ChangeHobbyHorsePart(HobbyHorseCustomizationPartInfo newPart)
    {
        CustomizationHolder holder = _customizationHolders.FirstOrDefault(x => x.HobbyHorsePart == newPart.HobbyHorsePart);
        if (holder == null) return;

        if (holder.CurrentPart != null)
            Destroy(holder.CurrentPart.gameObject);

        HobbyHorseCustomize createdPart = Instantiate(newPart.HobbyHorsePartPrefab, holder.Holder);
        holder.SetCurrentPart(newPart, createdPart);
    }

}

[Serializable]
public class CustomizationHolder
{
    public HobbyHorsePart HobbyHorsePart;
    public Transform Holder;
    public HobbyHorseCustomize CurrentPart => _currentPart;
    private HobbyHorseCustomize _currentPart;

    public HobbyHorseCustomizationPartInfo PartInfo => _currentPartInfo;
    private HobbyHorseCustomizationPartInfo _currentPartInfo;

    public void SetCurrentPart(HobbyHorseCustomizationPartInfo partInfo, HobbyHorseCustomize newPart)
    {
        _currentPart = newPart;
        _currentPartInfo = partInfo;
    }
}
