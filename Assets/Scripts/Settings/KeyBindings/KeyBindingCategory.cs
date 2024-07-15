using I2.Loc;
using UnityEngine;

public class KeyBindingCategory : MonoBehaviour
{
    [SerializeField] private Localize _label;

    public void Initialize(LocalizedString labelText)
    {
        _label.Term = labelText.mTerm;
    }
}