using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioEventVariant", menuName = "ScriptableObjects/Audio/EventVariant")]
public class AudioEventVariant : DatabaseElement<AudioClip>
{
    public AudioMixerGroup AudioMixerGroup;
    public bool ignoreListenerPause = false;
    public bool loop = false;
    [Range(0, 256)]
    public int priority = 128;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    [Range(-1f, 1f)]
    public float stereoPan = 0f;
    [Range(0f, 1f)]
    public float spatialBlend = 0f;
    [Range(0f, 1.1f)]
    public float reverbZoneMix = 1f;
    [Range(0f, 5f)]
    public float dopplerLevel = 1f;
    [Range(0, 360)]
    public float spread = 0f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    [Min(0)]
    public float minDistance = 1f;
    [Min(0.01f)]
    public float maxDistance = 500;
    public SerializableKeyframe[] keyFrames = { new SerializableKeyframe(new Keyframe(0f, 1f)), new SerializableKeyframe(new Keyframe(1f, 0f)) };

    [HideInInspector] public AnimationCurve curve;

    public AnimationCurve GetCurve()
    {
#if !UNITY_EDITOR
        if (curve != null)
        {
            return curve;
        }
#endif

        curve = new(SerializableKeyframe.ArrayToKeyframes(keyFrames));
        return curve;
    }

    public override List<string> GetPath()
    {
        throw new System.NotImplementedException();
    }
}
