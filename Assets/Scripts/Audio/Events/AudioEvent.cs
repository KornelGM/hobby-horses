using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioEvent", menuName = "ScriptableObjects/Audio/Event")]
public class AudioEvent : DatabaseElement<AudioEventVariant>
{
    public override List<string> GetPath()
    {
        throw new System.NotImplementedException();
    }
}
