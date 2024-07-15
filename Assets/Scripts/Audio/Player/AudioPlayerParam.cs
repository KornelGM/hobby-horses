using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerParam
{
    public delegate void Action(AudioPlayerParam param, AudioSource audioSource);
    public enum Type
    {
        Clip,
        IgnoreListenerPause,
        Loop,
        Priority,
        Volume,
        Pitch,
        StereoPan,
        SpatialBlend,
        ReverbZoneMix,
        DopplerLevel,
        Spread,
        RolloffMode,
        MinDistance,
        MaxDistance,
        AnimationCurve
    }

    public Type type;
    public object value;
    public Action Handler;

    private static readonly Dictionary<Type, Action> typeActionDict = new Dictionary<Type, Action>()
    {
        { Type.Clip, ClipHandler },
        { Type.IgnoreListenerPause, IgnoreListenerPauseHandler },
        { Type.Loop, LoopHandler },
        { Type.Priority, PriorityHandler },
        { Type.Volume, VolumeHandler },
        { Type.Pitch, PitchHandler },
        { Type.StereoPan, StereoPanHandler },
        { Type.SpatialBlend, SpatialBlendHandler },
        { Type.ReverbZoneMix, ReverbZoneMixHandler },
        { Type.DopplerLevel, DopplerLevelHandler },
        { Type.Spread, SpreadHandler },
        { Type.RolloffMode, RolloffModeHandler },
        { Type.MinDistance, MinDistanceHandler },
        { Type.MaxDistance, MaxDistanceHandler },
        { Type.AnimationCurve, AnimationCurveHandler }
    };

    private AudioPlayerParam(Type type, object value)
    {
        this.type = type;
        this.value = value;
    }

    public void Handle(AudioSource audioSource)
    {
        typeActionDict[type](this, audioSource);
    }

    public static AudioPlayerParam Clip(AudioClip value)
    {
        return new AudioPlayerParam(Type.Clip, value);
    }

    public static void ClipHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.clip = (AudioClip)param.value;
        audioSource.timeSamples = audioSource.pitch < 0f ? audioSource.clip.samples - 1 : 0;
    }

    public static AudioPlayerParam IgnoreListenerPause(bool value)
    {
        return new AudioPlayerParam(Type.IgnoreListenerPause, value);
    }

    public static void IgnoreListenerPauseHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.ignoreListenerPause = (bool)param.value;
    }

    public static AudioPlayerParam Loop(bool value)
    {
        return new AudioPlayerParam(Type.Loop, value);
    }

    public static void LoopHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.loop = (bool)param.value;
    }

    public static AudioPlayerParam Priority(int value)
    {
        return new AudioPlayerParam(Type.Priority, value);
    }

    public static void PriorityHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.priority = (int)param.value;
    }

    public static AudioPlayerParam Volume(float value)
    {
        return new AudioPlayerParam(Type.Volume, value);
    }

    public static void VolumeHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.volume = (float)param.value;
    }

    public static AudioPlayerParam Pitch(float value)
    {
        return new AudioPlayerParam(Type.Pitch, value);
    }

    public static void PitchHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.pitch = (float)param.value;
        audioSource.timeSamples = audioSource.pitch < 0f ? audioSource.clip.samples - 1 : 0;
    }

    public static AudioPlayerParam StereoPan(float value)
    {
        return new AudioPlayerParam(Type.StereoPan, value);
    }

    public static void StereoPanHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.panStereo = (float)param.value;
    }

    public static AudioPlayerParam SpatialBlend(float value)
    {
        return new AudioPlayerParam(Type.SpatialBlend, value);
    }

    public static void SpatialBlendHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.spatialBlend = (float)param.value;
    }

    public static AudioPlayerParam ReverbZoneMix(float value)
    {
        return new AudioPlayerParam(Type.ReverbZoneMix, value);
    }

    public static void ReverbZoneMixHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.reverbZoneMix = (float)param.value;
    }

    public static AudioPlayerParam DopplerLevel(float value)
    {
        return new AudioPlayerParam(Type.DopplerLevel, value);
    }

    public static void DopplerLevelHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.dopplerLevel = (float)param.value;
    }

    public static AudioPlayerParam Spread(float value)
    {
        return new AudioPlayerParam(Type.Spread, value);
    }

    public static void SpreadHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.spread = (float)param.value;
    }

    public static AudioPlayerParam RolloffMode(AudioRolloffMode value)
    {
        return new AudioPlayerParam(Type.RolloffMode, value);
    }

    public static void RolloffModeHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.rolloffMode = (AudioRolloffMode)param.value;
    }

    public static AudioPlayerParam MinDistance(float value)
    {
        return new AudioPlayerParam(Type.MinDistance, value);
    }

    public static void MinDistanceHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.minDistance = (float)param.value;
    }

    public static AudioPlayerParam MaxDistance(float value)
    {
        return new AudioPlayerParam(Type.MaxDistance, value);
    }

    public static void MaxDistanceHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.maxDistance = (float)param.value;
    }

    public static AudioPlayerParam AnimationCurve(AnimationCurve value)
    {
        return new AudioPlayerParam(Type.AnimationCurve, value);
    }

    public static void AnimationCurveHandler(AudioPlayerParam param, AudioSource audioSource)
    {
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, (AnimationCurve)param.value);
    }
}