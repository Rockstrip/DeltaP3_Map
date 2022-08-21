using System;
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
               FindObjectOfType<PageCommunication>().AddPoint(inputField.text);
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
                FindObjectOfType<PageCommunication>().FocusPoint(inputField.text);
            }
            catch (Exception e)
            {
                DebugGUI.Log("Wrong City name");
            }
        }
    }
}