using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIFloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text = null;

    [SerializeField] private float _speed = 1f;
    [SerializeField] private Vector3 _movement = new Vector3(0f,1f,0f);
    [SerializeField] private float _floatingTime = 2f;

    private bool _floating = false;

    void Update()
    {
        if (!_floating) return;

        transform.position += _movement * _speed * Time.deltaTime;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Launch(string text, Color color)
    {
        _text.text = text;
        _text.color = color;
        _text.gameObject.SetActive(true);

        StartCoroutine(Float());
    }

    private IEnumerator Float()
    {
        _floating = true;

        yield return new WaitForSeconds(_floatingTime);

        Destroy(gameObject);
    }
}
