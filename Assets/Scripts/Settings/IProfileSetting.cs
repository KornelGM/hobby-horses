using System;

/*
 * when option using this interface.
 * add [field: SerializeField] to Value properties
 * Override Initialize, OnValueChange, methods of setting.
 *
 * Example: CastShadowsSettings.cs, TextureQualitySetting.cs 
 */

public interface IProfileSetting<T>
{
    public T LowProfileValue { get; set; }
    public T MediumProfileValue { get; set; }
    public T HighProfileValue { get; set; }
    public T VeryHighProfileValue { get; set; }
    public ISetting<T> BaseSetting { get; set; }
    public event Action ValueChanged;
    public void SetProfile(T value);

}