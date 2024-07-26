using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsGroundChecker : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private PlayerInputReader _playerInputReader;
    [ServiceLocatorComponent] private GravityCharacterController _gravityCharacterController;

    [SerializeField] private Tags _tags;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private AudioSource _audioSource;

    public List<TerrainFootsteps> TerrainFootstepsList = new List<TerrainFootsteps>();
   
    bool _standingStill => _playerInputReader.Movement.x == 0 && _playerInputReader.Movement.z == 0;
    private int ticksPassed;
    private AudioStorage _currentGround;
    private TerrainChecker _terrainChecker;
    private AudioEventVariant _currentAudioVariant;
    private string _currentTerrain;
    private int _speedValue = 10;

    public float WalkSoundFrequencySeconds;
    public float RunSoundFrequencySeconds;

    private float _walkSoundFrequencySeconds => WalkSoundFrequencySeconds / _speedValue;
    private float _runSoundFrequencySeconds => RunSoundFrequencySeconds / _speedValue;

    public void CustomStart()
    {
        _terrainChecker = new TerrainChecker();
        InvokeRepeating(nameof(CheckGround), 0.1f, 0.1f);
        //InvokeRepeating(nameof(FootStepInterval), 0.07f, 0.07f);
        StartCoroutine(nameof(StepTimePassing));
    }

    private void CheckGround()
    {
        if (CheckMeshGround()) return;
        if (CheckTerrainGround()) return;
    }

    private void Update()
    {
        //if (!_gravityCharacterController.IsGrounded) return;

        if (_standingStill) return;

        if (_currentGround == null && _currentTerrain == null) return;

        if (ticksPassed >= _speedValue)
        {
            PlayFootstepSound();
            ticksPassed = 0;
        }
    }

    private IEnumerator StepTimePassing()
    {
        while (true)
        {
            float timeToWait = _playerInputReader.IsChargeJump ? _runSoundFrequencySeconds : _walkSoundFrequencySeconds;

            if (_currentGround != null && _currentGround.Stairs)
            {
                timeToWait = timeToWait * 0.5f;
            }

            yield return new WaitForSeconds(timeToWait);
            ticksPassed++;
        }
    }

    //private void FootStepInterval()
    //{
    //    ticksPassed += _playerInputReader.IsSprint ? 3 : 2;
    //}

    private bool CheckMeshGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3, _layerMask))
        {
            if (hit.collider.CompareTag(_tags.Tag_Ground))
            {
                _currentGround = hit.collider.GetComponent<AudioStorage>();
                _currentAudioVariant = _currentGround.GetRandomAudioEventVariant();
                _currentTerrain = string.Empty;
                return true;
            }
        }

        return false;
    }

    private bool CheckTerrainGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3, _layerMask))
        {
            if (hit.transform.GetComponent<Terrain>() != null)
            {
                Terrain terrain = hit.transform.GetComponent<Terrain>();

                if(_currentTerrain != _terrainChecker.GetTerrainName(transform.position, terrain))
                {
                    _currentTerrain = _terrainChecker.GetTerrainName(transform.position, terrain);
                    _currentAudioVariant = GetAudioFromTerrainFootsteps();
                }
            }
        }

        return false;
    }

    private AudioEventVariant GetAudioFromTerrainFootsteps()
    {
        TerrainFootsteps terrainFootsteps = TerrainFootstepsList.Find(terrain => terrain.name == _currentTerrain);
        return terrainFootsteps.GetRandomFootstepsAudioEventVariant();
    }

    private void PlayFootstepSound()
    {
        _audioPlayer.PlayOneShot(_currentAudioVariant, _audioSource);
        //_audioPlayer.PlayEvent(_currentAudioVariant, _audioSource);
    }

}
