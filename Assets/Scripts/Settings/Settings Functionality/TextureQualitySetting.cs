using System;
using UnityEngine;

public class TextureQualitySetting : DropdownSetting, IProfileSetting<int>
{
   [field: SerializeField] public int LowProfileValue { get; set; }
   [field: SerializeField] public int MediumProfileValue { get; set; }
   [field: SerializeField] public int HighProfileValue { get; set; }
   [field: SerializeField] public int VeryHighProfileValue { get; set; }
   public ISetting<int> BaseSetting { get; set; }
   public Profiles Profile { get; set; }
   public event Action ValueChanged;

   public override void Initialize(SettingsTab tab)
   {
       base.Initialize(tab);
       BaseSetting = this;
       ValueChanged += SettingsTab.SettingsWindow.OnValueChanged;
   }

   public void SetProfile(int value) => 
       SetCurrentValue(value);

   public override void OnValueChanged()
   {
       ValueChanged?.Invoke();
   }

   public override void Apply()
    {
        base.Apply();
        SetTextureQuality();
    }

    private void SetTextureQuality()
    {
        switch (CurrentValue)
        {
            case 0:
                QualitySettings.globalTextureMipmapLimit = 3;
                break;
            case 1:
                QualitySettings.globalTextureMipmapLimit = 2;
                break;
            case 2:
                QualitySettings.globalTextureMipmapLimit = 1;
                break;
            case 3:
                QualitySettings.globalTextureMipmapLimit = 0;
                break;
        }
    }
}
