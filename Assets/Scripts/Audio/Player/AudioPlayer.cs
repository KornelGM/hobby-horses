using System.Collections;
using UnityEngine;

public class AudioPlayer : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [Range(0f, 1f)]
    public float Volume = 1f;
    public int PrevIndex;

    [SerializeField] private AudioSource _audioSource;

    private ComponentObjectPool<AudioSource> objectPool;
    private int prevIndex;
    private bool _isPlaying;
    private int ticksPassed;

    public void PlayEvent(AudioEventVariant variant, AudioSource audioSource, params AudioPlayerParam[] overrideParams)
    {
        if (!audioSource) return;

        if (audioSource.isPlaying)
        {
            audioSource.volume = Volume * variant.volume;
            return;
        }

        audioSource.gameObject.SetActive(true);

        int randomIndex = UnityEngine.Random.Range(0, variant.EntryList.Count);

        if (randomIndex == prevIndex)
            randomIndex = (randomIndex + 1) % variant.EntryList.Count;

        prevIndex = randomIndex;

        audioSource.clip = variant.EntryList[randomIndex];
        audioSource.loop = variant.loop;
        audioSource.priority = variant.priority;
        audioSource.volume = Volume * variant.volume;
        audioSource.pitch = variant.pitch;
        audioSource.timeSamples = variant.pitch < 0f ? audioSource.clip.samples - 1 : 0;
        audioSource.panStereo = variant.stereoPan;
        audioSource.spatialBlend = variant.spatialBlend;
        audioSource.reverbZoneMix = variant.reverbZoneMix;
        audioSource.ignoreListenerPause = variant.ignoreListenerPause;
        audioSource.dopplerLevel = variant.dopplerLevel;
        audioSource.spread = variant.spread;
        audioSource.rolloffMode = variant.rolloffMode;
        audioSource.minDistance = variant.minDistance;
        audioSource.maxDistance = variant.maxDistance;
        audioSource.outputAudioMixerGroup = variant.AudioMixerGroup;

        if (audioSource.rolloffMode == AudioRolloffMode.Custom)
        {
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(SerializableKeyframe.ArrayToKeyframes(variant.keyFrames)));
        }

        foreach (AudioPlayerParam param in overrideParams)
        {
            param.Handle(audioSource);
        }

        if (audioSource.isActiveAndEnabled)
            audioSource.Play();

        if (!audioSource.loop)
        {
            StartCoroutine(StopAudio(audioSource, audioSource.clip, audioSource.clip.length / Mathf.Abs(audioSource.pitch)));
        }
    }

    public void PlayOneShot(AudioEventVariant audioVariant)
    {
        _audioSource.PlayOneShot(audioVariant.GetEntry(), audioVariant.volume * Volume);
    }

    public void PlayOneShot(AudioEventVariant audioVariant, AudioSource audioSource)
    {
        audioSource.PlayOneShot(audioVariant.GetEntry(), audioVariant.volume * Volume);
    }

    public void StopAll()
    {

    }

    public void StopAudio(AudioSource audioSource)
    {
        audioSource.Stop();
        Volume = 1f;
    }

    private IEnumerator StopAudio(AudioSource audioSource, AudioClip clip, float time)
    {
        yield return null;
    }

    public void PauseAudio()
    {
        if (_audioSource == null) return;
        if (!_audioSource.isPlaying) return;

        _audioSource.Pause();
        _isPlaying = true;
    }

    public void PauseAudio(AudioSource audioSource)
    {
        if (audioSource == null) return;
        if (!audioSource.isPlaying) return;

        audioSource.Pause();
        _isPlaying = true;
    }

    public void PlayAfterPauseAudio()
    {
        if (!_isPlaying) return;
        if (_audioSource == null) return;

        _audioSource.Play();
        _isPlaying = false;
    }

    public void PlayAtIntervals(int interval, AudioEventVariant variant, AudioSource audioSource)
    {
        ticksPassed++;
        if (ticksPassed == interval)
        {
            PlayEvent(variant, audioSource);
            ticksPassed = 0;
        }
    }
}