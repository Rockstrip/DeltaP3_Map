using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class PointPainter : MonoBehaviour
{
    [SerializeField] private FingerRotation fingerRotation;
    [SerializeField] private PointController point;
    [SerializeField] private float radius;
    [SerializeField] private string apiFormat = "https://api.openweathermap.org/geo/1.0/direct?q={0},{1},{2}&limit={3}&appid={4}";
    [SerializeField] private string apiKey = "22204c966eed63a66f0322d11f037762";

    private Dictionary<string, PointController> _points;
    private string jsonResponse;
    
    public void AddPoint(City city)
    {
        Debug.Log("AddPoint Started");

        StartCoroutine(Routine());

        IEnumerator Routine()
        {
            var cd = new CoroutineWithData(this, CityToCoord(city));
            yield return cd.Coroutine;
            if(cd.Result is Coord coord)
                AddPoint(new CityCoord(city, coord));
        }
    }    
    public void AddPoint(CityCoord cityCoord)
    {
        Debug.Log("AddPointCoord Started");

        var newDirection = Quaternion.Euler(-cityCoord.Coord.lat, -cityCoord.Coord.lon, 0) * Vector3.forward;
        newDirection = transform.rotation * newDirection; 
        var pointPos = transform.position + newDirection * radius;

        var pointController = Instantiate(point, pointPos, Quaternion.identity, transform);
        pointController.cityName.text = cityCoord.City.name;
        
        Debug.Log("AddPointCoord Stopped");
    }
    
    public void FocusPoint(City city)
    {
        StartCoroutine(Routine());

        IEnumerator Routine()
        {
            var cd = new CoroutineWithData(this, CityToCoord(city));
            yield return cd.Coroutine;
            if(cd.Result is Coord coord)
                FocusPoint(coord);
        }
    }    
    public void FocusPoint(Coord coord)
    {
        fingerRotation.RotatePointToCamera(new Vector3(-coord.lat, -coord.lon, 0));
    }

    private IEnumerator CityToCoord(City city, string limit = "")
    {
        Debug.Log("CityNameToCoord Started");

        var sb = new StringBuilder();
        sb.AppendFormat(apiFormat, city.name, city.state, city.country , limit, apiKey);
        yield return StartCoroutine(GetRequest(sb.ToString()));
        
        jsonResponse = jsonResponse.Substring(1, jsonResponse.Length - 2);
        if (jsonResponse.Length > 3)
            yield return JsonUtility.FromJson<Coord>(jsonResponse);
        
        Debug.Log("CityNameToCoord Stopped");

    }
    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    jsonResponse = webRequest.downloadHandler.text;
                    break;
            }
        }
    }

    public struct Coord
    {
        public float lon;
        public float lat;

        public Coord(float lon, float lat)
        {
            this.lon = lon;
            this.lat = lat;
        }
        
        public Vector2 ToVector2() => new Vector2(lat, lon);
    }

    public struct City
    {
        public string name;
        public string country;
        public string state;
        
        public City(string name, string country, string state)
        {
            this.name = name;
            this.country = country;
            this.state = state;
        }
    }

    public struct CityCoord
    {
        public City City;
        public Coord Coord;
        
        public CityCoord(City city, Coord coord)
        {
            City = city;
            Coord = coord;
        }
    }
}
