using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMesh = null;
    [SerializeField] private Material _inFrontMaterial;
    [SerializeField] private Material _depthMaterial;
    private Camera _mainCamera = null;

    void OnEnable()
    {
        _mainCamera= Camera.main;
    }

    void FixedUpdate()
    {
        LookAtCameraZAxis();
    }

    public void SetText(string text)
    {
        _textMesh.text = text;
    }

    public void ToggleInFront(bool toggle)
    {
        _textMesh.GetComponent<MeshRenderer>().material = toggle ? _inFrontMaterial : _depthMaterial;
    }
    
    public void SetTextColor(Color color)
    {
        _textMesh.color = color;
    }
    
    private void LookAtCameraZAxis()
    {
        transform.rotation = Quaternion.LookRotation( transform.position-_mainCamera.transform.position , Vector3.up);
    }
}
