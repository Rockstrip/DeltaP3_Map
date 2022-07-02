using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DebugUI : MonoBehaviour
{
    private static TextMeshProUGUI _textMeshProUGUI;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public static void Log(string text)
    {
        _textMeshProUGUI.text = text;
        Debug.Log(text);
    }
}
