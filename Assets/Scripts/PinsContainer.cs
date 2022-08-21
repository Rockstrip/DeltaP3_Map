using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class PinsContainer : MonoBehaviour
{
    public static PinsContainer Instance;
    public List<Pin> pins;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Texture2D FindTexture(Pin pin) => 
        pins.Find(x => x.assetType == pin.assetType && x.projectStage == pin.projectStage).icon;

    public static Sprite Texture2dToSprite(Texture2D tex) => 
        Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), 
            new Vector2(0.5f, 0.0f), 100.0f);
}
[Serializable]
public struct Pin {
    public string assetType;
    public string projectStage;
    public Texture2D icon;
    public string label;
}