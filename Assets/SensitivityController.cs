using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Common;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Serialization;

public class SensitivityController : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private LeanMultiUpdate leanMultiUpdate;
    [SerializeField] private LeanPinchCamera leanPinchCamera;
    [SerializeField] private float maxMultiplier;
    [SerializeField] private float minMultiplier;

    private void Update()
    {
        var zeroOne =
            Mathf.InverseLerp(leanPinchCamera.ClampMin, leanPinchCamera.ClampMax, camera.transform.position.z);
        leanMultiUpdate.Multiplier = Mathf.Lerp(minMultiplier, maxMultiplier, zeroOne);
    }
}
