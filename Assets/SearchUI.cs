using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SearchUI : MonoBehaviour
{
    [SerializeField] private PointPainter pointPainter;
    [SerializeField] private TMP_InputField inputField;

    public void AddPoint()
    {
        if (!string.IsNullOrEmpty(inputField.text))
            pointPainter.AddPoint(JsonUtility.FromJson<PointPainter.City>(inputField.text));
    } 
    
    public void FocusPoint()
    {
        if(!string.IsNullOrEmpty(inputField.text))
            pointPainter.FocusPoint(JsonUtility.FromJson<PointPainter.City>(inputField.text));
    }
}
