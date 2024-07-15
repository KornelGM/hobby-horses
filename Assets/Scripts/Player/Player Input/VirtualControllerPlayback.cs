using System.Collections.Generic;
using UnityEngine;

public class VirtualControllerPlayback : MonoBehaviour, IServiceLocatorComponent, IEarlyUpdate
{
    public ServiceLocator MyServiceLocator { get; set; }

    public bool Enabled => true;

    [ServiceLocatorComponent] private VirtualControllerRecorder _recorder;
    [ServiceLocatorComponent] private PlayerInputReader _controller;
    [ServiceLocatorComponent] private ITimeManager _timeManager;
    [ServiceLocatorComponent] private ICharacterRotator _characterRotator;
    [ServiceLocatorComponent] private PlayerCameraRotator _cameraRotator;
    [ServiceLocatorComponent] private FPSMovement _fpsMovement;

    private Record _record;
    private List<RecordedFrame> _transposedFrames;
    private bool _playing = false;
    private float _playbackTime;
    private int _playedFrame;
    private bool _framesLimited;
    private void OnEnable()
    {
        _recorder.RecordingFinished += SetupRecord;
    }

    private void OnDisable()
    {
        _recorder.RecordingFinished -= SetupRecord;
    }

    public void CustomEarlyUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (!_playing)
                StartPlayback();
            else
                StopPlayback();
        }

        if(_playing)PlayingPlayback();
    }

    private void PlayingPlayback()
    {
        float deltaTime = _timeManager.GetDeltaTime();
        _playbackTime += deltaTime;
       
        List<RecordedFrame> frames = new List<RecordedFrame>();
        while(deltaTime > 0)
        {
            if(_transposedFrames.Count == 0)
            {
                StopPlayback();
                return;
            }

            RecordedFrame frame = new(_transposedFrames[0]);
            if (frame == null)
            {
                StopPlayback();
                return;
            }

            if(frame.Time <= deltaTime)
            {
                _playedFrame++;
                deltaTime -= frame.Time;
                frames.Add(frame);
                _transposedFrames.RemoveAt(0);
                continue;
            }

            SeparateRecordedFrame(ref frame, out RecordedFrame least, deltaTime);
            frames.Add(frame);
            _transposedFrames.RemoveAt(0);
            _transposedFrames.Insert(0, least);
            deltaTime = 0;
        }

        PlayInThisFrame(frames);
    }

    private void PlayInThisFrame(List<RecordedFrame> frames)
    {
        foreach (RecordedFrame frame in frames)
        {
            foreach (string button in frame.ButtonDown)
            {
                _controller.SimulateButtonDown(button);
            }

            foreach (string button in frame.ButtonUp)
            {
                _controller.SimulateButtonUp(button);
            }

            foreach (string button in frame.Button)
            {
                _controller.SimulateButton(button);
            }

            foreach (KeyValuePair<string, float> button in frame.Axis)
            {
                float transposedValue = button.Value / frame.Time * Time.deltaTime;
                _controller.SetAxis(button.Key, transposedValue);
            }
        }
    }

    private void SetupRecord(Record record)
    {
        _record = record;
    }

    private void StartPlayback()
    {
        if (_record == null) return;

        _fpsMovement.CharacterController.enabled = false;
        _fpsMovement.CharacterController.transform.position = _record.Position;
        _fpsMovement.CharacterController.enabled = true;

        _characterRotator.RotateTo(_record.Rotation);
        _cameraRotator.RotateTo(_record.Rotation);

        _controller.ToggleController(true);
        _playbackTime = 0;
        _playedFrame = 0;
        _transposedFrames = new(_record.RecordedFrames);
        _playing = true;
    }

    private void StopPlayback()
    {
        _controller.ToggleController(false);
        _playbackTime = 0;
        _playedFrame = 0;
        _playing = false;
    }

    private void SeparateRecordedFrame(ref RecordedFrame first, out RecordedFrame second, float firstFrameTime)
    {
        float wholeTime = first.Time;
        float secondFrameTime = wholeTime - firstFrameTime;

        if (first == null || wholeTime < firstFrameTime)
        {
            first = null;
            second = null;
            return;
        }

        float firstFramePercent = firstFrameTime / wholeTime;
        float secondFramePercent = secondFrameTime / wholeTime;

        Dictionary<string, float> firstFrameAxis = new Dictionary<string, float>();
        Dictionary<string, float> secondFrameAxis = new Dictionary<string, float>();
        foreach (KeyValuePair<string, float> axis in first.Axis)
        {
            firstFrameAxis.Add(axis.Key, axis.Value * firstFramePercent);
            secondFrameAxis.Add(axis.Key, axis.Value * secondFramePercent);
        }

        second = new();
        second.Time = secondFrameTime;
        second.Button = first.Button;
        second.Axis = secondFrameAxis;

        first.Time = firstFrameTime;
        first.Axis = firstFrameAxis;
    }
}
