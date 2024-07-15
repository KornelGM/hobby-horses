using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStorage : MonoBehaviour, IServiceLocatorComponent
{
    public AudioEvent AudioDatabase => _audioDatabase;
    [field: SerializeField] public bool Stairs { get; set; }

    [SerializeField] private AudioEvent _audioDatabase;

    public ServiceLocator MyServiceLocator { get; set; }

    public void Start()
    {
        _audioDatabase.IsNotNull(this, nameof(_audioDatabase));
    }

    public AudioEventVariant GetRandomAudioEventVariant() => _audioDatabase.EntryList[Random.Range(0, _audioDatabase.EntryList.Count)];
}
