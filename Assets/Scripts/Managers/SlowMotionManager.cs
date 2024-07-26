using System;
using System.Collections;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    public Action<float> OnSlowMotionTimeChange;

    public float MaxSlowMotionTime => _maxSlowMotionTime;
    public float SlowMotionTime => _slowMotionTime;

    [SerializeField] private float _slowMotionScale;
    [SerializeField] private float _coldownTime;
    [SerializeField] private float _maxSlowMotionTime;

    private Coroutine _cooldownCoroutine;

    private float _slowMotionTime;
    private bool _isSlowMotion;
    private bool _canRegenerateTime;

    private void Start()
    {
        _slowMotionTime = _maxSlowMotionTime;
    }

    public void ChangeTime()
    {
        if (_isSlowMotion)
            DoNormal();
        else
            DoSlowMotion();
    }

    public void DoSlowMotion()
    {
        if (_isSlowMotion)
            return;

        Time.timeScale = _slowMotionScale;
        _isSlowMotion = true;
        _canRegenerateTime = false;
    }

    public void DoNormal()
    {
        if (!_isSlowMotion)
            return;

        Time.timeScale = 1;
        _isSlowMotion = false;

        if(_cooldownCoroutine != null)
            StopCoroutine(_cooldownCoroutine);

        _cooldownCoroutine = StartCoroutine(Cooldown());
    }

    private void Update()
    {
        ControlTime();
    }

    public void ControlTime()
    {
        CalculateSlowMotionTime();

        if (_slowMotionTime <= 0)
            DoNormal();
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(_coldownTime);
        _canRegenerateTime = true;
    }

    public float CalculateSlowMotionTime()
    {
        if (_isSlowMotion)
        {
            _slowMotionTime -= 1 * Time.unscaledDeltaTime;
            OnSlowMotionTimeChange?.Invoke(_slowMotionTime);
        }
        else if(_canRegenerateTime)
        {
            _slowMotionTime += 1 * Time.unscaledDeltaTime;
            OnSlowMotionTimeChange?.Invoke(_slowMotionTime);
        }

        _slowMotionTime = Mathf.Clamp(_slowMotionTime, 0, _maxSlowMotionTime);
        return _slowMotionTime;
    }
}
