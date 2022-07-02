using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PageCommunication : MonoBehaviour
{
    [SerializeField] private PointPainter _pointPainter;
    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    public void AddPoint(string cityJson)
    {
        if (!string.IsNullOrEmpty(cityJson))
        {
            var city = JsonUtility.FromJson<PointPainter.City>(cityJson);
            _pointPainter.AddPoint(city);
        }
    }

    public void FocusPoint(string cityJson)
    {
        if (!string.IsNullOrEmpty(cityJson))
        {
            var city = JsonUtility.FromJson<PointPainter.City>(cityJson);
            _pointPainter.FocusPoint(city);
        }
    }
}
