using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Common;
using Lean.Touch;
using UnityEngine;

[RequireComponent(typeof(LeanManualRotate))]
public class FingerRotation : MonoBehaviour
{
    [SerializeField] private LeanPinchCamera leanPinchCamera;
    [SerializeField] private new Camera camera;
    [SerializeField] private float limitDegreeB = 20f;
    [SerializeField] private float rotSpeedConst = 1f;
    [SerializeField] private float rotSpeedMult = 1f;
    [SerializeField] private AnimationCurve cameraSmooth;

    private LeanManualRotate _leanManualRotate;
    private Coroutine _rotateCoroutine;

    private void Awake()
    {
        _leanManualRotate = GetComponent<LeanManualRotate>();
    }

    private void LateUpdate()
    {
        var mod = transform.up;
        mod.z = 0;
        var diff = Vector3.SignedAngle(mod.normalized, Vector3.up, Vector3.forward);
        transform.Rotate(0, 0, diff / 2, Space.World);

        var diffUp = Vector3.SignedAngle(transform.up, Vector3.up, Vector3.right);

        if (diffUp > limitDegreeB)
            transform.Rotate(Vector3.right * (diffUp - limitDegreeB), Space.World);
        else if (diffUp < -limitDegreeB)
            transform.Rotate(Vector3.right * (diffUp + limitDegreeB), Space.World);
    }

    public void OnFingerDelta(Vector2 delta)
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);

        _leanManualRotate.RotateA(delta.x);



        _leanManualRotate.RotateB(-delta.y);
    }

    public void RotatePointToCamera(Vector3 angle)
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);
        _rotateCoroutine = StartCoroutine(Routine());

        IEnumerator Routine()
        {
            var position = transform.position;
            var cameraQ = Quaternion.LookRotation(camera.transform.position - position);
            var pointQ = cameraQ * Quaternion.Inverse(Quaternion.Euler(angle));
            var diffStart = Quaternion.Angle(transform.rotation, pointQ);
            float diff;
            
            var keys = cameraSmooth.keys.Length;
            for (var i = keys - 1; i >= 0; i--)
                cameraSmooth.RemoveKey(i);
            
            cameraSmooth.AddKey(0, leanPinchCamera.Zoom);
            cameraSmooth.AddKey(diffStart / 2, diffStart / 33);
            cameraSmooth.AddKey(diffStart, leanPinchCamera.ClampMin + 0.2f);
            
            do
            {
                var rotation = transform.rotation;
                diff = Quaternion.Angle(rotation, pointQ);
                transform.rotation = Quaternion.RotateTowards(
                    rotation, pointQ, (rotSpeedConst + rotSpeedMult * diff) * Time.deltaTime);

                leanPinchCamera.Zoom = cameraSmooth.Evaluate(diffStart - diff);

                yield return null;
            } while (diff != 0);
        }
    }
}