using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

[RequireComponent(typeof(LeanPinchCamera))]
public class MouseScroll : MonoBehaviour
{
    [SerializeField] private float force = 1f;
    private LeanPinchCamera _leanPinchCamera;

    private void Awake()
    {
        _leanPinchCamera = GetComponent<LeanPinchCamera>();
    }

    private void Update()
    {
        _leanPinchCamera.Zoom += Input.mouseScrollDelta.y * force;
    }
}
