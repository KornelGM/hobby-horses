using UnityEngine;

public class ShowObjectInfo : ShowAdditionalInfo
{
    // Required only if this script is not attached to the object with info
    [SerializeField] private GameObject _objectWithInfo;

    private IHasInfo _hasInfo;
    
    private void Awake()
    {
        if (TryGetComponent(out _hasInfo)) return;
        
        if (_objectWithInfo == null)
        {
            Debug.LogWarning($"Object with info is not set in {gameObject.name}");
            return;
        }
        
        if (_objectWithInfo.TryGetComponent(out _hasInfo)) return;
        
        Debug.LogWarning($"Could not find IHasInfo in {gameObject.name}");
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
    }

    public override void UpdateText()
    {
        if (AdditionalInfo == null) return;
        if (_hasInfo == null) return;

        if (AdditionalInfo.CheckCurrentShowAdditionalInfo(this))
            AdditionalInfo.SetAdditionalInfoText(_hasInfo.GetInfo());
    }
}
