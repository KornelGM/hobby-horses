using UnityEngine;
using UnityEngine.EventSystems;

public class HoverUI : MonoBehaviour, IHoverUI
{
	[SerializeField] private Animator _slotAnimator;	
	
	public void OnPointerEnter(PointerEventData eventData) => 
		PerformActionOnPointerEnter();

	public void OnPointerExit(PointerEventData eventData) => 
		PerformActionOnPointerExit();

	public void PerformActionOnPointerEnter()
	{
		print("pointer enter");
		_slotAnimator.SetBool("Hoverred", true);
	}

	public void PerformActionOnPointerExit()
	{
		print("pointer exit");
		_slotAnimator.SetBool("Hoverred", false);
	}
}