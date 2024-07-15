using UnityEngine;

public abstract class ShowAdditionalInfo : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] protected ItemInteractionTooltipManager _tooltipManager;

    public AdditionalInfo AdditionalInfo => _additionalInfo;
    public bool IsActive => _isActive;

    public bool ShowOnFocus => _showOnFocus;
    [SerializeField] private bool _showOnFocus = true;

    protected bool _isActive;
    protected AdditionalInfo _additionalInfo;

    public void CustomStart()
    {

    }
    
    public virtual void ShowInfo() 
    {
        if(_additionalInfo == null)
            _additionalInfo = _tooltipManager.AdditionalInfo;

        if (_additionalInfo == null) return;

        if (_isActive)
        {
            if (_additionalInfo.CheckCurrentShowAdditionalInfo(this))
                UpdateText();
                
            return;
        }

        _additionalInfo.SubscribeShowAdditionalInfo(this);
        UpdateText();

        _isActive = true;
        _additionalInfo.ShowAdditionalInfo();
    } 

    public virtual void HideInfo()
    {
        _isActive = false;

        if (_additionalInfo == null) return;
        if (!_additionalInfo.CheckCurrentShowAdditionalInfo(this)) return;

        _additionalInfo.HideAdditionalInfo();

    } 

    public void ChangeShowOnFocus(bool toggle) => _showOnFocus = toggle;


    public virtual void UpdateText() { }
}
