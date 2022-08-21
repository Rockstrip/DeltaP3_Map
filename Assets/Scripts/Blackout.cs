using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Blackout : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Image image;

    private void OnValidate()
    {
        if (!image)
            image = GetComponent<Image>();
    }

    public IEnumerator Show()
    {
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            var color = image.color;
            color.a = Mathf.Lerp(0, 1, time / duration);
            image.color = color;
            yield return null;
        }
    } 
    
    public IEnumerator Hide()
    {
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            var color = image.color;
            color.a = Mathf.InverseLerp(1, 0, time / duration);
            image.color = color;
            yield return null;
        }
    }
}