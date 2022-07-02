using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmoSizer : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private Vector2 cameraDistance;
    [SerializeField] private Vector2 scale;
    [SerializeField] private float multiplayer = 1f;

    private void Update()
    {
        var zeroOne = Mathf.InverseLerp(cameraDistance.x, cameraDistance.y, camera.transform.position.z);
        transform.localScale = Mathf.Pow(Mathf.Lerp(scale.x, scale.y, 1 - zeroOne), multiplayer) * Vector3.one;
    }
}
