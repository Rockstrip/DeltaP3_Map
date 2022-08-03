using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Serialization;

public class MapController : MonoBehaviour
{
    [SerializeField] private LeanPinchCamera leanPinch;
    [SerializeField] private FingerRotation fingerRotation;
    [SerializeField] private OnlineMapsSpriteRendererControl mapsRenderer;
    [SerializeField] private OnlineMaps onlineMaps;
    [SerializeField] private GameObject earth;
    [SerializeField] private Blackout blackout;
    
    private enum State { Earth, Map }

    private State _state = State.Earth;


    private IEnumerator Start()
    {
        while (true)
        {
            
            var currentCoord = fingerRotation.CurrentCoord.ToVector2();
            if (_state == State.Earth && Math.Abs(leanPinch.Zoom - leanPinch.ClampMin) < 0.1)
            {
                onlineMaps.SetPositionAndZoom(currentCoord.y, currentCoord.x, 4);
                leanPinch.Zoom = leanPinch.ClampMin + 0.15f;
                yield return StartCoroutine(blackout.Show());

                // onlineMaps.zoom = 4;
                fingerRotation.freezeRot = true;
                leanPinch.enabled = false;
                mapsRenderer.allowUserControl = true;
                mapsRenderer.allowZoom = true;
                _state = State.Map;

                Debug.Log("SwitchToMap");
                Debug.Log(currentCoord);

                onlineMaps.GetComponent<SpriteRenderer>().enabled = true;
                earth.gameObject.SetActive(false);
                
                onlineMaps.SetPositionAndZoom(currentCoord.y, currentCoord.x, 4);
                yield return new WaitForSeconds(0.2f);
                OnlineMapsMarkerManager.Redraw();
                onlineMaps.SetPositionAndZoom(currentCoord.y, currentCoord.x, 4);
                yield return StartCoroutine(blackout.Hide());
                onlineMaps.SetPositionAndZoom(currentCoord.y, currentCoord.x, 4);
            }
            else if (_state == State.Map && onlineMaps.zoom < 4)
            {
                yield return StartCoroutine(blackout.Show());

                //leanPinch.SetZoom(leanPinch.ClampMin + 0.15f);
                leanPinch.Zoom = leanPinch.ClampMin + 0.15f;

                
                fingerRotation.freezeRot = false;
                leanPinch.enabled = true;
                mapsRenderer.allowUserControl = false;
                mapsRenderer.allowZoom = false;
                
                onlineMaps.GetPosition(out var lon, out var lat);
                var angle = currentCoord - new Vector2((float) lat, (float) lon);
                fingerRotation.transform.rotation *= Quaternion.Inverse(Quaternion.Euler(angle));

                _state = State.Earth;
                onlineMaps.GetComponent<SpriteRenderer>().enabled = false;
                earth.gameObject.SetActive(true);

                yield return StartCoroutine(blackout.Hide());

                Debug.Log("SwitchToEarth");
                Debug.Log($"Y: {currentCoord.y} X: {currentCoord.x}");
                Debug.Log(fingerRotation.transform.rotation.eulerAngles);
            }

            //yield return new WaitForSeconds(0.5f);
            yield return null;
        }
    }
}
