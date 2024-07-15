using System.Linq;
using UnityEngine;

[System.Serializable]
public struct SerializableKeyframe
{
    public float time;
    public float value;
    public float inTangent;
    public float outTangent;
    public float inWeight;
    public float outWeight;
    public WeightedMode weightedMode;

    public SerializableKeyframe(Keyframe original)
    {
        time = Mathf.Clamp01(original.time);
        value = Mathf.Clamp(original.value, 0f, 1.1f);
        inTangent = original.inTangent;
        outTangent = original.outTangent;
        inWeight = original.inWeight;
        outWeight = original.outWeight;
        weightedMode = original.weightedMode;
    }

    public Keyframe ToKeyframe()
    {
        Keyframe keyframe = new(time, value, inTangent, outTangent, inWeight, outWeight);
        keyframe.weightedMode = weightedMode;
        return keyframe;
    }

    public static Keyframe[] ArrayToKeyframes(SerializableKeyframe[] origin)
    {
        return origin.Select(key => key.ToKeyframe()).ToArray();
    }

    public static SerializableKeyframe[] ArrayToSerializableKeyframes(Keyframe[] origin)
    {
        return origin.Select(key => new SerializableKeyframe(key)).ToArray();
    }
}
