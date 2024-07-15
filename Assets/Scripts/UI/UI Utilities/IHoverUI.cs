using UnityEngine.EventSystems;

public interface IHoverUI : IPointerEnterHandler, IPointerExitHandler
{
	public void PerformActionOnPointerEnter();
	public void PerformActionOnPointerExit();
}