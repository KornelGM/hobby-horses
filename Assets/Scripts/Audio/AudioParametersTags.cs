using UnityEngine;

[CreateAssetMenu(fileName = "AudioParametersTags", menuName = "ScriptableObjects/Databases/AudioParameterTags")]
public class AudioParametersTags : ScriptableObject
{
    public string CenterFrequency = "CenterFreq";
    public string OctaveRange = "OctaveRange";
    public string FrequencyGain = "FrequencyGain";
    public string CutOffFrequency = "CutoffFreq";
    public string Resonance = "Resonance";
}
