using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableGround : DatabaseSelector<AudioEvent, AudioEventVariant>
{
    public Dictionary<string, AudioEventVariant> AudioDictionary => activeDict;

    public void Start()
    {
        Database.IsNotNull(this, nameof(Database));
        GenerateDictionary();
    }
}
