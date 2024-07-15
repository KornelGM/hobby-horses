using UnityEngine;
using System;

public class FactoryInfo : MonoBehaviour, IServiceLocatorComponent
{
	public ServiceLocator MyServiceLocator { get; set; }
	[SerializeField] public SpriteDatabase SpriteDatabase;
	[field:SerializeField] public Sprite CurrentSprite { get; private set; }

	public event Action<string> OnFactoryNameSet;
	public event Action<Sprite, Sprite> OnFactoryIconChanged;

	public bool TryChangeFactoryName(string newName)
	{
		if (newName.Length == 0) return false;
		if (newName[newName.Length - 1] == ' ') return false;

        OnFactoryNameSet.Invoke(newName);
        return true;
	}

	public void SetNewIcon(Sprite sprite)
	{
		if (sprite == null) return;
		if (CurrentSprite == sprite) return;
		Sprite previous = CurrentSprite;
		CurrentSprite = sprite;
		OnFactoryIconChanged?.Invoke(previous, CurrentSprite);
	}

}
