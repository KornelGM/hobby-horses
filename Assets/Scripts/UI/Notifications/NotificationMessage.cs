using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotificationMessage : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private Image _notificationImage;
    [SerializeField] private bool _changeColor;

    [SerializeField, FoldoutGroup("Audio")] private AudioPlayer _audioPlayer;
    [SerializeField, FoldoutGroup("Audio")] private AudioSource _audioSource;
    [SerializeField, FoldoutGroup("Audio")] private AudioStorage _audioStorage;

    private UnityEvent _onEndAnimation;
    public void SetNotification(NotificationInfo info, Color notificationColor)
    {   
        if (_changeColor)
            _notificationImage.color = notificationColor;

        string message = info.Description;

        if (string.IsNullOrEmpty(info.Description))
            message = info.Description.mTerm;

        SetNotification(message);
    }

    public void ShowAnimation()
    {
        PlayAudio(_audioStorage);

        _animator.SetTrigger("Show");
    }
    public void DuplicateAnimation()
    {
        _animator.SetTrigger("Duplicate");
    }
    public void HideAnimation(UnityEvent onEndAnimation)
    {
        _onEndAnimation = onEndAnimation;
        _animator.SetTrigger("Hide");
    }

    public void OnEndAnimation()
    {
        _onEndAnimation?.Invoke();
    }

    private void SetNotification(string notificationText)
    {
        _notificationText.text = notificationText;
    }
    private void SetNotification(LocalizedString localizedNotification)
    {
        SetNotification(localizedNotification.ToString());
    }

    private void PlayAudio(AudioStorage audioStorage)
    {
        if (_audioPlayer == null || _audioSource == null || audioStorage == null || audioStorage.AudioDatabase == null) return;

        _audioPlayer.PlayEvent(audioStorage.GetRandomAudioEventVariant(), _audioSource);
    }
}
