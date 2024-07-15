#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioDatabase))]
public class AudioDatabaseEditor : DatabaseElementWithClassGeneratorEditor<AudioEvent>
{
    protected override string Postfix { get; set; } = "Audio";
    protected override string TopNameSubfix { get; set; } = "AudioEvent";
}
#endif