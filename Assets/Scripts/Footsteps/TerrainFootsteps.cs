using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrain Footsteps", menuName = "Create Terrain Footsteps")]
public class TerrainFootsteps : ScriptableObject
{
    public AudioEvent Footsteps;
    public AudioEvent Jump;

    public AudioEventVariant GetRandomFootstepsAudioEventVariant() => Footsteps.EntryList[Random.Range(0, Footsteps.EntryList.Count)];
    public AudioEventVariant GetRandomJumpAudioEventVariant() => Jump.EntryList[Random.Range(0, Jump.EntryList.Count)];
}
