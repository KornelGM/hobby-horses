using DG.Tweening;
using UnityEngine;
using System.Collections;

public class UnlockAction : BaseAction, IServiceLocatorComponent
{
	[System.Serializable]
	private class UnlockLevel
	{
		public GameObject[] ContainersWithObjectsToHide;
		public GameObject[] ContainersWithObjectsToShow;
	}

	public ServiceLocator MyServiceLocator { get; set; }
	public int Level => _index + (_disabledAtStart? 0 : 1);

	[Header("General")]
	[SerializeField] private bool _disabledAtStart = false;
	[SerializeField] private UnlockLevel[] _levels;

    [Header("Animation")]
	[SerializeField] private AnimationCurve _popOut = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,0));
	[SerializeField] private AnimationCurve _popIn = new AnimationCurve(new Keyframe(0,0), new Keyframe(1,1));
	[SerializeField] private float _tweenTime = 0.3f;

	private int _index = -1;

	public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller) => true;
	public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand) => Upgrade();

    private void Upgrade()
    {
		_index = Mathf.Clamp(_index + 1, 0, _levels.Length - 1);
		Upgrade(_index);
    }

	private void Upgrade(int index)
	{
        for (int i = 0; i < index; i++)
        {
			Debug.Log($"Upgraded {i} level without animation");
			UpgradeToLevel(i, false);
		}

		Debug.Log($"Upgraded {index} level with animation");
		UpgradeToLevel(index, true);
	}

	private void UpgradeToLevel(int index, bool animate)
	{
		if (index < 0) return;

		UnlockLevel unlockingLevel = _levels[index];
		foreach(GameObject obj in unlockingLevel.ContainersWithObjectsToHide)
        {
			ToggleObjectsInContainer(obj, false, animate);
        }

		foreach (GameObject obj in unlockingLevel.ContainersWithObjectsToShow)
		{
			ToggleObjectsInContainer(obj, true, animate);
		}

		StartCoroutine(UpdateGraph(_index));
	}

	private void ToggleObjectsInContainer(GameObject container, bool enable, bool animate)
    {
		container.SetActive(true);
		foreach(ItemDetectable itemDetectable in container.GetComponentsInChildren<ItemDetectable>())
        {
			itemDetectable.enabled = enable;
        }

        for (int i = 0; i < container.transform.childCount; i++)
        {
			GameObject child = container.transform.GetChild(i).gameObject;
            if (!animate)
            {
				child.SetActive(enable);
				continue;
            }

            if (enable)
            {
				TweenObject(child, 1, _popIn, _tweenTime);
			}
            else
            {
				TweenObject(child, 0, _popOut, _tweenTime, true);
            }
        }
	}

	private void TweenObject(GameObject obj, float newScale, AnimationCurve curve, float tweenTime, bool hide = false)
	{
		if (!hide) obj.transform.localScale = Vector3.zero;
		obj.transform.DOScale(newScale, tweenTime).SetEase(curve).OnComplete(() => obj.SetActive(!hide));
	}
	
	private IEnumerator UpdateGraph(int levelIndex)
	{
		if(!MyServiceLocator.TryGetServiceLocatorComponent(out GraphUpdater graphUpdater, true, false))yield break;
		yield return new WaitForSeconds(_tweenTime*2);

		graphUpdater.UpdateGraph(levelIndex);
	}
}
