/**
 * Description: To combat crappy Unity Image scaling with out using build-in mipmapping (which is affected by graphical texture settings and produces low-res images for UI when not at highest settings).
 * Authors: Kornel (Galactic Fox Studios)
 * Copyright: © 2023 Kornel (Galactic Fox Studios), MIT License.
**/

using Sirenix.OdinInspector;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIImageMipmap: MonoBehaviour
{
	private class MipmapSize
	{
		public const int Level64 = 64;
		public const int Level128 = 128;
		public const int Level256 = 256;
		public const int Level512 = 512;
		public const int Level1024 = 1024;
	}

	[Header("References")]
#if ODIN_INSPECTOR
	[Required]
#endif
	[SerializeField] private Image _image;

	[Header("Mipmap")]
#if ODIN_INSPECTOR
	[Required]
#endif
	[SerializeField, OnValueChanged("RefreshCurrentMinmap")] private UIMipmap _mipmap;
    [SerializeField] private bool _useAnimation;
#if ODIN_INSPECTOR
	[ShowIf("_useAnimation")]
#endif
	[SerializeField] private UIMipmap[] _mipmapKeyframes;
#if ODIN_INSPECTOR
	[ShowIf("_useAnimation")]
#endif
	[SerializeField] private int _currentKeyframe = 0;

	[Header("Pixels Per Unit Multiplier")]
	[SerializeField] private bool _overwritePPUM = false;
#if ODIN_INSPECTOR
	[ShowIf("_overwritePPUM")]
#endif
	[SerializeField] private float _64PPUM = 1;
#if ODIN_INSPECTOR
	[ShowIf("_overwritePPUM")]
#endif
	[SerializeField] private float _128PPUM = 1;
#if ODIN_INSPECTOR
	[ShowIf("_overwritePPUM")]
#endif
	[SerializeField] private float _256PPUM = 1;
#if ODIN_INSPECTOR
	[ShowIf("_overwritePPUM")]
#endif
	[SerializeField] private float _512PPUM = 1;
#if ODIN_INSPECTOR
	[ShowIf("_overwritePPUM")]
#endif
	[SerializeField] private float _1024PPUM = 1;

	[Header("Other")]
	[SerializeField] private int _overriteMultiplier = 1;

	private const int ScaleRatio = 2;

	private RectTransform _rectTransform;
	private UIMipmap _currentMipmap;
	private int _lastKeyframe = 0;

	void Start()
	{
		Assert.IsNotNull(_image, $"Please assign <b>{nameof(_image)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
		Assert.IsNotNull(_mipmap, $"Please assign <b>{nameof(_mipmap)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");

		if(_mipmap != null)
		{
			Assert.IsNotNull(_mipmap.M64, $"Please assign <b>{nameof(_mipmap.M64)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
			if(_mipmap.M256 != null)
			{
				Assert.IsNotNull(_mipmap.M128, $"Please assign <b>{nameof(_mipmap.M128)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
			}
			if(_mipmap.M512 != null)
			{
				Assert.IsNotNull(_mipmap.M128, $"Please assign <b>{nameof(_mipmap.M128)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
				Assert.IsNotNull(_mipmap.M256, $"Please assign <b>{nameof(_mipmap.M256)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
			}
			if(_mipmap.M1024 != null)
			{
				Assert.IsNotNull(_mipmap.M128, $"Please assign <b>{nameof(_mipmap.M128)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
				Assert.IsNotNull(_mipmap.M256, $"Please assign <b>{nameof(_mipmap.M256)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
				Assert.IsNotNull(_mipmap.M512, $"Please assign <b>{nameof(_mipmap.M512)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
			}
		}

		if(_useAnimation)
		{
			Assert.AreNotEqual(_mipmapKeyframes.Length, 0, $"Please assign keyframes to <b>{nameof(_mipmapKeyframes)}</b> field on <b>{GetType().Name}</b> script on <b>{name}</b> object");
		}
	}

	void OnValidate()
	{
		TryAssignRequiredReferences();
	}

	void OnEnable()
	{
		TryAssignRequiredReferences();
		TrySetNewMipmap();
	}

	void Update()
	{
		if(_useAnimation)
			TryUpdateKeyframe();
	}

	public void SetMipmap(UIMipmap mipmap)
	{
		if(mipmap == null)
		{
			Debug.LogError($"{nameof(mipmap)} in SetMipmaps can not be null", gameObject);
			return;
		}

		_mipmap = mipmap;
		_currentMipmap = mipmap;

		TrySetNewMipmap();
	}

	private void TryUpdateKeyframe()
	{
		if(_lastKeyframe == _currentKeyframe)
			return;

		if(_currentKeyframe < 0)
		{
			_currentKeyframe = 0;
			if(Application.isPlaying)
				Debug.LogError($"Keyframe index can not be smaller then 0.", gameObject);
		}
		if(_currentKeyframe >= _mipmapKeyframes.Length)
		{
			_currentKeyframe = _mipmapKeyframes.Length - 1;
			if(Application.isPlaying)
				Debug.LogError($"Keyframe index must be smaller then number of frames (index starts with 0).", gameObject);
		}

		if(_lastKeyframe == _currentKeyframe) // Yes, again.
			return;

		RefreshMipMap();
	}

	public void RefreshMipMap()
    {
		_lastKeyframe = _currentKeyframe;
		_currentMipmap = _mipmapKeyframes[_currentKeyframe];
		TrySetNewMipmap();
	}

	private void TrySetNewMipmap()
	{
		if(_currentMipmap == null)
			return;

		(int widthOnScreen, int heightOnScreen) = GetSizeOnScreen(_rectTransform);
		int size = widthOnScreen > heightOnScreen ? widthOnScreen : heightOnScreen;

		if(size > ( MipmapSize.Level1024 / ScaleRatio ) && _currentMipmap.M1024 != null)
		{
			Set1024();
			return;
		}
		else if(size > ( MipmapSize.Level512 / ScaleRatio ) && _currentMipmap.M512 != null)
		{
			Set512();
			return;
		}
		else if(size > ( MipmapSize.Level256 / ScaleRatio ) && _currentMipmap.M256 != null)
		{
			Set256();
			return;
		}
		else if(size > ( MipmapSize.Level128 / ScaleRatio ) && _currentMipmap.M128 != null)
		{
			Set128();
			return;
		}
		else if(_currentMipmap.M64 != null)
		{
			Set64();
			return;
		}

		Debug.LogWarning($"Failed to find suitable image size. Size on screen: {widthOnScreen}x{heightOnScreen}, biggest side: {size}. Please notify The UI Person ;)", gameObject);
	}

	private void TryAssignRequiredReferences()
	{
		if(_image == null)
			_image = GetComponent<Image>();

		if(_image != null && _rectTransform == null)
			_rectTransform = _image.GetComponent<RectTransform>();

		if(_currentMipmap == null)
			_currentMipmap = _mipmap;
	}

#if ODIN_INSPECTOR
	[Sirenix.OdinInspector.Button]
#else
	[ContextMenu("Set64")]
#endif
	private void Set64()
	{
		//Debug.Log($"{widthOnScreen}x{heightOnScreen}, chosen size: {64}");
		_image.sprite = _currentMipmap.M64;
		_image.pixelsPerUnitMultiplier = _overwritePPUM ? _64PPUM : _currentMipmap.M64PPUM;
	}

#if ODIN_INSPECTOR
	[Sirenix.OdinInspector.Button]
#else
	[ContextMenu("Set128")]
#endif
	private void Set128()
	{
		//Debug.Log($"{widthOnScreen}x{heightOnScreen}, chosen size: {128}");
		_image.sprite = _currentMipmap.M128;
		_image.pixelsPerUnitMultiplier = _overwritePPUM ? _128PPUM : _currentMipmap.M128PPUM;
	}

#if ODIN_INSPECTOR
	[Sirenix.OdinInspector.Button]
#else
	[ContextMenu("Set256")]
#endif
	private void Set256()
	{
		//Debug.Log($"{widthOnScreen}x{heightOnScreen}, chosen size: {256}");
		_image.sprite = _currentMipmap.M256;
		_image.pixelsPerUnitMultiplier = _overwritePPUM ? _256PPUM : _currentMipmap.M256PPUM;
	}

#if ODIN_INSPECTOR
	[Sirenix.OdinInspector.Button]
#else
	[ContextMenu("Set512")]
#endif
	private void Set512()
	{
		//Debug.Log($"{widthOnScreen}x{heightOnScreen}, chosen size: {512}");
		_image.sprite = _currentMipmap.M512;
		_image.pixelsPerUnitMultiplier = _overwritePPUM ? _512PPUM : _currentMipmap.M512PPUM;
	}

#if ODIN_INSPECTOR
	[Sirenix.OdinInspector.Button]
#else
	[ContextMenu("Set1024")]
#endif
	private void Set1024()
	{
		//Debug.Log($"{widthOnScreen}x{heightOnScreen}, chosen size: {1024}");
		_image.sprite = _currentMipmap.M1024;
		_image.pixelsPerUnitMultiplier = _overwritePPUM ? _1024PPUM : _currentMipmap.M1024PPUM;
	}

	private (int widthOnScreen, int heightOnScreen) GetSizeOnScreen(RectTransform rt)
	{
		if(rt == null)
			return (0,0);

		Vector3[] v = new Vector3[4];
		rt.GetWorldCorners(v);
		float widthOnScreen = (v[2].x - v[1].x) * _overriteMultiplier;
		float heightOnScreen = (v[1].y - v[0].y) * _overriteMultiplier;

		return ((int)widthOnScreen, (int)heightOnScreen);
	}

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
#else
	[ContextMenu("Refresh current minmap")]
#endif
    private void RefreshCurrentMinmap()
	{
		_currentMipmap = _mipmap;
		Set1024();

    }
}
