using UnityEngine;
using UnityEngine.Audio;

public class AudioSpace : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private PlayerManager _playerManager;

    [SerializeField] private AudioSpaceVent[] _vents = new AudioSpaceVent[0];
    [SerializeField] private AudioParametersTags _audioParametersTags;
    [SerializeField] private WorldAudioSpace _worldAudioSpace;
    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    [SerializeField] private string _name;
    [SerializeField] private Tags _tags;

    private PlayerServiceLocator _localPlayer;
    private AudioSpaceVent _closestVent;

    private bool _inProximity, _isMuffled, _isPlayerInside;
    private float _ventDistance, _ventAngle, currentPercent;

    [Header("Settings")]
    public float ProximityRange;
    public float ChecksFrequency;
    public float VentRange;

    //Min and Max effect values
    private float clearCenterFreq = 8000;
    private float clearOctaveRange = 1;
    private float clearFrequencyGain = 1;
    private float clearCutoffFrequency = 5000;
    private float clearResonance = 1;

    private float muffledCenterFreq = 2735;
    private float muffledOctaveRange = 1.65f;
    private float muffledFrequencyGain = 0.6f;
    private float muffledCutoffFrequency = 405;
    private float muffledResonance = 2;

    private float _transitionSpeed => 25 * Time.deltaTime;

    public float debugAnglePercent;
    public float debugDistancePercent;

    public void CustomStart()
    {
        AudioSpaceOnLocalPlayerSet(_playerManager.LocalPlayer);
    }

    private void AudioSpaceOnLocalPlayerSet(PlayerServiceLocator LocalPlayer)
    {
        _localPlayer = LocalPlayer;

        InvokeRepeating(nameof(MakeCalculations), ChecksFrequency, ChecksFrequency);
    }

    private void SetEffectValues()
    {
        currentPercent = Mathf.Lerp(currentPercent, CalculatePercent(), _transitionSpeed);
        _audioMixerGroup.audioMixer.SetFloat(_name + "_" + _audioParametersTags.CenterFrequency, CalculateEffectStrength(muffledCenterFreq, clearCenterFreq, currentPercent));
        _audioMixerGroup.audioMixer.SetFloat(_name + "_" + _audioParametersTags.OctaveRange, CalculateEffectStrength(muffledOctaveRange, clearOctaveRange, currentPercent));
        _audioMixerGroup.audioMixer.SetFloat(_name + "_" + _audioParametersTags.FrequencyGain, CalculateEffectStrength(muffledFrequencyGain, clearFrequencyGain, currentPercent));
        _audioMixerGroup.audioMixer.SetFloat(_name + "_" + _audioParametersTags.CutOffFrequency, CalculateEffectStrength(muffledCutoffFrequency, clearCutoffFrequency, currentPercent));
        _audioMixerGroup.audioMixer.SetFloat(_name + "_" + _audioParametersTags.Resonance, CalculateEffectStrength(muffledResonance, clearResonance, currentPercent));
    }

    private void PlayerEnter() => _isPlayerInside = true;
    private void PlayerExit() => _isPlayerInside = false;

    private void MakeCalculations()
    {
        if (_localPlayer == null) return;
       
        CalculateInProximity();

        if (!_inProximity) return;

        CalculateClosestVent();

        CalculateVentDistance();

        CalculateVentAngle();

        CalculatePercent();

        SetEffectValues();
    }

    private void CalculateInProximity() => _inProximity = Vector3.Distance(_localPlayer.transform.position, transform.position) <= ProximityRange;

    private void CalculateClosestVent()
    {
        float possibleClosestVentDistance = Vector3.Distance(_localPlayer.transform.position, _vents[0].transform.position);
        AudioSpaceVent possibleClosestVent = _vents[0];

        foreach (AudioSpaceVent vent in _vents)
        {
            float distanceToVent = Vector3.Distance(_localPlayer.transform.position, vent.transform.position);

            if(distanceToVent <= possibleClosestVentDistance)
            {
                possibleClosestVentDistance = distanceToVent;
                possibleClosestVent = vent;
            }
        }

        _closestVent = possibleClosestVent;
    }

    private bool IsInFront() => _ventAngle >= 0;
    private bool IsCloseEnough() => _ventDistance <= VentRange;

    private void CalculateVentDistance() => _ventDistance = Vector3.Distance(_localPlayer.transform.position, _closestVent.transform.position);
    private void CalculateVentAngle()
    {
        Vector3 direction = Vector3.Normalize(_localPlayer.transform.position - _closestVent.transform.position);
        _ventAngle = Vector3.Dot(_closestVent.transform.forward, direction);
    }

    private float CalculatePercent()
    {
        if (_isPlayerInside) return 100;

        if (!_closestVent.IsOpen) return 0;

        float distancePercent = _ventDistance >= VentRange ? 0 : ((VentRange - _ventDistance) / VentRange) * 100;
        float anglePercent = (Mathf.Clamp01(_ventAngle) / 1) * 100;
        float average = (distancePercent + anglePercent) / 2;

        return average;
    }

    private float CalculateEffectStrength(float minEffectValue, float maxEffectValue, float percent)
    {
        float reducedMaxValue = maxEffectValue - minEffectValue;
        float value = reducedMaxValue * (percent / 100);
        value += minEffectValue;

        return value;
    }
}
