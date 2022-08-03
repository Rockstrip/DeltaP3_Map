using System;
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
        {
            try
            {
                pointPainter.AddPoint(JsonUtility.FromJson<PointPainter.City>(inputField.text));
            }
            catch (Exception e)
            {
                DebugGUI.Log("Wrong City name");
            }
        }
    }

    public void FocusPoint()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            try
            {
                pointPainter.FocusPoint(JsonUtility.FromJson<PointPainter.City>(inputField.text));
            }
            catch (Exception e)
            {
                DebugGUI.Log("Wrong City name");
            }
        }
    }
}
