using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LayoutGroupFixer : MonoBehaviour
{
    private enum UpdateType
    {
        Start,
        Enable,
    }

    [InfoBox("If CanvasGroup is provided, alpha will be set to 0 before fixing and to 1 after fixing")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private UpdateType _updateType;

    [SerializeField] private bool _rebuildLayout;
    [SerializeField, ShowIf("_rebuildLayout")] private RectTransform _layoutTransform;

    public UnityAction FixLayout;

    private LayoutGroup _layoutGroup = null;

    public void Start()
    {
        _layoutGroup = GetComponent<LayoutGroup>();
        FixLayout += () => StartCoroutine(Fix());

        if (_updateType != UpdateType.Start) return;

        if (_rebuildLayout && _layoutTransform != null)
            StartCoroutine(Rebuild());
        else
            StartCoroutine(Fix());
    }


    private void OnEnable()
    {
        if (_updateType != UpdateType.Enable) return;

        if (_rebuildLayout && _layoutTransform != null)
            StartCoroutine(Rebuild());
        else
            StartCoroutine(Fix());
    }

    public void RebuildLayout() => LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutTransform);

    private IEnumerator Rebuild()
    {
        yield return null;
        RebuildLayout();
    }

    public IEnumerator Fix()
    {
        if (_layoutGroup == null) yield break;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;

        _layoutGroup.enabled = false;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        _layoutGroup.enabled = true;


        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;
    }
}
