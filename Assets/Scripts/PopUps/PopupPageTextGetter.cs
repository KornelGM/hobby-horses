using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

public class PopupPageTextGetter : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Texts")] private Localize _title;
    [SerializeField, FoldoutGroup("Texts")] private Localize _subtitle;
    [SerializeField, FoldoutGroup("Texts")] private Localize _description;
    [SerializeField, FoldoutGroup("Texts")] private string _nameTerm;
    [SerializeField, FoldoutGroup("Texts")] private int _number;


    private const string _titleTerm = "Title";
    private const string _subtitleTerm = "Subtitle";
    private const string _descriptionTerm = "Description";
    private const string _term = "Popups";

    [Button("Get Texts")]
    private void TryGetDialogues()
    {
        string number = _number == 0 ? "" : $"{_number}";

        string title = $"{_term}/{_nameTerm}/{_titleTerm}";
        string subtitle = $"{_term}/{_nameTerm}/{_subtitleTerm}" + number;
        string description = $"{_term}/{_nameTerm}/{_descriptionTerm}" + number;

        _title.Term = title;
        _subtitle.Term = subtitle;
        _description.Term = description;
    }
}
