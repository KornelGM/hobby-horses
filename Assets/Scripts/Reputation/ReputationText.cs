using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ReputationText : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

	[ServiceLocatorComponent] private ReputationManager _reputationManager;

	private TextMeshProUGUI _text;

    public void CustomAwake()
	{
		_text = GetComponent<TextMeshProUGUI>();
		SubscribeEvents();
		RefreshText(_reputationManager.Reputation);
	}

    private void OnEnable()
    {
        RefreshText(_reputationManager.Reputation);
    }

    private void OnDestroy()
    {
		UnsubscribeEvents();
	}

    private void SubscribeEvents()
	{
		_reputationManager.OnReputationChanged += RefreshText;
	}

	private void UnsubscribeEvents()
	{
		_reputationManager.OnReputationChanged -= RefreshText;
	}

	private void RefreshText(int reputation)
	{
		_text.text = reputation.ToString();
	}
}
