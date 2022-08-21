using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PageCommunication : MonoBehaviour
{
    [SerializeField] private PointPainter pointPainter;
    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    public void AddPoint(string cityPinJson)
    {
        if (!string.IsNullOrEmpty(cityPinJson))
        {
            try
            {
                var cityPin = JsonUtility.FromJson<CityPinCoordInfo>(cityPinJson);
                cityPin.pin.icon = PinsContainer.Instance.FindTexture(cityPin.pin);

                if (cityPinJson.Contains("\"coord\""))
                    pointPainter.AddPoint(cityPin.coord, cityPin.pin);
                else
                    pointPainter.AddPoint(cityPin.city, cityPin.pin);
            }
            catch (Exception e)
            {
                DebugGUI.Log(e.Message);
            }
        }
    }

    public void FocusPoint(string cityJson)
    {
        if (!string.IsNullOrEmpty(cityJson))
        {
            try
            {
                var city = JsonUtility.FromJson<City>(cityJson);
                pointPainter.FocusPoint(city);
            }
            catch (Exception e)
            {
                DebugGUI.Log(e.Message);
            }
        }
    }
}

public struct CityPinCoordInfo
{
    public City city;
    public Coord coord;
    public Pin pin;
}