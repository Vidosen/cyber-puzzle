using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCanvasTest : MonoBehaviour
{
    private RectTransform _thisTransform;

    public RectTransform ThisTransform =>
        _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;

    private Quaternion _defaultAlignment;


    public float MaxHorizontalAngle;
    public float MaxVerticalAngle;

    private void Awake()
    {
        _defaultAlignment = ThisTransform.rotation;
    }
    void Update()
    {
        var mouseInput = Input.mousePosition;
        var alignmentX = Mathf.Clamp01(mouseInput.x / Screen.width) * 2 - 1;
        var alignmentY = Mathf.Clamp01(mouseInput.y / Screen.height) * 2 - 1;
        
        ThisTransform.rotation = Quaternion.Euler(alignmentY * MaxVerticalAngle, alignmentX *MaxHorizontalAngle, 0);
    }
}
