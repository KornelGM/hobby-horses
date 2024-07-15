using UnityEngine;
using I2.Loc;
using TMPro;

public class QuestTitleUI : MonoBehaviour
{
    [SerializeField] Localize _nameLocalize;
    [SerializeField] TMP_Text _nameText;

    private void Start()
    {
        _nameLocalize.IsNotNull(this, nameof(_nameLocalize));
        _nameText.IsNotNull(this, nameof(_nameText));
    }

    public void SetQuestTitle(LocalizedString localizedString)
    {
        _nameLocalize.Term = localizedString.mTerm;
    }

}
