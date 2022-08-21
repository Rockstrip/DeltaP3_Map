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
    [SerializeField] private float spinSpeed = 0.5f;
    public Coord CurrentCoord;
    private LeanManualRotate _leanManualRotate;
    private Coroutine _rotateCoroutine;
    public bool freezeRot;

    private void Awake()
    {
        _leanManualRotate = GetComponent<LeanManualRotate>();
    }

    private void LateUpdate()
    {
        _leanManualRotate.RotateA(spinSpeed);

        var mod = transform.up;
        mod.z = 0;
        var diff = Vector3.SignedAngle(mod.normalized, Vector3.up, Vector3.forward);
        transform.Rotate(0, 0, diff / 2, Space.World);

        var diffUp = Vector3.SignedAngle(transform.up, Vector3.up, Vector3.right);

        if (diffUp > limitDegreeB)
            transform.Rotate(Vector3.right * (diffUp - limitDegreeB), Space.World);
        else if (diffUp < -limitDegreeB)
            transform.Rotate(Vector3.right * (diffUp + limitDegreeB), Space.World);

        var forwardXZ = transform.forward;
        forwardXZ.y = 0; 
        var forwardYZ = transform.forward;
        forwardYZ.x = 0;
        forwardYZ.y = forwardYZ.z < 0 ? -forwardYZ.y : forwardYZ.y;
        forwardYZ.z = Mathf.Abs(forwardYZ.z);
        var lat = Vector3.SignedAngle(forwardYZ, Vector3.forward, Vector3.left);
        var lon = Vector3.SignedAngle(forwardXZ, Vector3.forward, Vector3.down);
        CurrentCoord = new Coord(lon, lat);
        Debug.Log(CurrentCoord);
    }

    public void OnFingerDelta(Vector2 delta)
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);

        if (!freezeRot)
        {
            _leanManualRotate.RotateA(delta.x);
            _leanManualRotate.RotateB(-delta.y);
        }
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