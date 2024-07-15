using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecordedFrame
{
    public float Time;

    public List<string> ButtonDown = new();
    public List<string> ButtonUp = new();
    public List<string> Button = new();

    public Dictionary<string, float> Axis = new();

    public RecordedFrame() { }
    public RecordedFrame(RecordedFrame recordedFrame)
    {
        Time = recordedFrame.Time;
        ButtonDown = new (recordedFrame.ButtonDown);
        ButtonUp = new (recordedFrame.ButtonUp);
        Button = new (recordedFrame.Button);
        Axis = new (recordedFrame.Axis);
    }
}


[System.Serializable]
public class Record
{
    public Vector3 Position;
    public Quaternion Rotation;
    public List<RecordedFrame> RecordedFrames = new List<RecordedFrame>();

    public Record(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

public class VirtualControllerRecorder : MonoBehaviour, IServiceLocatorComponent, IUpdateable, IEarlyUpdate
{
    public ServiceLocator MyServiceLocator { get; set; }
    [ServiceLocatorComponent] private PlayerInputReader _inputReader;
    [ServiceLocatorComponent] private TimeManager _timeManager;
    [ServiceLocatorComponent] private PlayerCameraRotator _cameraRotator;

    public bool Enabled => true;
    public Action<Record> RecordingFinished;

    private Record _currentRecord = null;
    private RecordedFrame _currFrame = null;
    private bool _recording = false;

    private void OnDestroy()
    {
        if (_recording) FinishRecording();
    }

    public void CustomEarlyUpdate()
    {
        HandleCurrentFrame();
    }

    public void CustomUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            if (!_recording)
                StartRecording();
            else
                FinishRecording();
        }
    }

    private void LateUpdate()
    {
        if (!_recording) return;
        if (_currentRecord == null) return;

        _currFrame.Time = _timeManager.GetDeltaTime();
        _currentRecord.RecordedFrames.Add(_currFrame);
        _currFrame = null;
    }

    private void StartRecording() 
    {
        if (_recording) return;
        SubscribeInto(_inputReader);
        _currentRecord = new(MyServiceLocator.transform.position, _cameraRotator.Camera.transform.rotation);
        _recording = true;
    }

    private void FinishRecording()
    {
        if (!_recording) return;
        UnsubscribeFrom(_inputReader);
        RecordingFinished?.Invoke(_currentRecord);
        _currentRecord = null;
        _recording = false;
    }

    private void SubscribeInto(PlayerInputReader inputReader)
    {
        inputReader.OnButtonDown += OnButtonDown;
        inputReader.OnButtonUp += OnButtonUp;
        inputReader.OnButton += OnButton;
        inputReader.OnAxis += SetupAxis;
    }

    private void UnsubscribeFrom(PlayerInputReader inputReader)
    {
        inputReader.OnButtonDown -= OnButtonDown;
        inputReader.OnButtonUp -= OnButtonUp;
        inputReader.OnButton -= OnButton;
        inputReader.OnAxis -= SetupAxis;
    }

    private void OnButtonDown(string button)
    {
        _currFrame.ButtonDown.Add(button);
    }

    private void OnButtonUp(string button)
    {
        _currFrame.ButtonUp.Add(button);
    }

    private void OnButton(string button)
    {
        _currFrame.Button.Add(button);
    }

    private void SetupAxis(string axisName, float value)
    {
        if (value == 0) return;
        _currFrame.Axis.Add(axisName, value);
    }

    private void HandleCurrentFrame()
    {
        if (_currFrame == null) _currFrame = new();
    }
}
