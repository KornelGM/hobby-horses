using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "ScriptableObjects/Audio/Database")]
public class AudioDatabase : DatabaseElementWithClassGenerator<AudioEvent>
{
    public override List<string> GetPath()
    {
        throw new System.NotImplementedException();
    }
}
