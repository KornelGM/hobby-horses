using UnityEngine;
using UnityEngine.UI;

public class ChooseShelterIcon : MonoBehaviour, IServiceLocatorComponent, IAwake
{
	public Image Icon { get; set; }
	public ServiceLocator MyServiceLocator { get; set; }

	[ServiceLocatorComponent] private FactoryInfo _shelterInfo;
	[SerializeField] private GameObject _indicator;

	private Button _button;

	private void Awake()
	{
		Icon = GetComponent<Image>();
		_button = GetComponent<Button>();
		_button.onClick.AddListener(TryChooseIcon);
	}

	public void CustomAwake()
	{
		if (_shelterInfo != null) _shelterInfo.OnFactoryIconChanged += IconChanged;
		CheckIcon();
	}

	private void OnDestroy()
	{
		if (_shelterInfo != null) 
		_shelterInfo.OnFactoryIconChanged -= IconChanged;
		_button.onClick.RemoveAllListeners();
	}

	public void DeactiveIndicator() => _indicator.SetActive(false);

	private void ActiveIndicator() => _indicator.SetActive(true);


	public void TryChooseIcon()
	{
		_shelterInfo.SetNewIcon(Icon.sprite);
		_indicator.SetActive(true);
		ActiveIndicator();
	}

	private void IconChanged(Sprite previous, Sprite current)
	{
		if (_shelterInfo == null) return;
		if (Icon.sprite != current) DeactiveIndicator();
		else ActiveIndicator();
	}

	private void CheckIcon()
	{
		if (_shelterInfo == null) return;
		if (_shelterInfo.CurrentSprite == null) return;
		if (Icon.sprite == _shelterInfo.CurrentSprite) ActiveIndicator();
	}

}
