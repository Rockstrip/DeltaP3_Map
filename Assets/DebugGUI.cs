using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    private static string _text;
    public static void Log(string text)
    {
        _text += text + "\n";
        Debug.Log(text);
    }
    public static void Clear()
    {
        _text = "";
    }
    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 1000, 100), _text);
    }
}
