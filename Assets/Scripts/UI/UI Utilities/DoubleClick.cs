using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DoubleClick : MonoBehaviour
{
    [SerializeField] private float _doubleClickMaxTime = 0.5f;
    [SerializeField] private UnityEvent _onClick = new UnityEvent();
    [SerializeField] private UnityEvent _onDoubleClick = new UnityEvent();

    private float _leastTimeToDoubleClick = 0;

    private void Update()
    {
        if (_leastTimeToDoubleClick > 0) _leastTimeToDoubleClick -= Time.unscaledDeltaTime;
        else if (_leastTimeToDoubleClick < 0) _leastTimeToDoubleClick = 0;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (_leastTimeToDoubleClick > 0)
        {
            _onDoubleClick?.Invoke();
            _leastTimeToDoubleClick = 0;
            return;
        }

        _leastTimeToDoubleClick = _doubleClickMaxTime;
        _onClick?.Invoke();
    }
}
