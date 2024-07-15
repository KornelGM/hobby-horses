using UnityEngine;
using I2.Loc;
using TMPro;
using Sirenix.OdinInspector;

public class QuestTaskUI : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] Localize _nameLocalize;
    [SerializeField] TMP_Text _nameText;
    [SerializeField] Localize _helpLocalize;
    [SerializeField] TMP_Text _helpText;
    [SerializeField] LocalizationParamsManager _paramsManager;
    [SerializeField] int _marginSize = 10;

    [FoldoutGroup("Audio"), SerializeField] private AudioPlayer _audioPlayer;
    [FoldoutGroup("Audio"), SerializeField] private AudioStorage _audioStorage;
    [FoldoutGroup("Audio"), SerializeField] private AudioSource _audioSource;

    static readonly string PARAM_FINISH = "IsFinished";
    static readonly string PARAM_COUNTER = "COUNTER";

    private void Start()
    {
        _animator.IsNotNull(this, nameof(_animator));
        _nameLocalize.IsNotNull(this, nameof(_nameLocalize));
        _nameText.IsNotNull(this, nameof(_nameText));
        _helpLocalize.IsNotNull(this, nameof(_helpLocalize));
        _helpText.IsNotNull(this, nameof(_helpText));
        _paramsManager.IsNotNull(this, nameof(_paramsManager));
    }

    public void SetTaskName(LocalizedString localizedString, LocalizedString helpLocalizedString, bool showHelpText, string counter, int margin = 0)
    {
        _nameLocalize.Term = localizedString.mTerm;
        _helpLocalize.Term = helpLocalizedString.mTerm;
        _helpText.gameObject.SetActive(showHelpText);
        _paramsManager.SetParameterValue(PARAM_COUNTER, counter);

        if (margin != 0)
        {
            _nameText.margin = new(0f, 0f, margin * _marginSize, 0f);
            _helpText.margin = new(0f, 0f, margin * _marginSize, 0f);
        }
    }

    public void UpdateCounter(string counter)
    {
        _paramsManager.SetParameterValue(PARAM_COUNTER, counter);
    }

    public void FinishTask()
    {
        PlayAudioRinging();
        _animator.SetBool(PARAM_FINISH, true);
    }

    private void PlayAudioRinging()
    {
        if (_audioPlayer == null || _audioSource== null || _audioStorage == null || _audioStorage.AudioDatabase == null) return;

        _audioPlayer.PlayEvent(_audioStorage.GetRandomAudioEventVariant(), _audioSource);
    }
}
