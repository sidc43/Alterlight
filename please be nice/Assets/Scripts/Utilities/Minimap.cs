using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ExtensionMethods.ExtensionMethods;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float maxZoomOut;
    [SerializeField] private float maxZoomIn;
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (cam.orthographicSize < maxZoomOut)
            {
                cam.orthographicSize += 0.5f;
                Print(cam.orthographicSize);
            }
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (cam.orthographicSize > maxZoomIn)
            {
                cam.orthographicSize -= 0.5f;
                Print(cam.orthographicSize);
            }
        }

    }
}
