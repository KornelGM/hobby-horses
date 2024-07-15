using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardPanel : MonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }
	[ServiceLocatorComponent] private LeaderboardManager _leaderboardManager;

	public int Place { get; set; }

	public TextMeshProUGUI PlaceUI;
	public Image Panel;
	
	[SerializeField] private Image _image;
	[SerializeField] private TextMeshProUGUI _points;
	[SerializeField] private TextMeshProUGUI _name;

	public Factory CurrentFactory { get; private set; } = null;

	private void OnEnable() => Refresh();
	
	public void Initialize(Factory factory)
	{
		if (factory == null) return;
		if (CurrentFactory != null)
		{
			CurrentFactory.OnMyFactoryIconChanged -= UpdateIcon;
			CurrentFactory.OnMyFactoryNameChanged -= UpdateName;
			CurrentFactory.OnMyFactoryPointsChanged -= PointsTextUpdate;
			_image.sprite = null;
			_points.text = "0";
			_name.text = "Empty";
		}


		CurrentFactory = factory;
		CurrentFactory.OnMyFactoryIconChanged += UpdateIcon;
		CurrentFactory.OnMyFactoryNameChanged += UpdateName;
		CurrentFactory.OnMyFactoryPointsChanged += PointsTextUpdate;
		Panel.color = CurrentFactory.Color;

		Refresh();
		
	}

	
	public void Refresh()
	{
		if (CurrentFactory == null) return;
		UpdateIcon();
		UpdateName();
		PointsTextUpdate();
    }

	private void UpdateIcon() => _image.sprite = CurrentFactory.Icon;
	
	private void UpdateName()
	{
		if (_name == null) return;

		if (CurrentFactory.IsMyFactory)
		{
			if (_leaderboardManager == null) return;

			if (_leaderboardManager.MyFactory.MyFactoryName == null || _leaderboardManager.MyFactory.MyFactoryName == string.Empty)
				_name.text = "";
			else
				_name.text = _leaderboardManager.MyFactory.MyFactoryName.ToString();
		}
		else
		{
			if (CurrentFactory.Name == null) return;
			_name.text = CurrentFactory.Name.ToString();
		}
	}

	public void PointsTextUpdate()
	{
		if (CurrentFactory == null) return;
		_points.text = CurrentFactory.Points + ".";
	}
}
