using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointController : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private float radius = 1;
    [SerializeField] private float distanceLimit = 0.5f;
    public TextMeshPro cityName; 

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        var transform1 = transform;

        var scale = Vector3.Distance(transform1.position, camera.transform.position) / 100;
        transform1.localScale = Vector3.one * scale;

        var radiusPos = scale * 2.5f;
        var min = transform1.localPosition.normalized * radius;
        var pos = min * radiusPos;
        pos = Vector3.ClampMagnitude(pos, distanceLimit);
        transform1.localPosition = min + pos;
        
        transform.LookAt(camera.transform);
        
        var color = cityName.color;
        color.a = transform1.position.z < 0.1f 
            ? (transform1.position.z + 0.1f) * 5f 
            : 1;
        cityName.color = color;
    }
}
