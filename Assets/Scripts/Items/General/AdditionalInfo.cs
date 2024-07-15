using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdditionalInfo : MonoBehaviour, IAwake, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    public Animator Animator => _animator;
    public RadarChart RadarChart => _radarChart;
    public  ShowAdditionalInfo CurrentShowAdditionalInfo => _currentShowAdditionalInfo;
    public bool Showed => _showed;

    [SerializeField] private TextMeshProUGUI _additionalInfo;
    [SerializeField] private Animator _animator;
    [SerializeField] private RadarChart _radarChart;
    [SerializeField] private RectTransform _layoutTransform;

    [FoldoutGroup("Prefabs"), SerializeField] private AdditionalInfoField _additionalInfoFieldPrefab;

    private List<AdditionalInfoField> _spawnedFields = new();
    private List<AdditionalInfoField> _activeFields = new();
    private UnityAction _onHideTooltips;
    private ShowAdditionalInfo _currentShowAdditionalInfo;

    private bool _showed;

    public void FixLayout() => LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutTransform);

    public void SetOnHideToolTipsEvent(UnityAction onHideTooltips)
    {
        _onHideTooltips = onHideTooltips; 
    }

    public void SetAdditionalInfoText(List<AdditionalInfoFieldContent> contents)
    {

        if(contents.Count != _activeFields.Count)
            PoolFields(contents);   

        for (int i = 0; i < contents.Count; i++)
        {
            _activeFields[i].Initialize(contents[i]);

            _activeFields[i].gameObject.SetActive(true);

            if (!_showed)
                _spawnedFields[i].Toggle(false, 0f);
        }

        if (!_showed)
        {
            FadeOnActiveFields(0.5f, 0.5f);
        }
        FixLayout();
    }

    private void PoolFields(List<AdditionalInfoFieldContent> contents)
    {
        if (contents.Count > _spawnedFields.Count)
            SpawnFields(contents.Count);

        for (int i = 0; i < contents.Count; i++)
        {
            if(!_activeFields.Contains(_spawnedFields[i]))
            {
                _activeFields.Add(_spawnedFields[i]);

                _spawnedFields[i].gameObject.SetActive(true);

                if (_showed)
                    _spawnedFields[i].Toggle(true, 0f);
            }
        }

        for (int i = _spawnedFields.Count - 1; i >= contents.Count; i--)
        {
            if (_activeFields.Contains(_spawnedFields[i]))
            {
                _activeFields.Remove(_spawnedFields[i]);
            }

            _spawnedFields[i].gameObject.SetActive(false);
        }
 
    }

    private void SpawnFields(int count)
    {
        while (_spawnedFields.Count != count)
        {
            AdditionalInfoField spawnedField = Instantiate(_additionalInfoFieldPrefab, _layoutTransform);
            spawnedField.Toggle(false, 0f);

            _spawnedFields.Add(spawnedField);
        }
    }

    public void FadeOnActiveFields(float duration ,float waitBefore = 0)
    {
        foreach (var item in _activeFields)
        {
            item.Toggle(true, duration, waitBefore);
        }
    }

    public void FadeOffActiveFields(float duration, float waitBefore = 0)
    {
        foreach (var item in _spawnedFields)
        {
            item.Toggle(false, duration, waitBefore);
        }
    }

    public void SubscribeShowAdditionalInfo(ShowAdditionalInfo showAdditionalInfo)
    {
        _currentShowAdditionalInfo = showAdditionalInfo;
        gameObject.SetActive(true);
    }

    public void ShowAdditionalInfo()
    {
        _showed = true;
        _animator.SetBool("ShowHide", true);

        FixLayout();
    }

    public void HideAdditionalInfo()
    {
        _showed = false;
        FadeOffActiveFields(0);

        _currentShowAdditionalInfo = null;
        _animator.SetBool("ShowHide", false);
    }

    public void DisableInfo()
    {
        _onHideTooltips?.Invoke();
        gameObject.SetActive(false);
    }

    public bool CheckCurrentShowAdditionalInfo(ShowAdditionalInfo showAdditionalInfo)
    {
        return _currentShowAdditionalInfo == showAdditionalInfo;
    }

    public void CustomAwake()
    {

    }

}
