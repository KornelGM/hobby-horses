using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicZone : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public event Action<MusicZone, bool> OnZoneEntered;
    public event Action<MusicZone> OnZoneExit;

    [SerializeField] Tags tags;
    [SerializeField] AudioEventVariant audioEventVariant;
    public float EnterSwitchTime = 1f;

    private AudioSource audioSource;
    private float lastExitTime = -1f;
    private bool shouldPlay = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        tags.IsNotNull(this, nameof(tags));
        audioEventVariant.IsNotNull(this, nameof(audioEventVariant));
    }

    private void Update()
    {
        if (shouldPlay && !audioSource.isPlaying)
        {
            PlayMusic(audioSource.volume / audioEventVariant.volume);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tags.Tag_Player))
        {
            StopAllCoroutines();
            OnZoneEntered?.Invoke(this, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tags.Tag_Player))
        {
            OnZoneExit?.Invoke(this);
        }
    }

    public void PlayMusic(float baseVolume, bool forceNewMusic = false)
    {
        if (audioSource.isPlaying)
        {
            ChangeVolume(EnterSwitchTime, baseVolume * audioEventVariant.volume);
            return;
        }

        float diffTime = Time.unscaledTime - lastExitTime;

        if (lastExitTime > 0f && !forceNewMusic &&
            ((audioEventVariant.pitch > 0f && audioSource.time + (diffTime * audioEventVariant.pitch) < audioSource.clip.length)
            || (audioEventVariant.pitch < 0f && audioSource.time + (diffTime * audioEventVariant.pitch) > 0f)))
        {
            audioSource.time += diffTime * audioEventVariant.pitch;
        }
        else
        {
            audioSource.clip = audioEventVariant.GetEntry();
            audioSource.timeSamples = audioEventVariant.pitch < 0f ? audioSource.clip.samples - 1 : 0;
        }

        audioSource.loop = audioEventVariant.loop;
        audioSource.priority = audioEventVariant.priority;
        audioSource.pitch = audioEventVariant.pitch;
        audioSource.panStereo = audioEventVariant.stereoPan;
        audioSource.spatialBlend = audioEventVariant.spatialBlend;
        audioSource.reverbZoneMix = audioEventVariant.reverbZoneMix;
        audioSource.ignoreListenerPause = audioEventVariant.ignoreListenerPause;
        audioSource.dopplerLevel = audioEventVariant.dopplerLevel;
        audioSource.spread = audioEventVariant.spread;
        audioSource.rolloffMode = audioEventVariant.rolloffMode;
        audioSource.minDistance = audioEventVariant.minDistance;
        audioSource.maxDistance = audioEventVariant.maxDistance;

        if (audioSource.rolloffMode == AudioRolloffMode.Custom)
        {
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(SerializableKeyframe.ArrayToKeyframes(audioEventVariant.keyFrames)));
        }

        audioSource.Play();
        ChangeVolume(EnterSwitchTime, baseVolume * audioEventVariant.volume);
        shouldPlay = true;
    }

    public void ChangeVolume(float fadeTime, float targetVolume)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeVolumeInternal(fadeTime, targetVolume));
    }

    public IEnumerator ChangeVolumeInternal(float switchTime, float targetVolume)
    {
        float speed = 1f / switchTime * Mathf.Abs(audioSource.volume - targetVolume);
        float time = Time.unscaledTime;
        while (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            yield return null;
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, (Time.unscaledTime - time) * speed);
            time = Time.unscaledTime;
        }

        if (Mathf.Approximately(targetVolume, 0f))
        {
            audioSource.Pause();
            lastExitTime = Time.unscaledTime;
            shouldPlay = false;
        }
    }
}
