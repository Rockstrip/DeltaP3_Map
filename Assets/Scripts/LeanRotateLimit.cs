using System;
using System.Collections;
using System.Collections.Generic;
using CW.Common;
using Lean.Common;
using UnityEngine;

public class LeanRotateLimit : LeanManualRotate
{
    [SerializeField] private bool freezeZ;
    [SerializeField] private float limitX;
    private bool isResetting;
    public override void ResetRotation()
    {
	    base.ResetRotation();
	    StartCoroutine(Routine());

	    IEnumerator Routine()
	    {
		    isResetting = true;
		    yield return new WaitForSeconds(0.2f);
		    isResetting = false;
	    }
    }

    public override void RotateAB(Vector2 delta)
    {
	    var finalTransform = target != null ? target.transform : transform;
	    var oldRotation = finalTransform.localRotation;

	    if (scaleByTime == true)
	    {
		    delta *= Time.deltaTime;
	    }

	    if (rotateAxesToCamera == true)
	    {
		    var finalCamera = CwHelper.GetCamera(_camera);

		    if (finalCamera != null)
		    {
			    var finalAxisA = finalCamera.transform.TransformDirection(axisA);
			    var finalAxisB = finalCamera.transform.TransformDirection(axisB);

			    finalTransform.Rotate(finalAxisA, delta.x * multiplier, Space.World);
			    finalTransform.Rotate(finalAxisB, delta.y * multiplier, Space.World);
		    }
	    }
	    else
	    {
		    finalTransform.Rotate(axisA, delta.x * multiplier, space);
		    finalTransform.Rotate(axisB, delta.y * multiplier, space);
	    }

	    var finalEuler = finalTransform.eulerAngles;
	    if (freezeZ)
	    {
		    finalEuler = new Vector3(finalEuler.x > 180 ? finalEuler.x - 360 : finalEuler.x, finalEuler.y,
			    freezeZ ? 0f : finalEuler.z);
	    }

	    if (limitX != 0f)
	    {
		    finalEuler = finalEuler.x > limitX
			    ? new Vector3(limitX, finalEuler.y, 0)
			    : finalEuler.x < -limitX
				    ? new Vector3(-limitX, finalEuler.y, 0)
				    : new Vector3(finalEuler.x, finalEuler.y, 0);
	    }

	    if (freezeZ || limitX != 0f)
		    finalTransform.eulerAngles = finalEuler;


	    remainingDelta *= Quaternion.Inverse(oldRotation) * finalTransform.localRotation;

	    // Revert
	    finalTransform.localRotation = oldRotation;
    }
    protected override void UpdateRotation(float factor)
    {
	    var finalTransform = target != null ? target.transform : transform;
	    var newDelta       = Quaternion.Slerp(remainingDelta, Quaternion.identity, factor);

	    finalTransform.localRotation = finalTransform.localRotation * Quaternion.Inverse(newDelta) * remainingDelta;
	    var finalEuler = finalTransform.eulerAngles;
	    if (!isResetting)
	    {
		    if (freezeZ)
		    {
			    finalEuler = new Vector3(finalEuler.x > 180 ? finalEuler.x - 360 : finalEuler.x, finalEuler.y,
				    freezeZ ? 0f : finalEuler.z);
		    }

		    if (limitX != 0f)
		    {
			    finalEuler = finalEuler.x > limitX
				    ? new Vector3(limitX, finalEuler.y, 0)
				    : finalEuler.x < -limitX
					    ? new Vector3(-limitX, finalEuler.y, 0)
					    : new Vector3(finalEuler.x, finalEuler.y, 0);
		    }

		    if (freezeZ || limitX != 0f)
			    finalTransform.eulerAngles = finalEuler;
	    }

	    remainingDelta = newDelta;
    }
}
