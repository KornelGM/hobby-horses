#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioEventVariant))]
[CanEditMultipleObjects]
public class AudioEventVariantEditor : DatabaseElementEditor<AudioClip>
{
    AudioSource copyAudioSource;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AudioEventVariant variant = target as AudioEventVariant;

        if (variant.rolloffMode == AudioRolloffMode.Custom)
        {
            AnimationCurve curve = new(SerializableKeyframe.ArrayToKeyframes(variant.keyFrames));
            curve = EditorGUILayout.CurveField(curve, Color.red, new Rect(0, 0, 1f, 1.1f));
            variant.keyFrames = SerializableKeyframe.ArrayToSerializableKeyframes(curve.keys);
        }

        EditorGUILayout.Space();
        copyAudioSource = EditorGUILayout.ObjectField("Audio source", copyAudioSource, typeof(AudioSource), true) as AudioSource;

        EditorGUI.BeginDisabledGroup(copyAudioSource == null);
        if (GUILayout.Button("Import settings from AudioSource"))
        {
            variant.ignoreListenerPause = copyAudioSource.ignoreListenerPause;
            variant.loop = copyAudioSource.loop;
            variant.priority = copyAudioSource.priority;
            variant.volume = copyAudioSource.volume;
            variant.pitch = copyAudioSource.pitch;
            variant.stereoPan = copyAudioSource.panStereo;
            variant.spatialBlend = copyAudioSource.spatialBlend;
            variant.reverbZoneMix = copyAudioSource.reverbZoneMix;
            variant.dopplerLevel = copyAudioSource.dopplerLevel;
            variant.spread = copyAudioSource.spread;
            variant.rolloffMode = copyAudioSource.rolloffMode;
            variant.minDistance = copyAudioSource.minDistance;
            variant.maxDistance = copyAudioSource.maxDistance;

            if (variant.rolloffMode == AudioRolloffMode.Custom)
            {
                variant.keyFrames = SerializableKeyframe.ArrayToSerializableKeyframes(copyAudioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff).keys);
            }
        }

        if (GUILayout.Button("Export settings to AudioSource"))
        {
            copyAudioSource.ignoreListenerPause = variant.ignoreListenerPause;
            copyAudioSource.loop = variant.loop;
            copyAudioSource.priority = variant.priority;
            copyAudioSource.volume = variant.volume;
            copyAudioSource.pitch = variant.pitch;
            copyAudioSource.panStereo = variant.stereoPan;
            copyAudioSource.spatialBlend = variant.spatialBlend;
            copyAudioSource.reverbZoneMix = variant.reverbZoneMix;
            copyAudioSource.clip = variant.GetEntry();
            copyAudioSource.dopplerLevel = variant.dopplerLevel;
            copyAudioSource.spread = variant.spread;
            copyAudioSource.rolloffMode = variant.rolloffMode;
            copyAudioSource.minDistance = variant.minDistance;
            copyAudioSource.maxDistance = variant.maxDistance;

            if (variant.rolloffMode == AudioRolloffMode.Custom)
            {
                copyAudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(SerializableKeyframe.ArrayToKeyframes(variant.keyFrames)));
            }
        }
        EditorGUI.EndDisabledGroup();
    }
}
#endif