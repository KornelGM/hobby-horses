using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Image Mipmap", menuName = "UI/Image Mipmap")]
public class UIMipmap: ScriptableObject
{
	[Header("Sprites (first required, rest is optional)")]
#if ODIN_INSPECTOR
	[PreviewField, Required("64 version is required.")]
#endif
	public Sprite M64;

#if ODIN_INSPECTOR
	[PreviewField, InfoBox("128 version is required because higher res versions added.", InfoMessageType.Error, "ShowM128Required")]
#endif
	public Sprite M128;

#if ODIN_INSPECTOR
	[PreviewField, InfoBox("256 version is required because higher res versions added.", InfoMessageType.Error, "ShowM256Required")]
#endif
	public Sprite M256;

#if ODIN_INSPECTOR
	[PreviewField, InfoBox("512 version is required because higher res versions added.", InfoMessageType.Error, "ShowM512Required")]
#endif
	public Sprite M512;

#if ODIN_INSPECTOR
	[PreviewField]
#endif
	public Sprite M1024;

	[Header("Default Pixels Per Unit Multiplier")]
	public float M64PPUM = 1;
	public float M128PPUM = 1;
	public float M256PPUM = 1;
	public float M512PPUM = 1;
	public float M1024PPUM = 1;

	private bool ShowM128Required() => ( M256 != null || ShowM256Required() ) && M128 == null;

	private bool ShowM256Required() => ( M512 != null || ShowM512Required() ) && M256 == null;

	private bool ShowM512Required() => M1024 != null && M512 == null;
}
