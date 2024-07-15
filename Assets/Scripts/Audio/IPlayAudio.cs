using JetBrains.Annotations;
using UnityEngine;

public interface IPlayAudio
{
    public void PlayAudio([CanBeNull]AudioPlayer audioPlayer, [CanBeNull] AudioSource audioSource, 
        [CanBeNull] AudioStorage audioStorage)
    {
        if (audioPlayer == null || audioSource == null || audioStorage == null || audioStorage.AudioDatabase == null) 
            return;

        audioPlayer.PlayEvent(audioStorage.GetRandomAudioEventVariant(), audioSource);
    }

    public void PlayAudio([CanBeNull] AudioPlayer audioPlayer, [CanBeNull] AudioSource audioSource,
        [CanBeNull] AudioEventVariant audioEventVariant)
    {
        if (audioPlayer == null || audioSource == null || audioEventVariant == null) return;

        audioPlayer.PlayEvent(audioEventVariant, audioSource);
    }

    public void PlayAudio([CanBeNull] AudioPlayer audioPlayer, [CanBeNull] AudioSource audioSource,
        [CanBeNull] AudioStorage audioStorage, float volume)
    {
        if (audioPlayer == null || audioSource == null || audioStorage == null || audioStorage.AudioDatabase == null) 
            return;

        audioPlayer.Volume = volume;
        audioPlayer.PlayEvent(audioStorage.GetRandomAudioEventVariant(), audioSource);
    }

    public void StopAudio([CanBeNull] AudioPlayer audioPlayer, [CanBeNull] AudioSource audioSource)
    {
        if (audioPlayer == null || audioSource == null) return;

        audioPlayer.StopAudio(audioSource);
    }

    public void PauseAudio([CanBeNull] AudioPlayer audioPlayer, [CanBeNull] AudioSource audioSource)
    {
        if (audioPlayer == null || audioSource == null) return;

        audioPlayer.PauseAudio(audioSource);
    }

    public void PlayIntervalAudio(int triggerInterval, [CanBeNull] AudioPlayer audioPlayer,
        [CanBeNull] AudioSource audioSource, [CanBeNull] AudioStorage audioStorage)
    {
        if (audioPlayer == null || audioSource == null || audioStorage == null || audioStorage.AudioDatabase == null)
            return;

        audioPlayer.PlayAtIntervals(triggerInterval, audioStorage.GetRandomAudioEventVariant(), audioSource);
    }

    public void PlayOneShot([CanBeNull] AudioPlayer audioPlayer, [CanBeNull] AudioSource audioSource,
        [CanBeNull] AudioEventVariant audioVariant)
    {
        if (audioPlayer == null || audioSource == null || audioVariant == null) return;

        audioSource.PlayOneShot(audioVariant.GetEntry(), audioPlayer.Volume);
    }

    public void PlayOneShotWithAudioSourceSetting([CanBeNull] AudioPlayer audioPlayer,
        [CanBeNull] AudioSource audioSource, [CanBeNull] AudioEventVariant audioVariant)
    {
        if (audioPlayer == null || audioSource == null || audioVariant == null) return;

        SetAudioSourceSetting(audioPlayer, audioSource, audioVariant);
        audioSource.PlayOneShot(audioVariant.GetEntry(), audioPlayer.Volume);
    }

    public void SetAudioSourceSetting([CanBeNull] AudioPlayer audioPlayer, [CanBeNull] AudioSource audioSource,
        [CanBeNull] AudioEventVariant variant)
    {
        if (audioPlayer == null || audioSource == null || variant == null) return;

        if (audioSource.isPlaying)
        {
            audioSource.volume = audioPlayer.Volume * variant.volume;
            return;
        }
        audioSource.gameObject.SetActive(true);

        int randomIndex = UnityEngine.Random.Range(0, variant.EntryList.Count);

        if (randomIndex == audioPlayer.PrevIndex)
            randomIndex = (randomIndex + 1) % variant.EntryList.Count;

        audioPlayer.PrevIndex = randomIndex;

        audioSource.clip = variant.EntryList[randomIndex];
        audioSource.loop = variant.loop;
        audioSource.priority = variant.priority;
        audioSource.volume = audioPlayer.Volume * variant.volume;
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
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff,
                new AnimationCurve(SerializableKeyframe.ArrayToKeyframes(variant.keyFrames)));
        }
    }
}
