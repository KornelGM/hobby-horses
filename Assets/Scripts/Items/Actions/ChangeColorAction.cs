using UnityEngine;

using System;

public class ChangeColorAction: BaseAction, IServiceLocatorComponent, ISaveable<WallSaveData>
{
	public ServiceLocator MyServiceLocator { get; set; }
	public Action<ChangeColorAction, int> OnColorChanged { get; set; }

	[SerializeField] private MeshRenderer _renderer;
	[SerializeField] private Material[] _materials;

	 private int _materialIndex = 0;

    private int GetNextIndex => (_materialIndex + 1) % _materials.Length;
    private int ClampedIndex => _materialIndex % _materials.Length;

    public override void Perform(ServiceLocator playerServiceLocator, ServiceLocator itemInInteractionServiceLocator, ServiceLocator itemInHand) => ChangeColor(GetNextIndex);

	public override bool Available(ServiceLocator playerServiceLocator, ServiceLocator interactionItem, ServiceLocator caller) => true;
	private void ChangeColor(int index)
	{
        if (_materialIndex == index) return;

        OnColorChanged?.Invoke(this, index);
        _materialIndex = index;
		ChangeColor(_materials[ClampedIndex]);
	}

	public void ChangeWithoutNotify(int index)
	{
		if (_materialIndex == index) return;
		_materialIndex = index;
		ChangeColor(_materials[ClampedIndex]);
	}

	private void ChangeColor(Material material)
	{
		if(_renderer == null)
		{
			Debug.LogError($"No renderer assigned to {name}");
			return;
		}

		if(_materials == null || _materials.Length == 0)
		{
			Debug.LogError($"No materials assigned to {name}");
			return;
		}

		_renderer.material = material;
	}

    public WallSaveData CollectData(WallSaveData data)
    {
		data.MaterialID = _materialIndex;
		return data;
    }

    public void Initialize(WallSaveData save)
    {
		if (save == null) return;
		ChangeColor(save.MaterialID);
	}
}
